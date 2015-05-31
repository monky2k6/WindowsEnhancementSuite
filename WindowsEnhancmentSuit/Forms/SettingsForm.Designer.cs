namespace WindowsEnhancementSuit.Forms
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxCustomEditor = new System.Windows.Forms.TextBox();
            this.radioButtonCustomTextEditor = new System.Windows.Forms.RadioButton();
            this.radioButtonDefaultTextEditor = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelQuality = new System.Windows.Forms.Label();
            this.trackBarJpegQuality = new System.Windows.Forms.TrackBar();
            this.radioButtonJpeg = new System.Windows.Forms.RadioButton();
            this.radioButtonPng = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxHotkeyWatch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxHotkeyTextfile = new System.Windows.Forms.TextBox();
            this.textBoxHotkeyClipboard = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxHotkeyBrowserHistory = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarJpegQuality)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxCustomEditor);
            this.groupBox1.Controls.Add(this.radioButtonCustomTextEditor);
            this.groupBox1.Controls.Add(this.radioButtonDefaultTextEditor);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 73);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text Editor";
            // 
            // textBoxCustomEditor
            // 
            this.textBoxCustomEditor.Location = new System.Drawing.Point(105, 41);
            this.textBoxCustomEditor.Name = "textBoxCustomEditor";
            this.textBoxCustomEditor.ReadOnly = true;
            this.textBoxCustomEditor.Size = new System.Drawing.Size(162, 20);
            this.textBoxCustomEditor.TabIndex = 2;
            // 
            // radioButtonCustomTextEditor
            // 
            this.radioButtonCustomTextEditor.AutoSize = true;
            this.radioButtonCustomTextEditor.Location = new System.Drawing.Point(6, 42);
            this.radioButtonCustomTextEditor.Name = "radioButtonCustomTextEditor";
            this.radioButtonCustomTextEditor.Size = new System.Drawing.Size(93, 17);
            this.radioButtonCustomTextEditor.TabIndex = 1;
            this.radioButtonCustomTextEditor.Text = "Custom Editor:";
            this.radioButtonCustomTextEditor.UseVisualStyleBackColor = true;
            this.radioButtonCustomTextEditor.CheckedChanged += new System.EventHandler(this.radioButtonCustomTextEditor_CheckedChanged);
            // 
            // radioButtonDefaultTextEditor
            // 
            this.radioButtonDefaultTextEditor.AutoSize = true;
            this.radioButtonDefaultTextEditor.Checked = true;
            this.radioButtonDefaultTextEditor.Location = new System.Drawing.Point(6, 19);
            this.radioButtonDefaultTextEditor.Name = "radioButtonDefaultTextEditor";
            this.radioButtonDefaultTextEditor.Size = new System.Drawing.Size(113, 17);
            this.radioButtonDefaultTextEditor.TabIndex = 0;
            this.radioButtonDefaultTextEditor.TabStop = true;
            this.radioButtonDefaultTextEditor.Text = "Default Text Editor";
            this.radioButtonDefaultTextEditor.UseVisualStyleBackColor = true;
            this.radioButtonDefaultTextEditor.CheckedChanged += new System.EventHandler(this.radioButtonDefaultTextEditor_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelQuality);
            this.groupBox2.Controls.Add(this.trackBarJpegQuality);
            this.groupBox2.Controls.Add(this.radioButtonJpeg);
            this.groupBox2.Controls.Add(this.radioButtonPng);
            this.groupBox2.Location = new System.Drawing.Point(12, 91);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 85);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image Settings";
            // 
            // labelQuality
            // 
            this.labelQuality.AutoSize = true;
            this.labelQuality.Location = new System.Drawing.Point(166, 19);
            this.labelQuality.Name = "labelQuality";
            this.labelQuality.Size = new System.Drawing.Size(39, 13);
            this.labelQuality.TabIndex = 3;
            this.labelQuality.Text = "Quality";
            // 
            // trackBarJpegQuality
            // 
            this.trackBarJpegQuality.Location = new System.Drawing.Point(105, 35);
            this.trackBarJpegQuality.Maximum = 100;
            this.trackBarJpegQuality.Name = "trackBarJpegQuality";
            this.trackBarJpegQuality.Size = new System.Drawing.Size(162, 45);
            this.trackBarJpegQuality.TabIndex = 2;
            this.trackBarJpegQuality.TickFrequency = 10;
            this.trackBarJpegQuality.Value = 80;
            // 
            // radioButtonJpeg
            // 
            this.radioButtonJpeg.AutoSize = true;
            this.radioButtonJpeg.Location = new System.Drawing.Point(6, 42);
            this.radioButtonJpeg.Name = "radioButtonJpeg";
            this.radioButtonJpeg.Size = new System.Drawing.Size(52, 17);
            this.radioButtonJpeg.TabIndex = 1;
            this.radioButtonJpeg.Text = "JPEG";
            this.radioButtonJpeg.UseVisualStyleBackColor = true;
            // 
            // radioButtonPng
            // 
            this.radioButtonPng.AutoSize = true;
            this.radioButtonPng.Checked = true;
            this.radioButtonPng.Location = new System.Drawing.Point(6, 19);
            this.radioButtonPng.Name = "radioButtonPng";
            this.radioButtonPng.Size = new System.Drawing.Size(48, 17);
            this.radioButtonPng.TabIndex = 0;
            this.radioButtonPng.TabStop = true;
            this.radioButtonPng.Text = "PNG";
            this.radioButtonPng.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(210, 306);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBoxHotkeyBrowserHistory);
            this.groupBox3.Controls.Add(this.textBoxHotkeyWatch);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxHotkeyTextfile);
            this.groupBox3.Controls.Add(this.textBoxHotkeyClipboard);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(12, 182);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(273, 118);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Hotkeys";
            // 
            // textBoxHotkeyWatch
            // 
            this.textBoxHotkeyWatch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxHotkeyWatch.Location = new System.Drawing.Point(132, 39);
            this.textBoxHotkeyWatch.MaxLength = 50;
            this.textBoxHotkeyWatch.Name = "textBoxHotkeyWatch";
            this.textBoxHotkeyWatch.ReadOnly = true;
            this.textBoxHotkeyWatch.Size = new System.Drawing.Size(135, 20);
            this.textBoxHotkeyWatch.TabIndex = 5;
            this.textBoxHotkeyWatch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxHotkeyWatch_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Show Clipboard:";
            // 
            // textBoxHotkeyTextfile
            // 
            this.textBoxHotkeyTextfile.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxHotkeyTextfile.Location = new System.Drawing.Point(132, 65);
            this.textBoxHotkeyTextfile.MaxLength = 50;
            this.textBoxHotkeyTextfile.Name = "textBoxHotkeyTextfile";
            this.textBoxHotkeyTextfile.ReadOnly = true;
            this.textBoxHotkeyTextfile.Size = new System.Drawing.Size(135, 20);
            this.textBoxHotkeyTextfile.TabIndex = 3;
            this.textBoxHotkeyTextfile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxHotkeyTextfile_KeyDown);
            // 
            // textBoxHotkeyClipboard
            // 
            this.textBoxHotkeyClipboard.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxHotkeyClipboard.Location = new System.Drawing.Point(132, 13);
            this.textBoxHotkeyClipboard.MaxLength = 50;
            this.textBoxHotkeyClipboard.Name = "textBoxHotkeyClipboard";
            this.textBoxHotkeyClipboard.ReadOnly = true;
            this.textBoxHotkeyClipboard.Size = new System.Drawing.Size(135, 20);
            this.textBoxHotkeyClipboard.TabIndex = 2;
            this.textBoxHotkeyClipboard.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxHotkeyClipboard_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "New Textfile:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Clipboard to File:";
            // 
            // textBoxHotkeyBrowserHistory
            // 
            this.textBoxHotkeyBrowserHistory.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxHotkeyBrowserHistory.Location = new System.Drawing.Point(132, 91);
            this.textBoxHotkeyBrowserHistory.MaxLength = 50;
            this.textBoxHotkeyBrowserHistory.Name = "textBoxHotkeyBrowserHistory";
            this.textBoxHotkeyBrowserHistory.ReadOnly = true;
            this.textBoxHotkeyBrowserHistory.Size = new System.Drawing.Size(135, 20);
            this.textBoxHotkeyBrowserHistory.TabIndex = 6;
            this.textBoxHotkeyBrowserHistory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxHotkeyBrowserHistory_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Show Browserhistory:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 341);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarJpegQuality)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonCustomTextEditor;
        private System.Windows.Forms.RadioButton radioButtonDefaultTextEditor;
        private System.Windows.Forms.TextBox textBoxCustomEditor;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RadioButton radioButtonPng;
        private System.Windows.Forms.RadioButton radioButtonJpeg;
        private System.Windows.Forms.TrackBar trackBarJpegQuality;
        private System.Windows.Forms.Label labelQuality;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxHotkeyTextfile;
        private System.Windows.Forms.TextBox textBoxHotkeyClipboard;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxHotkeyWatch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxHotkeyBrowserHistory;
    }
}