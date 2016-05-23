using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using WindowsEnhancementSuite.Properties;
using WindowsEnhancementSuite.ValueObjects;
using TextBox = System.Windows.Controls.TextBox;
using WinCursor = System.Windows.Forms.Cursor;

namespace WindowsEnhancementSuite.Services
{
    public class CommandBarService
    {
        // WPF Controls
        private readonly Window commandBarWindow;
        private readonly TextBox commandTextBox;
        private readonly ListBox commandListBox;

        // Service Variables
        private readonly ObservableCollection<CommandBarEntry> commandBoxEntries;
        private readonly List<CommandBarEntry> histories;
        private CancellationTokenSource cancellationTokenSource;
        private string searchText;
        private string searchUserParameter;

        // Additional Service Collections
        private readonly CommandBarOptions commandBarOptions;

        public CommandBarService() : this(new CommandBarOptions())
        {
        }

        public CommandBarService(CommandBarOptions options)
        {
            using (var stream = Resources.CommandBar.ToStream<MemoryStream>())
            {
                commandBarWindow = XamlReader.Load(stream) as Window;
            }

            if (commandBarWindow == null) return;
            commandBarWindow.Topmost = true;
            commandBarWindow.ShowActivated = true;
            commandBarWindow.Activated += (sender, args) => commandTextBox.SelectAll();
            
            commandBarOptions = options;
            histories = new List<CommandBarEntry>();
            cancellationTokenSource = new CancellationTokenSource();

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
                commandBoxEntries = new ObservableCollection<CommandBarEntry>();
                commandListBox.ItemsSource = commandBoxEntries;
                commandListBox.Items.SortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Descending));
                commandListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

                commandListBox.KeyDown += commandTextBoxOnKeyDown;
                commandListBox.MouseDoubleClick += commandListBoxOnMouseDoubleClick;
            }
        }

        public bool ShowCommandBar()
        {
            ElementHost.EnableModelessKeyboardInterop(commandBarWindow);
            commandBarWindow.Show();

            return true;
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
            if (keyEventArgs.Key == Key.Escape)
            {
                this.cancellationTokenSource.Cancel();
                commandBarWindow.Hide();                
                return;
            }

            if (keyEventArgs.Key == Key.Return || keyEventArgs.Key == Key.Enter)
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
            try
            {
                this.cancellationTokenSource.Cancel();
                var commandEntry = (commandListBox.Items.CurrentItem as CommandBarEntry) ??
                                   new CommandBarEntry(searchText + searchUserParameter, CommandEntryKind.History);

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

        private void executeCommand(CommandBarEntry entry, bool asAdmin)
        {
            Task.Run(() =>
            {
                if (entry.Kind == CommandEntryKind.History && histories.All(e => e.Command != entry.Command)) histories.Add(entry);
                if (entry.Kind == CommandEntryKind.Directory || entry.Kind == CommandEntryKind.Explorer) searchUserParameter = "";
                RankingHelper.IncreaseRank(entry);

                if (asAdmin)
                {
                    // Start as Admin
                    UacAssistService.RunAsAdmin(entry.Command, searchUserParameter.Trim());
                    return;
                }

                Process.Start(new ProcessStartInfo(entry.Command, searchUserParameter.Trim()));
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
            commandBoxEntries.Clear();
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
            searchLastVisited(cancelToken);
            searchSystemPath(cancelToken);
            searchFileSystem(cancelToken);
        }

        private void searchCommandHistory(CancellationToken token)
        {
            Task.Run(() =>
            {
                foreach (var entry in histories)
                {
                    if (token.IsCancellationRequested) return;

                    if (entry.Command.ToLower().Contains(searchText)) addCommandBarEntry(entry);
                }
            }, token);
        }

        private void searchLastVisited(CancellationToken token)
        {            
            Task.Run(() =>
            {
                if (commandBarOptions.ExplorerService == null) return;
                foreach (var entry in commandBarOptions.ExplorerService.ExplorerHistories)
                {
                    if (token.IsCancellationRequested) return;

                    if (entry.Command.ToLower().Contains(searchText)) addCommandBarEntry(entry);
                }
            }, token);
        }

        private void searchSystemPath(CancellationToken token)
        {
            Task.Run(() =>
            {
                string systemSearch = Regex.Replace(searchText, "[^a-zA-Z0-9 !_-]", "", RegexOptions.Compiled);
                if (String.IsNullOrWhiteSpace(systemSearch)) return;

                string searchTerm = String.Format("{0}*.exe", systemSearch);

                string enviromentPath = Environment.GetEnvironmentVariable("PATH");
                if (String.IsNullOrWhiteSpace(enviromentPath)) return;

                foreach (string path in enviromentPath.Split(';'))
                {
                    if (token.IsCancellationRequested) return;

                    if (!Directory.Exists(path)) continue;
                    string directory = path;
                    Task.Run(() =>
                    {
                        var files = Directory.GetFiles(directory, searchTerm, SearchOption.TopDirectoryOnly);
                        foreach (string file in files)
                        {
                            if (token.IsCancellationRequested) return;
                            addCommandBarEntry(new CommandBarEntry(file, CommandEntryKind.Command, Path.GetFileNameWithoutExtension(file)));
                        }
                    }, token);
                }
            }, token);
        }

        private void searchFileSystem(CancellationToken token)
        {
            Task.Run(() =>
            {
                string systemSearch = Regex.Replace(searchText, "[^a-zA-Z0-9 !_-]", "", RegexOptions.Compiled);
                if (String.IsNullOrWhiteSpace(systemSearch)) return;

                string searchTerm = String.Format("{0}*.*", systemSearch);

                foreach (string path in commandBarOptions.SystemSearchPaths)
                {
                    if (token.IsCancellationRequested) return;

                    // todo: Implement fast, ressource genlte FileSearch
                    //searchFilesRecursive(path, searchTerm, token);
                }
            }, token);
        }

        private void addCommandBarEntry(CommandBarEntry entry)
        {
            commandListBox.Dispatcher.BeginInvoke((Action)delegate
            {
                this.commandBoxEntries.Add(entry);
                this.commandListBox.Items.MoveCurrentToFirst();
            });
        }

        private void searchFilesRecursive(string path, string pattern, CancellationToken token)
        {
            Task.Run(() =>
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(path))
                    {
                        if (token.IsCancellationRequested) return;
                        searchFilesRecursive(directory, pattern, token);
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }  

                try
                {
                    foreach (string file in Directory.GetFiles(path, pattern))
                    {
                        if (token.IsCancellationRequested) return;
                        addCommandBarEntry(new CommandBarEntry(file, CommandEntryKind.File));
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }                
            }, token);
        }
        #endregion
    }
}
