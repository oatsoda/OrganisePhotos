using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace OrganisePhotos.Core
{
    public class CleanupFile
    {
        private readonly FileInfo m_FileInfo;
        private readonly CleanupJobSettings m_Settings;
        private readonly Action<string> m_ProgressAction;
        
        private bool m_ExitRequested;

        private string m_RawDateTaken;
        private DateTime? m_DateTaken;
        private bool m_DateTakenFixable;

        public CleanupFile(FileInfo fileInfo, CleanupJobSettings settings, Action<string> progressAction)
        {
            m_FileInfo = fileInfo;
            m_Settings = settings;
            m_ProgressAction = progressAction;
        }

        public async Task<bool> ProcessFile()
        {
            await ReadExifRawDateTaken();
            ParseExifRawDateTaken();
            await FixIncorrectDateTakenFormat();

            if (m_ExitRequested)
                return false;

            await FixMissingDateTaken();
            
            if (m_ExitRequested)
                return false;

            await ChangeCreatedDateToDateTaken();

            return !m_ExitRequested;
        }

        private async Task ReadExifRawDateTaken()
        {
            // Don't read it if we don't need it
            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Ignore && 
                m_Settings.ChangeCreatedDateToDateTaken == CleanupAction.Ignore &&
                m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Ignore)
                return;

            IExifValue<string> rawValue;
            await using (var fs = m_FileInfo.OpenRead())
            {
                var info = await Image.IdentifyAsync(fs);
                rawValue = info.Metadata.ExifProfile?.GetValue(ExifTag.DateTime);
            }

            m_RawDateTaken = string.IsNullOrWhiteSpace(rawValue?.Value) 
                                 ? null 
                                 : rawValue.Value;
        }

        private void ParseExifRawDateTaken()
        {
            // Don't read it if we don't need it
            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Ignore && 
                m_Settings.ChangeCreatedDateToDateTaken == CleanupAction.Ignore &&
                m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Ignore)
                return;

            if (m_RawDateTaken == null)
                return;

            if (DateTime.TryParseExact(m_RawDateTaken, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out var dateTaken))
            {
                m_DateTaken = dateTaken;
                return;
            }

            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Ignore)
                return;

            if (DateTime.TryParseExact(m_RawDateTaken, "yyyy:MM:dd:HH:mm:ss", null, DateTimeStyles.None, out var incorrectDateTaken))
            {
                m_DateTaken = incorrectDateTaken;
                m_DateTakenFixable = true;
                return;
            }
            // TODO: Other expected broken formats?
            
            // If one of the fixes requiring Date Taken is ON then just log if encounter invalid format
            m_ProgressAction($"[Log] Unrecognised Date Taken '{m_RawDateTaken}' for file {m_FileInfo.FullName}");
        }

        private async Task FixIncorrectDateTakenFormat()
        {
            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Ignore)
                return;

            if (!m_DateTakenFixable)
                return;

            var newValue = m_DateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss");

            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Could fix Date Taken '{m_RawDateTaken}' with '{newValue}' for file {m_FileInfo.FullName}");
                return;
            }

            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Prompt)
            {
                var result = Prompt($"Replace Date Taken '{m_RawDateTaken}' with '{newValue}' for file {m_FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return;
            }

            await SetDateTaken(newValue);
            m_RawDateTaken = newValue;
            m_DateTakenFixable = false;
            
            m_ProgressAction($"Fixed Date Taken on {m_FileInfo.FullName}");
        }
        
        private async Task FixMissingDateTaken()
        {
            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Ignore)
                return;
            
            if (m_DateTaken.HasValue)
                return;

            var createdDate = m_FileInfo.CreationTime.ToString("yyyy:MM:dd HH:mm:ss");

            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Could set missing Date Taken to Created Date '{createdDate}' for file {m_FileInfo.FullName}");
                return;
            }

            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Prompt)
            {
                var result = Prompt($"Set missing Date Taken to Created Date '{createdDate}' for file {m_FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return;
            }

            // TODO set value
        }
        
        private async Task ChangeCreatedDateToDateTaken()
        {
            if (m_Settings.ChangeCreatedDateToDateTaken == CleanupAction.Ignore)
                return;

            if (!m_DateTaken.HasValue)
                return;
            
            var createdDate = m_FileInfo.CreationTime.ToString("O");
            var dateTaken = m_DateTaken.Value.ToString("O");

            if (m_Settings.ChangeCreatedDateToDateTaken == CleanupAction.Log)
            {
                m_ProgressAction($"[Log] Could change Created Date from '{createdDate}' to Date Taken to '{dateTaken}' for file {m_FileInfo.FullName}");
                return;
            }

            if (m_Settings.SetDateTakenFromCreatedDateIfNotSet == CleanupAction.Prompt)
            {
                var result = Prompt($"Change Created Date from '{createdDate}' to Date Taken to '{dateTaken}' for file {m_FileInfo.FullName}");
                if (result != PromptResult.Fix)
                    return;
            }

            // TODO set value
        }

        private async Task SetDateTaken(string newRawValue)
        {
            await using var fileStream = m_FileInfo.OpenRead();
            using var image = await Image.LoadAsync(fileStream);
            var exif = image.Metadata.ExifProfile;
            var rawValue = exif.GetValue(ExifTag.DateTime);
            rawValue.TrySetValue(newRawValue);
            await image.SaveAsync(m_FileInfo.FullName);
        }

        private PromptResult Prompt(string message)
        {
            var result = m_Settings.Prompt(message);
            if (result == PromptResult.Exit)
                m_ExitRequested = true;

            return result;
        }
    }
}