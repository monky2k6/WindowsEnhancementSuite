using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsEnhancementSuite.Enums;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.ValueObjects;
using SHDocVw;

namespace WindowsEnhancementSuite.Services
{
    public class ExplorerBrowserService
    {
        private const short MAX_HISTORY = 20;

        private readonly ShellWindows shellWindows;
        private Queue<ExplorerHistory> historyQueue;

        public ReadOnlyCollection<CommandBoxEntry> ExplorerHistories
        {
            get
            {
                return historyQueue.Select(e => new CommandBoxEntry(e.Path, CommandEntryKind.Directory)).ToList().AsReadOnly();
            }
        }

        public ExplorerBrowserService()
        {
            this.historyQueue = new Queue<ExplorerHistory>();

            this.shellWindows = new ShellWindows();
            this.shellWindows.WindowRegistered += this.shellWindowsOnWindowRegistered;

            this.registerEvent(true);
        }

        public bool ShowExplorerHistory()
        {
            // Todo: Check if we're not in a Game/FullScreen Application
            var historyList = this.historyQueue.OrderByDescending(h => h.VisitDate).Select(h => h.Path).ToList();
            ThreadHelper.RunAsStaThread(() => Application.Run(new HistoryForm(historyList)));
            return true;
        }

        private void registerEvent(bool overwrite)
        {
            try
            {
                foreach (var explorer in this.shellWindows.Cast<InternetExplorer>().ToList())
                {
                    if (explorer.FullName == null) continue;
                    if (!Path.GetFileNameWithoutExtension(explorer.FullName).ToLower().Equals("explorer")) continue;

                    object registeredProperty = explorer.GetProperty("EventRegistered");
                    if (overwrite || registeredProperty == null || !(bool)registeredProperty)
                    {
                        explorer.PutProperty("EventRegistered", true);
                        explorer.NavigateComplete2 += this.windowsExplorerOnNavigateComplete2;
                        
                        if (String.IsNullOrWhiteSpace(explorer.LocationURL)) continue;
                        this.addExplorerPath(Utils.DecodeUrl(new Uri(explorer.LocationURL)));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void addExplorerPath(string path)
        {
            // Deduplicating
            this.historyQueue = new Queue<ExplorerHistory>(this.historyQueue.Where(h => h.Path != path));

            while (this.historyQueue.Count >= MAX_HISTORY)
            {
                this.historyQueue.Dequeue();
            }

            this.historyQueue.Enqueue(new ExplorerHistory(path));
        }

        private void shellWindowsOnWindowRegistered(int lCookie)
        {
            this.registerEvent(false);
        }

        private void windowsExplorerOnNavigateComplete2(object pDisp, ref object url)
        {
            this.addExplorerPath(url.ToString());
        }

        private sealed class ExplorerHistory
        {
            public DateTime VisitDate { get; private set; }
            public string Path { get; private set; }

            public ExplorerHistory(string path)
            {
                this.VisitDate = DateTime.Now;
                this.Path = path;
            }
        }

        private sealed class HistoryForm : Form
        {
            public HistoryForm(IEnumerable<string> historyList)
            {
                this.Size = new Size(250, 270);

                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(MousePosition.X - (this.Size.Width/2), MousePosition.Y - 5);

                this.MinimizeBox = false;

                this.FormBorderStyle = FormBorderStyle.None;
                this.ShowIcon = false;
                this.ShowInTaskbar = false;

                this.Shown += (sender, args) =>
                {
                    this.TopMost = true;
                    this.Focus();
                };

                var historyBox = new ListBox
                {
                    Parent = this,
                    Size = this.ClientSize,
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
                };

                this.GotFocus += (sender, args) => historyBox.Focus();

                historyBox.MouseLeave += (sender, args) => this.Close();
                historyBox.MouseMove += (sender, args) =>
                {
                    int hoverIndex = historyBox.IndexFromPoint(args.Location);
                    historyBox.SelectedIndex = hoverIndex;
                };

                historyBox.MouseClick += (sender, args) =>
                {
                    int selectedIndex = historyBox.IndexFromPoint(args.Location);
                    if (selectedIndex > -1)
                    {
                        string path = historyBox.Items[selectedIndex].ToString();
                        try
                        {
                            Process.Start(path);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            this.Close();
                        }
                    }
                };

                foreach (string s in historyList)
                {
                    historyBox.Items.Add(s);
                }

                this.Controls.Add(historyBox);
            }
        }
    }
}
