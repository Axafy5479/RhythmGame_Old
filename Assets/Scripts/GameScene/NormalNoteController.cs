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
            //音を鳴らす
            SEManager.I.MakeSound(IsPlayer, 1);


            if (HasChild)
            {
                TimeKeeper.I(!IsPlayer).LaunchChildNote(Trn, NoteId);
            }

            Return();

        }

        public override bool UnTapped(JudgeEnum judge = JudgeEnum.None)//(float beatedTime)
        {
            //何もしない
            return false;
        }

        protected override Sequence MakeSequence(Vector3 stopPoint)
        {
            return DOTween.Sequence()
                .Append(this.transform.DOMove(stopPoint, (BeatTime - BGMSource.I.CurrentTime) * 1.2f))
            .OnComplete(() =>
            {
                //ここに到達したということは、叩かれることなく、消滅ポイントに到達したということ

                float current = BGMSource.I.CurrentTime;
                //Debug.LogError($"既に非アクティブです : noteid = {NoteId}, noteType = {NoteType}, idealBeatTime = {BeatTime},currentTyme = {current}");
                

                //すべきこと二つ 
                //1.  LaneInputManagerやJudgeを介さずにMissを記録する
                //2.  もし子notesがあるなら、それを LaneInputManagerに渡す


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
