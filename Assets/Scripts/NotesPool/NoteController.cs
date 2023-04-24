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
        /// ���v���C���쐬���邽�� (Judge���� id �ƃN���b�N���Ԃ��L�^)
        /// </summary>
        public int NoteId { get; protected set; } = -1;

        /// <summary>
        /// �v���C���[���@���ׂ�Note���ۂ�
        /// </summary>
        public bool IsPlayer => isPlayer;

        /// <summary>
        /// ����Note������ChildNote(����Note)
        /// </summary>
        public bool HasChild { get; private set; } 

        /// <summary>
        /// �@�����ׂ�����
        /// </summary>
        public float BeatTime { get; private set; } = -1;
         
        /// <summary>
        /// note�𔭎˂��郁�\�b�h�B
        /// �ϐ��̏����� -> �A�j���[�V�����̍쐬 -> �A�j���[�V�����J�n
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
                Debug.LogError("���ł�Bind����Ă��܂��B�v�C��");
            }

            //�ϐ��̏�����
            this.transform.position = spawnTrn.position;
            Trn.position = spawnTrn.position; 
            BeatTime = noteData.BeatTime;
            NoteId = noteData.NoteId;
            HasChild = hasChild;

            //���t�o�[��Transform
            Transform beatTrn = (isPlayer ? playerBeatPos : rivalBeatPos)[noteData.Pos];
            
            //�ڕW�n�_
            Vector3 stopPoint = this.transform.position + (beatTrn.position - this.transform.position) * 1.2f;

            //�A�j���[�V�����̍쐬
            sequence = MakeSequence(stopPoint);

            //�A�j���[�V�����J�n
            sequence.Play();
        }

        /// <summary>
        /// note���@���ꂽ�ۂɌĂԃ��\�b�h
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <param name="judge"></param>
        public virtual void Beated(float beatedTime, JudgeEnum judge)
        {
            //binding����Ă���Ȃ�Abinder���v�[���ɕԋp����
            if (Binding != null)
            {
                Binding.Return();
                Binding = null;
            }

            //����ȏ�Tween�����Ȃ�
            sequence.Kill();
        }
        public abstract bool UnTapped(JudgeEnum judge = JudgeEnum.None);// (float beatedTime);

        /// <summary>
        /// note����\���ɂȂ����ۂ́A���ˌ�̕ύX���ł�����胊�Z�b�g����
        /// </summary>
        protected virtual void OnDisable()
        {
            BeatTime = -1;
            if (Binding != null)
            {
                Debug.LogError("�����ł�Binding��null�ł���K�v������܂�");
                Binding = null;
            }
            NoteId = -1;

            // Tween�j��
            if (DOTween.instance != null)
            {
                sequence.Kill();
            }
        }

        /// <summary>
        /// ���ˌ�̃A�j���[�V�����𐶐����郁�\�b�h
        /// note�̎�ނɂ���ċ������قȂ�
        /// </summary>
        /// <param name="stopPoint"></param>
        /// <returns></returns>
        protected abstract Sequence MakeSequence(Vector3 stopPoint);

        /// <summary>
        /// note ctrl ��pool�ɕԋp����
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
