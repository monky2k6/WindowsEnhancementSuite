using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Markup;
using WindowsEnhancementSuite.Enums;
using WindowsEnhancementSuite.Extensions;
using WindowsEnhancementSuite.Properties;
using WindowsEnhancementSuite.ValueObjects;
using TextBox = System.Windows.Controls.TextBox;
using WinCursor = System.Windows.Forms.Cursor;

namespace WindowsEnhancementSuite.Services
{
    public class CommandBarService
    {
        private readonly Window commandBarWindow;
        private readonly TextBox commandTextBox;
        private readonly ListBox commandListBox;

        private readonly ObservableCollection<CommandBoxEntry> commandBoxEntries;
        private CancellationTokenSource cancellationTokenSource;
        private string searchText;
        private string searchUserParameter;

        public CommandBarService()
        {
            using (var stream = Resources.CommandBar.ToStream<MemoryStream>())
            {
                commandBarWindow = XamlReader.Load(stream) as Window;
            }

            if (commandBarWindow == null) return;
            commandBarWindow.Activated += (sender, args) => commandTextBox.SelectAll();

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
                commandBoxEntries = new ObservableCollection<CommandBoxEntry>();
                commandListBox.ItemsSource = commandBoxEntries;
                commandListBox.DataContextChanged += commandListBoxOnDataContextChanged;
            }
        }        

        private void commandListBoxOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            commandListBox.Items.MoveCurrentToFirst();
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
                commandBarWindow.Hide();
                return;
            }

            if (keyEventArgs.Key == Key.Return || keyEventArgs.Key == Key.Enter)
            {
                var commandEntry = (commandListBox.Items.CurrentItem as CommandBoxEntry);
                if (commandEntry == null) return;

                executeCommand(commandEntry, keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Shift);
            }
        }

        private void commandTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            this.search();
        }

        private void executeCommand(CommandBoxEntry entry, bool asAdmin)
        {
            string command = entry.Command.ConditionalAttach(entry.Kind != CommandEntryKind.Directory, searchUserParameter);

            if (asAdmin)
            {
                // Start as Admin
                //Process.Start(commandEntry.Command);
                MessageBox.Show("Admin: " + command);
                return;
            }

            //Process.Start(commandEntry.Command);
            MessageBox.Show(command);   
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

            searchLastVisited(cancelToken);
            searchSystemPath(cancelToken);
            searchFileSystem(cancelToken);
        }

        private void searchLastVisited(CancellationToken token)
        {

        }

        private void searchSystemPath(CancellationToken token)
        {
            string searchTerm = String.Format("{0}*.exe", searchText);
            Task.Run(() =>
            {
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

                            var entry = new CommandBoxEntry(file, CommandEntryKind.File);
                            commandListBox.Dispatcher.BeginInvoke((Action)delegate
                            {
                                this.commandBoxEntries.Add(entry);
                                this.commandListBox.Items.MoveCurrentToFirst();
                            });
                        }
                    }, token);
                }
            }, token);
        }

        private void searchFileSystem(CancellationToken token)
        {
            
        }
        #endregion
    }
}
