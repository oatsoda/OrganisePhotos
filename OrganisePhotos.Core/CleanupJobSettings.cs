using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrganisePhotos.Core
{
    public class CleanupJobSettings
    {
        public string RootFolderPath { get; set; }
        public List<string> IgnoreFoldersStartingWith { get; set; }
        public CleanupAction FixIncorrectDateTakenFormat { get; set; }
        public CleanupAction ChangeCreatedDateToDateTaken { get; set; }
        public CleanupAction SetDateTakenFromCreatedDateIfNotSet { get; set; }
        public CleanupAction RenameDupeFiles { get; set; }

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

        private static readonly List<string> s_ImageExtensions = new List<string>
                                                                 {
                                                                     ".bmp",
                                                                     ".gif",
                                                                     ".heic",
                                                                     ".ico",
                                                                     ".jpg",
                                                                     ".jpeg",
                                                                     ".png",
                                                                     ".tiff"
                                                                 };

        public List<string> ImageExtensions()
        {
            return s_ImageExtensions;
        }
    }
}