using System.Diagnostics;
using System.IO;
using UnityEditor;
using WhiteArrow.DataSaving;

namespace WhiteArrowEditor.DataSaving
{
    public static class DataSavingMenu
    {
        [MenuItem("Tools/WhiteArrow/Local Database/Open data folder")]
        private static void OpenDataFolder()
        {
            if (Directory.Exists(LocalSavingMetadata.SavingsFolderPath))
                Process.Start(LocalSavingMetadata.SavingsFolderPath);
            else UnityEngine.Debug.Log("Folder isn't exist.");
        }

        [MenuItem("Tools/WhiteArrow/Local Database/Delete saved data")]
        private static void DeleteSavedData()
        {
            if (Directory.Exists(LocalSavingMetadata.SavingsFolderPath))
            {
                Directory.Delete(LocalSavingMetadata.SavingsFolderPath, true);
                UnityEngine.Debug.Log("Folder with saved data has be deleted.");
            }
        }
    }
}