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
        public DateTime? DateTaken { get; private set; }

        public string DateTakenOriginalRaw { get; private set; }
        public string DateTakenDigitzedRaw { get; private set; }

        public bool DatesTakenOutOfSync => DateTakenRaw != DateTakenDigitzedRaw ||
                                           DateTakenRaw != DateTakenOriginalRaw ||
                                           DateTakenOriginalRaw != DateTakenDigitzedRaw;

        public bool DateTakenMatchesFileLastWrite => DateTaken.HasValue && DatesWithinSeconds(DateTaken.Value, File.LastWriteTime, 1);

        public bool FileDatesMatch => DatesWithinSeconds(File.CreationTime, File.LastWriteTime, 1);

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
            IExifValue<string> rawValueOriginal;
            IExifValue<string> rawValueDigitized;
            await using (var fs = File.OpenRead())
            {
                var info = await Image.IdentifyAsync(fs);
                rawValue = info.Metadata.ExifProfile?.GetValue(ExifTag.DateTime);
                rawValueOriginal = info.Metadata.ExifProfile?.GetValue(ExifTag.DateTimeOriginal);
                rawValueDigitized = info.Metadata.ExifProfile?.GetValue(ExifTag.DateTimeDigitized);
            }

            DateTakenRaw = rawValue?.Value;
            DateTakenOriginalRaw = rawValueOriginal?.Value;
            DateTakenDigitzedRaw = rawValueDigitized?.Value;

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
            if (!IsImage || !DateTakenLoaded || DateTakenValid || !DateTakenFixable)
                return;

            var correctValue = DateTakenCorrectRaw;

            await UpdateDateTaken(correctValue, ExifTag.DateTime);

            DateTakenRaw = correctValue;
            DateTakenValid = true;
            DateTakenFixable = false;
            OnFileUpdated();
        }

        public async Task SetDateTakenManually(DateTime value)
        {
            if (!IsImage)
                return;

            var rawValue = value.ToString("yyyy:MM:dd HH:mm:ss");
            await UpdateDateTaken(rawValue, ExifTag.DateTime);

            DateTakenLoaded = true;
            DateTakenRaw = rawValue;
            DateTakenValid = true;
            DateTakenFixable = false;
            DateTaken = value;
            OnFileUpdated();
        }

        public async Task SyncDatesTaken(SyncDateTaken setFrom)
        {
            if (!IsImage || !DateTakenLoaded)
                return;

            if (setFrom == SyncDateTaken.FromDateTaken && !DateTakenValid)
                return;

            switch (setFrom)
            {
                case SyncDateTaken.FromDateTaken:
                    await UpdateDateTaken(DateTakenRaw, ExifTag.DateTimeOriginal, ExifTag.DateTimeDigitized);
                    DateTakenDigitzedRaw = DateTakenOriginalRaw = DateTakenRaw;
                    break;

                case SyncDateTaken.FromDateDigitized:
                    await UpdateDateTaken(DateTakenDigitzedRaw, ExifTag.DateTime, ExifTag.DateTimeOriginal);
                    DateTakenLoaded = false;
                    await LoadDateTaken();
                    break;

                case SyncDateTaken.FromDateOriginallyTaken:
                    await UpdateDateTaken(DateTakenOriginalRaw, ExifTag.DateTime, ExifTag.DateTimeDigitized);
                    DateTakenLoaded = false;
                    await LoadDateTaken();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(setFrom), setFrom, null);
            }

            OnFileUpdated();
        }
        
        public async Task SetFileDatesFromDateTaken()
        {
            if (!IsImage || !DateTakenLoaded || !DateTakenValid || !DateTaken.HasValue)
                return;

            await Task.Run(() =>
                     {
                         File.LastWriteTime = DateTaken.Value;
                         File.CreationTime = DateTaken.Value;
                     }).ConfigureAwait(false);
            OnFileUpdated();
        }

        public async Task SetCreatedDateFromLastWrite()
        {
            await Task.Run(() =>
                           {
                               File.CreationTime = File.LastWriteTime;
                           }).ConfigureAwait(false);
            OnFileUpdated();
        }

        public async Task SetFileDatesManually(DateTime value)
        {
            await Task.Run(() =>
                           {
                               File.LastWriteTime = value;
                               File.CreationTime = value;
                           }).ConfigureAwait(false);
            OnFileUpdated();
        }
        
        public async Task SetMissingDateTakenFromLastWrite()
        {
            if (!IsImage || !DateTakenLoaded || DateTaken.HasValue)
                return;

            var lastWrite = File.LastWriteTime;
            var rawValue = lastWrite.ToString("yyyy:MM:dd HH:mm:ss");
            await UpdateDateTaken(rawValue, ExifTag.DateTime);

            DateTakenLoaded = true;
            DateTakenRaw = rawValue;
            DateTakenValid = true;
            DateTakenFixable = false;
            DateTaken = lastWrite;
            OnFileUpdated();
        }
        
        private async Task UpdateDateTaken(string dateValue, params ExifTag<string>[] exifDateTags)
        {
            await Task.Run(async () =>
                           {

                               var lastWrite = File.LastWriteTime;
                               var created = File.CreationTime;
                               await WriteExifDateTaken(dateValue, exifDateTags);
                               //Set last write / create back
                               File.LastWriteTime = lastWrite;
                               File.CreationTime = created;
                           }).ConfigureAwait(false);
        }

        private async Task WriteExifDateTaken(string dateValue, params ExifTag<string>[] exifDateTags)
        {
            Image image;
            await using (var fileStream = File.OpenRead())
                image = await Image.LoadAsync(fileStream);

            var exif = image.Metadata.ExifProfile;
            if (exif == null)
            {
                exif = new ExifProfile();
                image.Metadata.ExifProfile = exif;
            }

            foreach (var exifDateTag in exifDateTags)
            {
                var rawValue = exif.GetValue(exifDateTag);
                if (rawValue == null)
                    exif.SetValue(exifDateTag, dateValue);
                else
                    rawValue.TrySetValue(dateValue);
            }

            await image.SaveAsync(File.FullName);
        }

        protected virtual void OnFileUpdated()
        {
            FileUpdated?.Invoke(this, EventArgs.Empty);
        }

        public string DisplayName
        {
            get
            {
                var lastWrite = File.LastWriteTime;
                var created = File.CreationTime;
                var validDisplay = DateTakenValid ? "" : DateTakenFixable ? "Fixable" : "Invalid";
                var dateTaken = !DateTakenLoaded
                                    ? "(not loaded)"
                                    : $"{DateTakenRaw}{validDisplay} [Orig: {DateTakenOriginalRaw} Digit: {DateTakenDigitzedRaw}]";

                return $"{File.Name} | Write: {lastWrite:dd/MM/yyyy HH:mm:ss} | Create: {created:dd/MM/yyyy HH:mm:ss} | Taken: {dateTaken}";
            }
        }

        private static bool DatesWithinSeconds(DateTime one, DateTime two, double seconds) => Math.Abs((one - two).TotalSeconds) <= seconds;

        public enum SyncDateTaken
        {
            FromDateTaken,
            FromDateDigitized,
            FromDateOriginallyTaken
        }
    }
}