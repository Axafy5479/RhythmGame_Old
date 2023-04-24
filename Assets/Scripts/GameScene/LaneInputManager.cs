using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene {
    public abstract class LaneInputManager : MonoBehaviour
    {

        #region 初期化すべき変数
        //このレーンが何番目のものか
        public int LaneIndex { get; private set; }

        //このレーンがプレイヤーのものかどうか
        public bool IsPlayer { get; private set; }

        //BGMのAudioSource 時間を取得するため
        protected AudioSource Source { get; private set; }

        public virtual void Initialize(bool isPlayer, AudioSource source,int laneIndex)
        {
            IsPlayer = isPlayer;
            this.Source = source;
            LaneIndex = laneIndex;
        }
        #endregion

        public PriorityQueue<float,NoteController> NotesOnLane { get; private set; } = new PriorityQueue<float,NoteController>((x)=>x.BeatTime,false);
        protected LongNoteController Lnc { get; set; }

        /// <summary>
        /// レーンにNoteを配置し、発射する
        /// </summary>
        /// <param name="note"></param>
        public void SetAndLaunchNote(NoteController note)
        {
            NotesOnLane.Enqueue(note);
        }



        private void Update()
        {
            //見逃しノーツは除去する
            if(NotesOnLane.Count>0&& !NotesOnLane.Peek().gameObject.activeInHierarchy)
            {
                NotesOnLane.Dequeue();
            }
        }

        /// <summary>
        /// どのNoteに判定を出したかを決定する
        /// 判定自体はせず、ジャッジするNoteを決めるだけ
        /// </summary>
        /// <param name="currentTime">クリックされた時間</param>
        public void LaneBeatedAndDequeue(float beatedTime,JudgeEnum judgeEnum = JudgeEnum.None)
        {

            //0番目を取り出して、miss判定以内かどうかを調べる
            while (NotesOnLane.Count>0)
            {
                NoteController note = NotesOnLane.Peek();
                float delta = note.BeatTime - beatedTime;


                //早すぎて判定に入っていない場合
                if (delta>Judge.JudgeTime(JudgeEnum.Miss)) //早すぎてミス、以前の問題
                {
                    //何もしない
                    return;
                }
                else if(Mathf.Abs(note.BeatTime - beatedTime) < Judge.JudgeTime(JudgeEnum.Miss)) //判定内
                {
                    if (note is LongNoteController) Lnc = note as LongNoteController;

                    //取り出して、
                    NotesOnLane.Dequeue();

                    if (judgeEnum == JudgeEnum.None)
                    {
                        //ジャッジに出す
                        JudgeEnum judge = Judge.I.JudgeNote(note.BeatTime - beatedTime);
                        Recorder.I.AddRecord(IsPlayer, note.NoteId, beatedTime, judge);
                        note.Beated(beatedTime, judge);
                    }
                    else
                    {
                        //ジャッジに出す
                        Recorder.I.AddRecord(IsPlayer, note.NoteId, beatedTime, judgeEnum);
                        note.Beated(beatedTime, judgeEnum);
                    }
                    return;

                }
                else //遅すぎて判定外 (とっくの昔に通り過ぎている)
                {
                    NotesOnLane.Dequeue();
                }
            }

            //ここまで到達したということは、queueが空 = 画面内に判定のあるNotesはない
       
        }


    }

    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly List<T> _data = new List<T>();
        private readonly IComparer<T> _comparer;
        private readonly bool _isDescending;

        public PriorityQueue(IComparer<T> comparer, bool isDescending = true)
        {
            _comparer = comparer;
            _isDescending = isDescending;
        }

        public PriorityQueue(Comparison<T> comparison, bool isDescending = true)
            : this(Comparer<T>.Create(comparison), isDescending)
        {
        }

        public PriorityQueue(bool isDescending = true)
            : this(Comparer<T>.Default, isDescending)
        {
        }

        public void Enqueue(T item)
        {
            _data.Add(item);
            var childIndex = _data.Count - 1;
            while (childIndex > 0)
            {
                var parentIndex = (childIndex - 1) / 2;
                if (Compare(_data[childIndex], _data[parentIndex]) >= 0)
                    break;
                Swap(childIndex, parentIndex);
                childIndex = parentIndex;
            }
        }

        public T Dequeue()
        {
            var lastIndex = _data.Count - 1;
            var firstItem = _data[0];
            _data[0] = _data[lastIndex];
            _data.RemoveAt(lastIndex--);
            var parentIndex = 0;
            while (true)
            {
                var childIndex = parentIndex * 2 + 1;
                if (childIndex > lastIndex)
                    break;
                var rightChild = childIndex + 1;
                if (rightChild <= lastIndex && Compare(_data[rightChild], _data[childIndex]) < 0)
                    childIndex = rightChild;
                if (Compare(_data[parentIndex], _data[childIndex]) <= 0)
                    break;
                Swap(parentIndex, childIndex);
                parentIndex = childIndex;
            }
            return firstItem;
        }

        public T Peek()
        {
            return _data[0];
        }

        private void Swap(int a, int b)
        {
            var tmp = _data[a];
            _data[a] = _data[b];
            _data[b] = tmp;
        }

        private int Compare(T a, T b)
        {
            return _isDescending ? _comparer.Compare(b, a) : _comparer.Compare(a, b);
        }

        public int Count => _data.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class PriorityQueue<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly List<KeyValuePair<TKey, TValue>> _data = new List<KeyValuePair<TKey, TValue>>();
        private readonly bool _isDescending;
        private readonly Func<TValue, TKey> _keySelector;
        private readonly IComparer<TKey> _keyComparer;

        public PriorityQueue(Func<TValue, TKey> keySelector, bool isDescending = true)
            : this(keySelector, Comparer<TKey>.Default, isDescending)
        {
        }

        public PriorityQueue(Func<TValue, TKey> keySelector, IComparer<TKey> keyComparer, bool isDescending = true)
        {
            _keySelector = keySelector;
            _keyComparer = keyComparer;
            _isDescending = isDescending;
        }

        public void Enqueue(TValue item)
        {
            _data.Add(new KeyValuePair<TKey, TValue>(_keySelector(item), item));
            var childIndex = _data.Count - 1;
            while (childIndex > 0)
            {
                var parentIndex = (childIndex - 1) / 2;
                if (Compare(_data[childIndex].Key, _data[parentIndex].Key) >= 0)
                    break;
                Swap(childIndex, parentIndex);
                childIndex = parentIndex;
            }
        }

        public TValue Dequeue()
        {
            var lastIndex = _data.Count - 1;
            var firstItem = _data[0];
            _data[0] = _data[lastIndex];
            _data.RemoveAt(lastIndex--);
            var parentIndex = 0;
            while (true)
            {
                var childIndex = parentIndex * 2 + 1;
                if (childIndex > lastIndex)
                    break;
                var rightChild = childIndex + 1;
                if (rightChild <= lastIndex && Compare(_data[rightChild].Key, _data[childIndex].Key) < 0)
                    childIndex = rightChild;
                if (Compare(_data[parentIndex].Key, _data[childIndex].Key) <= 0)
                    break;
                Swap(parentIndex, childIndex);
                parentIndex = childIndex;
            }
            return firstItem.Value;
        }

        public TValue Peek()
        {
            return _data[0].Value;
        }

        private void Swap(int a, int b)
        {
            var tmp = _data[a];
            _data[a] = _data[b];
            _data[b] = tmp;
        }

        private int Compare(TKey a, TKey b)
        {
            return _isDescending ? _keyComparer.Compare(b, a) : _keyComparer.Compare(a, b);
        }

        public int Count => _data.Count;

        public IEnumerator<TValue> GetEnumerator()
        {
            return _data.Select(r => r.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
