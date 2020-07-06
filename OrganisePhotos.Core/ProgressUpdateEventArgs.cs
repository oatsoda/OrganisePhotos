namespace OrganisePhotos.Core
{
    public class ProgressUpdateEventArgs
    {
        public int FilesProcessed { get; }
        public long FilesSizeProcessed { get; }
        public int FoldersProcessed { get; set; }
        public string Message { get; }

        public ProgressUpdateEventArgs(int filesProcessed, long filesSizeProcessed, int foldersProcessed, string message)
        {
            FilesProcessed = filesProcessed;
            FilesSizeProcessed = filesSizeProcessed;
            FoldersProcessed = foldersProcessed;
            Message = message;
        }
    }
}