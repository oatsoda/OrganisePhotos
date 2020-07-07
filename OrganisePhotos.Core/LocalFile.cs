using System.IO;

namespace OrganisePhotos.Core
{
    public class LocalFile
    {
        public FileInfo File { get; }

        public LocalFile(FileInfo file)
        {
            File = file;
        }
    }
}