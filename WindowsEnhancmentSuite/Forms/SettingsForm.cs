using System;
using System.Windows.Forms;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Forms
{
    public partial class SettingsForm : Form
    {
        private int clipboardHotkey;
        private int textHotkey;
        private int showHotkey;
        private int showBrowserHistory;

        public SettingsForm()
        {
            this.InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(Settings.Default.TextApplication))
            {
                this.textBoxCustomEditor.Text = Settings.Default.TextApplication;
                this.radioButtonCustomTextEditor.Checked = true;
            }

            this.trackBarJpegQuality.Value = Settings.Default.JpegCompression > 100 ? 70 : Settings.Default.JpegCompression;
            this.radioButtonJpeg.Checked = Settings.Default.ImageSaveFormat == 1;

            this.textBoxHotkeyClipboard_KeyDown(null, new KeyEventArgs((Keys)Settings.Default.CliboardHotkey));
            this.textBoxHotkeyTextfile_KeyDown(null, new KeyEventArgs((Keys)Settings.Default.TextfileHotkey));
            this.textBoxHotkeyWatch_KeyDown(null, new KeyEventArgs((Keys)Settings.Default.ShowHotkey));
            this.textBoxHotkeyBrowserHistory_KeyDown(null, new KeyEventArgs((Keys)Settings.Default.ShowBrowserHistory));
        }

        private void radioButtonDefaultTextEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonDefaultTextEditor.Checked)
            {
                this.textBoxCustomEditor.ReadOnly = true;
            }
        }

        private void radioButtonCustomTextEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonCustomTextEditor.Checked)
            {
                this.textBoxCustomEditor.ReadOnly = false;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Einstellungen für die Text-Datei
            if (this.radioButtonCustomTextEditor.Checked && !String.IsNullOrWhiteSpace(this.textBoxCustomEditor.Text))
            {
                Settings.Default.TextApplication = this.textBoxCustomEditor.Text;
            }
            else
            {
                Settings.Default.TextApplication = String.Empty;
            }

            // Einstellungen für die Bildspeicherung
            Settings.Default.JpegCompression = Convert.ToByte(this.trackBarJpegQuality.Value);
            Settings.Default.ImageSaveFormat = this.radioButtonJpeg.Checked ? (byte)1 : (byte)0;
            Settings.Default.TextfileHotkey = this.textHotkey;
            Settings.Default.CliboardHotkey = this.clipboardHotkey;
            Settings.Default.ShowHotkey = this.showHotkey;
            Settings.Default.ShowBrowserHistory = this.showBrowserHistory;

            Settings.Default.Save();
            this.Close();
        }

        private void textBoxHotkeyTextfile_KeyDown(object sender, KeyEventArgs e)
        {
            this.textBoxHotkeyTextfile.Text = this.setHotKey(e, out this.textHotkey);
        }

        private void textBoxHotkeyClipboard_KeyDown(object sender, KeyEventArgs e)
        {
            this.textBoxHotkeyClipboard.Text = this.setHotKey(e, out this.clipboardHotkey);
        }

        private void textBoxHotkeyWatch_KeyDown(object sender, KeyEventArgs e)
        {
            this.textBoxHotkeyWatch.Text = this.setHotKey(e, out this.showHotkey);
        }

        private void textBoxHotkeyBrowserHistory_KeyDown(object sender, KeyEventArgs e)
        {
            this.textBoxHotkeyBrowserHistory.Text = this.setHotKey(e, out this.showBrowserHistory);
        }

        private string setHotKey(KeyEventArgs e, out int keyCode)
        {
            if (e.KeyCode == Keys.Escape)
            {
                keyCode = 0;
                return @"[NONE]";
            }

            var text = e.KeyCode.ToString();
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
            {
                text = "";
            }

            if (e.Control)
            {
                text = @"[CTRL] + " + text;
            }

            if (e.Shift)
            {
                text = @"[SHIFT] + " + text;
            }

            keyCode = (int)e.KeyData;
            return text;
        }
    }
}
