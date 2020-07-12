using System;
using System.Collections.Generic;
using System.Linq;

namespace OrganisePhotos.Core
{
    public class CleanupJobSettings
    {
        public CleanupAction FixIncorrectDateTakenFormat { get; set; }
        public CleanupAction ChangeCreatedDateToDateTaken { get; set; }
        public CleanupAction SetDateTakenFromCreatedDateIfNotSet { get; set; }
        public CleanupAction RenameDupeFiles { get; set; }

        public Func<string, PromptResult> Prompt { get; set; }

        private CleanupAction[] SettingValues => new[]
                                                 {
                                                     FixIncorrectDateTakenFormat,
                                                     ChangeCreatedDateToDateTaken,
                                                     SetDateTakenFromCreatedDateIfNotSet,
                                                     RenameDupeFiles
                                                 };

        public bool Validate(out IEnumerable<string> errors)
        {
            var errorList = new List<string>(1);

            if (SettingValues.All(v => v == CleanupAction.Ignore))
                errorList.Add("At least one option must not be Ignore.");
            
            if (SettingValues.Any(v => v == CleanupAction.Prompt) && Prompt == null)
                errorList.Add("You must supply a Prompt action.");
            
            errors = errorList;
            return !errorList.Any();
        }

    }
}