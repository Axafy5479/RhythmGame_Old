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
            (1,8,"�v���C���肪�Ƃ��������܂� ! \n �����`���[�g���A�����n�߂܂�" ),
            (9,15,"���̃��Y���Q�[����1��1�̑ΐ�^�ł�\n(���݂�CP�͕K��All Perfect�����܂��B���x�C�����܂�)" ),
            (16,23,"�|�C���g���l������ɂ�\n����Ă����(�m�[�c)���A�^�C�~���O�悭���C����Œ@����OK" ),
            (24,31,"������Ă���m�[�c�́A���肪�@���ׂ����̂Ȃ̂Ō������ĉ�����" ),
            (30,37,"���낻�뉺�Ɍ������m�[�c������̂ŁA���C����Œ@���Ă݂Ă�������" ),
            (59,65,"�ǂ��ł��傤��\n�Ȃ�ƂȂ�������܂�����?" ),
            (66,73,"���̃Q�[���ɂ�\n���̊ۂ��m�[�c�ȊO�ɂ�����m�[�c�����݂��܂�" ),
            (74,80,"���ꂪ�������m�[�c�ł��B" ),
            (81,86,"���̂悤�ɁA��̃m�[�c���Ȃ������`�����Ă��܂��B" ),
         �@�@(87,94,"�擪�̃m�[�c�̈ʒu�ŉ����n�߂āA\n����̃m�[�c�܂ŉ��������Ă��������B" ),
            (95,99,"�ł͂���Ă݂܂��傤!" ),
            (120,128,"�������m�[�c�́A\n�u�����v�^�C�~���O���x��Ă��S����肠��܂���" ),
            (129,134,"�������̓_���Ȃ̂Œ��ӂ��Ă�������" ),
            (135,140,"���̃t���[�Y���Ō�ł�!" ),
        };


    }
}
