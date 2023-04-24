using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace MakeSheetScene
{
    public enum NoteTypeForSheet
    {
        None = 0,
        Normal = 1,
        LongBegin = 2,
        LongMiddle = 3,
        LongEnd = 4,
        BothNormal = 100
    }

    [System.Serializable]
    public class SheetData
    {
        
        public float offset = 0;
        [SerializeField] private int allNoteNumber_player;//セーブ時に子の変数を変更
        [SerializeField] private int allNoteNumber_rival;//セーブ時に子の変数を変更
        [SerializeField] private string sheetId = "";//サーバーにアップロードする際に設定される
        [SerializeField] private int nextId = 0;
        public List<BarData> barDatas;
        public int AllBarNum => barDatas.Count;
        public int AllNoteNumber_player { get => allNoteNumber_player; private set => allNoteNumber_player = value; }
        public int AllNoteNumber_rival { get => allNoteNumber_rival; private set => allNoteNumber_rival = value; }

        public int GetNoteId()
        {
            nextId++;
            return nextId-1;
        }

        private bool Initialized { get; set; } = false;


        private (int,int) InitializeAllNoteNumber()
        {
            int num_player = 0;
            int num_rival = 0;


            for (int bar = 0; bar < AllBarNum; bar++)
            {
                var n = barDatas[bar].InitializeNotesNumber(UpdateNoteNumber);
                num_player += n.Item1;
                num_rival += n.Item2;

            }
            Initialized = true;
            return (num_player, num_rival);
        }


        public void SetSheetId(string newSheetId)
        {
            if (sheetId != "")
            {
                Debug.LogError("既に設定されています。");
            }
            else
            {
                sheetId = newSheetId;
            }
        }

        public void ResetSheetId()
        {
            sheetId = "";
        }

        public string GetSheetId()
        {
            return sheetId;
        }

        public int UpdateNoteNumber(bool isPlayer)
        {
            if(!Initialized)
            {
                var n = InitializeAllNoteNumber();
                allNoteNumber_player = n.Item1;
                allNoteNumber_rival = n.Item2;
                return isPlayer ? allNoteNumber_player : allNoteNumber_rival;
            }



            if (isPlayer)
            {
                AllNoteNumber_player = 0;

                foreach (var bar in barDatas)
                {
                    AllNoteNumber_player += bar.AllNoteNumber_player;
                }
                return AllNoteNumber_player;
            }
            else
            {
                AllNoteNumber_rival = 0;

                foreach (var bar in barDatas)
                {
                    AllNoteNumber_rival += bar.AllNoteNumber_rival;
                }
                return AllNoteNumber_rival;
            }
        }


        public BarData AddBar()
        {
            float bpm = barDatas[AllBarNum - 1].BPM;

            BarData newBarData = new BarData(AllBarNum,4, bpm, UpdateNoteNumber);

            barDatas.Add(newBarData);

            return newBarData;
        }

        public void InitializeOneBar(int barNum,int containsDivNum)
        {
            float prevBpm = barDatas[barNum].BPM;
            barDatas[barNum] = new BarData(barNum, containsDivNum, prevBpm, UpdateNoteNumber);
        }

        public SheetData()
        {
            barDatas = new List<BarData>();
            barDatas.Add(new BarData(0,4, 100, UpdateNoteNumber));
        }
    }


    [System.Serializable]
    public class BarData
    {
        
        public float BPM;
        [SerializeField]
        private DivData[] d;
        public DivData[] DivDatas { get => d; set => d = value; }
        private int BarNum { get; }

        public int AllNoteNumber_player { get; private set; } = -999;
        public int AllNoteNumber_rival { get; private set; } = -99;
        private Func<bool, int> NotenumChanged { get; set; }


        public void FindParentById_SetNewChild(bool isPlayer,int parentId,int newChildId)
        {
            foreach (var div in DivDatas)
            {
                int[][] notes = div.GetNotes(isPlayer);
                for (int pos = 0; pos < 4; pos++)
                {
                    if(notes[pos][2] == parentId)
                    {
                        div.SetNewChild(isPlayer, newChildId, pos);
                        return;
                    }
                }
            }
            Debug.LogError("見つかりません");
        }

        public void FindChildById_SetNewParent(bool isPlayer, int childId, int newParentId)
        {
            foreach (var div in DivDatas)
            {
                int[][] notes = div.GetNotes(isPlayer);
                for (int pos = 0; pos < 4; pos++)
                {
                    if (notes[pos][2] == childId)
                    {
                        div.SetNewParent(isPlayer, newParentId, pos);
                        return;
                    }
                }
            }
            Debug.LogError("見つかりません");

        }

        public (int,int) InitializeNotesNumber(Func<bool, int> notenumChanged)
        {
            NotenumChanged = notenumChanged;
            int num_player = 0;
            int num_rival = 0;


            for (int j = 0; j < DivDatas.Length; j++)
            {
                var n = DivDatas[j].InitializeNotesNumber(UpdateNoteNumber);
                num_player += n.Item1;
                num_rival += n.Item2;
            }

            AllNoteNumber_player = num_player;
            AllNoteNumber_rival = num_rival;

            return (num_player, num_rival);
        }

        private int UpdateNoteNumber(bool isPlayer)
        {
            if (isPlayer)
            {
                AllNoteNumber_player = 0;

                foreach (var div in DivDatas)
                {
                    AllNoteNumber_player += div.AllNotesNumber_player;
                }
            }
            else
            {
                AllNoteNumber_rival = 0;

                foreach (var div in DivDatas)
                {
                    AllNoteNumber_rival += div.AllNotesNumber_rival;
                }
            }

            return NotenumChanged(isPlayer);
        }

        public BarData(int barNum,int division, float bpm,Func<bool,int> notenumChanged)
        {
            AllNoteNumber_player = 0;
            AllNoteNumber_rival = 0;
            NotenumChanged = notenumChanged;
            BarNum = barNum;
            BPM = bpm;
            DivDatas = new DivData[division];
            for (int i = 0; i < division; i++)
            {
                DivDatas[i] = new DivData(BarNum, UpdateNoteNumber);
            }
        }
    }

    [System.Serializable]
    public class DivData
    {
        [SerializeField]
        private float m;//bpmModify (ストレージの節約のため、この変数名)
        //[SerializeField]
        //public float t;
        [SerializeField]
        private int[] p; //[noteTypeForMaker,from,noteId,親note,子note]の配列
        [SerializeField]
        private int[] r; //[noteTypeForMaker,from,noteId,親note,子note]の配列

        private int BarNumber { get; }
        public int AllNotesNumber_player { get; private set; }
        public int AllNotesNumber_rival { get; private set; }


        private Func<bool, int> NoteNumChanged { get; set; }
        public float BpmModify { get => m; private set => m = value; }
        //public float Time { get => t; private set => t = value; }




        public (int, int) InitializeNotesNumber(Func<bool, int> notenumChanged)
        {
            NoteNumChanged = notenumChanged;
            int playerNum = GetNotes(true).Where(x => (x[0] == 1 || x[0] == 2 || x[0] == 4)).Count();
            int rivalNum = GetNotes(false).Where(x => (x[0] == 1 || x[0] == 2 || x[0] == 4)).Count();
            AllNotesNumber_player = playerNum;
            AllNotesNumber_rival = rivalNum;
            return (playerNum,rivalNum);

        }



        /// <summary>
        /// Noteを変更し、楽譜の全Notes数を返す
        /// </summary>
        /// <param name="isPlayer">変更したいNoteはプレイヤーのものかどうか</param>
        /// <param name="type">変更先NoteType</param>
        /// <param name="from">どこから飛んでくるか</param>
        /// <param name="pos">beat位置</param>
        /// <param name="noteId">noteを一意に決める数値</param>
        /// <param name="haveParent">このnoteが反射により生成されるか否か</param>
        public int SetNotes(bool isPlayer,NoteTypeForSheet type,int from, int pos,int noteId)
        {
            int[] table = isPlayer ? p : r;

            table[pos * 5] = (int)type;
            table[pos * 5 + 1] = from;
            table[pos * 5 + 2] = noteId;
            table[pos * 5 + 3] = -1;
            table[pos * 5 + 4] = -1;




            if (isPlayer) AllNotesNumber_player = GetNotes(true).Where(x => (x[0] == 1 || x[0] == 2 || x[0] == 4)).Count();
            else AllNotesNumber_rival = GetNotes(false).Where(x => (x[0] == 1 || x[0] == 2 || x[0] == 4)).Count();

            return NoteNumChanged(isPlayer);

        }

        public (int noteNumber,int parentId,int childId) Erase(bool isPlayer, int pos)
        {
            int[] note = GetNotes(isPlayer)[pos];
            return (SetNotes(isPlayer, NoteTypeForSheet.None, -1, pos, -1),note[3],note[4]);
        }

        public int SetLongMiddle(bool isPlayer, int pos)
        {
            return SetNotes(isPlayer, NoteTypeForSheet.LongMiddle, -1, pos, -1);
        }


        /// <summary>
        /// このdivに存在するノーツを取得する
        /// [noteTypeForMaker,from,noteId,親note]の配列
        /// </summary>
        /// <param name="isPlayer"></param>
        /// <returns></returns>
        public int[][] GetNotes(bool isPlayer)
        {
            int[] table = isPlayer ? p : r;
            int[][] noteTable = new int[4][];
            for (int i = 0; i < 4; i++)
            {
                noteTable[i] = new int[5];
                for (int j = 0; j < 5; j++)
                {
                    int index = i * 5 + j;
                    noteTable[i][j] = table[index];
                }
            }

            return noteTable;
        }

        public void SetNewParent(bool isPlayer,int parentId,int pos)
        {
    

            int[] table = isPlayer ? p : r;
            //有効なノートであることを確認
            if (table[pos * 5] !=1 && table[pos * 5] != 2)
            {
                Debug.LogError("NormalまたはLongBeginのみが親ノートを持ちえます");
                return;
            }
            if(table[pos * 5 + 3] != -1)
            {
                Debug.Log("既に親が登録されています");
                return;
            }
                table[pos * 5+3] = parentId;
        }

        public void SetNewChild(bool isPlayer, int childId, int pos)
        {
            int[] table = isPlayer ? p : r;
            //有効なノートであることを確認
            if (table[pos * 5] != 1)
            {
                Debug.LogError("Normalノートのみが子ノートを持ちえます");
                return;
            }
            if (table[pos * 5 + 4] != -1)
            {
                Debug.Log("既に子が登録されています");
                return;
            }
            table[pos * 5 + 4] = childId;
        }



        public DivData(int barNum, Func<bool, int> noteNumChanged)
        {
            NoteNumChanged = noteNumChanged;
            BarNumber = barNum;
            p = new int[20] { 0, -1, -1, -1, -1, 0, -1, -1, -1, -1, 0, -1, -1, -1, -1, 0, -1, -1, -1, -1 };
            r = new int[20] { 0, -1, -1, -1, -1, 0, -1, -1, -1, -1, 0, -1, -1, -1, -1, 0, -1, -1, -1, -1 };


            m = 1;
        }
    }

}
