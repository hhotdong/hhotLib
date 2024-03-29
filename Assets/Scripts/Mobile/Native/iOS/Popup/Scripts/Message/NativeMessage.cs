﻿// The App Guruz(http://www.theappguruz.com/blog/ios-native-popup-using-unity)

using UnityEngine;
using System.Collections;

namespace NativePopup_iOS
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
#if UNITY_IPHONE
        IOSMessage.Create(title, message, ok);
#endif
		}

		#endregion
	}
}