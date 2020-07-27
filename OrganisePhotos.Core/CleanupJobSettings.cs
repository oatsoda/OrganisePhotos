namespace OrganisePhotos.Core
{
    public class CleanupJobSettings
    {
        public CleanupAction AppendDateIfShortFileName { get; set; }
        public bool ReportFileDatesOutOfSync { get; set; }
    }
}