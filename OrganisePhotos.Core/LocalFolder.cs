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
        private bool m_LoadCompleted;

        internal bool IsRootFolder { get; }

        public CleanupJobSettings Settings { get; }
        public DirectoryInfo Dir { get; }

        public List<LocalFile> Files { get; private set; }
        public List<LocalFolder> Folders { get; private set; }

        public int TotalFiles { get; private set; }
        public long TotalFileSize { get; private set; }
        public int TotalFolders { get; private set; }

        public event EventHandler<LoadProgressEventArgs> LoadProgress;
        
        public LocalFolder(CleanupJobSettings settings)
        {
            if (!settings.Validate(out var errors))
                throw new ArgumentException($"Invalid settings:\r\n{string.Join("\r\n", errors)}");

            Settings = settings;
            IsRootFolder = true;
            Dir = Settings.RootFolderDir;
        }

        internal LocalFolder(DirectoryInfo dir, CleanupJobSettings settings)
        {
            Dir = dir;
            IsRootFolder = false;
            Settings = settings;
        }

        public async Task Load(CancellationToken cancellationToken)
        {
            TotalFolders = 1; // This folder

            try
            {


            await Task.Run(() =>
                           {
                               var files = new List<LocalFile>();
                               foreach (var file in Dir.EnumerateFiles())
                               {
                                   if (cancellationToken.IsCancellationRequested)
                                       return;

                                   if (Settings.IsFileIgnored(file))
                                       continue;
                                   
                                   files.Add(new LocalFile(file, Settings));

                                   TotalFiles++;
                                   TotalFileSize += file.Length;

                                   if (files.Count % 10 == 0)
                                       OnLoadProgress();
                               }

                               Files = files;

                           }, cancellationToken);

            await Task.Run(async () =>
                           {

                               var folders = new List<LocalFolder>();

                               foreach (var dir in Dir.EnumerateDirectories().OrderBy(d => d.Name))
                               {
                                   if (cancellationToken.IsCancellationRequested)
                                       return;

                                   if (Settings.IsFolderIgnored(dir))
                                       continue;

                                   var folder = new LocalFolder(dir, Settings);
                                   folders.Add(folder);
                                   await folder.Load(cancellationToken);

                                   TotalFiles += folder.TotalFiles;
                                   TotalFileSize += folder.TotalFileSize;
                                   TotalFolders += folder.TotalFolders;
                                   OnLoadProgress();
                               }

                               Folders = folders;

                           }, cancellationToken);
                
            }
            catch (TaskCanceledException)
            {
                return;
            }

            m_LoadCompleted = IsRootFolder && !cancellationToken.IsCancellationRequested;
            OnLoadProgress();
        }

        protected virtual void OnLoadProgress()
        {
            var e = new LoadProgressEventArgs(TotalFiles, TotalFileSize, TotalFolders, m_LoadCompleted);
            LoadProgress?.Invoke(this, e);
        }
    }
}