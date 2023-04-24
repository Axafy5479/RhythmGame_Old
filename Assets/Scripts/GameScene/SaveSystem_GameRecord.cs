using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GameScene
{
    public class SaveSystem_GameRecord
    {

        public SaveSystem_GameRecord(MusicFileData musicData,Course course)
        {
            MusicData = musicData;
            directory = $"{Application.persistentDataPath}/PlayData/{musicData.MusicId}/{course}";
       
            Load(musicData);
        }

        public string directory { get; }

        public MusicFileData MusicData { get; }

        public void Save(int score,Record record)
        {

            DirectoryInfo di = new DirectoryInfo(directory);
            
            if (!di.Exists)
            {
                di.Create();
            }
            else
            {
                FileInfo[] files = di.GetFiles("*", System.IO.SearchOption.AllDirectories);

                List<int> scores = new List<int>();

                foreach (var f in files)
                {
                    try
                    {
                        scores.Add(int.Parse(f.Name));
                    }
                    catch
                    {
                        Debug.Log("ファイル名が不正です");
                    }

                }

                if (scores.All(s => s < score))
                {
                    files.For((x, i) => x.Delete());
                }

            }






            string path = directory +"/"+ score;
            string jsonData = JsonUtility.ToJson(record);
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(jsonData);
            writer.Flush();
            writer.Close();
        }

        public void Load(MusicFileData musicData)
        {
            //Debug.Log(Path);
            //if (!File.Exists(directory))
            //{
            //    new DirectoryInfo(directory).Create();
            //    if (!File.Exists(Path))
            //    {
            //        SheetData = new SheetData();
            //        Save();
            //    }
            //}

            //StreamReader reader = new StreamReader(Path);
            //string jsonData = reader.ReadToEnd();
            //SheetData = JsonUtility.FromJson<SheetData>(jsonData);
            //reader.Close();
        }




    }
}
