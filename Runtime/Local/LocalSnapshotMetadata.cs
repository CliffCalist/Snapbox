using System;
using System.IO;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class LocalSnapshotMetadata : ISnapshotMetadata
    {
        public string SnapshotName { get; private set; }
        public Type SnapshotType { get; private set; }
        public string CastedFolderPath { get; private set; }
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }


        public static string SavingsFolderPath => Path.Combine(Application.persistentDataPath, "Snapbox");


        public LocalSnapshotMetadata(string snapshotName, Type snapshotType, string folderPath = null)
        {
            if (string.IsNullOrWhiteSpace(snapshotName))
                throw new ArgumentException(nameof(snapshotName));

            SnapshotName = snapshotName;
            SnapshotType = snapshotType ?? throw new ArgumentNullException(nameof(snapshotType), "Snapshot type cannot be null.");

            if (!string.IsNullOrWhiteSpace(folderPath))
                CastedFolderPath = Path.Combine(SavingsFolderPath, folderPath);
            else CastedFolderPath = SavingsFolderPath;
        }
    }
}