using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class CleanupJob
    {
        private readonly CleanupJobSettings m_Settings;
        private DirectoryInfo RootFolder => m_Settings.RootFolderDir;
        
        private readonly List<DirectoryInfo> m_FoldersFound = new List<DirectoryInfo>();
        private readonly List<FileInfo> m_FilesFound = new List<FileInfo>();
        
        private readonly CancellationToken m_CancellationToken;

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;


        public CleanupJob(CleanupJobSettings settings, CancellationToken cancellationToken)
        {
            m_Settings = settings;
            m_CancellationToken = cancellationToken;

            if (!m_Settings.Validate(out var errors))
                throw new ArgumentException($"Invalid settings:\r\n{string.Join("\r\n", errors)}");
            
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
                               var result = await ProcessDirectory(RootFolder);
                               if (result == false)
                                   OnProgress("Run cancelled.");
                           });
        }

        private async Task<bool> ProcessDirectory(DirectoryInfo dir)
        {
            if (m_CancellationToken.IsCancellationRequested)
                return false;

            if (dir == RootFolder)
            {
                OnProgress($"Starting scan on root folder: {dir.FullName}");
            }
            else
            {
                if (IsFolderIgnored(dir))
                    return true;

                DirFound(dir);
            }

            foreach (var file in dir.EnumerateFiles())
                if (!await ProcessFile(file))
                    return false;

            foreach (var subDir in dir.EnumerateDirectories())
                if (!await ProcessDirectory(subDir))
                    return false;

            return true;
        }
        
        private void DirFound(DirectoryInfo dir)
        {
            m_FoldersFound.Add(dir);
            OnProgress($"Folder found: {dir.FullName}");
        }

        private async Task<bool> ProcessFile(FileInfo file)
        {
            if (m_CancellationToken.IsCancellationRequested)
                return false;

            m_FilesFound.Add(file);
            OnProgress(null);

            if (IsFileIgnored(file))
                return true;

            var cleanupFile = new CleanupFile(file, m_Settings, OnProgress);
            return await cleanupFile.ProcessFile();
        }

        private bool IsFolderIgnored(DirectoryInfo dir)
        {
            return m_Settings.IgnoreFoldersStartingWith.Any(f => dir.Name.StartsWith(f, StringComparison.CurrentCultureIgnoreCase));
        }

        private static bool IsFileIgnored(FileInfo file)
        {
            return string.Equals(file.Name, "thumbs.db", StringComparison.CurrentCultureIgnoreCase);
        }

        private void OnProgress(string message)
        {
            // TODO: optimise this
            var filesProcessed = m_FilesFound.Count;
            var filesSizeProcessed = m_FilesFound.Sum(f => f.Length);
            var foldersProcessed = m_FoldersFound.Count;
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs(filesProcessed, filesSizeProcessed, foldersProcessed, message));
        }
    }
}
