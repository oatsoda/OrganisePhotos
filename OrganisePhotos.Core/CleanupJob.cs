using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class CleanupJob
    {
        private readonly CleanupJobSettings m_Settings;

        private int m_FoldersProcessed;
        private int m_FilesProcessed;
        private long m_FilesSizeProcessed;

        private readonly CancellationToken m_CancellationToken;

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        public LocalFolder RootFolder { get; }

        public CleanupJob(LocalFolder rootFolder, CleanupJobSettings settings, CancellationToken cancellationToken)
        {
            if (!rootFolder.IsRootFolder)
                throw new ArgumentException("Folder must be Root Folder", nameof(rootFolder));

            m_Settings = settings;
            m_CancellationToken = cancellationToken;

            RootFolder = rootFolder;

            /*
             * Cycle through all folders and files and:
             *
             * - Flag up unknown file types (non-video/image: e.g. https://developers.google.com/photos/library/guides/upload-media#file-types-sizes) and prompt to delete?
             *     > Ignore known files like Thumbs.db ?
             * - Build up list of filenames across the whole tree and then detect dupes and suggest renaming by prefixing date taken?
             * - Detect missing Date Taken?
             * - Detect Date Taken which looks partial (1st month, no time etc?)
             * - Detect files with invalid format of Date Taken: yyyy:MM:dd:HH:mm:ss
             * - Update file Create Date from Date Taken             *
             */
        }

        public async Task Run()
        {
            await Task.Run(async () =>
                           {
                               OnProgress("Starting processing.");
                               var result = await ProcessDirectory(RootFolder);

                               OnProgress(result == false ? "Run cancelled." : "Processing finished.");

                           }, m_CancellationToken);
        }

        private async Task<bool> ProcessDirectory(LocalFolder folder)
        {
            if (m_CancellationToken.IsCancellationRequested)
                return false;

            OnProgress();

            var x = 0;
            foreach (var file in folder.Files)
            {
                if (!await ProcessFile(file))
                    return false;

                if (++x % 10 == 0)
                    OnProgress();
            }

            foreach (var subDir in folder.Folders)
            {
                if (!await ProcessDirectory(subDir))
                    return false;
            }

            m_FoldersProcessed++;
            OnProgress();

            return true;
        }

        private async Task<bool> ProcessFile(LocalFile file)
        {
            if (m_CancellationToken.IsCancellationRequested)
                return false;
            
            if (m_Settings.ReportFileDatesOutOfSync && file.DatesTakenOutOfSync)
                OnProgress($"File dates: [Create:Write: {file.File.LastWriteTime:dd/MM/yyyy HH:mm:ss} | Create: {file.File.CreationTime:dd/MM/yyyy HH:mm:ss} - {file.File.FullName}");

            if (m_Settings.AppendDateIfShortFileName != CleanupAction.Ignore)
            {
                if (file.FileNameDoesNotIncludeDate)
                {
                    if (m_Settings.AppendDateIfShortFileName == CleanupAction.Log)
                    {
                        OnProgress($"File name no date {file.File.Name} -> {file.FileNameWithDateSuffix} [{file.File.FullName}]");
                    }
                    else if (m_Settings.AppendDateIfShortFileName == CleanupAction.Fix)
                    {
                        OnProgress($"Renaming {file.File.Name} -> {file.FileNameWithDateSuffix} [{file.File.FullName}]");
                        await file.AddDateToFileName();
                    }
                }
            }

            m_FilesProcessed++;
            m_FilesSizeProcessed += file.File.Length;

            return true;
        }
        
        private void OnProgress(string message = null)
        {
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs(m_FilesProcessed, m_FilesSizeProcessed, m_FoldersProcessed,
                                                                     message));
        }
    }
}
