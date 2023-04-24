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
        /// �N���b�N���Ԃ̕]�������A�����Ԃ�
        /// </summary>
        /// <param name="note"></param>
        public JudgeEnum JudgeNote(float delta)
        {
            delta = Mathf.Abs(delta);
            JudgeEnum result;
            if (delta < JudgeTime(JudgeEnum.Perfect))
            {
                //�p�[�t�F�N�g
                result = JudgeEnum.Perfect;
            }
            else if(delta < JudgeTime(JudgeEnum.Good))
            {
                //�O�b�h
                result = JudgeEnum.Good;
            }
            else
            {
                //�~�X
                result = JudgeEnum.Miss;
            }

            return result;
        }
    }
}


