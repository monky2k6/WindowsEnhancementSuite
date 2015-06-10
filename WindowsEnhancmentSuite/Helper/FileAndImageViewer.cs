﻿using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsEnhancementSuite.Extensions;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Helper
{
    public class FileAndImageViewer
    {
        public void ShowClipboardContent()
        {
            if (Clipboard.ContainsText())
            {
                this.showText();
                return;
            }

            if (Clipboard.ContainsImage())
            {
                this.showImage();
                return;
            }

            if (Clipboard.ContainsFileDropList())
            {
                this.showFileDropList();
                return;
            }
        }

        private void showText()
        {
            Application.Run(new WatchForm(Clipboard.GetText()));
        }

        private void showImage()
        {
            var imageData = Clipboard.GetImage();
            if (imageData == null) return;
            Application.Run(new WatchForm(imageData));
        }

        private void showFileDropList()
        {
            var fileList = Clipboard.GetFileDropList();
            Application.Run(new WatchForm(fileList));
        }

        private sealed class WatchForm : Form
        {
            private const string NODE_TYPE_EXECUTABLE = "Executable";
            private const string NODE_TYPE_FOLDER = "Folder";
            private const string NODE_TYPE_OTHER = "Other";

            private void init()
            {
                var formSize = new Size(450, 350);

                this.Text = "";

                this.ClientSize = SystemInformation.VirtualScreen.Size;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(MousePosition.X - (formSize.Width / 2), MousePosition.Y - 20);

                this.MinimizeBox = true;
                this.MinimumSize = new Size(300, 250);
                this.MaximumSize = SystemInformation.VirtualScreen.Size;

                this.Icon = Resources.WES;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.ShowIcon = true;
                this.ShowInTaskbar = true;

                this.Size = formSize;

                this.Shown += (sender, args) =>
                {
                    this.BringToFront();
                    this.TopMost = true;
                };

                this.Disposed += (sender, args) => GC.Collect();
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

                var viewRichTextBox = new RichTextBox
                {
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Size = new Size(this.ClientSize.Width, this.ClientSize.Height - statusStrip.Height),
                    ReadOnly = true,
                    Visible = true,
                    Text = text,
                    DetectUrls = true
                };

                viewRichTextBox.LinkClicked += (sender, args) =>
                {
                    try
                    {
                        Process.Start(args.LinkText);
                    }
                    catch (Exception)
                    {
                    }
                };

                viewRichTextBox.SelectionChanged += (sender, args) =>
                {
                    string selectedText = viewRichTextBox.SelectedText;
                    charCountItem.Text = "Selected Chars: " + selectedText.Count(s => s != '\r').ToString().PadLeft(6);
                    wordCountItem.Text = "Selected Words: " + selectedText.Split(' ', '\n').Select(s => s.Trim()).Count(s => !String.IsNullOrWhiteSpace(s))
                    .ToString().PadLeft(6);
                };

                this.Controls.Add(statusStrip);
                this.Controls.Add(viewRichTextBox);

                this.AttachToolBar(() => Clipboard.SetText(text));
            }

            public WatchForm(Image image)
            {
                this.init();
                this.Text = "Image";

                var picturePanel = new Panel
                {
                    AutoScroll = true,
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Size = this.ClientSize,
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
                    Tag = image,
                    Image = image
                };

                this.MouseWheel += (sender, e) =>
                {
                    var originalImage = (Image)viewPictureBox.Tag;

                    int changeX = viewPictureBox.Image.Width * e.Delta / 1000;
                    int changeY = viewPictureBox.Image.Height * e.Delta / 1000;

                    var newSize = new Size(viewPictureBox.Image.Width + changeX, viewPictureBox.Image.Height + changeY);

                    if (newSize.Width < 20 || newSize.Height < 20) return;
                    if (newSize.Width > 8000 || newSize.Height > 8000) return;

                    var newImage = new Bitmap(originalImage, newSize);
                    viewPictureBox.Image = newImage;
                };

                picturePanel.Controls.Add(viewPictureBox);

                this.Controls.Add(picturePanel);

                // Bild soweit wie möglich maximieren
                var screenSize = this.MaximumSize;
                int x = image.Width > screenSize.Width ? screenSize.Width : image.Width;
                int y = image.Height > screenSize.Height ? screenSize.Height : image.Height;
                this.ClientSize = new Size(x, y);

                // Bild mittig auf dem Cursor positionieren
                x = Math.Min(Math.Max(MousePosition.X - (x / 2), 0), screenSize.Width - this.Size.Width);
                y = Math.Min(Math.Max(MousePosition.Y - (y / 2), 0), screenSize.Height - this.Size.Height);
                this.Location = new Point(x, y);

                this.AttachToolBar(() => Clipboard.SetImage(image));
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

                this.AttachToolBar(() => Clipboard.SetFileDropList(fileList));
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

                var fileEnding = Path.GetExtension(node.Name).ToLower();

                // Executables
                if (fileEnding.Contains(".exe"))
                {
                    string nodeName = node.Name.ToLower();
                    if (!imageList.Images.ContainsKey(nodeName))
                    {
                        var nodeIcon = Icon.ExtractAssociatedIcon(node.Name);

                        if (nodeIcon == null)
                        {
                            node.ImageKey = NODE_TYPE_EXECUTABLE;
                            node.SelectedImageKey = NODE_TYPE_EXECUTABLE;

                            return;
                        }

                        imageList.Images.Add(nodeName, nodeIcon);
                    }

                    node.ImageKey = nodeName;
                    node.SelectedImageKey = nodeName;

                    return;
                }

                // Icons & Links
                if (fileEnding.Contains(".ico") || fileEnding.Contains(".lnk"))
                {
                    string nodeName = node.Name.ToLower();
                    if (!imageList.Images.ContainsKey(nodeName))
                    {
                        var nodeIcon = Icon.ExtractAssociatedIcon(node.Name);

                        if (nodeIcon != null)
                        {
                            imageList.Images.Add(nodeName, nodeIcon);

                            node.ImageKey = nodeName;
                            node.SelectedImageKey = nodeName;

                            return;
                        }
                    }
                }

                // Other
                if (!imageList.Images.ContainsKey(fileEnding))
                {
                    var nodeIcon = Icon.ExtractAssociatedIcon(node.Name);

                    if (nodeIcon == null)
                    {
                        node.ImageKey = NODE_TYPE_OTHER;
                        node.SelectedImageKey = NODE_TYPE_OTHER;

                        return;
                    }

                    imageList.Images.Add(fileEnding, nodeIcon);
                }

                node.ImageKey = fileEnding;
                node.SelectedImageKey = fileEnding;
            }
        }
    }
}