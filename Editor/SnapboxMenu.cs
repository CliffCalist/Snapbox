using System.Diagnostics;
using System.IO;
using UnityEditor;
using WhiteArrow.Snapbox;

namespace WhiteArrowEditor.SnapboxSDK
{
    public static class SnapboxMenu
    {
        [MenuItem("Tools/WhiteArrow/Snapbox/Open snapshots folder")]
        private static void OpenSnapshotsFolder()
        {
            if (Directory.Exists(LocalSnapshotMetadata.SavingsFolderPath))
                Process.Start(LocalSnapshotMetadata.SavingsFolderPath);
            else UnityEngine.Debug.Log("Folder isn't exist.");
        }

        [MenuItem("Tools/WhiteArrow/Snapbox/Delete saved snapshots")]
        private static void DeleteSavedSnapshots()
        {
            if (Directory.Exists(LocalSnapshotMetadata.SavingsFolderPath))
            {
                Directory.Delete(LocalSnapshotMetadata.SavingsFolderPath, true);
                UnityEngine.Debug.Log("Folder with saved snapshots has be deleted.");
            }
        }
    }
}