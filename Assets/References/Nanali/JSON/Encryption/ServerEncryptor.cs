using UnityEngine;
using System.Collections;



public class ServerEncryptor
{
	private RC4 m_RC4 = null;
	private string fruit_k = "FruitGlenfiddichGoGoRight?";
	private string your_k = "SaltBaseFruit";
	private uint drop_count = 27;

	#region Constructors
	public ServerEncryptor()
	{
		m_RC4 = new RC4 (fruit_k, drop_count);
	}

	public string DecryptData(string encryptedString)
	{
		// To Do :
		// 1) make md5-hash with 'salt'
		// 2) make string(jsondata+md5-hash)
		// 3) make rc4encrypted(string)
		// 4) return base_64 (rc4encrypted)

		byte[] base64Decrypted = System.Convert.FromBase64String (encryptedString);
		string dataPackJSON = m_RC4.DecryptString (base64Decrypted, System.Text.Encoding.UTF8);

		Hashtable dataTable=Procurios.Public.JSON.JsonDecode(dataPackJSON) as Hashtable;
		string dataString = dataTable ["d"].ToString ();
		string hashString = dataTable ["h"].ToString ();
		string checkHash = MD5String.MD5Hash (dataString, your_k, System.Text.Encoding.UTF8);

		if (checkHash.Equals (hashString) == false)
		{
			Debug.Log("Invalid Encryption Hash Key");
			return "";
		}

		return dataString;
	}

	public string EncryptData(string dataString)
	{
		// MD5
		string hashString =  MD5String.MD5Hash (dataString, your_k, System.Text.Encoding.UTF8);

		Hashtable dataTable=new Hashtable();
		dataTable ["d"] = dataString;
		dataTable ["h"] = hashString;
		string dataPackJSON = Procurios.Public.JSON.JsonEncode(dataTable);
		byte[] rc4Encrypted = m_RC4.Encrypt (dataPackJSON, System.Text.Encoding.UTF8);
		string base64String = System.Convert.ToBase64String (rc4Encrypted);
		return base64String;
	}

//	public RC4(string Key, uint DropCount)
//	{
//		Init(System.Text.Encoding.ASCII.GetBytes(Key), DropCount);
//	}
//	
//	public RC4(string Key)
//	{
//		Init(System.Text.Encoding.ASCII.GetBytes(Key), 768);
//	}
	#endregion


}
