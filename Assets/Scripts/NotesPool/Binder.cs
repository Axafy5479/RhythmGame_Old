using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class Binder : MonoBehaviour
    {
        [SerializeField] private Transform spriteTrn;
        [SerializeField] private SpriteRenderer sprite;

        float w;

        private Transform Note1Trn { get; set; }
        private Transform Note2Trn { get; set; }

        private NoteController Note1 { get; set; }
        private NoteController Note2 { get; set; }


        private Transform trn;

        private bool set = false;

        private void Awake()
        {
            w = sprite.bounds.size.x; ;
        }
        public void NotesSetting(NoteController note1, NoteController note2)
        {

            if(trn==null)trn = transform;
            Note1Trn = note1.Trn;
            Note2Trn = note2.Trn;
            Note1 = note1;
            Note2 = note2;
            trn.position = note1.Trn.position;
            set = true;
            sprite.color = Color.white;
        }

        private void Update()
        {
            if (Note1Trn == null || Note2Trn == null) return;

            trn.position = Note1Trn.position;
         
            spriteTrn.localScale = new Vector3( (Note2Trn.position.x-Note1Trn.position.x)/w , 1,1);
           
            
        }

        public void Return()
        {
            Note1.Binding = null;
            Note2.Binding = null;


            Note1Trn = null;
            Note2Trn = null;
            Note1 = null;
            Note2 = null;
            set = false;

            NotesPool.NotesPoolManager.Instance.ReturnBinder(this);

        }
    }
}
