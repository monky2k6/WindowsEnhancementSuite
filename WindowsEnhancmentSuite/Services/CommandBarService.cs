using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Markup;
using WindowsEnhancementSuite.Enums;
using WindowsEnhancementSuite.Extensions;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.Helper.Windows;
using WindowsEnhancementSuite.Properties;
using WindowsEnhancementSuite.ValueObjects;
using Microsoft.Win32;

namespace WindowsEnhancementSuite.Services
{
    public class CommandBarService
    {
        // Required constants
        private const string COMMANDS_REGEX = @"[^a-zA-Z0-9 ()!+_-]";
        private const string FILES_REGEX = @"[^a-zA-Z0-9 \\()!+:_.-]";

        // WPF Controls
        private readonly Window commandBarWindow;
        private readonly TextBox commandTextBox;
        private readonly ListBox commandListBox;

        // Service Variables
        private readonly CommandBarOptions commandBarOptions;
        private readonly ObservableCollection<CommandBarEntry> commandBarEntries;
        private readonly ConcurrentBag<CommandBarEntry> histories;
        private CancellationTokenSource cancellationTokenSource;
        private string searchText;
        private string searchUserParameter;

        public CommandBarService(CommandBarOptions options)
        {
            // Load WPF Window
            using (var stream = Resources.CommandBar.ToStream<MemoryStream>())
            {
                commandBarWindow = XamlReader.Load(stream) as Window;
            }

            if (commandBarWindow == null) return;
            ElementHost.EnableModelessKeyboardInterop(commandBarWindow);
            commandBarWindow.Topmost = true;
            commandBarWindow.Activated += (sender, args) => commandTextBox.SelectAll();

            // Init Service
            commandBarOptions = options;
            cancellationTokenSource = new CancellationTokenSource();

            // Load CommandHistoryList
            histories = new ConcurrentBag<CommandBarEntry>();
            if (commandBarOptions.CommandHistoryList != null)
            {
                foreach (string s in commandBarOptions.CommandHistoryList)
                {
                    histories.Add(new CommandBarEntry(s, CommandEntryKind.History));
                }
            }

            // Get all WPF-Controls
            commandTextBox = commandBarWindow.FindName("commandTextBox") as TextBox;
            if (commandTextBox != null)
            {
                commandTextBox.PreviewKeyDown += commandTextBoxOnPreviewKeyDown;
                commandTextBox.KeyDown += commandTextBoxOnKeyDown;
                commandTextBox.TextChanged += commandTextBoxOnTextChanged;
            }

            commandListBox = commandBarWindow.FindName("commandListBox") as ListBox;
            if (commandListBox != null)
            {
                this.commandBarEntries = new ObservableCollection<CommandBarEntry>();
                commandListBox.ItemsSource = this.commandBarEntries;
                commandListBox.Items.SortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Descending));
                commandListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

                commandListBox.KeyDown += commandTextBoxOnKeyDown;
                commandListBox.MouseDoubleClick += commandListBoxOnMouseDoubleClick;
            }
        }

        public bool ShowCommandBar()
        {
            setCommandBarPosition();
            commandBarWindow.Show();
            commandBarWindow.Activate();

            return true;
        }

        public IEnumerable<string> GetCommandHistory()
        {
            return histories.Select(e => e.Command);
        }

        private void commandTextBoxOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Up)
            {
                commandListBox.Items.MoveCurrentToPrevious();
                return;
            }

            if (keyEventArgs.Key == Key.Down)
            {
                commandListBox.Items.MoveCurrentToNext();
                return;
            }
        }

        private void commandTextBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Tab)
            {
                keyEventArgs.Handled = true;
                var commandEntry = (commandListBox.Items.CurrentItem as CommandBarEntry);
                if (commandEntry == null) return;

                commandTextBox.Text = commandEntry.ToString();
                commandTextBox.CaretIndex = commandEntry.Name.Length;
                return;
            }

            if (keyEventArgs.Key == Key.Escape)
            {
                this.cancellationTokenSource.Cancel();
                commandBarWindow.Hide();
                return;
            }

            if (keyEventArgs.Key.In(Key.Return, Key.Enter))
            {
                try
                {
                    this.cancellationTokenSource.Cancel();
                    var commandEntry = (commandListBox.Items.CurrentItem as CommandBarEntry) ??
                                       new CommandBarEntry(searchText + searchUserParameter, CommandEntryKind.History);

                    executeCommand(commandEntry, keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Shift);
                }
                finally
                {
                    commandBarWindow.Hide();
                }
            }
        }

        private void commandListBoxOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            this.cancellationTokenSource.Cancel();
            var commandEntry = (commandListBox.Items.CurrentItem as CommandBarEntry);
            if (commandEntry == null) return;

            try
            {
                executeCommand(commandEntry, false);
            }
            finally
            {
                commandBarWindow.Hide();
            }
        }

        private void commandTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            this.search();
        }

        private void setCommandBarPosition()
        {
            var currentScreen = FormPositionHelper.GetActiveScreenBounds();
            commandBarWindow.Left = currentScreen.Left;
            commandBarWindow.Top = currentScreen.Top;
            commandBarWindow.Width = currentScreen.Width;
            commandBarWindow.Height = currentScreen.Height;
        }

        private void executeCommand(CommandBarEntry entry, bool asAdmin)
        {
            Task.Run(() =>
            {
                if (entry.Kind == CommandEntryKind.History && histories.All(e => e.Command != entry.Command)) histories.Add(entry);
                if (entry.Kind.In(CommandEntryKind.Directory, CommandEntryKind.TopDirectory, CommandEntryKind.Explorer)) searchUserParameter = "";
                RankingHelper.IncreaseRank(entry);

                try
                {
                    if (entry.Kind == CommandEntryKind.Window)
                    {
                        var handle = (IntPtr)Convert.ToInt32(entry.Command);
                        WindowsMethods.ActivateWindow(handle);
                    }

                    if (asAdmin)
                    {
                        // Start as Admin
                        UacAssistService.RunAsAdmin(entry.Command, searchUserParameter.Trim());
                        return;
                    }

                    Process.Start(new ProcessStartInfo(entry.Command, searchUserParameter.Trim()));
                }
                catch (Win32Exception)
                {
                }
            });
        }

        #region Search
        private void search()
        {
            // Cancel all running SearchTasks
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
            var cancelToken = this.cancellationTokenSource.Token;

            // Clear results and parameters
            this.commandBarEntries.Clear();
            searchText = commandTextBox.Text.Trim();
            searchUserParameter = String.Empty;

            if (String.IsNullOrWhiteSpace(searchText)) return;
            if (searchText.Contains(" "))
            {
                ushort space = Convert.ToUInt16(searchText.IndexOf(" "));
                searchUserParameter = searchText.Substring(space);
                searchText = searchText.Substring(0, space);
            }

            // Start individual searches, each starts a new Task
            searchCommandHistory(cancelToken);
            searchPlugins(cancelToken);
            searchEvaluate(cancelToken);
            searchOpenWindows(cancelToken);
            searchFileSystem(cancelToken);
            searchLastVisited(cancelToken);
            searchApplications(cancelToken);
            searchSystemPath(cancelToken);
        }

        private void searchCommandHistory(CancellationToken token)
        {
            Task.Run(() =>
            {
                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                string searchTerm = String.Concat(searchText, searchUserParameter).ToLower();
                Parallel.ForEach(histories, parallelOptions, entry =>
                {
                    if (entry.Command.ToLower().Contains(searchTerm)) addCommandBarEntry(entry);
                });
            }, token);
        }

        private void searchLastVisited(CancellationToken token)
        {
            Task.Run(() =>
            {
                if (commandBarOptions.ExplorerHistoryFunc == null) return;
                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                string searchTerm = String.Concat(searchText, searchUserParameter).ToLower();
                Parallel.ForEach(commandBarOptions.ExplorerHistoryFunc(), parallelOptions, path =>
                {
                    if (path.ToLower().Contains(searchTerm)) addCommandBarEntry(new CommandBarEntry(path, CommandEntryKind.Explorer));
                });
            }, token);
        }

        private void searchPlugins(CancellationToken token)
        {
            // Todo: Implement Plugin-System
        }

        private void searchEvaluate(CancellationToken token)
        {
            Task.Run(() =>
            {
                try
                {
                    var dataTable = new DataTable();
                    var evalValue = dataTable.Compute(searchText + searchUserParameter, "");
                    if (String.IsNullOrWhiteSpace(evalValue.ToString())) return;
                    this.addCommandBarEntry(new CommandBarEntry("", CommandEntryKind.Evaluation, evalValue.ToString()));
                }
                catch (SyntaxErrorException) { }
                catch (EvaluateException) { }
            }, token);
        }

        private void searchOpenWindows(CancellationToken token)
        {
            Task.Run(() =>
            {
                var paralellOptions = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                string searchTerm = String.Concat(searchText, searchUserParameter).ToLower();
                Parallel.ForEach(WindowsMethods.GetOpenWindows(), paralellOptions, window =>
                {
                    string pointer = window.Value.ToInt32().ToString();
                    if (window.Key.ToLower().Contains(searchTerm)) addCommandBarEntry(new CommandBarEntry(pointer, CommandEntryKind.Window, window.Key));
                });
            }, token);
        }

        private void searchSystemPath(CancellationToken token)
        {
            Task.Run(() =>
            {
                string systemSearch = Regex.Replace(searchText, COMMANDS_REGEX, "", RegexOptions.Compiled);
                if (String.IsNullOrWhiteSpace(systemSearch)) return;

                string searchTerm = String.Format("{0}*.exe", systemSearch);

                string enviromentPath = Environment.GetEnvironmentVariable("PATH");
                if (String.IsNullOrWhiteSpace(enviromentPath)) return;

                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                Parallel.ForEach(enviromentPath.Split(';'), parallelOptions, path =>
                {
                    if (!Directory.Exists(path)) return;
                    var files = Directory.GetFiles(path, searchTerm, SearchOption.TopDirectoryOnly);
                    Parallel.ForEach(files, parallelOptions, file =>
                    {
                        addCommandBarEntry(new CommandBarEntry(file, CommandEntryKind.Command, Path.GetFileNameWithoutExtension(file)));
                    });
                });
            }, token);
        }

        private void searchApplications(CancellationToken token)
        {
            Task.Run(() =>
            {
                var rootKey = Registry.ClassesRoot.OpenSubKey(@"Applications");
                if (rootKey == null) return;
                if (token.IsCancellationRequested) return;

                var options = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                string appSearchText = searchText.ToLower();
                Parallel.ForEach(rootKey.GetSubKeyNames(), options, keyName =>
                {
                    if (!keyName.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase)) return;
                    if (!keyName.ToLower().Contains(appSearchText)) return;

                    if (token.IsCancellationRequested) return;

                    string applicationName = keyName.Substring(0, keyName.LastIndexOf("."));
                    var shellKey = rootKey.OpenSubKey(keyName + @"\shell\open\command");
                    if (shellKey != null)
                    {
                        string shellKeyValue = shellKey.GetValue(null).ToString().TrimStart('"');
                        shellKeyValue = shellKeyValue.Substring(0, Math.Max(shellKeyValue.IndexOf('"'), 0));
                        shellKeyValue = StringHelper.GetFirstNonEmpty(shellKeyValue, applicationName);

                        addCommandBarEntry(new CommandBarEntry(shellKeyValue, CommandEntryKind.Application, applicationName));
                        return;
                    }

                    addCommandBarEntry(new CommandBarEntry(applicationName, CommandEntryKind.Application));
                });
            }, token);
        }

        private void searchFileSystem(CancellationToken token)
        {
            Task.Run(() =>
            {
                string searchPath = Environment.ExpandEnvironmentVariables(searchText + searchUserParameter);
                searchPath = Regex.Replace(searchPath, FILES_REGEX, "", RegexOptions.Compiled);
                if (String.IsNullOrWhiteSpace(searchPath)) return;

                if (!Path.IsPathRooted(searchPath)) return;
                if (!searchPath.Contains("\\")) return;

                int charIndex = searchPath.LastIndexOf("\\") + 1;
                string searchWord = searchPath.Substring(charIndex).ToLower();
                searchPath = searchPath.Substring(0, charIndex);

                try { searchPath = Path.GetFullPath(searchPath); } catch { return; }

                if (String.IsNullOrWhiteSpace(searchWord))
                {
                    var commandEntry = new CommandBarEntry(new DirectoryInfo(searchPath).FullName, CommandEntryKind.TopDirectory);
                    this.addCommandBarEntry(commandEntry);
                }

                var options = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                Parallel.ForEach(getPathContent(searchPath, true), options, dir =>
                {
                    string dirName = new DirectoryInfo(dir).Name;
                    if (dirName.ToLower().Contains(searchWord))
                    {
                        addCommandBarEntry(new CommandBarEntry(dir + "\\", CommandEntryKind.Directory));
                    }
                });

                Parallel.ForEach(getPathContent(searchPath, false), options, file =>
                {
                    string fileName = new FileInfo(file).Name;
                    if (fileName.ToLower().Contains(searchWord))
                    {
                        addCommandBarEntry(new CommandBarEntry(file, CommandEntryKind.File));
                    }
                });               
            }, token);
        }

        private IEnumerable<string> getPathContent(string searchPath, bool directories)
        {
            try
            {
                if (directories) return Directory.EnumerateDirectories(searchPath);
                return Directory.EnumerateFiles(searchPath);
            }
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { }
            catch (DirectoryNotFoundException) { }

            return new List<string>();
        }

        private void addCommandBarEntry(CommandBarEntry entry)
        {
            commandListBox.Dispatcher.BeginInvoke((Action)delegate
            {
                var oldEntry = this.commandBarEntries.FirstOrDefault(e => e.Name == entry.Name);
                if (oldEntry != null)
                {
                    if (oldEntry.Kind > entry.Kind) return;
                    this.commandBarEntries.Remove(oldEntry);
                }

                this.commandBarEntries.Add(entry);
                this.commandListBox.Items.MoveCurrentToFirst();
            });
        }
        #endregion
    }
}
