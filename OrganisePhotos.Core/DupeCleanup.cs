using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrganisePhotos.Core
{
    public class DupeCleanup
    {
        private readonly LocalFolder m_RootFolder;

        public List<LocalFile> ExactDupes { get; private set; }
        public List<LocalFile> NameDupes { get; private set; }

        public DupeCleanup(LocalFolder rootFolder)
        {
            m_RootFolder = rootFolder;
        }

        public async Task<bool> Check()
        {
            return await Task.Run(() =>
                                  {
                                      var allFiles = new List<LocalFile>(m_RootFolder.TotalFiles);
                                      AddFiles(m_RootFolder, allFiles);
                                      return AnalyseForDupes(allFiles);
                                  });
        }

        private static void AddFiles(LocalFolder currentDir, List<LocalFile> allFiles)
        {
            allFiles.AddRange(currentDir.Files);
            foreach (var childDir in currentDir.Folders)
                AddFiles(childDir, allFiles);
        }

        private bool AnalyseForDupes(List<LocalFile> allFiles)
        {
            ExactDupes = allFiles.Where(f => allFiles.Any(f2 => f != f2 &&
                                                                f.File.Name == f2.File.Name &&
                                                                f.File.Length == f2.File.Length)
                                       ).ToList();
            
            NameDupes = allFiles.Where(f => allFiles.Any(f2 => f != f2 &&
                                                               f.File.Name == f2.File.Name &&
                                                               f.File.Length != f2.File.Length)
                                      ).ToList();

            return ExactDupes.Any() || NameDupes.Any();
        }

        public async Task SaveReport(string filename)
        {
            // TEMP stuff - not optimal

            var file = new FileInfo(filename);
            if (file.Exists)
                file.Delete();

            var exact = ExactDupes.Select(f => $"{f.File.Name} {f.File.Length} in {f.File.Directory.FullName}")
                                  .OrderBy(v => v);

            var name = NameDupes.Select(f => $"{f.File.Name} {f.File.Length} in {f.File.Directory.FullName}")
                                .OrderBy(v => v);

            var lines = new List<string>(ExactDupes.Count + NameDupes.Count + 2);
            lines.Add("Exact Matches");
            lines.AddRange(exact);
            lines.Add("Name Matches");
            lines.AddRange(name);

            await File.WriteAllLinesAsync(filename, lines);
        }
    }
}