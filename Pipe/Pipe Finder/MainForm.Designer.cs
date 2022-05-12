namespace Pipe_Finder
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.PipesBox = new System.Windows.Forms.ListBox();
            this.PipesBoxContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.PipesBoxContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // PipesBox
            // 
            this.PipesBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PipesBox.ContextMenuStrip = this.PipesBoxContext;
            this.PipesBox.FormattingEnabled = true;
            this.PipesBox.Location = new System.Drawing.Point(12, 12);
            this.PipesBox.Name = "PipesBox";
            this.PipesBox.Size = new System.Drawing.Size(325, 390);
            this.PipesBox.TabIndex = 0;
            this.PipesBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PipesBox_MouseMove);
            // 
            // PipesBoxContext
            // 
            this.PipesBoxContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToClipboardToolStripMenuItem});
            this.PipesBoxContext.Name = "contextMenuStrip1";
            this.PipesBoxContext.Size = new System.Drawing.Size(173, 26);
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            this.copyToClipboardToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.copyToClipboardToolStripMenuItem.Text = "Copy To Clipboard";
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.CopyToClipboardButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(12, 408);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(104, 27);
            this.ClearButton.TabIndex = 1;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(123, 408);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(104, 27);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Start Listening";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Enabled = false;
            this.StopButton.Location = new System.Drawing.Point(233, 408);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(104, 27);
            this.StopButton.TabIndex = 3;
            this.StopButton.Text = "Stop Listening";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 448);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.PipesBox);
            this.Name = "MainForm";
            this.Text = "Pipe Listener";
            this.PipesBoxContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox PipesBox;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.ContextMenuStrip PipesBoxContext;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button StopButton;
    }
}

