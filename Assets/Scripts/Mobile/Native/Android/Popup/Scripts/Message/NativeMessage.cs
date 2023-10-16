// The App Guruz(http://www.theappguruz.com/blog/android-native-popup-using-unity)

using UnityEngine;
using System.Collections;

namespace NativePopup_Android
{
	public class NativeMessage
	{
		#region PUBLIC_FUNCTIONS

		public NativeMessage(string title, string message)
		{
			init(title, message, "Ok");
		}

		public NativeMessage(string title, string message, string ok)
		{
			init(title, message, ok);
		}

		private void init(string title, string message, string ok)
		{
#if UNITY_ANDROID
        AndroidMessage.Create(title, message, ok);
#endif
		}

		#endregion
	}
}