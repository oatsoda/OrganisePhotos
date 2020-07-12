using System;
using System.IO;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class CleanupFile
    {
        private readonly LocalFile m_LocalFile;
        private FileInfo FileInfo => m_LocalFile.File;
        private readonly CleanupJobSettings m_Settings;
        private readonly Action<string> m_ProgressAction;
        
        private bool m_ExitRequested;

        // BUG: Not loading DateTaken if other options aren't set - e.g. only Dupes option is chosen
        private string DupeRenameSuffix => (m_LocalFile.DateTaken ?? FileInfo.CreationTime).ToString("_yyyyMMdd_hhmmss");
        private string DupeRenameFilename => $"{Path.GetFileNameWithoutExtension(FileInfo.Name)}{DupeRenameSuffix}{FileInfo.Extension}";

        public CleanupFile(LocalFile fileInfo, CleanupJobSettings settings, Action<string> progressAction)
        {
            m_LocalFile = fileInfo;
            m_Settings = settings;
            m_ProgressAction = progressAction;
        }

        public void RenameWithDateSuffix()
        {
            var destination = Path.Combine(FileInfo.Directory.FullName, DupeRenameFilename);
            FileInfo.MoveTo(destination);
        }

        public async Task<bool> ProcessFile()
        {
            await ReadExifRawDateTaken();
            await FixIncorrectDateTakenFormat();

            if (m_ExitRequested)
                return false;

            await FixMissingDateTaken();
            
            if (m_ExitRequested)
                return false;

            await ChangeCreatedDateToDateTaken();

            return !m_ExitRequested;
        }

        private async Task ReadExifRawDateTaken()
        {
            await m_LocalFile.LoadDateTaken();
            
            // All settings require DataTaken, so report invalid ones
            if (!m_LocalFile.DateTakenValid)
                m_ProgressAction($"[Log] Unrecognised Date Taken '{m_LocalFile.DateTakenRaw}' for file {FileInfo.FullName}");
        }
        
        private async Task FixIncorrectDateTakenFormat()
        {
            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Ignore)
                return;

            if (!m_LocalFile.DateTakenFixable)
                return;

            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Could fix Date Taken '{m_LocalFile.DateTakenRaw}' with '{m_LocalFile.DateTakenCorrectRaw}' for file {FileInfo.FullName}");
                return;
            }

            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Prompt)
            {
                var result = Prompt($"Replace Date Taken '{m_LocalFile.DateTakenRaw}' with '{m_LocalFile.DateTakenCorrectRaw}' for file {FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return;
            }

            await m_LocalFile.FixInvalidDateTaken();
            
            m_ProgressAction($"Fixed Date Taken on {FileInfo.FullName}");
        }
        
        private async Task FixMissingDateTaken()
        {
            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Ignore)
                return;
            
            if (m_LocalFile.DateTakenValid || m_LocalFile.DateTaken.HasValue)
                return;

            var lastWrite = FileInfo.LastWriteTime.ToString("yyyy:MM:dd HH:mm:ss");

            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Could set missing Date Taken to Last Write '{lastWrite}' for file {FileInfo.FullName}");
                return;
            }

            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Prompt)
            {
                var result = Prompt($"Set missing Date Taken to Last Write '{lastWrite}' for file {FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return;
            }

            await m_LocalFile.SetMissingDateTakenFromLastWrite();
            m_ProgressAction($"Set missing Date Taken to Last Write '{lastWrite}' for file {FileInfo.FullName}");
        }
        
        private async Task ChangeCreatedDateToDateTaken()
        {
            if (m_Settings.ChangeCreatedDateToDateTaken == CleanupAction.Ignore)
                return;

            if (!m_LocalFile.DateTaken.HasValue)
                return;
            
            var createdDate = FileInfo.CreationTime.ToString("O");
            var lastWrite = FileInfo.LastWriteTime.ToString("O");
            var dateTaken = m_LocalFile.DateTaken.Value.ToString("O");

            if (m_Settings.ChangeCreatedDateToDateTaken == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Could change Created '{createdDate}' and Last Write '{lastWrite}' to Date Taken '{dateTaken}' for file {FileInfo.FullName}");
                return;
            }

            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Prompt)
            {
                var result = Prompt($"Change Created '{createdDate}' and Last Write '{lastWrite}' to Date Taken '{dateTaken}' for file {FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return;
            }
            
            await m_LocalFile.SetFileDatesFromDateTaken();
            m_ProgressAction($"Set Create and Last Write dates to  Date Taken '{dateTaken}' for file {FileInfo.FullName}");
        }

        
        public async Task<bool> FixDuplicateFileName()
        {
            if (m_Settings.RenameDupeFiles == CleanupAction.Ignore)
                return true;

            if (m_Settings.RenameDupeFiles == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Name dupe: {FileInfo.Name} in {FileInfo.Directory.FullName}");
                return true;
            }

            if (m_Settings.RenameDupeFiles == CleanupAction.Prompt)
            {
                var result = Prompt($"Rename dupe file from '{FileInfo.Name}' to '{DupeRenameFilename}' for file {FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return true; 
            }

            RenameWithDateSuffix();
            return true;
        }
        
        private PromptResult Prompt(string message)
        {
            var result = m_Settings.Prompt(message);
            if (result == PromptResult.Exit)
                m_ExitRequested = true;

            return result;
        }
    }
}