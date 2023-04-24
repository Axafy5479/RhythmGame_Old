using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene
{
    public class LaneInputManager_Player : LaneInputManager, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ParticleSystem particle;


        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsPlayer)
            {
                particle.Play();
                LaneBeatedAndDequeue(Source.time);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsPlayer)
            {
                if (Lnc == null) return;

                Lnc.UnTapped();
                Lnc = null;
            }
        }
    }
}
