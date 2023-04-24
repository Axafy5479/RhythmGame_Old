using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeSheetScene
{
    public class DivObj : MonoBehaviour
    {
        [SerializeField]private Note[] oneNotes;

        private int divisionIndex = -1;
        public int Bar { get; private set; }

        public Note[] Notes => oneNotes;
        public void Initialize(int bar, int index)
        {
            Bar = bar;
            divisionIndex = index;
            for (int finger = 0; finger < 4; finger++)
            {
                Notes[finger].Initialize(bar, divisionIndex, finger);
            }
        }

  
    }
}