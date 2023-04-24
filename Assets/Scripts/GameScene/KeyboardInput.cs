using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene {
    public class KeyboardInput : MonoBehaviour
    {
        [SerializeField] private LaneInputManager_Player[] beatPoints;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {

               // if (beatPoints[0].active) 
                    beatPoints[0].OnPointerDown(null);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
               // if (beatPoints[0].active)
               beatPoints[0].OnPointerUp(null);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
               // if (beatPoints[1].active)
               beatPoints[1].OnPointerDown(null);
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
               //if (beatPoints[1].active)
               beatPoints[1].OnPointerUp(null);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                //if (beatPoints[2].active)
                beatPoints[2].OnPointerDown(null);
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                //if (beatPoints[2].active) 
                    beatPoints[2].OnPointerUp(null);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
              //  if (beatPoints[3].active) 
                    beatPoints[3].OnPointerDown(null);
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
               // if (beatPoints[3].active)
                    beatPoints[3].OnPointerUp(null);
            }
        }
    }
}
