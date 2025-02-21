using System;

namespace WhiteArrow.DataSaving
{
    public interface ISavingMetadata
    {
        public string DataName { get; }
        public Type DataType { get; }
        public object FolderPath { get; }
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }
    }
}
