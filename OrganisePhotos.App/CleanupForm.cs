using OrganisePhotos.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrganisePhotos.App
{
    public partial class CleanupForm : Form
    {
        private readonly CleanupJobSettings m_CleanupJobSettings;
        
        private CancellationTokenSource m_CancelSource;

        private ToolStripMenuItem MenuLoadDateTaken => toolStripMenuItem1;
        private ToolStripMenuItem MenuFixDateTaken => toolStripMenuItem2;
        private ToolStripMenuItem MenuSetMissingDateTaken => toolStripMenuItem3;
        private ToolStripMenuItem MenuSetFileDatesToDateTaken => toolStripMenuItem4;
        private ToolStripMenuItem MenuSetDateTakenManually => toolStripMenuItem5;

        private readonly Dictionary<LocalFile, TreeNode> m_FileNodes = new Dictionary<LocalFile, TreeNode>();

        public CleanupForm()
        {
            m_CleanupJobSettings = new CleanupJobSettings
                                   {
                                       Prompt = Prompt
                                   };
            InitializeComponent();
            cboFixDateTaken.BindToEnum<CleanupAction>();
            cboSetCreatedDateFromDateTaken.BindToEnum<CleanupAction>();
            cboSetMissingDateTaken.BindToEnum<CleanupAction>();
            cboRenameDupeFiles.BindToEnum<CleanupAction>();
        }
        
        private async void btnStart_Click(object sender, EventArgs e)
        {
            m_CancelSource = new CancellationTokenSource();
            
            var job = new LocalFolder(m_CleanupJobSettings); 
            job.LoadProgress += JobOnLoadProgress;

            btnLoad.Enabled = grpSource.Enabled = false;
            lblFilesProcessed.Text = lblFileBytesProcessed.Text = lblFoldersProcessed.Text = "-";
            lblFilesFound.Text = lblFileBytesFound.Text = lblFoldersFound.Text = "-";
            listLog.Items.Clear();
            treeFolders.Nodes.Clear();
            treeFolders.Enabled = false;
            foreach (var localFile in m_FileNodes.Keys)
                localFile.FileUpdated -= LocalFileOnFileUpdated;
            m_FileNodes.Clear();

            btnCancel.Enabled = true;

            //await Task.Run(async () => await job.Load(m_CancelSource.Token));
            await job.Load(m_CancelSource.Token);
            if (m_CancelSource.IsCancellationRequested)
                lblFilesFound.Text += " (Cancelled)";

            btnLoad.Enabled = grpSource.Enabled = true;
            btnCancel.Enabled = false;
            treeFolders.Enabled = true;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            m_CancelSource.Cancel();
        }

        private void txtFolderPath_TextChanged(object sender, EventArgs e) => UpdateSettings();
        private void cboFixDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateSettings();
        private void cboSetCreatedDateFromDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateSettings();
        private void cboSetMissingDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateSettings();
        private void cboRenameDupeFiles_SelectedIndexChanged(object sender, EventArgs e) => UpdateSettings();
        private void txtIgnoreFoldersStart_TextChanged(object sender, EventArgs e) => UpdateSettings();

        private void UpdateSettings()
        {
            m_CleanupJobSettings.RootFolderPath = txtFolderPath.Text;
            m_CleanupJobSettings.IgnoreFoldersStartingWith = txtIgnoreFoldersStart.Text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
            m_CleanupJobSettings.FixIncorrectDateTakenFormat = cboFixDateTaken.SelectedEnum<CleanupAction>();
            m_CleanupJobSettings.ChangeCreatedDateToDateTaken = cboSetCreatedDateFromDateTaken.SelectedEnum<CleanupAction>();
            m_CleanupJobSettings.SetDateTakenFromCreatedDateIfNotSet = cboSetMissingDateTaken.SelectedEnum<CleanupAction>();
            m_CleanupJobSettings.RenameDupeFiles = cboRenameDupeFiles.SelectedEnum<CleanupAction>();
        }
        
        private PromptResult Prompt(string question)
        {
            DialogResult? result = null;
            this.InvokeIfRequired(() =>
                                  {
                                      result = MessageBox.Show(question, "Continue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                  }, true);
            return result switch
                   {
                       DialogResult.Yes => PromptResult.Fix,
                       DialogResult.No => PromptResult.Skip,
                       DialogResult.Cancel => PromptResult.Exit,
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        
        private void JobOnLoadProgress(object? sender, LoadProgressEventArgs e)
        {
            this.InvokeIfRequired(() =>
                                  {
                                      lblFilesFound.Text = e.TotalFiles.ToString();
                                      lblFileBytesFound.Text = e.TotalFileSize.ToString();
                                      lblFoldersFound.Text = e.TotalFolders.ToString();

                                      if (e.LoadCompleted)
                                          BindFolderTree(((LocalFolder)sender));
                                  });
        }

        private void JobOnProgressUpdate(object sender, ProgressUpdateEventArgs e)
        {
            this.InvokeIfRequired(() =>
                                  {
                                      if (e.Message != null)
                                      {
                                          listLog.Items.Add($"{e.Message}");
                                          listLog.SelectedIndex = listLog.Items.Count - 1;
                                          listLog.SelectedIndex = -1;
                                      }
                                      
                                      lblFilesProcessed.Text = e.FilesProcessed.ToString();
                                      lblFileBytesProcessed.Text = e.FilesSizeProcessed.ToString();
                                      lblFoldersProcessed.Text = e.FoldersProcessed.ToString();
                                  });
        }
        
        private void menuItems_Click(object sender, EventArgs e)
        {
            var node = treeFolders.SelectedNode;
            if (sender == MenuLoadDateTaken)
            {
                RunUpdate(UpdateFileDateTaken, node);
            }
            else if (sender == MenuFixDateTaken)
            {
                RunUpdate(FixFileDateTaken, node);
            }
            else if (sender == MenuSetFileDatesToDateTaken)
            {
                RunUpdate(SetFileDatesFromDateTaken, node);
            }
            else if (sender == MenuSetMissingDateTaken)
            {
                RunUpdate(SetDateTakenFromLastWrite, node);
            }
            else if (sender == MenuSetDateTakenManually)
            {
                var dateInput = new DateInputForm();
                var result = dateInput.ShowDialog(this);
                if (result != DialogResult.OK)
                    return;

                RunUpdate(n => SetFileDateTaken(n, dateInput.SelectedValue), node);
            }
        }

        private void treeFolders_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Ensure right-click also sets selected node, before context menu displayed
            treeFolders.SelectedNode = e.Node;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var rootNode = treeFolders.Nodes[0];
            if (treeFolders.SelectedNode == rootNode && rootNode.Nodes.Cast<TreeNode>().Any(n => n.Tag is LocalFolder))
                e.Cancel = true;

            // TODO: Menu item disabling based on data?
        }


        #region Bind Tree Methods
        
        private void BindFolderTree(LocalFolder rootFolder)
        {
            if (treeFolders.Nodes.Count > 0)
                return;

            treeFolders.Nodes.Add(CreateFolderNode(rootFolder));
            treeFolders.Enabled = true;
        }

        private TreeNode CreateFolderNode(LocalFolder localFolder)
        {
            var treeNode = new TreeNode(localFolder.Dir.Name)
                           {
                               Tag = localFolder
                           };
            if (localFolder.Folders.Any())
                treeNode.Nodes.AddRange(localFolder.Folders.Select(CreateFolderNode).ToArray());
            
            treeNode.Nodes.AddRange(localFolder.Files.Select(CreateFileNode).ToArray());
            return treeNode;
        }

        private TreeNode CreateFileNode(LocalFile localFile)
        {
            var node = new TreeNode(LocalFileNodeName(localFile))
                   {
                       Name = localFile.File.FullName,
                       Tag = localFile
                   };
            localFile.FileUpdated += LocalFileOnFileUpdated;
            m_FileNodes.Add(localFile, node);
            return node;
        }

        private void LocalFileOnFileUpdated(object sender, EventArgs e)
        {
            var localFile = (LocalFile)sender;
            var node = m_FileNodes[localFile];
            this.InvokeIfRequired(() =>
                                  {
                                      node.Text = LocalFileNodeName(localFile);
                                  });
        }

        private static string LocalFileNodeName(LocalFile localFile)
        {
            var lastWrite = localFile.File.LastWriteTime;
            var created = localFile.File.CreationTime;
            var validDisplay = localFile.DateTakenValid ? "" : localFile.DateTakenFixable ? " Fixable" : " Invalid";
            var dateTaken = !localFile.DateTakenLoaded
                                ? "(not loaded)"
                                : $"{localFile.DateTakenRaw}{validDisplay}";
                
            return $"{localFile.File.Name} | Write: {lastWrite.ToShortDateString()} {lastWrite.ToShortTimeString()} | Create: {created.ToShortDateString()} {created.ToShortTimeString()} | Taken: {dateTaken}";
        }

        #endregion

        #region Update Tree Methods

        private static void RunUpdate(Func<TreeNode, Task> action, TreeNode node)
        {
            // If already file, just single update, else get child files.  Not recursive, so single level only
            var nodes = node.Tag is LocalFile 
                            ? new [] { node }
                            : node.Nodes.Cast<TreeNode>().Where(n => n.Tag is LocalFile).ToArray();

            Task.Run(() =>
                     {
                         Parallel.ForEach(nodes, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async n =>
                                                                                                      {
                                                                                                          await action(n);
                                                                                                      });
                     });
        }

        private async Task UpdateFileDateTaken(TreeNode node)
        {
            if (!(node.Tag is LocalFile localFile))
                return;

            await localFile.LoadDateTaken();
        }

        private async Task FixFileDateTaken(TreeNode node)
        {
            if (!(node.Tag is LocalFile localFile))
                return;

            await localFile.FixInvalidDateTaken();
        }
        
        private async Task SetFileDateTaken(TreeNode node, DateTime value)
        {
            if (!(node.Tag is LocalFile localFile))
                return;

            await localFile.SetDateTakenManually(value);
        }
        
        private async Task SetFileDatesFromDateTaken(TreeNode node)
        {
            if (!(node.Tag is LocalFile localFile))
                return;
            await localFile.SetFileDatesFromDateTaken();
        }
        
        private async Task SetDateTakenFromLastWrite(TreeNode node)
        {
            if (!(node.Tag is LocalFile localFile))
                return;
            await localFile.SetMissingDateTakenFromLastWrite();
        }
        
        #endregion
    }

}
