using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace OrganisePhotos.Core
{
    public class LocalFile
    {

        public FileInfo File { get; }

        public bool IsImage { get; }
        
        public bool DateTakenLoaded { get; private set; }
        public string DateTakenRaw { get; private set; }
        public bool DateTakenValid { get; private set; }
        public bool DateTakenFixable { get; private set; }
        public DateTime? DateTaken { get; private set;  }

        public string DateTakenCorrectRaw => DateTakenFixable ? DateTaken?.ToString("yyyy:MM:dd HH:mm:ss") : null;

        public LocalFile(FileInfo file, CleanupJobSettings settings)
        {
            File = file;
            IsImage = settings.ImageExtensions().Contains(File.Extension.ToLower());
        }

        public async Task LoadDateTaken()
        {
            if (DateTakenLoaded || !IsImage)
                return;

            DateTakenLoaded = true;

            IExifValue<string> rawValue;
            await using (var fs = File.OpenRead())
            {
                var info = await Image.IdentifyAsync(fs);
                rawValue = info.Metadata.ExifProfile?.GetValue(ExifTag.DateTime);
            }

            DateTakenRaw = rawValue?.Value;

            if (string.IsNullOrWhiteSpace(DateTakenRaw))
                return;

            if (DateTime.TryParseExact(DateTakenRaw, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out var dateTaken))
            {
                DateTaken = dateTaken;
                DateTakenValid = true;
                return;
            }

            if (DateTime.TryParseExact(DateTakenRaw, "yyyy:MM:dd:HH:mm:ss", null, DateTimeStyles.None, out var incorrectDateTaken))
            {
                DateTaken = incorrectDateTaken;
                DateTakenFixable = true;
            }
        }

        public async Task FixInvalidDateTaken()
        {
            if (DateTakenValid || !DateTakenFixable)
                return;

            var correctValue = DateTakenCorrectRaw;

            Image image;
            await using (var fileStream = File.OpenRead())
                image = await Image.LoadAsync(fileStream);

            var exif = image.Metadata.ExifProfile;
            var rawValue = exif.GetValue(ExifTag.DateTime);
            rawValue.TrySetValue(correctValue);
            await image.SaveAsync(File.FullName);

            DateTakenRaw = correctValue;
            DateTakenValid = true;
            DateTakenFixable = false;
        }
    }
}