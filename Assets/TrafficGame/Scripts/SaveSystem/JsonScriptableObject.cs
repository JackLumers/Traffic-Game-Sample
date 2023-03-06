using System.IO;
using UnityEngine;

namespace TrafficGame.Scripts.SaveSystem
{
    public abstract class JsonScriptableObject : ScriptableObject
    {
        private const string SavedDataDictionary = "SavedData";

        public abstract void ResetToDefaultValues();

        public void SaveToFile()
        {
            var dirPath = Path.Combine(Application.persistentDataPath, SavedDataDictionary);
            
            var filePath = Path.Combine(dirPath, $"{name}.json");

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            var json = JsonUtility.ToJson(this);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile()
        {
            var filePath = Path.Combine(Application.persistentDataPath, SavedDataDictionary, $"{name}.json");

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"File \"{filePath}\" not found! Current values stayed.", this);
                return;
            }

            var json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public void DeleteSave()
        {
            if(!IsSaveExist()) return;
            
            var filePath = Path.Combine(Application.persistentDataPath, SavedDataDictionary, $"{name}.json");
            File.Delete(filePath);
        }

        public bool IsSaveExist()
        {
            var filePath = Path.Combine(Application.persistentDataPath, SavedDataDictionary, $"{name}.json");

            return File.Exists(filePath);
        }

#if UNITY_EDITOR
        [ContextMenu("Delete Save")]
        private void ContextMenuDeleteSave()
        {
            DeleteSave();
        }
        
        [ContextMenu("Load From Save")]
        private void ContextMenuLoadFromSave()
        {
            LoadFromFile();
        }
#endif
    }
}