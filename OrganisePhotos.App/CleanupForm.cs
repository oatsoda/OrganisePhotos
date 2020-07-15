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
        private readonly LoadSettings m_LoadSettings;
        private readonly CleanupJobSettings m_CleanupJobSettings;
        
        private CancellationTokenSource m_LoadCancelSource;
        private CancellationTokenSource m_ProcessCancelSource;

        private ToolStripMenuItem MenuLoadDateTaken => toolStripMenuItem1;
        private ToolStripMenuItem MenuFixDateTaken => toolStripMenuItem2;
        private ToolStripMenuItem MenuSetMissingDateTaken => toolStripMenuItem3;
        private ToolStripMenuItem MenuSetFileDatesToDateTaken => toolStripMenuItem4;
        private ToolStripMenuItem MenuSetDateTakenManually => toolStripMenuItem5;

        private readonly Dictionary<LocalFile, TreeNode> m_FileNodes = new Dictionary<LocalFile, TreeNode>();

        private LocalFolder m_LoadedRootFolder;

        public CleanupForm()
        {
            m_LoadSettings = new LoadSettings();
            m_CleanupJobSettings = new CleanupJobSettings
                                   {
                                       Prompt = Prompt
                                   };
            InitializeComponent();
            cboFixDateTaken.BindToEnum<CleanupAction>();
            cboSetCreatedDateFromDateTaken.BindToEnum<CleanupAction>();
            cboSetMissingDateTaken.BindToEnum<CleanupAction>();
            cboRenameDupeFiles.BindToEnum<CleanupAction>();

            UpdateLoadSettings();
            UpdateProcessSettings();
            SetState(AppState.Unloaded);
        }

        public enum AppState
        {
            Unloaded,
            Loading,
            Loaded,
            Processing
        }

        private void SetState(AppState state)
        {
            void ClearLoadData()
            {
                lblFilesFound.Text = lblFileBytesFound.Text = lblFoldersFound.Text = "-";

                treeFolders.Nodes.Clear();
                foreach (var localFile in m_FileNodes.Keys)
                    localFile.FileUpdated -= LocalFileOnFileUpdated;

                m_FileNodes.Clear();
            }

            void ClearProcessData()
            {
                listLog.Items.Clear();
                lblFilesProcessed.Text = lblFileBytesProcessed.Text = lblFoldersProcessed.Text = "-";
            }

            switch (state)
            {
                case AppState.Unloaded:
                    btnLoad.Enabled = grpSource.Enabled = true;
                    btnCancel.Enabled = false;
                    treeFolders.Enabled = false;

                    btnProcess.Enabled = btnCancelProcess.Enabled = grpAutoCleanupSettings.Enabled = grpAutoCleanupProgress.Enabled = false;

                    ClearProcessData();

                    break;

                case AppState.Loading:
                    btnLoad.Enabled = grpSource.Enabled = false;
                    btnCancel.Enabled = true;
                    treeFolders.Enabled = false;

                    btnProcess.Enabled = btnCancelProcess.Enabled = grpAutoCleanupSettings.Enabled = grpAutoCleanupProgress.Enabled = false;

                    ClearLoadData();
                    ClearProcessData();

                    break;

                case AppState.Loaded:
                    
                    btnLoad.Enabled = grpSource.Enabled = true;
                    btnCancel.Enabled = false;
                    treeFolders.Enabled = true;

                    btnProcess.Enabled = grpAutoCleanupSettings.Enabled = true;
                    btnCancelProcess.Enabled = grpAutoCleanupProgress.Enabled = false;

                    break;

                case AppState.Processing:

                    btnLoad.Enabled = grpSource.Enabled = false;
                    btnCancel.Enabled = false;
                    treeFolders.Enabled = true;

                    btnProcess.Enabled = grpAutoCleanupSettings.Enabled = false;
                    btnCancelProcess.Enabled = grpAutoCleanupProgress.Enabled = true;

                    ClearProcessData();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void txtFolderPath_TextChanged(object sender, EventArgs e) => UpdateLoadSettings();
        private void txtIgnoreFoldersStart_TextChanged(object sender, EventArgs e) => UpdateLoadSettings();

        private void UpdateLoadSettings()
        {
            m_LoadSettings.RootFolderPath = txtFolderPath.Text;
            m_LoadSettings.IgnoreFoldersStartingWith = txtIgnoreFoldersStart.Text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            SetState(AppState.Loading);
            
            m_LoadCancelSource = new CancellationTokenSource();
            
            var localFolder = new LocalFolder(m_LoadSettings); 
            localFolder.LoadProgress += LocalFolderOnLoadProgress;

            await localFolder.Load(m_LoadCancelSource.Token);
            localFolder.LoadProgress -= LocalFolderOnLoadProgress;

            if (m_LoadCancelSource.IsCancellationRequested)
            {
                lblFilesFound.Text += " (Cancelled)";
                SetState(AppState.Unloaded);
            }
            else
            {
                m_LoadedRootFolder = localFolder;
                SetState(AppState.Loaded);
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            m_LoadCancelSource.Cancel();
        }

        private void LocalFolderOnLoadProgress(object? sender, LoadProgressEventArgs e)
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
            
            treeNode.Nodes.AddRange(localFolder.Files.OrderBy(f => f.File.Name).Select(CreateFileNode).ToArray());
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
        
        #region Tree Context Menu Methods
        
        private void treeFolders_AfterExpand(object sender, TreeViewEventArgs e)
        {
            // Load DateTaken on expand
            if (e.Action != TreeViewAction.Expand)
                return;

            RunUpdate(UpdateFileDateTaken, e.Node);
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

        #endregion
        
        private void cboFixDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateProcessSettings();
        private void cboSetCreatedDateFromDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateProcessSettings();
        private void cboSetMissingDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateProcessSettings();
        private void cboRenameDupeFiles_SelectedIndexChanged(object sender, EventArgs e) => UpdateProcessSettings();
        
        private void UpdateProcessSettings()
        {
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

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            SetState(AppState.Processing);
            m_ProcessCancelSource = new CancellationTokenSource();

            var cleanupJob = new CleanupJob(m_LoadedRootFolder, m_CleanupJobSettings, m_ProcessCancelSource.Token); 
            cleanupJob.ProgressUpdate += JobOnProgressUpdate;

            await cleanupJob.Run();
            cleanupJob.ProgressUpdate -= JobOnProgressUpdate;

            if (m_ProcessCancelSource.IsCancellationRequested)
                lblFilesProcessed.Text += " (Cancelled)";

            SetState(AppState.Loaded);
        }

        private void btnCancelProcess_Click(object sender, EventArgs e)
        {
            btnCancelProcess.Enabled = false;
            m_ProcessCancelSource.Cancel();
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

    }

}
