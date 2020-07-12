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

        public event EventHandler FileUpdated;

        public LocalFile(FileInfo file, LoadSettings settings)
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
            {
                OnFileUpdated();
                return;
            }

            if (DateTime.TryParseExact(DateTakenRaw, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out var dateTaken))
            {
                DateTaken = dateTaken;
                DateTakenValid = true;
            }
            else if (DateTime.TryParseExact(DateTakenRaw, "yyyy:MM:dd:HH:mm:ss", null, DateTimeStyles.None, out var incorrectDateTaken))
            {
                DateTaken = incorrectDateTaken;
                DateTakenFixable = true;
            }
            
            OnFileUpdated();
        }

        public async Task FixInvalidDateTaken()
        {
            if (!DateTakenLoaded || DateTakenValid || !DateTakenFixable)
                return;

            var correctValue = DateTakenCorrectRaw;

            await UpdateDateTaken(correctValue);

            DateTakenRaw = correctValue;
            DateTakenValid = true;
            DateTakenFixable = false;
            OnFileUpdated();
        }
        
        public async Task SetDateTakenManually(DateTime value)
        {
            var rawValue = value.ToString("yyyy:MM:dd HH:mm:ss");
            await UpdateDateTaken(rawValue);

            DateTakenLoaded = true;
            DateTakenRaw = rawValue;
            DateTakenValid = true;
            DateTakenFixable = false;
            DateTaken = value;
            OnFileUpdated();
        }

        public async Task SetFileDatesFromDateTaken()
        {
            if (!DateTakenLoaded || !DateTakenValid || !DateTaken.HasValue)
                return;

            await Task.Run(() =>
                     {
                         File.LastWriteTime = DateTaken.Value;
                         File.CreationTime = DateTaken.Value;
                     });
            OnFileUpdated();
        }
        
        public async Task SetMissingDateTakenFromLastWrite()
        {
            if (!DateTakenLoaded || DateTaken.HasValue)
                return;

            var lastWrite = File.LastWriteTime;
            var rawValue = lastWrite.ToString("yyyy:MM:dd HH:mm:ss");
            await UpdateDateTaken(rawValue);

            DateTakenLoaded = true;
            DateTakenRaw = rawValue;
            DateTakenValid = true;
            DateTakenFixable = false;
            DateTaken = lastWrite;
            OnFileUpdated();
        }
        
        private async Task UpdateDateTaken(string dateValue)
        {
            Image image;
            await using (var fileStream = File.OpenRead())
                image = await Image.LoadAsync(fileStream);

            var exif = image.Metadata.ExifProfile;
            var rawValue = exif.GetValue(ExifTag.DateTime);
            rawValue.TrySetValue(dateValue);
            await image.SaveAsync(File.FullName);
        }
        
        protected virtual void OnFileUpdated()
        {
            FileUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}