﻿// The App Guruz(http://www.theappguruz.com/blog/android-native-popup-using-unity)

using UnityEngine;
using System.Collections;

namespace NativePopup_Android
{
	public class NativeDialog
	{
		#region PUBLIC_VARIABLES

		string title;
		string message;
		string yesButton;
		string noButton;


		public string urlString;

		#endregion

		#region PUBLIC_FUNCTIONS

		public NativeDialog(string title, string message)
		{
			this.title = title;
			this.message = message;
			this.yesButton = "Yes";
			this.noButton = "No";
		}

		public NativeDialog(string title, string message, string yesButtonText, string noButtonText)
		{
			this.title = title;
			this.message = message;
			this.yesButton = yesButtonText;
			this.noButton = noButtonText;
		}

		public void SetUrlString(string urlString)
		{
			this.urlString = urlString;
		}

		public void init()
		{
#if UNITY_ANDROID
        AndroidDialog dialog = AndroidDialog.Create(title, message, yesButton, noButton);
        dialog.urlString = urlString;
#endif
		}

		#endregion
	}
}