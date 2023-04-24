using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class NormalNoteController : NoteController
    {
        public float BeatTimeRival { get; set; }
        public override NoteType NoteType => NoteType.Normal;

        public override void Beated(float beatedTime, JudgeEnum judge)
        {
            base.Beated(beatedTime,judge);
            //����炷
            SEManager.I.MakeSound(IsPlayer, 1);


            if (HasChild)
            {
                TimeKeeper.I(!IsPlayer).LaunchChildNote(Trn, NoteId);
            }

            Return();

        }

        public override bool UnTapped(JudgeEnum judge = JudgeEnum.None)//(float beatedTime)
        {
            //�������Ȃ�
            return false;
        }

        protected override Sequence MakeSequence(Vector3 stopPoint)
        {
            return DOTween.Sequence()
                .Append(this.transform.DOMove(stopPoint, (BeatTime - BGMSource.I.CurrentTime) * 1.2f))
            .OnComplete(() =>
            {
                //�����ɓ��B�����Ƃ������Ƃ́A�@����邱�ƂȂ��A���Ń|�C���g�ɓ��B�����Ƃ�������

                float current = BGMSource.I.CurrentTime;
                //Debug.LogError($"���ɔ�A�N�e�B�u�ł� : noteid = {NoteId}, noteType = {NoteType}, idealBeatTime = {BeatTime},currentTyme = {current}");
                

                //���ׂ����Ɠ�� 
                //1.  LaneInputManager��Judge�������Miss���L�^����
                //2.  �����qnotes������Ȃ�A����� LaneInputManager�ɓn��


                //  1.
                Recorder.I.AddRecord(IsPlayer, NoteId, (int)1e8, JudgeEnum.Miss);

                //  2.

                if (HasChild)
                {
                    TimeKeeper.I(!IsPlayer).LaunchChildNote(Trn, NoteId);
                }


                Return();
            });
        }
    }
}
