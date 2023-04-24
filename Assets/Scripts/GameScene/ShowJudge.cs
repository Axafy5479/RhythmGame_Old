using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameScene
{
    public class ShowJudge : MonoSingleton<ShowJudge>
    {
        [SerializeField] private TextMeshPro playerText;
        [SerializeField] private TextMeshPro enemyText;

        [SerializeField] private Color playerColor;
        [SerializeField] private Color enemyColor;


        public void Show(bool player, JudgeEnum judge)
        {
            var text = player ? playerText : enemyText;
            var c = player ? playerColor : enemyColor;


            switch (judge)
            {
                case JudgeEnum.Perfect:
                    text.text = "Perfect";
                    text.color = c;
                    break;
                case JudgeEnum.Good:
                    text.text = "Good";
                    text.color = c;
                    break;
                case JudgeEnum.Miss:
                    text.text = "Miss";
                    c = Color.black;
                    break;
                default:
                    break;
            }


            DOVirtual.Float(1f, 0f, 0.15f, value =>
            {
                c.a = value;
                text.color = c;
            });

        }
    }

}