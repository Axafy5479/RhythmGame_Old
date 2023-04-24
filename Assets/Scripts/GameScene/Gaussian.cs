using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    internal static class Gaussian
    {
        public static JudgeEnum GetRandomJudge8000()
        {
            float rnd = Random.Range(0, 1f);

            if (rnd < 0.61f) return JudgeEnum.Perfect;
            else if (rnd < 0.99f) return JudgeEnum.Good;
            else return JudgeEnum.Miss;
        }

        public static JudgeEnum GetRandomJudge7000()
        {
            float rnd = Random.Range(0, 1f);

            if (rnd < 0.5f) return JudgeEnum.Perfect;
            else if (rnd < 0.9f) return JudgeEnum.Good;
            else return JudgeEnum.Miss;
        }
    }
}

