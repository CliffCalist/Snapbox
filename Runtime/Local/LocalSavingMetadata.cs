using System;
using System.IO;
using UnityEngine;

namespace WhiteArrow.DataSaving
{
    public class LocalSavingMetadata : ISavingMetadata
    {
        public string DataName { get; private set; }
        public Type DataType { get; private set; }
        public object FolderPath { get; private set; }
        public string CastedFolderPath { get; private set; }
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }


        public static string SavingsFolderPath => Path.Combine(Application.persistentDataPath, "DS");


        public LocalSavingMetadata(string dataName, Type dataType, string folderPath = null)
        {
            if (string.IsNullOrWhiteSpace(dataName))
                throw new ArgumentException(nameof(dataName));
            DataName = dataName;

            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType), "Data type cannot be null.");

            if (!string.IsNullOrWhiteSpace(folderPath))
                CastedFolderPath = Path.Combine(SavingsFolderPath, folderPath);
            else CastedFolderPath = SavingsFolderPath;
            FolderPath = CastedFolderPath;
        }
    }
}