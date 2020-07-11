using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class CleanupJob
    {
        private readonly CleanupJobSettings m_Settings;

        private bool m_FinishedScan;

        private int m_FoldersProcessed;
        private int m_FilesProcessed;
        private long m_FilesSizeProcessed;

        private readonly CancellationToken m_CancellationToken;

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        public LocalFolder RootFolder { get; }

        public CleanupJob(CleanupJobSettings settings, CancellationToken cancellationToken)
        {
            m_Settings = settings;
            m_CancellationToken = cancellationToken;

            if (!m_Settings.Validate(out var errors))
                throw new ArgumentException($"Invalid settings:\r\n{string.Join("\r\n", errors)}");
            
            RootFolder = new LocalFolder(m_Settings.RootFolderDir, m_CancellationToken);

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
                               await RootFolder.Load(IsFolderIgnored, IsFileIgnored, m_Settings, () => OnProgress(null));
                               m_FinishedScan = true;

                               OnProgress("Folders and files found. Starting processing.");
                               var result = await ProcessDirectory(RootFolder);

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
            
            var cleanupFile = new CleanupFile(file, m_Settings, OnProgress);
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

            // TODO: Run Dupe check before Processing all - DupeCleanup mark the LocalFiles and then it can rename as part of the rest of the cleanup

            var dupeCleanup = new DupeCleanup(RootFolder);
            var isDupes = await dupeCleanup.Check();

            if (!isDupes)
            {
                OnProgress("No duplicate files detected");
                return;
            }

            OnProgress($"Exact Dupes detected: {dupeCleanup.ExactDupes.Count} [Won't fix] Exact; {dupeCleanup.NameDupes.Count} By Name");
            
            foreach (var exactDupe in dupeCleanup.ExactDupes.OrderBy(d => d.File.Name))
                OnProgress($"[Log] Exact dupe: {exactDupe.File.Name} in {exactDupe.File.Directory.FullName}");
            
            foreach (var nameDupe in dupeCleanup.NameDupes.OrderBy(d => d.File.Name))
            {
                // BUG: DateTaken might not be loaded
                if (!await new CleanupFile(nameDupe, m_Settings, OnProgress).FixDuplicateFileName())
                    return;
            }
        }

        private void OnProgress(string message)
        {
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs(RootFolder.TotalFiles, RootFolder.TotalFileSize, RootFolder.TotalFolders, 
                                                                     m_FinishedScan,
                                                                     m_FilesProcessed, m_FilesSizeProcessed, m_FoldersProcessed,
                                                                     message));
        }
    }
}
