using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSelectScene
{
    public class Initializer : MonoBehaviour, IInitializer
    {
        [SerializeField] private MusicListController listMaker;
        [SerializeField] private CourseSelector courseSelector;
        [SerializeField] private UserDataShower userDataShower;

        public IEnumerator Initialize()
        {
            yield return initialize();
        }


        private IEnumerator initialize()
        {

            //yield return Data.GetUserData.I.GetUserData_Connect();
            //yield return Data.GetUserData.I.GetAllMusic_Connect();


            courseSelector.Initialize();
            userDataShower.Initialize(Data.GetUserData.I.UserData);
            listMaker.Initialize(Data.GetUserData.I.Music);
            yield return null;
        }

        // Start is called before the first frame update
        //void Start()
        //{
        //    if (Application.platform == RuntimePlatform.WindowsEditor)
        //    {
        //        SaveSystem.I.Load();
        //        StartCoroutine(initialize());
        //    }
        //}

        // Update is called once per frame
        void Update()
        {

        }
    }
}
