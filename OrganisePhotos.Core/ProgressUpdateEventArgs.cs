namespace OrganisePhotos.Core
{
    public class ProgressUpdateEventArgs
    {
        public int TotalFiles { get; }
        public long TotalFileSize { get; }
        public int TotalFolders { get; }
        
        public bool FinishedScan { get; }

        public int FilesProcessed { get; }
        public long FilesSizeProcessed { get; }
        public int FoldersProcessed { get; }

        public string Message { get; }

        public ProgressUpdateEventArgs(int totalFiles, long totalFileSize, int totalFolders, bool finishedScan, int filesProcessed, long filesSizeProcessed, int foldersProcessed, string message)
        {
            TotalFiles = totalFiles;
            TotalFileSize = totalFileSize;
            TotalFolders = totalFolders;

            FinishedScan = finishedScan;

            FilesProcessed = filesProcessed;
            FilesSizeProcessed = filesSizeProcessed;
            FoldersProcessed = foldersProcessed;

            Message = message;
        }
    }
}