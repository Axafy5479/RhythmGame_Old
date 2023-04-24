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
        /// noteが叩かれた際に呼ぶメソッド
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <param name="judge"></param>
        public override void Beated(float beatedTime, JudgeEnum judge)
        {
            base.Beated(beatedTime,judge);

            //音を鳴らす
            SEManager.I.MakeSound(IsPlayer, 1);

            //beginがMissなら、オブジェクトは消滅し、Endもミスになる
            if (judge != JudgeEnum.Miss)
            {
                pressCoroutine = StartCoroutine(JudgeLongPress(beatedTime));
            }
            else
            {
                //EndのオブジェクトのみMissとする
                Recorder.I.AddRecord(IsPlayer, EndNoteId, 0,　JudgeEnum.Miss);

                Return();
            }

            //長押しの間は、見た目を少し太くする
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
        /// 後何秒長押しが必要か
        /// </summary>
        private float restPressTime = -1;

        /// <summary>
        /// 指定時間長押しのカウントダウン
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <returns></returns>
        private IEnumerator JudgeLongPress(float beatedTime)
        {
            //Longオブジェクトをプレスし続ける時間
            restPressTime = endTime - beatedTime;
            while (restPressTime>0)
            {
                yield return new WaitForSeconds(1/60f);
                restPressTime = endTime-BGMSource.I.CurrentTime;
            }
     

            //ここに到達していれば(= 指定時間以上pressしていた)、EndはPerfectとする
            SEManager.I.MakeSound(IsPlayer, 1);
            Recorder.I.AddRecord(IsPlayer, EndNoteId, 0 , JudgeEnum.Perfect);
            Return();

            pressCoroutine = null;
        }

        /// <summary>
        /// 指が離れたら呼ばれる
        /// Longオブジェクトの判定のためのメソッドで
        ///　JudgeLongPress　のコルーチンが回っていたら、強制的に終了させ、
        ///　Press時間が足りていないならばMissとする
        /// </summary>
        /// <param name="beatedTime"></param>
        /// <returns></returns>
        public override bool UnTapped(JudgeEnum judge = JudgeEnum.None)//(float beatdTime)
        {
            if (pressCoroutine == null) return false;

            //長押し時間を計測するコルーチンを終了
            StopCoroutine(pressCoroutine);

            //Endのオブジェクトのみ判定する
            JudgeEnum endNoteJudge = judge == JudgeEnum.None? Judge.I.JudgeNote(restPressTime):judge;
            SEManager.I.MakeSound(true,1);

            //決定した判定を記録する
            Recorder.I.AddRecord(IsPlayer, EndNoteId, 0, endNoteJudge);



            //long noteをプールに返す
            Return();
            return true;
        }

        /// <summary>
        /// noteを発射する。この際初期化を行う
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="playerBeatPos"></param>
        /// <param name="rivalBeatPos"></param>
        /// <param name="noteData"></param>
        /// <param name="hasChild"></param>
        /// <param name="launchedTrn"></param>
        public override void Launch(Transform spawnPos,Transform[] playerBeatPos, Transform[] rivalBeatPos, OneNoteData noteData,bool hasChild, Transform launchedTrn)
        {
            //long noteは親になれないというルールに違反していないかチェックする
            if (hasChild){Debug.LogError("ロングNoteは親になりえません");}

            LongNoteData longData = noteData as LongNoteData;

            EndNoteId = longData.EndNoteId;
            this.endTime = longData.PressEndTime;

            base.Launch(spawnPos, playerBeatPos, rivalBeatPos, noteData, launchedTrn);

            endTrn.position = spawnPos.position;

            ResetToInitialShape(playerBeatPos, rivalBeatPos, noteData);

        }

        /// <summary>
        /// long note はlaunch後に変形するため、初期化時には元の形に戻さなければならない
        /// </summary>
        private void ResetToInitialShape(Transform[] playerBeatPos, Transform[] rivalBeatPos, OneNoteData noteData)
        {
            //角度の調節
            Vector3 dv = (IsPlayer ? playerBeatPos : rivalBeatPos)[noteData.Pos].position - this.transform.position;
            float atan = Mathf.Atan(dv.x / dv.y);
            float angle = atan * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0);
            this.transform.Rotate(new Vector3(0, 0, -1), angle);

            //長押しの間は、見た目を少し太くする
            topTrn.localScale = new Vector3(1, 1, 1);
            endTrn.localScale = new Vector3(1, 1, 1);
            pathTrn.localScale = new Vector3(0.5f, 0.5f, 1);
            var col = spriteRenderer.color;
            col.a = 0.4f;
            spriteRenderer.color = col;
        }

        /// <summary>
        /// 発射後のアニメーションを作成する
        /// </summary>
        /// <param name="stopPoint"></param>
        /// <returns></returns>
        protected override Sequence MakeSequence(Vector3 stopPoint)
        {
            return DOTween.Sequence()
                .Append(topTrn.DOMove(stopPoint, (BeatTime - BGMSource.I.CurrentTime) * 1.2f))
                .Insert((BeatTime - BGMSource.I.CurrentTime) / 5, endTrn.DOMove(stopPoint, (BeatTime - BGMSource.I.CurrentTime) * 1.2f))
            .OnComplete(() => {

                //Beginがクリックされずに放置されたら、BeginとEndの両方がMissとなる
                Recorder.I.AddRecord(IsPlayer, NoteId, (int)1e8, JudgeEnum.Miss);
                Recorder.I.AddRecord(IsPlayer, EndNoteId, (int)1e8, JudgeEnum.Miss);

                //long note は子を持ちえないため、子ノートの発射は行わない
                Return ();
            });
        }

        /// <summary>
        /// 発射後に行った変化を元に戻す
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
