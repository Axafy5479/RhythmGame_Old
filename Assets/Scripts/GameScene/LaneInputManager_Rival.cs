using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class LaneInputManager_Rival : LaneInputManager
    {

        private Coroutine coroutine;

        private Func<JudgeEnum> getJudge = ()=> JudgeEnum.Perfect;


        public override void Initialize(bool isPlayer, AudioSource source, int laneIndex)
        {
            if (!GameManager.I.Auto)
            {
                if(GameManager.I.Course == Course.Lunatic)
                {
                    getJudge = Gaussian.GetRandomJudge8000;
                }
                else
                {
                    getJudge = Gaussian.GetRandomJudge7000;
                }
            }
            base.Initialize(isPlayer, source, laneIndex);
            coroutine = StartCoroutine(BeatQueueNotes());
        }


        private void OnDisable()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }


        private IEnumerator BeatQueueNotes()
        {

            while (true)
            {
                yield return new WaitWhile(() => NotesOnLane.Count == 0);


                if (NotesOnLane.Peek().BeatTime > Source.time) continue;
                NoteController note = NotesOnLane.Peek();

                if (note is NormalNoteController)
                {
                    LaneBeatedAndDequeue(note.BeatTime,getJudge());
                }
                else if (note is LongNoteController)
                {
                    var lnc = (note as LongNoteController);
                    LaneBeatedAndDequeue(note.BeatTime, getJudge());
                    lnc.PointerUpTimeRival = null;//ÉIÅ[Ég


                    if (lnc.PointerUpTimeRival != null)
                    {
                        float delta2 = (float)lnc.PointerUpTimeRival - Source.time;
                        yield return new WaitForSeconds(delta2);
                        lnc.UnTapped(getJudge());
                    }
                }
                else
                {
                    yield return null;
                }



            }
        }


    }
}
