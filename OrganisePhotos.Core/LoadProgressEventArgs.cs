namespace OrganisePhotos.Core
{
    public class LoadProgressEventArgs
    {
        public int TotalFiles { get; }
        public long TotalFileSize { get; }
        public int TotalFolders { get; }

        public bool LoadCompleted { get; }

        public LoadProgressEventArgs(int totalFiles, long totalFileSize, int totalFolders, bool loadCompleted)
        {
            TotalFiles = totalFiles;
            TotalFileSize = totalFileSize;
            TotalFolders = totalFolders;

            LoadCompleted = loadCompleted;
        }
    }
}