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
            //�J�n���ɂ͔�\��
            window.CloseWindow();
        }

        public void Show(bool isPlayer, BarData barData,int clickedNoteId)
        {
            foreach (Transform item in barTrn)
            {
                Destroy(item.gameObject);
            }

            //���O�̏��߂�normal notes �̂����A�q�������Ȃ����̂�\������
            //1div���Ƃɕ\�����Ă���
            foreach (var div in barData.DivDatas)
            {
                //div��ǉ�
                Transform trn = Instantiate(divPrefabForDecidingParent, barTrn).transform;
                trn.SetAsFirstSibling();
                OneDivForDecidingParent divObj = trn.GetComponent<OneDivForDecidingParent>();

                //����div�ɑ��݂���u�ΐ푊��́vnotes���擾
                int[][] notes = div.GetNotes(!isPlayer);

                //note���ЂƂÂm�F���A�\�����ׂ����̂�I������
                for (int pos = 0; pos < 4; pos++)
                {
                    //�����͓��
                    //1. NormalNote�ł���
                    bool condition1 = notes[pos][0] == 1;

                    //2. �q�������Ȃ�
                    bool condition2 = notes[pos][4] == -1;

                    if (condition1 && condition2) {
                        //�����𖞂���note�ł���Ȃ�\��
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