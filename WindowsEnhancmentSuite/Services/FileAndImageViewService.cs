using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsEnhancementSuite.Bases;
using WindowsEnhancementSuite.Extensions;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.Helper.Windows;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Services
{
    public class FileAndImageViewService
    {
        public bool ShowClipboardContent()
        {
            if (this.showText()) return true;
            if (this.showImage()) return true;
            if (this.showFileDropList()) return true;

            return false;
        }

        private bool showText()
        {
            if (!Clipboard.ContainsText()) return false;
            string clipboard = Clipboard.GetText();            
            var form = new WatchForm(clipboard);
            form.Show();

            return true;
        }

        private bool showImage()
        {
            if (!Clipboard.ContainsImage()) return false;
            var imageData = Clipboard.GetImage();
            if (imageData == null) return false;
            var form = new WatchForm(imageData);
            form.Show();

            return true;
        }

        private bool showFileDropList()
        {
            if (!Clipboard.ContainsFileDropList()) return false;
            var fileList = Clipboard.GetFileDropList();
            var form = new WatchForm(fileList);
            form.Show();

            return true;
        }

        private sealed class WatchForm : DraggableBaseForm
        {
            private const string NODE_TYPE_EXECUTABLE = "Executable";
            private const string NODE_TYPE_FOLDER = "Folder";
            private const string NODE_TYPE_OTHER = "Other";

            private const int MENU_CLIPBOARD = 0x1;
            private const int MENU_TOPMOST = 0x2;
            private const int MENU_LOCK = 0x3;

            private IntPtr menuHandle;
            private readonly Action clipboardAction;
            private readonly Control lockControl;

            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);

                // Expand SystemMenu 
                menuHandle = WindowsMethods.GetFormMenuHandle(this.Handle);
                WindowsMethods.AddFormMenuSeparator(menuHandle);
                WindowsMethods.AddFormMenuItem(menuHandle, MENU_CLIPBOARD, "Copy to &Clipboard");
                WindowsMethods.AddFormMenuSeparator(menuHandle);
                WindowsMethods.AddFormMenuItem(menuHandle, MENU_TOPMOST, "TopMost");
                WindowsMethods.AddFormMenuItem(menuHandle, MENU_LOCK, "Lock");
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                #region Handle SystemMenu
                if (WindowsMethods.CheckMenuEvent(m, MENU_CLIPBOARD))
                {
                    if (clipboardAction != null) clipboardAction.Invoke();
                    return;
                }

                if (WindowsMethods.CheckMenuEvent(m, MENU_TOPMOST))
                {
                    this.TopMost = !this.TopMost;
                    WindowsMethods.SetFormMenuCheckBox(menuHandle, MENU_TOPMOST, this.TopMost);
                    return;
                }

                if (WindowsMethods.CheckMenuEvent(m, MENU_LOCK))
                {
                    if (this.lockControl != null)
                    {
                        this.lockControl.Enabled = !this.lockControl.Enabled;
                        WindowsMethods.SetFormMenuCheckBox(menuHandle, MENU_LOCK, !this.lockControl.Enabled);
                        return;
                    }
                }
                #endregion
            }

            private void init()
            {
                var formSize = new Size(450, 350);

                this.Text = "";

                this.StartPosition = FormStartPosition.Manual;
                this.Location = getFormLocation(formSize, SystemInformation.VirtualScreen.Size);

                this.MinimizeBox = true;
                this.MinimumSize = new Size(180, 100);
                this.MaximumSize = SystemInformation.VirtualScreen.Size;

                this.Icon = Resources.WES;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.ShowIcon = true;
                this.ShowInTaskbar = true;

                this.Size = formSize;

                this.KeyDown += onKeyDown;
                this.KeyPreview = true;

                this.Shown += (sender, args) =>
                {
                    this.TopMost = true;
                    this.BringToFront();
                    this.TopMost = false;
                };
                this.Disposed += (sender, args) => GC.Collect();
            }

            private void onKeyDown(object sender, KeyEventArgs keyEventArgs)
            {
                if (keyEventArgs.Handled) return;
                if (keyEventArgs.Alt && keyEventArgs.KeyCode == Keys.C)
                {
                    keyEventArgs.Handled = true;
                    if (clipboardAction != null) clipboardAction.Invoke();
                }
            }

            public WatchForm(string text)
            {
                this.init();
                this.Text = "Text";

                var statusStrip = new StatusStrip
                {
                    Parent = this,
                    Stretch = true,
                    Height = 22,
                    Dock = DockStyle.Bottom,
                    Visible = true
                };

                var charCountItem = statusStrip.Items.Add("Selected Chars: ");
                charCountItem.TextAlign = ContentAlignment.MiddleLeft;
                var wordCountItem = statusStrip.Items.Add("Selected Words: ");
                wordCountItem.TextAlign = ContentAlignment.MiddleLeft;

                var allCharItem = statusStrip.Items.Add("All Chars: " + text.Count(s => s != '\r').ToString().PadLeft(6));
                allCharItem.TextAlign = ContentAlignment.MiddleLeft;
                var allWordItem = statusStrip.Items.Add("All Words: "+ text.Split(' ', '\n').Select(s => s.Trim()).Count(s => !String.IsNullOrWhiteSpace(s))
                    .ToString().PadLeft(6));
                allWordItem.TextAlign = ContentAlignment.MiddleLeft;

                var codeHighlighter = Factories.CodeHighlighter.Create(text, this,
                    this.ClientSize.Adjust(0, -statusStrip.Height));

                this.Controls.Add(statusStrip);

                var setCurrentText = new Action(() =>
                {
                    var invokeAction = new Action(() =>
                    {
                        string currentText = codeHighlighter.Text;
                        ThreadHelper.RunAsStaThread(() =>
                        {
                            Clipboard.SetText(currentText);
                        });
                    });

                    if (codeHighlighter.InvokeRequired)
                    {
                        codeHighlighter.Invoke(invokeAction);
                        return;
                    }

                    invokeAction();
                });

                var scintillaStyle = codeHighlighter.Styles.First();
                using (var font = new Font(scintillaStyle.Font, scintillaStyle.SizeF))
                {
                    var textSize = TextRenderer.MeasureText(text, font);
                    var screenSize = this.MaximumSize;
                    int x = Math.Min(textSize.Width, screenSize.Width);
                    int y = Math.Min(textSize.Height, screenSize.Height) + statusStrip.Height;
                    this.ClientSize = new Size(x, y);
                }

                this.Location = getFormLocation(this.ClientSize, this.MaximumSize);

                this.clipboardAction = setCurrentText;
                this.lockControl = codeHighlighter;
            }

            public WatchForm(Image image)
            {
                this.init();
                this.Text = "Image";

                var originalImage = image;

                var picturePanel = new Panel
                {
                    AutoScroll = true,
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Size = this.ClientSize,
                    Parent = this,
                    Visible = true
                };

                var viewPictureBox = new PictureBox
                {
                    Parent = picturePanel,
                    Location = new Point(0, 0),
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Size = this.ClientSize,
                    Visible = true,
                    Image = image
                };

                picturePanel.MouseWheel += (sender, e) =>
                {
                    if (!picturePanel.Enabled) return;

                    int changeX = viewPictureBox.Image.Width * e.Delta / 1000;
                    int changeY = viewPictureBox.Image.Height * e.Delta / 1000;

                    var newSize = new Size(viewPictureBox.Image.Width + changeX, viewPictureBox.Image.Height + changeY);

                    if (newSize.Width < 20 || newSize.Height < 20) return;
                    if (newSize.Width > 8000 || newSize.Height > 8000) return;

                    var newImage = new Bitmap(originalImage, newSize);
                    viewPictureBox.Image = newImage;
                };

                var restorePicture = new MouseEventHandler((sender, args) =>
                {
                    viewPictureBox.Image = originalImage;
                });

                viewPictureBox.MouseDoubleClick += restorePicture;
                picturePanel.MouseDoubleClick += restorePicture;

                picturePanel.Controls.Add(viewPictureBox);

                this.Controls.Add(picturePanel);

                // Bild soweit wie möglich maximieren
                var screenSize = this.MaximumSize;
                int x = Math.Min(image.Width, screenSize.Width);
                int y = Math.Min(image.Height, screenSize.Height);
                this.ClientSize = new Size(x, y);

                this.Location = getFormLocation(this.ClientSize, screenSize);

                this.clipboardAction = () => Clipboard.SetImage(image);
                this.lockControl = picturePanel;
            }

            public WatchForm(StringCollection fileList)
            {
                this.init();
                this.Text = "FileList";

                var imageList = new ImageList
                {
                    ColorDepth = ColorDepth.Depth32Bit,
                    ImageSize = new Size(24, 24)
                };
                imageList.Images.Add(NODE_TYPE_EXECUTABLE, Resources.executable);
                imageList.Images.Add(NODE_TYPE_FOLDER, Resources.folder);
                imageList.Images.Add(NODE_TYPE_OTHER, Resources.other);

                var treeView = new TreeView
                {
                    Parent = this,
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Size = this.ClientSize,
                    ImageList = imageList,
                    Font = new Font("Arial", 11),
                    Visible = true
                };

                treeView.KeyDown += (sender, args) =>
                {
                    if (args.KeyData == Keys.F5)
                    {
                        var currentNode = treeView.SelectedNode;
                        if (currentNode == null) return;

                        if (!File.Exists(currentNode.Name) && !Directory.Exists(currentNode.Name))
                        {
                            currentNode.Remove();
                            return;
                        }

                        this.setNode(currentNode, imageList);

                        if (File.GetAttributes(currentNode.Name).HasFlag(FileAttributes.Directory))
                        {
                            currentNode.Nodes.Cast<TreeNode>().ToList().ForEach(node => node.Remove());

                            var subFolderList = Directory.GetDirectories(currentNode.Name, "*", SearchOption.TopDirectoryOnly);
                            var subFileList = Directory.GetFiles(currentNode.Name, "*", SearchOption.TopDirectoryOnly);

                            foreach (var file in subFolderList.Concat(subFileList))
                            {
                                this.setNode(currentNode.Nodes.Add(file, Path.GetFileName(file)), imageList);
                            }
                        }

                        currentNode.Tag = true;
                    }
                };

                treeView.ItemDrag += (sender, args) =>
                {
                    var dragNodeItem = (TreeNode) args.Item;

                    var dragDataItem = new DataObject(DataFormats.FileDrop, new[] {dragNodeItem.Name});
                    treeView.DoDragDrop(dragDataItem, DragDropEffects.Copy);
                };

                treeView.AfterSelect += (sender, args) =>
                {
                    if ((bool)args.Node.Tag) return;

                    string fileItem = args.Node.Name;
                    if (!File.Exists(fileItem) && !Directory.Exists(fileItem)) return;

                    if (File.GetAttributes(fileItem).HasFlag(FileAttributes.Directory))
                    {
                        var subFolderList = Directory.GetDirectories(fileItem, "*", SearchOption.TopDirectoryOnly);
                        var subFileList = Directory.GetFiles(fileItem, "*", SearchOption.TopDirectoryOnly);

                        foreach (var file in subFolderList.Concat(subFileList))
                        {
                            this.setNode(args.Node.Nodes.Add(file, Path.GetFileName(file)), imageList);
                        }
                    }

                    args.Node.Tag = true;

                    GC.Collect();
                };

                treeView.NodeMouseDoubleClick += (sender, args) => Process.Start(args.Node.Name);

                foreach (var fileItem in fileList)
                {
                    if (!File.Exists(fileItem) && !Directory.Exists(fileItem)) continue;
                    this.setNode(treeView.Nodes.Add(fileItem, Path.GetFileName(fileItem)), imageList);
                }

                this.clipboardAction = () => Clipboard.SetFileDropList(fileList);
                this.lockControl = treeView;
            }

            private Point getFormLocation(Size formSize, Size formMaxSize)
            {
                // Form im Workspace anzeigen
                int x = Math.Min(Math.Max(MousePosition.X - (formSize.Width / 2), 0), formMaxSize.Width - formSize.Width);
                int y = Math.Min(Math.Max(MousePosition.Y - (formSize.Height / 2), 0), formMaxSize.Height - formSize.Height);
                return new Point(x, y);
            }

            private void setNode(TreeNode node, ImageList imageList)
            {
                node.Tag = false;                

                // Folder
                if (File.GetAttributes(node.Name).HasFlag(FileAttributes.Directory))
                {
                    node.ImageKey = NODE_TYPE_FOLDER;
                    node.SelectedImageKey = NODE_TYPE_FOLDER;

                    return;
                }

                // FileIcon
                string nodeName = node.Name.ToLower();
                if (imageList.Images.ContainsKey(nodeName))
                {
                    node.ImageKey = nodeName;
                    node.SelectedImageKey = nodeName;
                    
                    return;
                }

                var nodeIcon = Icon.ExtractAssociatedIcon(node.Name);
                if (nodeIcon != null)
                {
                    imageList.Images.Add(nodeName, nodeIcon);
                    node.ImageKey = nodeName;
                    node.SelectedImageKey = nodeName;

                    return;
                }                

                // Executables
                string fileEnding = Path.GetExtension(node.Name).ToLower();
                if (fileEnding.Contains(".exe"))
                {
                    node.ImageKey = NODE_TYPE_EXECUTABLE;
                    node.SelectedImageKey = NODE_TYPE_EXECUTABLE;

                    return;
                }

                // Other
                node.ImageKey = NODE_TYPE_OTHER;
                node.SelectedImageKey = NODE_TYPE_OTHER;
            }
        }
    }
}
