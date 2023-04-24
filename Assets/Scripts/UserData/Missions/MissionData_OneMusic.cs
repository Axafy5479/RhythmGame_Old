using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Mission
{
    [CreateAssetMenu(menuName ="Mission/OneMusic",fileName ="NewMission")]
    public class MissionData_OneMusic : MissionData_Base
    {
        [SerializeField] private int musicId;
        [SerializeField] private Course course;
        [SerializeField] private int score;
        [SerializeField] private bool fullCombo;
        //[SerializeField] private bool win;

        public override bool Check(UserData userData)
        {
            CourseUserRecord record = userData.MusicDatas.Find(d=>d.MusicId == musicId && d.Course == course);
            if (record == null)
            {
                return false;
            }

            return record.Score >= score && !(fullCombo && record.Miss1 > 0);
        }
    }
}
