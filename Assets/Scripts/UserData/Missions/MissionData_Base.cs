using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Mission
{
    public abstract class MissionData_Base : ScriptableObject
    {
        [SerializeField] private int missionId;
        public int MissionId => missionId;

        public abstract bool Check(UserData userData);
  
    }
}
