using OrganisePhotos.Core;
using System;
using System.Windows.Forms;

namespace OrganisePhotos.App
{
    public partial class CleanupForm : Form
    {
        private readonly CleanupJobSettings m_CleanupJobSettings;

        public CleanupForm()
        {
            m_CleanupJobSettings = new CleanupJobSettings
                                   {
                                       Prompt = Prompt
                                   };
            InitializeComponent();
            cboFixDateTaken.BindToEnum<CleanupAction>();
        }


        private async void btnStart_Click(object sender, EventArgs e)
        {
            var job = new CleanupJob(m_CleanupJobSettings);
            job.ProgressUpdate += JobOnProgressUpdate;

            btnStart.Enabled = grpSettings.Enabled = false;
            lblFilesFound.Text = lblFileBytesFound.Text = lblFoldersFound.Text = "-";
            listLog.Items.Clear();

            await job.Run();
            job.ProgressUpdate -= JobOnProgressUpdate;

            MessageBox.Show("Run Ended", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnStart.Enabled = grpSettings.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) => UpdateSettings();
        private void cboFixDateTaken_SelectedIndexChanged(object sender, EventArgs e) => UpdateSettings();

        private void UpdateSettings()
        {
            m_CleanupJobSettings.RootFolderPath = txtFolderPath.Text;
            m_CleanupJobSettings.FixIncorrectDateTakenFormat = cboFixDateTaken.SelectedEnum<CleanupAction>();
        }
        
        private PromptResult Prompt(string question)
        {
            var result = MessageBox.Show(question, "Continue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

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
                                          listLog.Items.Add($"{e.Message}");

                                      lblFilesFound.Text = e.FilesProcessed.ToString();
                                      lblFileBytesFound.Text = e.FilesSizeProcessed.ToString();
                                      lblFoldersFound.Text = e.FoldersProcessed.ToString();
                                  });
        }
    }


    public static class CheckBoxExtensions
    {
        public static CleanupAction ToCleanupAction(this CheckBox checkbox)
        {
            return checkbox.CheckState switch
                   {
                       CheckState.Unchecked => CleanupAction.Ignore,
                       CheckState.Checked => CleanupAction.Fix,
                       CheckState.Indeterminate => CleanupAction.Prompt,
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }
    }
}
