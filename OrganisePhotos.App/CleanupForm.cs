using OrganisePhotos.Core;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace OrganisePhotos.App
{
    public partial class CleanupForm : Form
    {
        private readonly CleanupJobSettings m_CleanupJobSettings;
        
        private CancellationTokenSource m_CancelSource;

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
            
            var job = new CleanupJob(m_CleanupJobSettings, m_CancelSource.Token);
            job.ProgressUpdate += JobOnProgressUpdate;

            btnStart.Enabled = grpSettings.Enabled = false;
            lblFilesFound.Text = lblFileBytesFound.Text = lblFoldersFound.Text = "-";
            listLog.Items.Clear();
            btnCancel.Enabled = true;

            await job.Run();
            job.ProgressUpdate -= JobOnProgressUpdate;

            MessageBox.Show("Run Ended", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnStart.Enabled = grpSettings.Enabled = true;
            btnCancel.Enabled = false;
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

        private void JobOnProgressUpdate(object? sender, ProgressUpdateEventArgs e)
        {
            this.InvokeIfRequired(() =>
                                  {
                                      if (e.Message != null)
                                      {
                                          listLog.Items.Add($"{e.Message}");
                                          listLog.SelectedIndex = listLog.Items.Count - 1;
                                          listLog.SelectedIndex = -1;
                                      }

                                      lblFilesFound.Text = e.FilesProcessed.ToString();
                                      lblFileBytesFound.Text = e.FilesSizeProcessed.ToString();
                                      lblFoldersFound.Text = e.FoldersProcessed.ToString();
                                  });
        }

    }

}
