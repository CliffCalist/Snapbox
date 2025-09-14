using System;

namespace WhiteArrow.Snapbox
{
    public class MigrationResult
    {
        public MigrationStatus Status { get; }
        public Exception Exception { get; }
        public string MissingSnapshot { get; }



        private MigrationResult(MigrationStatus status, Exception exception = null, string missing = null)
        {
            Status = status;
            Exception = exception;
            MissingSnapshot = missing;
        }

        public static MigrationResult Success()
        {
            return new MigrationResult(MigrationStatus.Success);
        }

        public static MigrationResult MissingData(string missing)
        {
            return new MigrationResult(MigrationStatus.MissingData, null, missing);
        }

        public static MigrationResult Error(Exception ex)
        {
            return new MigrationResult(MigrationStatus.Error, ex);
        }
    }
}