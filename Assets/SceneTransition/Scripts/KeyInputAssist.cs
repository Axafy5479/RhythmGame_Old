using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WBTransition
{
    public class KeyInputAssist
    {

		#region Singleton
		private static KeyInputAssist instance = new KeyInputAssist();
		public static KeyInputAssist Instance => instance;


		#endregion
		private KeyInputAssist() { }

		private bool enable = true;
		public bool Enable
		{
			get => enable; 
			set
			{
				enable = value;

			}
		}

		public bool GetKeyDown(KeyCode keyCode)
		{
			if (!Enable) return false;
			else return Input.GetKeyDown(keyCode);
		}
	}
}
