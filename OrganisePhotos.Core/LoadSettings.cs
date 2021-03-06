﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrganisePhotos.Core
{
    public class LoadSettings 
    {
        public string RootFolderPath
        {
            get => m_RootFolderPath;
            set => m_RootFolderPath = value.TrimEnd('\\', '/');
        }

        public List<string> IgnoreFoldersStartingWith { get; set; }

        public DirectoryInfo RootFolderDir => new DirectoryInfo(RootFolderPath);

        private bool? m_IsValid;
        private string m_RootFolderPath;

        public bool Validate(out IEnumerable<string> errors)
        {
            errors = null;

            // TODO: Make type immutable
            if (m_IsValid.HasValue)
                return m_IsValid.Value;
            
            var errorList = new List<string>(2);

            if (!RootFolderDir.Exists)
                errorList.Add($"Folder does not exist: {RootFolderDir.FullName}");
            
            errors = errorList;
            m_IsValid = !errorList.Any();
            return m_IsValid.Value;
        }

        private static readonly List<string> s_ImageExtensions = new List<string>
                                                                 {
                                                                     ".bmp",
                                                                     ".gif",
                                                                     ".jpg",
                                                                     ".jpeg",
                                                                     ".png"
                                                                 };

        public List<string> ImageExtensions()
        {
            return s_ImageExtensions;
        }

        public bool IsImage(FileInfo file)
        {
            return ImageExtensions().Contains(file.Extension.ToLower());
        }

        internal bool IsFolderIgnored(DirectoryInfo dir)
        {
            return IgnoreFoldersStartingWith.Any(f => dir.Name.StartsWith(f, StringComparison.CurrentCultureIgnoreCase));
        }

        internal bool IsFileIgnored(FileInfo file)
        {
            return string.Equals(file.Name, "thumbs.db", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}