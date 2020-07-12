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
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.txtIgnoreFoldersStart = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cboRenameDupeFiles = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboSetMissingDateTaken = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboSetCreatedDateFromDateTaken = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboFixDateTaken = new System.Windows.Forms.ComboBox();
            this.listLog = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblFilesProcessed = new System.Windows.Forms.Label();
            this.lblFileBytesProcessed = new System.Windows.Forms.Label();
            this.lblFoldersProcessed = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpLoadedTotals = new System.Windows.Forms.GroupBox();
            this.lblFilesFound = new System.Windows.Forms.Label();
            this.lblFileBytesFound = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblFoldersFound = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.treeFolders = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.grpAutoCleanupSettings = new System.Windows.Forms.GroupBox();
            this.grpAutoCleanupProgress = new System.Windows.Forms.GroupBox();
            this.grpSource.SuspendLayout();
            this.grpLoadedTotals.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.grpAutoCleanupSettings.SuspendLayout();
            this.grpAutoCleanupProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(338, 165);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnStart_Click);
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
            // grpSource
            // 
            this.grpSource.Controls.Add(this.txtIgnoreFoldersStart);
            this.grpSource.Controls.Add(this.label9);
            this.grpSource.Controls.Add(this.label1);
            this.grpSource.Controls.Add(this.txtFolderPath);
            this.grpSource.Location = new System.Drawing.Point(12, 12);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(401, 147);
            this.grpSource.TabIndex = 2;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Settings";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Root Folder Path";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(191, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "Rename dupe Files with Date suffix";
            this.toolTips.SetToolTip(this.label8, "If multiple files are found with the same name, rename all with date suffix");
            // 
            // cboRenameDupeFiles
            // 
            this.cboRenameDupeFiles.FormattingEnabled = true;
            this.cboRenameDupeFiles.Location = new System.Drawing.Point(255, 121);
            this.cboRenameDupeFiles.Name = "cboRenameDupeFiles";
            this.cboRenameDupeFiles.Size = new System.Drawing.Size(94, 23);
            this.cboRenameDupeFiles.TabIndex = 4;
            this.cboRenameDupeFiles.SelectedIndexChanged += new System.EventHandler(this.cboRenameDupeFiles_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(212, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "Set missing Date Taken to Created Date";
            this.toolTips.SetToolTip(this.label7, "If Date Taken is missing set to Created Date");
            // 
            // cboSetMissingDateTaken
            // 
            this.cboSetMissingDateTaken.FormattingEnabled = true;
            this.cboSetMissingDateTaken.Location = new System.Drawing.Point(255, 92);
            this.cboSetMissingDateTaken.Name = "cboSetMissingDateTaken";
            this.cboSetMissingDateTaken.Size = new System.Drawing.Size(94, 23);
            this.cboSetMissingDateTaken.TabIndex = 4;
            this.cboSetMissingDateTaken.SelectedIndexChanged += new System.EventHandler(this.cboSetMissingDateTaken_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(183, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "Set Created Date from Date Taken";
            this.toolTips.SetToolTip(this.label6, "If Date Taken is set, update file Created Date");
            // 
            // cboSetCreatedDateFromDateTaken
            // 
            this.cboSetCreatedDateFromDateTaken.FormattingEnabled = true;
            this.cboSetCreatedDateFromDateTaken.Location = new System.Drawing.Point(255, 63);
            this.cboSetCreatedDateFromDateTaken.Name = "cboSetCreatedDateFromDateTaken";
            this.cboSetCreatedDateFromDateTaken.Size = new System.Drawing.Size(94, 23);
            this.cboSetCreatedDateFromDateTaken.TabIndex = 4;
            this.cboSetCreatedDateFromDateTaken.SelectedIndexChanged += new System.EventHandler(this.cboSetCreatedDateFromDateTaken_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 37);
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
            this.cboFixDateTaken.Location = new System.Drawing.Point(255, 34);
            this.cboFixDateTaken.Name = "cboFixDateTaken";
            this.cboFixDateTaken.Size = new System.Drawing.Size(94, 23);
            this.cboFixDateTaken.TabIndex = 4;
            this.cboFixDateTaken.SelectedIndexChanged += new System.EventHandler(this.cboFixDateTaken_SelectedIndexChanged);
            // 
            // listLog
            // 
            this.listLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listLog.FormattingEnabled = true;
            this.listLog.HorizontalScrollbar = true;
            this.listLog.ItemHeight = 15;
            this.listLog.Location = new System.Drawing.Point(12, 612);
            this.listLog.Name = "listLog";
            this.listLog.ScrollAlwaysVisible = true;
            this.listLog.Size = new System.Drawing.Size(1108, 259);
            this.listLog.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(83, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Files Processed";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "File Bytes Processed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(68, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Folders Processed";
            // 
            // lblFilesProcessed
            // 
            this.lblFilesProcessed.AutoSize = true;
            this.lblFilesProcessed.Location = new System.Drawing.Point(187, 28);
            this.lblFilesProcessed.Name = "lblFilesProcessed";
            this.lblFilesProcessed.Size = new System.Drawing.Size(12, 15);
            this.lblFilesProcessed.TabIndex = 4;
            this.lblFilesProcessed.Text = "-";
            // 
            // lblFileBytesProcessed
            // 
            this.lblFileBytesProcessed.AutoSize = true;
            this.lblFileBytesProcessed.Location = new System.Drawing.Point(187, 53);
            this.lblFileBytesProcessed.Name = "lblFileBytesProcessed";
            this.lblFileBytesProcessed.Size = new System.Drawing.Size(12, 15);
            this.lblFileBytesProcessed.TabIndex = 4;
            this.lblFileBytesProcessed.Text = "-";
            // 
            // lblFoldersProcessed
            // 
            this.lblFoldersProcessed.AutoSize = true;
            this.lblFoldersProcessed.Location = new System.Drawing.Point(187, 79);
            this.lblFoldersProcessed.Name = "lblFoldersProcessed";
            this.lblFoldersProcessed.Size = new System.Drawing.Size(12, 15);
            this.lblFoldersProcessed.TabIndex = 4;
            this.lblFoldersProcessed.Text = "-";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(256, 165);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpLoadedTotals
            // 
            this.grpLoadedTotals.Controls.Add(this.lblFilesFound);
            this.grpLoadedTotals.Controls.Add(this.lblFileBytesFound);
            this.grpLoadedTotals.Controls.Add(this.label13);
            this.grpLoadedTotals.Controls.Add(this.lblFoldersFound);
            this.grpLoadedTotals.Controls.Add(this.label11);
            this.grpLoadedTotals.Controls.Add(this.label10);
            this.grpLoadedTotals.Location = new System.Drawing.Point(12, 194);
            this.grpLoadedTotals.Name = "grpLoadedTotals";
            this.grpLoadedTotals.Size = new System.Drawing.Size(401, 115);
            this.grpLoadedTotals.TabIndex = 6;
            this.grpLoadedTotals.TabStop = false;
            this.grpLoadedTotals.Text = "Loaded Totals";
            // 
            // lblFilesFound
            // 
            this.lblFilesFound.AutoSize = true;
            this.lblFilesFound.Location = new System.Drawing.Point(187, 30);
            this.lblFilesFound.Name = "lblFilesFound";
            this.lblFilesFound.Size = new System.Drawing.Size(12, 15);
            this.lblFilesFound.TabIndex = 4;
            this.lblFilesFound.Text = "-";
            // 
            // lblFileBytesFound
            // 
            this.lblFileBytesFound.AutoSize = true;
            this.lblFileBytesFound.Location = new System.Drawing.Point(187, 55);
            this.lblFileBytesFound.Name = "lblFileBytesFound";
            this.lblFileBytesFound.Size = new System.Drawing.Size(12, 15);
            this.lblFileBytesFound.TabIndex = 4;
            this.lblFileBytesFound.Text = "-";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(87, 81);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 15);
            this.label13.TabIndex = 4;
            this.label13.Text = "Folders Found";
            // 
            // lblFoldersFound
            // 
            this.lblFoldersFound.AutoSize = true;
            this.lblFoldersFound.Location = new System.Drawing.Point(187, 81);
            this.lblFoldersFound.Name = "lblFoldersFound";
            this.lblFoldersFound.Size = new System.Drawing.Size(12, 15);
            this.lblFoldersFound.TabIndex = 4;
            this.lblFoldersFound.Text = "-";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(76, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(93, 15);
            this.label11.TabIndex = 4;
            this.label11.Text = "File Bytes Found";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(102, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "Files Found";
            // 
            // treeFolders
            // 
            this.treeFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeFolders.ContextMenuStrip = this.contextMenuStrip1;
            this.treeFolders.Location = new System.Drawing.Point(432, 11);
            this.treeFolders.Name = "treeFolders";
            this.treeFolders.Size = new System.Drawing.Size(688, 588);
            this.treeFolders.TabIndex = 7;
            this.treeFolders.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeFolders_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.contextMenuStrip1.Name = "contextMenuTreeFolders";
            this.contextMenuStrip1.Size = new System.Drawing.Size(279, 114);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(278, 22);
            this.toolStripMenuItem1.Text = "Load Date Taken";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.menuItems_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(278, 22);
            this.toolStripMenuItem2.Text = "Fix Invalid DateTaken";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.menuItems_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(278, 22);
            this.toolStripMenuItem3.Text = "Set missing Date Taken from Last Write";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.menuItems_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(278, 22);
            this.toolStripMenuItem4.Text = "Set Created / Last Write to Date Taken";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.menuItems_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(278, 22);
            this.toolStripMenuItem5.Text = "Set Date Taken Manually";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.menuItems_Click);
            // 
            // grpAutoCleanupSettings
            // 
            this.grpAutoCleanupSettings.Controls.Add(this.label2);
            this.grpAutoCleanupSettings.Controls.Add(this.cboFixDateTaken);
            this.grpAutoCleanupSettings.Controls.Add(this.label8);
            this.grpAutoCleanupSettings.Controls.Add(this.cboSetCreatedDateFromDateTaken);
            this.grpAutoCleanupSettings.Controls.Add(this.cboRenameDupeFiles);
            this.grpAutoCleanupSettings.Controls.Add(this.label6);
            this.grpAutoCleanupSettings.Controls.Add(this.label7);
            this.grpAutoCleanupSettings.Controls.Add(this.cboSetMissingDateTaken);
            this.grpAutoCleanupSettings.Enabled = false;
            this.grpAutoCleanupSettings.Location = new System.Drawing.Point(12, 316);
            this.grpAutoCleanupSettings.Name = "grpAutoCleanupSettings";
            this.grpAutoCleanupSettings.Size = new System.Drawing.Size(401, 166);
            this.grpAutoCleanupSettings.TabIndex = 8;
            this.grpAutoCleanupSettings.TabStop = false;
            this.grpAutoCleanupSettings.Text = "Auto Cleanup";
            // 
            // grpAutoCleanupProgress
            // 
            this.grpAutoCleanupProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAutoCleanupProgress.Controls.Add(this.label3);
            this.grpAutoCleanupProgress.Controls.Add(this.lblFilesProcessed);
            this.grpAutoCleanupProgress.Controls.Add(this.lblFileBytesProcessed);
            this.grpAutoCleanupProgress.Controls.Add(this.label5);
            this.grpAutoCleanupProgress.Controls.Add(this.lblFoldersProcessed);
            this.grpAutoCleanupProgress.Controls.Add(this.label4);
            this.grpAutoCleanupProgress.Enabled = false;
            this.grpAutoCleanupProgress.Location = new System.Drawing.Point(12, 488);
            this.grpAutoCleanupProgress.Name = "grpAutoCleanupProgress";
            this.grpAutoCleanupProgress.Size = new System.Drawing.Size(401, 111);
            this.grpAutoCleanupProgress.TabIndex = 9;
            this.grpAutoCleanupProgress.TabStop = false;
            this.grpAutoCleanupProgress.Text = "Cleanup Progress";
            // 
            // CleanupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 881);
            this.Controls.Add(this.grpAutoCleanupProgress);
            this.Controls.Add(this.grpAutoCleanupSettings);
            this.Controls.Add(this.treeFolders);
            this.Controls.Add(this.grpLoadedTotals);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.listLog);
            this.Controls.Add(this.grpSource);
            this.Controls.Add(this.btnLoad);
            this.Name = "CleanupForm";
            this.Text = "Cleanup Photos Folder";
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpLoadedTotals.ResumeLayout(false);
            this.grpLoadedTotals.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.grpAutoCleanupSettings.ResumeLayout(false);
            this.grpAutoCleanupSettings.PerformLayout();
            this.grpAutoCleanupProgress.ResumeLayout(false);
            this.grpAutoCleanupProgress.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listLog;
        private System.Windows.Forms.ComboBox cboFixDateTaken;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFilesProcessed;
        private System.Windows.Forms.Label lblFileBytesProcessed;
        private System.Windows.Forms.Label lblFoldersProcessed;
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
        private System.Windows.Forms.GroupBox grpLoadedTotals;
        private System.Windows.Forms.Label lblFilesFound;
        private System.Windows.Forms.Label lblFileBytesFound;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblFoldersFound;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TreeView treeFolders;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.GroupBox grpAutoCleanupSettings;
        private System.Windows.Forms.GroupBox grpAutoCleanupProgress;
    }
}

