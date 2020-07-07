using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class CleanupJob
    {
        private readonly CleanupJobSettings m_Settings;

        private readonly LocalFolder m_RootFolder;
        
        private int m_FoldersProcessed;
        private int m_FilesProcessed;
        private long m_FilesSizeProcessed;

        private readonly CancellationToken m_CancellationToken;

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;


        public CleanupJob(CleanupJobSettings settings, CancellationToken cancellationToken)
        {
            m_Settings = settings;
            m_CancellationToken = cancellationToken;

            if (!m_Settings.Validate(out var errors))
                throw new ArgumentException($"Invalid settings:\r\n{string.Join("\r\n", errors)}");
            
            m_RootFolder = new LocalFolder(m_Settings.RootFolderDir, m_CancellationToken);

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
                               OnProgress("Finding folders and files.");
                               await m_RootFolder.Load(IsFolderIgnored, IsFileIgnored, () => OnProgress(null));

                               OnProgress("Folders and files found. Starting processing.");
                               var result = await ProcessDirectory(m_RootFolder);

                               await ProcessDupes();

                               if (result == false)
                                   OnProgress("Run cancelled.");

                           }, m_CancellationToken);
        }

        private async Task<bool> ProcessDirectory(LocalFolder folder)
        {
            if (m_CancellationToken.IsCancellationRequested)
                return false;

            OnProgress($"Processing folder: {folder.Dir.FullName}");

            var x = 0;
            foreach (var file in folder.Files)
            {
                if (!await ProcessFile(file))
                    return false;

                if (++x % 10 == 0)
                    OnProgress(null);
            }

            foreach (var subDir in folder.Folders)
            {
                if (!await ProcessDirectory(subDir))
                    return false;
            }

            m_FoldersProcessed++;
            OnProgress(null);

            return true;
        }

        private async Task<bool> ProcessFile(LocalFile file)
        {
            if (m_CancellationToken.IsCancellationRequested)
                return false;
            
            var cleanupFile = new CleanupFile(file.File, m_Settings, OnProgress);
            var result = await cleanupFile.ProcessFile();

            m_FilesProcessed++;
            m_FilesSizeProcessed += file.File.Length;

            return result;
        }

        private bool IsFolderIgnored(DirectoryInfo dir)
        {
            return m_Settings.IgnoreFoldersStartingWith.Any(f => dir.Name.StartsWith(f, StringComparison.CurrentCultureIgnoreCase));
        }

        private static bool IsFileIgnored(FileInfo file)
        {
            return string.Equals(file.Name, "thumbs.db", StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task ProcessDupes()
        {
            if (m_Settings.RenameDupeFiles == CleanupAction.Ignore)
                return;

            var dupeCleanup = new DupeCleanup(m_RootFolder);
            var isDupes = await dupeCleanup.Check();

            if (!isDupes)
            {
                OnProgress("No duplicate files detected");
                return;
            }

            OnProgress($"Dupes detected: {dupeCleanup.ExactDupes.Count} Exact; {dupeCleanup.NameDupes.Count} By Name");
            await dupeCleanup.SaveReport("dupes.txt");
        }

        private void OnProgress(string message)
        {
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs(m_RootFolder.TotalFiles, m_RootFolder.TotalFileSize, m_RootFolder.TotalFolders, 
                                                                     m_FilesProcessed, m_FilesSizeProcessed, m_FoldersProcessed,
                                                                     message));
        }
    }
}
