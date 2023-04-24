using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

namespace MakeSheetScene
{
    public class DecideParentWindow : MonoSingleton<DecideParentWindow>
    {
        [SerializeField] private ModalWindowManager window;
        [SerializeField] private GameObject divPrefabForDecidingParent;
        [SerializeField] private Transform barTrn;

        // Start is called before the first frame update
        void Start()
        {
            //開始時には非表示
            window.CloseWindow();
        }

        public void Show(bool isPlayer, BarData barData,int clickedNoteId)
        {
            foreach (Transform item in barTrn)
            {
                Destroy(item.gameObject);
            }

            //直前の小節のnormal notes のうち、子を持たないものを表示する
            //1divごとに表示していく
            foreach (var div in barData.DivDatas)
            {
                //divを追加
                Transform trn = Instantiate(divPrefabForDecidingParent, barTrn).transform;
                trn.SetAsFirstSibling();
                OneDivForDecidingParent divObj = trn.GetComponent<OneDivForDecidingParent>();

                //このdivに存在する「対戦相手の」notesを取得
                int[][] notes = div.GetNotes(!isPlayer);

                //noteをひとつづつ確認し、表示すべきものを選択する
                for (int pos = 0; pos < 4; pos++)
                {
                    //条件は二つ
                    //1. NormalNoteである
                    bool condition1 = notes[pos][0] == 1;

                    //2. 子を持たない
                    bool condition2 = notes[pos][4] == -1;

                    if (condition1 && condition2) {
                        //条件を満たすnoteであるなら表示
                        divObj.Show(isPlayer, pos, clickedNoteId, notes[pos][2]);
                    }

                }
            }
            window.OpenWindow();
        }

  public void Close()
        {
            window.CloseWindow();

        }


    }

}