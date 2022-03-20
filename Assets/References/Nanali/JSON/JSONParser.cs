using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSONParser : MonoBehaviour
{
	public static void setDictionary(string key, object obj)
	{
		//Encode To Json String.
		var json=Procurios.Public.JSON.JsonEncode(obj);
		
		//Save at Local.
		PlayerPrefs.SetString( key, json );
	}
	
	public static object getDictionary(string key)
	{
		//Load at Local.
		var json = PlayerPrefs.GetString(key,"");
		
		//if(key=="Item")
		//	Debug.Log(json);
		
		object obj=new object();
		if(json!="")
		{
			try
			{
				//Decode To Hashtable.
				obj = Procurios.Public.JSON.JsonDecode(json) as object;				
			}
			catch
			{
			}
		}
		
		//Return Object;
		return obj;
	}
}
