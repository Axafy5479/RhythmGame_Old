using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MakeSheetScene
{
    public class MusicSheetSaveSystem
    {

        public MusicSheetSaveSystem(MusicFileData musicData)
        {
            KeyForPlayerPrefs = "madeSheet_" + musicData.MusicId;
            MusicData = musicData;
            Path = directory + musicData.MusicId + ".json";
            Load(musicData);
        }

        public string directory => Application.persistentDataPath + "/MusicSheets2/";
        private string Path { get; }
        public SheetData SheetData { get; private set; }
        public MusicFileData MusicData { get; }
        private string KeyForPlayerPrefs { get; set; }

        public void Save()
        {
            string jsonData = JsonUtility.ToJson(SheetData);

            if (Setting.SAVE_FILE)
            {
                StreamWriter writer = new StreamWriter(Path, false);
                writer.WriteLine(jsonData);
                writer.Flush();
                writer.Close();
            }
            else
            {
                PlayerPrefs.SetString(KeyForPlayerPrefs, jsonData);
            }
        }

        public void Load(MusicFileData musicData)
        {
            if (Setting.SAVE_FILE)
            {
                //Debug.Log(Path);
                if (!File.Exists(directory))
                {
                    new DirectoryInfo(directory).Create();
                    if (!File.Exists(Path))
                    {
                        SheetData = new SheetData();
                        Save();
                    }
                }

                StreamReader reader = new StreamReader(Path);
                string jsonData = reader.ReadToEnd();
                SheetData = JsonUtility.FromJson<SheetData>(jsonData);
                reader.Close();

                SheetData.ResetSheetId();
            }
            else
            {
                if(!PlayerPrefs.HasKey(KeyForPlayerPrefs))
                {
                    SheetData = new SheetData();
                    Save();
                }

                string jsonData = PlayerPrefs.GetString(KeyForPlayerPrefs);
                SheetData = JsonUtility.FromJson<SheetData>(jsonData);
                SheetData.ResetSheetId();
            }

        }




    }
}