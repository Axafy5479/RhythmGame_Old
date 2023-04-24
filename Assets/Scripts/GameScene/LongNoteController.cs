using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class LongNoteController : NoteController
    {
        public float BeatTimeRival { get; set; }
        public float? PointerUpTimeRival { get; set; }
        public int EndNoteId { get; private set; }

        [SerializeField] private Transform topTrn;
        [SerializeField] private Transform pathTrn;
        [SerializeField] private Transform endTrn;
        [SerializeField] private SpriteRenderer spriteRenderer;
        public float endTime { get; private set; }

        public override NoteType NoteType => NoteType.Long;

        Coroutine pressCoroutine = null;

        /// <summary>
        /// note���@���ꂽ�ۂɌĂԃ��\�b�h
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <param name="judge"></param>
        public override void Beated(float beatedTime, JudgeEnum judge)
        {
            base.Beated(beatedTime,judge);

            //����炷
            SEManager.I.MakeSound(IsPlayer, 1);

            //begin��Miss�Ȃ�A�I�u�W�F�N�g�͏��ł��AEnd���~�X�ɂȂ�
            if (judge != JudgeEnum.Miss)
            {
                pressCoroutine = StartCoroutine(JudgeLongPress(beatedTime));
            }
            else
            {
                //End�̃I�u�W�F�N�g�̂�Miss�Ƃ���
                Recorder.I.AddRecord(IsPlayer, EndNoteId, 0,�@JudgeEnum.Miss);

                Return();
            }

            //�������̊Ԃ́A�����ڂ�������������
            var scale = topTrn.localScale;
            var pathWid = pathTrn.localScale;
            pathWid.x *= 1.4f;
            topTrn.DOScale(scale * 1.4f, 0.1f);
            endTrn.DOScale(scale * 1.4f, 0.1f);
            pathTrn.DOScale(pathWid, 0.1f);
            var col = spriteRenderer.color;
            col.a = 0.6f;
            spriteRenderer.color = col;


            endTrn.DOMove(topTrn.position, endTime - beatedTime);
            //return judge;
        }


        /// <summary>
        /// �㉽�b���������K�v��
        /// </summary>
        private float restPressTime = -1;

        /// <summary>
        /// �w�莞�Ԓ������̃J�E���g�_�E��
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <returns></returns>
        private IEnumerator JudgeLongPress(float beatedTime)
        {
            //Long�I�u�W�F�N�g���v���X�������鎞��
            restPressTime = endTime - beatedTime;
            while (restPressTime>0)
            {
                yield return new WaitForSeconds(1/60f);
                restPressTime = endTime-BGMSource.I.CurrentTime;
            }
     

            //�����ɓ��B���Ă����(= �w�莞�Ԉȏ�press���Ă���)�AEnd��Perfect�Ƃ���
            SEManager.I.MakeSound(IsPlayer, 1);
            Recorder.I.AddRecord(IsPlayer, EndNoteId, 0 , JudgeEnum.Perfect);
            Return();

            pressCoroutine = null;
        }

        /// <summary>
        /// �w�����ꂽ��Ă΂��
        /// Long�I�u�W�F�N�g�̔���̂��߂̃��\�b�h��
        ///�@JudgeLongPress�@�̃R���[�`��������Ă�����A�����I�ɏI�������A
        ///�@Press���Ԃ�����Ă��Ȃ��Ȃ��Miss�Ƃ���
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <returns></returns>
        public override bool UnTapped(JudgeEnum judge = JudgeEnum.None)//(float beatdTime)
        {
            if (pressCoroutine == null) return false;

            //���������Ԃ��v������R���[�`�����I��
            StopCoroutine(pressCoroutine);

            //End�̃I�u�W�F�N�g�̂ݔ��肷��
            JudgeEnum endNoteJudge = judge == JudgeEnum.None? Judge.I.JudgeNote(restPressTime):judge;
            SEManager.I.MakeSound(true,1);

            //���肵��������L�^����
            Recorder.I.AddRecord(IsPlayer, EndNoteId, 0, endNoteJudge);



            //long note���v�[���ɕԂ�
            Return();
            return true;
        }

        /// <summary>
        /// note�𔭎˂���B���̍ۏ��������s��
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="playerBeatPos"></param>
        /// <param name="rivalBeatPos"></param>
        /// <param name="noteData"></param>
        /// <param name="hasChild"></param>
        /// <param name="launchedTrn"></param>
        public override void Launch(Transform spawnPos,Transform[] playerBeatPos, Transform[] rivalBeatPos, OneNoteData noteData,bool hasChild, Transform launchedTrn)
        {
            //long note�͐e�ɂȂ�Ȃ��Ƃ������[���Ɉᔽ���Ă��Ȃ����`�F�b�N����
            if (hasChild){Debug.LogError("�����ONote�͐e�ɂȂ肦�܂���");}

            LongNoteData longData = noteData as LongNoteData;

            EndNoteId = longData.EndNoteId;
            this.endTime = longData.PressEndTime;

            base.Launch(spawnPos, playerBeatPos, rivalBeatPos, noteData, launchedTrn);

            endTrn.position = spawnPos.position;

            ResetToInitialShape(playerBeatPos, rivalBeatPos, noteData);

        }

        /// <summary>
        /// long note ��launch��ɕό`���邽�߁A���������ɂ͌��̌`�ɖ߂��Ȃ���΂Ȃ�Ȃ�
        /// </summary>
        private void ResetToInitialShape(Transform[] playerBeatPos, Transform[] rivalBeatPos, OneNoteData noteData)
        {
            //�p�x�̒���
            Vector3 dv = (IsPlayer ? playerBeatPos : rivalBeatPos)[noteData.Pos].position - this.transform.position;
            float atan = Mathf.Atan(dv.x / dv.y);
            float angle = atan * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0);
            this.transform.Rotate(new Vector3(0, 0, -1), angle);

            //�������̊Ԃ́A�����ڂ�������������
            topTrn.localScale = new Vector3(1, 1, 1);
            endTrn.localScale = new Vector3(1, 1, 1);
            pathTrn.localScale = new Vector3(0.5f, 0.5f, 1);
            var col = spriteRenderer.color;
            col.a = 0.4f;
            spriteRenderer.color = col;
        }

        /// <summary>
        /// ���ˌ�̃A�j���[�V�������쐬����
        /// </summary>
        /// <param name="stopPoint"></param>
        /// <returns></returns>
        protected override Sequence MakeSequence(Vector3 stopPoint)
        {
            return DOTween.Sequence()
                .Append(topTrn.DOMove(stopPoint, (BeatTime - BGMSource.I.CurrentTime) * 1.2f))
                .Insert((BeatTime - BGMSource.I.CurrentTime) / 5, endTrn.DOMove(stopPoint, (BeatTime - BGMSource.I.CurrentTime) * 1.2f))
            .OnComplete(() => {

                //Begin���N���b�N���ꂸ�ɕ��u���ꂽ��ABegin��End�̗�����Miss�ƂȂ�
                Recorder.I.AddRecord(IsPlayer, NoteId, (int)1e8, JudgeEnum.Miss);
                Recorder.I.AddRecord(IsPlayer, EndNoteId, (int)1e8, JudgeEnum.Miss);

                //long note �͎q���������Ȃ����߁A�q�m�[�g�̔��˂͍s��Ȃ�
                Return ();
            });
        }

        /// <summary>
        /// ���ˌ�ɍs�����ω������ɖ߂�
        /// </summary>
        protected override void OnDisable()
        {
            if (pressCoroutine != null)
            {
                StopCoroutine(pressCoroutine);
                pressCoroutine = null;
            }
            BeatTimeRival = -1;
            PointerUpTimeRival = null;
            EndNoteId = -1;
            endTime = -1;
            base.OnDisable();
        }


    }
}
