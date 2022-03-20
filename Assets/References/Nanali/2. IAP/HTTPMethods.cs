/**
 * Created by Nanali Inc. (minki)
 * 
 * connected server class.
 **/
using UnityEngine;
using System.Collections;
using System.Text;
using System;
using UnityEngine.Networking;

public class HTTPMethods : MonoBehaviour
{
	//싱글톤.
	static HTTPMethods _instance;
	public static HTTPMethods Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(HTTPMethods)) as HTTPMethods;
			return _instance;
		}
	}

	public int _timeOut = 5;

	public void GET(string uri, Action<string, string> callback)
	{
		UnityWebRequest request = UnityWebRequest.Get(uri);
		StartCoroutine(WaitForRequest(request, callback));
	}

	public void POST(string uri, string JSONBody, Action<string, string> callback)
	{
		UnityWebRequest request = new UnityWebRequest(uri, "POST");
		byte[] jsonToSend = new UTF8Encoding().GetBytes(JSONBody);
		request.uploadHandler = new UploadHandlerRaw(jsonToSend);
		request.SetRequestHeader("Content-Type", "application/json");

		StartCoroutine(WaitForRequest(request, callback));
	}

	public void DELETE(string uri, Hashtable ht_requestHeader, Action<string, string> callback)
	{
		UnityWebRequest request = UnityWebRequest.Delete(uri);
		request.SetRequestHeader("Content-Type", "text/json");

		if (ht_requestHeader != null)
		{
			IDictionaryEnumerator e = ht_requestHeader.GetEnumerator();
			while (e.MoveNext())
				request.SetRequestHeader(e.Key.ToString(), e.Value.ToString());
		}

		StartCoroutine(WaitForRequest(request, callback));
	}



	///////////////////Wait for request.
	IEnumerator WaitForRequest(UnityWebRequest request, Action<string, string> callback)
	{
		request.downloadHandler = new DownloadHandlerBuffer();
		request.timeout = _timeOut;

		yield return request.SendWebRequest();

		callback(request.error, request.downloadHandler.text);
	}
}
