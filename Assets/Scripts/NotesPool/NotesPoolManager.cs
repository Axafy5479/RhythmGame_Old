using GameScene;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace NotesPool
{
    public class NotesPoolManager
    {

        #region Singleton
        private static NotesPoolManager instance = new NotesPoolManager();
        public static NotesPoolManager Instance => instance;
        #endregion
        private NotesPoolManager() 
        {
            Transform notesPoolTrn = new GameObject("NotesPool").transform;
            notesPoolTrn.position = new Vector3(0, 3000, 0);
            GameObject.DontDestroyOnLoad(notesPoolTrn.gameObject);


            playerPoolMap.Add(NoteType.Normal, new NotesPool(notesPoolTrn,Resources.Load<GameObject>("Notes/PlayerNormalNote").GetComponent<NoteController>()));
            playerPoolMap.Add(NoteType.Long, new NotesPool(notesPoolTrn, Resources.Load<GameObject>("Notes/PlayerLongNote").GetComponent<NoteController>()));
            rivalPoolMap.Add(NoteType.Normal, new NotesPool(notesPoolTrn, Resources.Load<GameObject>("Notes/RivalNormalNote").GetComponent<NoteController>()));
            rivalPoolMap.Add(NoteType.Long, new NotesPool(notesPoolTrn, Resources.Load<GameObject>("Notes/RivalLongNote").GetComponent<NoteController>()));
            BindersPool = new BindersPool(notesPoolTrn, Resources.Load<GameObject>("Notes/Binder").GetComponent<Binder>());

            playerPoolMap[NoteType.Normal].PreloadAsync(40, 1).Subscribe();
            playerPoolMap[NoteType.Long].PreloadAsync(20, 1).Subscribe();
            rivalPoolMap[NoteType.Normal].PreloadAsync(40, 1).Subscribe();
            rivalPoolMap[NoteType.Long].PreloadAsync(20, 1).Subscribe();
            BindersPool.PreloadAsync(20, 1).Subscribe();

        }


        private Dictionary<NoteType, NotesPool> playerPoolMap = new Dictionary<NoteType, NotesPool>();
        private Dictionary<NoteType, NotesPool> rivalPoolMap = new Dictionary<NoteType, NotesPool>();
        private BindersPool BindersPool { get; }

        HashSet<NoteController> rented = new HashSet<NoteController>();
        HashSet<Binder> rentedBinder = new HashSet<Binder>();



        public NoteController Rent(bool isPlayer,NoteType noteType)
        {
            NoteController rent = (isPlayer ? playerPoolMap : rivalPoolMap)[noteType].Rent();
            rented.Add(rent);
            return rent;
        }

        public Binder RentBinder()
        {
            Binder rent = BindersPool.Rent();
            rentedBinder.Add(rent);
            return rent;
        }

        public void Return(NoteController ctrl)
        {
            rented.Remove(ctrl);
            (ctrl.IsPlayer ? playerPoolMap : rivalPoolMap)[ctrl.NoteType].Return(ctrl);
        }

        public void ReturnBinder(Binder ctrl)
        {
            rentedBinder.Remove(ctrl);
            BindersPool.Return(ctrl);
        }

        public void ReturnAll()
        {
            foreach (var item in new List<NoteController>(rented))
            {
                (item.IsPlayer ? playerPoolMap : rivalPoolMap)[item.NoteType].Return(item);
                rented.Remove(item);
            }

            foreach (var item in new List<Binder>(rentedBinder))
            {
                BindersPool.Return(item);
                rentedBinder.Remove(item);
            }
        }
    }
}
