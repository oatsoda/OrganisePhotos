using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class LocalFolder
    {
        private readonly CancellationToken m_CancellationToken;

        public DirectoryInfo Dir { get; }

        public List<LocalFile> Files { get; private set; }
        public List<LocalFolder> Folders { get; private set; }

        public int TotalFiles { get; private set; }
        public long TotalFileSize { get; private set; }
        public int TotalFolders { get; private set; }

        public LocalFolder(DirectoryInfo dir, CancellationToken cancellationToken)
        {
            Dir = dir;
            m_CancellationToken = cancellationToken;
        }

        public async Task Load(Func<DirectoryInfo, bool> isIgnoredDir, Func<FileInfo, bool> isIgnoredFile, CleanupJobSettings settings, Action onProgress)
        {
            TotalFolders = 1; // This folder

            await Task.Run(() =>
                           {
                               var files = new List<LocalFile>();
                               foreach (var file in Dir.EnumerateFiles())
                               {
                                   if (m_CancellationToken.IsCancellationRequested)
                                       return;

                                   if (isIgnoredFile(file))
                                       continue;
                                   
                                   files.Add(new LocalFile(file, settings));

                                   TotalFiles++;
                                   TotalFileSize += file.Length;
                               }

                               Files = files;

                           }, m_CancellationToken);

            await Task.Run(async () =>
                           {

                               var folders = new List<LocalFolder>();

                               foreach (var dir in Dir.EnumerateDirectories().OrderBy(d => d.Name))
                               {
                                   if (m_CancellationToken.IsCancellationRequested)
                                       return;

                                   if (isIgnoredDir(dir))
                                       continue;

                                   var folder = new LocalFolder(dir, m_CancellationToken);
                                   folders.Add(folder);
                                   await folder.Load(isIgnoredDir, isIgnoredFile, settings, onProgress);

                                   TotalFiles += folder.TotalFiles;
                                   TotalFileSize += folder.TotalFileSize;
                                   TotalFolders += folder.TotalFolders;
                                   onProgress();
                               }

                               Folders = folders;

                           }, m_CancellationToken);
        }
    }
}