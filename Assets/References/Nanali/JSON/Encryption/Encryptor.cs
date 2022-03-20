using UnityEngine;
using System.Collections;

public class Encryptor : MonoBehaviour
{
	public EncrytType encrytType;
	
	public static string GetEncryptString(string data,bool Encode)
	{
		string encryptString="";
		string encode="";
		
		try
		{
			string firstKey="nanali";
			string secondKey="mk";

			//Create DataPack.
			//DataPack dataPack=new DataPack();
			
			//Input WWWForm.
			PostDataSet [] postDatas=new PostDataSet[3];
			postDatas[0]=new PostDataSet("firstkey",firstKey);
			postDatas[1]=new PostDataSet("data",data);
			postDatas[2]=new PostDataSet("lastkey",secondKey);
			
			encryptString=Mng_Encryption.Instance.GetHash(postDatas);
			if(Encode)
				encode = System.Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (encryptString));
			else
				encode = encryptString;
		}
		catch
		{
			Debug.Log("****** 암호화 에러 ******");
		}
		
		return encode;
	}

	public static string GetDecryptString(string data,bool Decode)
	{
		string EncrytData="";
		string JSON="";

		if(Decode)
		{
			try
			{
				EncrytData=System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data));
			}
			catch
			{
				EncrytData=data;
				Debug.Log("****** 디코딩 에러 ******");
			}
		}
		else
			EncrytData=data;

		try
		{
			JSON=Mng_Encryption.Instance.GetDecodeData(EncrytData);
		}
		catch
		{
			Debug.Log("****** 복호화 에러 ******");
		}

		return JSON;
	}
}

public enum EncrytType
{
	GameData=0,
	LevelSheet=1,
	ListLocalFriends=2
}
