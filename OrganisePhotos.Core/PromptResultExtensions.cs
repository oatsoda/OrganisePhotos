namespace OrganisePhotos.Core
{
    public static class PromptResultExtensions
    {
        public static bool ToReturnValue(this PromptResult result)
        {
            return result != PromptResult.Exit;
        }
    }
}