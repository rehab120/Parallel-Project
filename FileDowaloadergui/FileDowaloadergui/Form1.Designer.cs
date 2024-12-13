namespace FileDowaloadergui
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtUrl = new TextBox();
            btnDownaload = new Button();
            lstFiles = new ListBox();
            btnAddFile = new Button();
            btnCancelDownload = new Button();
            panelProgressBars = new Panel();
            btnReset = new Button();
            SuspendLayout();
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(102, 36);
            txtUrl.Multiline = true;
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(566, 43);
            txtUrl.TabIndex = 0;
            // 
            // btnDownaload
            // 
            btnDownaload.Location = new Point(289, 285);
            btnDownaload.Name = "btnDownaload";
            btnDownaload.Size = new Size(125, 40);
            btnDownaload.TabIndex = 1;
            btnDownaload.Text = "Downaload\r\n";
            btnDownaload.UseVisualStyleBackColor = true;
            btnDownaload.Click += btnDownaload_Click;
            // 
            // lstFiles
            // 
            lstFiles.FormattingEnabled = true;
            lstFiles.Location = new Point(232, 143);
            lstFiles.Name = "lstFiles";
            lstFiles.Size = new Size(251, 104);
            lstFiles.TabIndex = 2;
            lstFiles.SelectedIndexChanged += lstFiles_SelectedIndexChanged;
            // 
            // btnAddFile
            // 
            btnAddFile.Location = new Point(320, 95);
            btnAddFile.Name = "btnAddFile";
            btnAddFile.Size = new Size(94, 29);
            btnAddFile.TabIndex = 3;
            btnAddFile.Text = "Add";
            btnAddFile.UseVisualStyleBackColor = true;
            btnAddFile.Click += btnAddFile_Click;
            // 
            // btnCancelDownload
            // 
            btnCancelDownload.Location = new Point(300, 383);
            btnCancelDownload.Name = "btnCancelDownload";
            btnCancelDownload.Size = new Size(94, 29);
            btnCancelDownload.TabIndex = 7;
            btnCancelDownload.Text = "Cancel";
            btnCancelDownload.UseVisualStyleBackColor = true;
            btnCancelDownload.Click += btnCancelDownload_Click;
            // 
            // panelProgressBars
            // 
            panelProgressBars.AutoScroll = true;
            panelProgressBars.Location = new Point(489, 118);
            panelProgressBars.Name = "panelProgressBars";
            panelProgressBars.Size = new Size(299, 182);
            panelProgressBars.TabIndex = 8;
            panelProgressBars.Paint += panelProgressBars_Paint;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(30, 383);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(94, 29);
            btnReset.TabIndex = 9;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(783, 424);
            Controls.Add(btnReset);
            Controls.Add(panelProgressBars);
            Controls.Add(btnCancelDownload);
            Controls.Add(btnAddFile);
            Controls.Add(lstFiles);
            Controls.Add(btnDownaload);
            Controls.Add(txtUrl);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUrl;
        private Button btnDownaload;
        private ListBox lstFiles;
        private Button btnAddFile;
        private Button btnCancelDownload;
        private Panel panelProgressBars;
        private Button btnReset;
    }
}
