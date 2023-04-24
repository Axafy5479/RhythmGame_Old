using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeSheetScene
{
    public class OneDivForDecidingParent : MonoBehaviour
    {
        [SerializeField] private NoteForDecidingParent[] notes;


        public void Show(bool isPlayer,int pos,int clickedNoteId,int parentNoteId)
        {

            notes[pos].transform.Find(isPlayer ? "_EnemyNormal" : "_PlayerNormal").gameObject.SetActive(true);
            notes[pos].Activate(isPlayer, clickedNoteId, parentNoteId);
        }
    }
}
