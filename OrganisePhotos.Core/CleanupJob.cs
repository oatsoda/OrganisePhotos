using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class CleanupJob
    {
        private readonly CleanupJobSettings m_Settings;
        private DirectoryInfo RootFolder => m_Settings.RootFolderDir;
        
        private readonly List<DirectoryInfo> m_FoldersFound = new List<DirectoryInfo>();
        private readonly List<FileInfo> m_FilesFound = new List<FileInfo>();
        
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        public CleanupJob(CleanupJobSettings settings)
        {
            m_Settings = settings;
            if (!m_Settings.Validate(out var errors))
                throw new ArgumentException($"Invalid settings:\r\n{string.Join("\r\n", errors)}");
            
            /*
             * Cycle through all folders and files and:
             *
             * - Flag up unknown file types (non-video/image: e.g. https://developers.google.com/photos/library/guides/upload-media#file-types-sizes) and prompt to delete?
             *     > Ignore known files like Thumbs.db ?
             * - Build up list of filenames across the whole tree and then detect dupes and suggest renaming by prefixing date taken?
             * - Detect missing Date Taken?
             * - Detect Date Taken which looks partial (1st month, no time etc?)
             * - Detect files with invalid format of Date Taken: yyyy:MM:dd:HH:mm:ss
             * - Update file Create Date from Date Taken             *
             */

        }

        public async Task Run()
        {
            await Task.Run(async () =>
                           {
                               var result = await ProcessDirectory(RootFolder);
                               if (result == false)
                                   OnProgress("Run cancelled.");
                           });
        }

        private async Task<bool> ProcessDirectory(DirectoryInfo dir)
        {
            if (dir != RootFolder)
                DirFound(dir);
            else
                OnProgress($"Starting scan on root folder: {dir.FullName}");

            foreach (var file in dir.EnumerateFiles())
                if (!await ProcessFile(file))
                    return false;

            foreach (var subDir in dir.EnumerateDirectories())
                if (!await ProcessDirectory(subDir))
                    return false;

            return true;
        }

        private async Task<bool> ProcessFile(FileInfo file)
        {
            m_FilesFound.Add(file);
            OnProgress(null);

            if (IsFileIgnored(file))
                return true;

            // TODO: Detect if image file
            if (!await GetDateTakenFromImage(file))
                return false;

            return true;
        }

        private void DirFound(DirectoryInfo dir)
        {
            m_FoldersFound.Add(dir);
            OnProgress($"Folder found: {dir.FullName}");
        }

        private bool IsFileIgnored(FileInfo file)
        {
            return string.Equals(file.Name, "thumbs.db", StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task<bool> GetDateTakenFromImage(FileInfo imageFile)
        {
            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Ignore)
                return true;

            IExifValue<string> rawValue;
            await using (var fs = imageFile.OpenRead())
            {
                var info = await Image.IdentifyAsync(fs);
                rawValue = info.Metadata.ExifProfile?.GetValue(ExifTag.DateTime);
            }
            
            if (rawValue == null || string.IsNullOrWhiteSpace(rawValue.Value))
            {
                Debug.WriteLine($"Empty Date value [{imageFile.Name}]");
                return true;
            }

            if (DateTime.TryParseExact(rawValue.Value, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out var dateValue))
            {
                Debug.WriteLine($"Correct Date value: '{dateValue:O}' [{imageFile.Name}]");
                return true;
            }

            if (!DateTime.TryParseExact(rawValue.Value, "yyyy:MM:dd:HH:mm:ss", null, DateTimeStyles.None, out dateValue))
                return true; // Date format not fixable as not known

            var newValue = dateValue.ToString("yyyy:MM:dd HH:mm:ss");

            if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Prompt)
            {
                var result = m_Settings.Prompt($"Replace Date Taken '{rawValue.Value}' with '{newValue}' for file {imageFile.FullName}");
                if (result != PromptResult.Fix)
                    return result.ToReturnValue();
            }
            else if (m_Settings.FixIncorrectDateTakenFormat == CleanupAction.Log)
            {
                OnProgress($"[Log] Could fix Date Taken '{rawValue.Value}' with '{newValue}' for file {imageFile.FullName}");
                return true;
            }
                
            await using var filestream = imageFile.OpenRead();
            using var image = await Image.LoadAsync(filestream);
            var exif = image.Metadata.ExifProfile;
            rawValue = exif.GetValue(ExifTag.DateTime);
            rawValue.TrySetValue(newValue);
            await image.SaveAsync(imageFile.FullName);

            OnProgress($"Fixed Date Taken on {imageFile.FullName}");

            return true;
        }
        
        private void OnProgress(string message)
        {
            // TODO: optimise this
            var filesProcessed = m_FilesFound.Count;
            var filesSizeProcessed = m_FilesFound.Sum(f => f.Length);
            var foldersProcessed = m_FoldersFound.Count;
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs(filesProcessed, filesSizeProcessed, foldersProcessed, message));
        }
    }

    public class CleanupJobSettings
    {
        public string RootFolderPath { get; set; }
        public CleanupAction FixIncorrectDateTakenFormat { get; set; }
        public CleanupAction ChangeCreatedDateToDateTaken { get; set; }
        public CleanupAction SetDateTakenFromCreatedDateIfNotSet { get; set; }

        public Func<string, PromptResult> Prompt { get; set; }

        public DirectoryInfo RootFolderDir { get; private set; }

        public bool Validate(out IEnumerable<string> errors)
        {
            RootFolderDir = new DirectoryInfo(RootFolderPath);

            var errorList = new List<string>(2);

            if (!RootFolderDir.Exists)
                errorList.Add($"Folder does not exist: {RootFolderDir.FullName}");

            if (Prompt == null)
                errorList.Add("You must supply a Prompt action.");

            errors = errorList;
            return !errorList.Any();
        }
    }

    public enum CleanupAction
    {
        Ignore = 0,
        Log,
        Prompt,
        Fix
    }

    public enum PromptResult
    {
        Skip = 0,
        Fix,
        Exit
    }

    public static class PromptResultExtensions
    {
        public static bool ToReturnValue(this PromptResult result)
        {
            return result != PromptResult.Exit;
        }
    }
    
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
