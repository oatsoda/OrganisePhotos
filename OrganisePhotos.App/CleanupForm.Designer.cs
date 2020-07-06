﻿namespace OrganisePhotos.App
{
    partial class CleanupForm
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
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.txtIgnoreFoldersStart = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cboRenameDupeFiles = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboSetMissingDateTaken = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboSetCreatedDateFromDateTaken = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboFixDateTaken = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listLog = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblFilesFound = new System.Windows.Forms.Label();
            this.lblFileBytesFound = new System.Windows.Forms.Label();
            this.lblFoldersFound = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(338, 304);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(15, 46);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(368, 23);
            this.txtFolderPath.TabIndex = 1;
            this.txtFolderPath.Text = "\\\\192.168.98.201\\photos\\2003.00.00 Ropley Station";
            this.txtFolderPath.TextChanged += new System.EventHandler(this.txtFolderPath_TextChanged);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.txtIgnoreFoldersStart);
            this.grpSettings.Controls.Add(this.label9);
            this.grpSettings.Controls.Add(this.label8);
            this.grpSettings.Controls.Add(this.cboRenameDupeFiles);
            this.grpSettings.Controls.Add(this.label7);
            this.grpSettings.Controls.Add(this.cboSetMissingDateTaken);
            this.grpSettings.Controls.Add(this.label6);
            this.grpSettings.Controls.Add(this.cboSetCreatedDateFromDateTaken);
            this.grpSettings.Controls.Add(this.label2);
            this.grpSettings.Controls.Add(this.cboFixDateTaken);
            this.grpSettings.Controls.Add(this.label1);
            this.grpSettings.Controls.Add(this.txtFolderPath);
            this.grpSettings.Location = new System.Drawing.Point(12, 12);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(401, 286);
            this.grpSettings.TabIndex = 2;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // txtIgnoreFoldersStart
            // 
            this.txtIgnoreFoldersStart.Location = new System.Drawing.Point(15, 99);
            this.txtIgnoreFoldersStart.Name = "txtIgnoreFoldersStart";
            this.txtIgnoreFoldersStart.Size = new System.Drawing.Size(368, 23);
            this.txtIgnoreFoldersStart.TabIndex = 1;
            this.txtIgnoreFoldersStart.Text = "_, 0., 1., 2.";
            this.txtIgnoreFoldersStart.TextChanged += new System.EventHandler(this.txtIgnoreFoldersStart_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(154, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "Ignore Folders Starting With";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 240);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(191, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "Rename dupe Files with Date suffix";
            this.toolTips.SetToolTip(this.label8, "If multiple files are found with the same name, rename all with date suffix");
            // 
            // cboRenameDupeFiles
            // 
            this.cboRenameDupeFiles.FormattingEnabled = true;
            this.cboRenameDupeFiles.Location = new System.Drawing.Point(244, 237);
            this.cboRenameDupeFiles.Name = "cboRenameDupeFiles";
            this.cboRenameDupeFiles.Size = new System.Drawing.Size(94, 23);
            this.cboRenameDupeFiles.TabIndex = 4;
            this.cboRenameDupeFiles.SelectedIndexChanged += new System.EventHandler(this.cboRenameDupeFiles_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 211);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(212, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "Set missing Date Taken to Created Date";
            this.toolTips.SetToolTip(this.label7, "If Date Taken is missing set to Created Date");
            // 
            // cboSetMissingDateTaken
            // 
            this.cboSetMissingDateTaken.FormattingEnabled = true;
            this.cboSetMissingDateTaken.Location = new System.Drawing.Point(244, 208);
            this.cboSetMissingDateTaken.Name = "cboSetMissingDateTaken";
            this.cboSetMissingDateTaken.Size = new System.Drawing.Size(94, 23);
            this.cboSetMissingDateTaken.TabIndex = 4;
            this.cboSetMissingDateTaken.SelectedIndexChanged += new System.EventHandler(this.cboSetMissingDateTaken_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(183, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "Set Created Date from Date Taken";
            this.toolTips.SetToolTip(this.label6, "If Date Taken is set, update file Created Date");
            // 
            // cboSetCreatedDateFromDateTaken
            // 
            this.cboSetCreatedDateFromDateTaken.FormattingEnabled = true;
            this.cboSetCreatedDateFromDateTaken.Location = new System.Drawing.Point(244, 179);
            this.cboSetCreatedDateFromDateTaken.Name = "cboSetCreatedDateFromDateTaken";
            this.cboSetCreatedDateFromDateTaken.Size = new System.Drawing.Size(94, 23);
            this.cboSetCreatedDateFromDateTaken.TabIndex = 4;
            this.cboSetCreatedDateFromDateTaken.SelectedIndexChanged += new System.EventHandler(this.cboSetCreatedDateFromDateTaken_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 153);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Fix incorrect Date Taken";
            this.toolTips.SetToolTip(this.label2, "Detect known incorrect formats of stored Exif Date Taken values and correct to ex" +
        "pected format.");
            // 
            // cboFixDateTaken
            // 
            this.cboFixDateTaken.FormattingEnabled = true;
            this.cboFixDateTaken.Location = new System.Drawing.Point(244, 150);
            this.cboFixDateTaken.Name = "cboFixDateTaken";
            this.cboFixDateTaken.Size = new System.Drawing.Size(94, 23);
            this.cboFixDateTaken.TabIndex = 4;
            this.cboFixDateTaken.SelectedIndexChanged += new System.EventHandler(this.cboFixDateTaken_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Root Folder Path";
            // 
            // listLog
            // 
            this.listLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listLog.FormattingEnabled = true;
            this.listLog.ItemHeight = 15;
            this.listLog.Location = new System.Drawing.Point(429, 12);
            this.listLog.Name = "listLog";
            this.listLog.ScrollAlwaysVisible = true;
            this.listLog.Size = new System.Drawing.Size(534, 604);
            this.listLog.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 326);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Files Found";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 351);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "File Bytes Found";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 377);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Folders Found";
            // 
            // lblFilesFound
            // 
            this.lblFilesFound.AutoSize = true;
            this.lblFilesFound.Location = new System.Drawing.Point(123, 326);
            this.lblFilesFound.Name = "lblFilesFound";
            this.lblFilesFound.Size = new System.Drawing.Size(12, 15);
            this.lblFilesFound.TabIndex = 4;
            this.lblFilesFound.Text = "-";
            // 
            // lblFileBytesFound
            // 
            this.lblFileBytesFound.AutoSize = true;
            this.lblFileBytesFound.Location = new System.Drawing.Point(123, 351);
            this.lblFileBytesFound.Name = "lblFileBytesFound";
            this.lblFileBytesFound.Size = new System.Drawing.Size(12, 15);
            this.lblFileBytesFound.TabIndex = 4;
            this.lblFileBytesFound.Text = "-";
            // 
            // lblFoldersFound
            // 
            this.lblFoldersFound.AutoSize = true;
            this.lblFoldersFound.Location = new System.Drawing.Point(123, 377);
            this.lblFoldersFound.Name = "lblFoldersFound";
            this.lblFoldersFound.Size = new System.Drawing.Size(12, 15);
            this.lblFoldersFound.TabIndex = 4;
            this.lblFoldersFound.Text = "-";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(256, 304);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CleanupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 628);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblFoldersFound);
            this.Controls.Add(this.lblFileBytesFound);
            this.Controls.Add(this.lblFilesFound);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listLog);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.btnStart);
            this.Name = "CleanupForm";
            this.Text = "Cleanup Photos Folder";
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listLog;
        private System.Windows.Forms.ComboBox cboFixDateTaken;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFilesFound;
        private System.Windows.Forms.Label lblFileBytesFound;
        private System.Windows.Forms.Label lblFoldersFound;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboSetMissingDateTaken;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboSetCreatedDateFromDateTaken;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cboRenameDupeFiles;
        private System.Windows.Forms.TextBox txtIgnoreFoldersStart;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnCancel;
    }
}

