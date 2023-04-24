using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class Judge : MonoSingleton<Judge>
    {





        public static float JudgeTime(JudgeEnum j)
        {
            return j switch
            {
                JudgeEnum.Perfect => 0.04f,
                JudgeEnum.Good => 0.117f,
                JudgeEnum.Miss => 0.15f,
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// クリック時間の評価をし、判定を返す
        /// </summary>
        /// <param name="note"></param>
        public JudgeEnum JudgeNote(float delta)
        {
            delta = Mathf.Abs(delta);
            JudgeEnum result;
            if (delta < JudgeTime(JudgeEnum.Perfect))
            {
                //パーフェクト
                result = JudgeEnum.Perfect;
            }
            else if(delta < JudgeTime(JudgeEnum.Good))
            {
                //グッド
                result = JudgeEnum.Good;
            }
            else
            {
                //ミス
                result = JudgeEnum.Miss;
            }

            return result;
        }
    }
}


