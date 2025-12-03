namespace ExecViewHrk.WebUI.Helpers
{
    public class CustomErrorMessages
    {
        public const string ERROR_RECORD_ALREADY_IN_USE = "Record can not be deleted." + "<br />" + "Record already in use or does not exists.";

        public const string ERROR_ALREADY_DEFINED = " already exists.";
        public const string ERROR_LOOKUP_ALREADY_DEFINED = " with same code or description already exists.";
        public const string ERROR_LOOKUP_DUPLICATE_RECORD = "{0} with same code or description already exists.";
        //public const string ERROR_CODE_CANNOT_BE_INACTIVE = " Code can not be marked In-Active, Record already in use";

        public const string ERROR_CANNOT_BE_INACTIVE = " Can not Inactive due to record is in Use";

        public const string ERROR_DUPLICATE_RECORD = "Record with same {0} already exist.";

        public const string ERROR_CANNOT_BE_INACTIVE_Active = "Can not Inactive due to record is in Use";

        public const string ERROR_NOCHANGES_RECORD = "No changes to update the record.";
        //public const string ERROR_RECORD_NOT_UPDATED = "<br />" + "Record could not be updated at this time.";
        public const string ERROR_DELETING_RECORD_ALREADY_IN_USE = "Record already in use!";
    }
}