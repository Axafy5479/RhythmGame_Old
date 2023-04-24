using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameScene.Tutorial
{
    public class Typing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI explanation;
        [SerializeField] private GameObject keyBoardIcon;
        // Start is called before the first frame update



        public void StartTyping()
        {
            StartCoroutine(Type());
        }

        private IEnumerator Type()
        {
            foreach (var item in contents)
            {
                yield return new WaitWhile(()=>item.start>BGMSource.I.CurrentTime);
                explanation.text = item.sentence;
                yield return new WaitWhile(() => item.end > BGMSource.I.CurrentTime);
                explanation.text = "";

                if(BGMSource.I.CurrentTime>16&& BGMSource.I.CurrentTime<99 && !keyBoardIcon.activeInHierarchy)
                {
                    keyBoardIcon.SetActive(true);
                }
                
                if ((BGMSource.I.CurrentTime < 16 || BGMSource.I.CurrentTime > 99) && keyBoardIcon.activeInHierarchy)
                {
                    keyBoardIcon.SetActive(false);
                }


            }
        }

        List<(float start,float end,string sentence)> contents = new List<(float start, float end, string sentence)>()
        {
            (1,8,"プレイありがとうございます ! \n 早速チュートリアルを始めます" ),
            (9,15,"このリズムゲームは1対1の対戦型です\n(現在はCPは必ずAll Perfectをします。今度修正します)" ),
            (16,23,"ポイントを獲得するには\n流れてくる玉(ノーツ)を、タイミングよくライン上で叩けばOK" ),
            (24,31,"今流れているノーツは、相手が叩くべきものなので見送って下さい" ),
            (30,37,"そろそろ下に向かうノーツが来るので、ライン上で叩いてみてください" ),
            (59,65,"どうでしょうか\nなんとなく分かりましたか?" ),
            (66,73,"このゲームには\nこの丸いノーツ以外にもう一つノーツが存在します" ),
            (74,80,"それが長押しノーツです。" ),
            (81,86,"このように、二つのノーツがつながった形をしています。" ),
         　　(87,94,"先頭のノーツの位置で押し始めて、\n後方のノーツまで押し続けてください。" ),
            (95,99,"ではやってみましょう!" ),
            (120,128,"長押しノーツは、\n「離す」タイミングが遅れても全く問題ありません" ),
            (129,134,"早すぎはダメなので注意してください" ),
            (135,140,"次のフレーズが最後です!" ),
        };


    }
}
