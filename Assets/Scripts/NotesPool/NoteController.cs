using UnityEngine;
using DG.Tweening;

namespace GameScene
{
    public enum NoteType
    {
        None = 0,
        Normal = 1,
        Long = 2,
    }

    public abstract class NoteController : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;
        [SerializeField] private Transform trn;

        protected Sequence sequence;

        public abstract NoteType NoteType { get; }
        public Transform Trn => trn;

        public Binder Binding { get; set; } = null;

        /// <summary>
        /// リプレイを作成するため (Judge時に id とクリック時間を記録)
        /// </summary>
        public int NoteId { get; protected set; } = -1;

        /// <summary>
        /// プレイヤーが叩くべきNoteか否か
        /// </summary>
        public bool IsPlayer => isPlayer;

        /// <summary>
        /// このNoteがもつChildNote(反射Note)
        /// </summary>
        public bool HasChild { get; private set; } 

        /// <summary>
        /// 叩かれるべき時間
        /// </summary>
        public float BeatTime { get; private set; } = -1;
         
        /// <summary>
        /// noteを発射するメソッド。
        /// 変数の初期化 -> アニメーションの作成 -> アニメーション開始
        /// </summary>
        /// <param name="spawnTrn"></param>
        /// <param name="playerBeatPos"></param>
        /// <param name="rivalBeatPos"></param>
        /// <param name="noteData"></param>
        /// <param name="hasChild"></param>
        /// <param name="launchedTrn"></param>
        public virtual void Launch(Transform spawnTrn,Transform[] playerBeatPos,Transform[] rivalBeatPos,OneNoteData noteData,bool hasChild,Transform launchedTrn = null)
        {

            if (Binding != null)
            {
                Debug.LogError("すでにBindされています。要修正");
            }

            //変数の初期化
            this.transform.position = spawnTrn.position;
            Trn.position = spawnTrn.position; 
            BeatTime = noteData.BeatTime;
            NoteId = noteData.NoteId;
            HasChild = hasChild;

            //演奏バーのTransform
            Transform beatTrn = (isPlayer ? playerBeatPos : rivalBeatPos)[noteData.Pos];
            
            //目標地点
            Vector3 stopPoint = this.transform.position + (beatTrn.position - this.transform.position) * 1.2f;

            //アニメーションの作成
            sequence = MakeSequence(stopPoint);

            //アニメーション開始
            sequence.Play();
        }

        /// <summary>
        /// noteが叩かれた際に呼ぶメソッド
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <param name="judge"></param>
        public virtual void Beated(float beatedTime, JudgeEnum judge)
        {
            //bindingされているなら、binderをプールに返却する
            if (Binding != null)
            {
                Binding.Return();
                Binding = null;
            }

            //これ以上Tweenさせない
            sequence.Kill();
        }
        public abstract bool UnTapped(JudgeEnum judge = JudgeEnum.None);// (float beatedTime);

        /// <summary>
        /// noteが非表示になった際は、発射後の変更をできる限りリセットする
        /// </summary>
        protected virtual void OnDisable()
        {
            BeatTime = -1;
            if (Binding != null)
            {
                Debug.LogError("ここではBindingはnullである必要があります");
                Binding = null;
            }
            NoteId = -1;

            // Tween破棄
            if (DOTween.instance != null)
            {
                sequence.Kill();
            }
        }

        /// <summary>
        /// 発射後のアニメーションを生成するメソッド
        /// noteの種類によって挙動が異なる
        /// </summary>
        /// <param name="stopPoint"></param>
        /// <returns></returns>
        protected abstract Sequence MakeSequence(Vector3 stopPoint);

        /// <summary>
        /// note ctrl をpoolに返却する
        /// </summary>
        protected void Return()
        {
            if (Binding != null)
            {
                Binding.Return();
                Binding = null;
            }

            NotesPool.NotesPoolManager.Instance.Return(this);
        }
    }


    #region NoteData
    public class OneDivData
    {
        public OneDivData(OneNoteData[] notes, float launchTime)
        {
            Notes = notes;
            LaunchTime = launchTime;
        }

        public OneNoteData[] Notes { get; }
        public float LaunchTime { get; }


    }
    public abstract class OneNoteData
    {
        public OneNoteData(int noteId, int from, int pos, int parentId, int childId,float beatTime)
        {
            NoteId = noteId;
            From = from;
            Pos = pos;
            ParentId = parentId;
            ChildId = childId;
            BeatTime = beatTime;
        }

        public int NoteId { get; }
        public abstract NoteType NoteType { get; }
        public float BeatTime { get; }
        public int From { get; }
        public int Pos { get; }
        public int ParentId { get; }
        public int ChildId { get; }

    }
    public class NormalNoteData : OneNoteData
    {
        public override NoteType NoteType =>  NoteType.Normal;

        public NormalNoteData(int noteId, int from, int pos, int parentId, int childId,float beatTime) : base(noteId,  from, pos, parentId, childId, beatTime) { }
        
    }
    public class LongNoteData : OneNoteData
    {
        public LongNoteData(int noteId,  int from, int pos, int parentId, int childId, (float pressEndTime,int endNoteId) endInfo, float beatTime) : base(noteId,  from, pos, parentId, childId, beatTime)
        {
            PressEndTime = endInfo.pressEndTime;
            EndNoteId = endInfo.endNoteId;
        }
        public override NoteType NoteType => NoteType.Long;

        public float PressEndTime { get; }
        public int EndNoteId { get; }
    }
    #endregion

}
