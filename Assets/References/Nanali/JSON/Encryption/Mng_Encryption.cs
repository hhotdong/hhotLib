using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using System.IO.Compression;

public static class CV_Encryption
{
	public const string SecretKey="nanaliencryption";

	public const int EncryptionKey=175;
	public const int EncryptionByteSize=4;
	public const int EncryptionBitInterval=8;
}

public class Mng_Encryption : MonoBehaviour
{
	private static Mng_Encryption _instance;
	public static Mng_Encryption Instance
	{
		get{
			if(_instance == null)
				_instance = FindObjectOfType(typeof(Mng_Encryption)) as Mng_Encryption;
			return _instance;
		}
	}
	
	private AES Aes;
	public WATCrypt m_crypt = new WATCrypt("20180611");
	
	public static byte [] secret;
	
	void Awake()
	{
		Mng_Encryption [] mngEncryption=FindObjectsOfType(typeof(Mng_Encryption)) as Mng_Encryption[];
		if(mngEncryption.Length>=2)
		{
			Destroy(gameObject);
		}
		else
		{
			Aes = gameObject.AddComponent<AES>();
			Aes.Setting("8wlD9C0QLM0Lz7eG7ctxW65f79CvyJyB","H905QWB0eR85cb8m");
			
			_rsa = new RSACryptoServiceProvider(1024);
			_privateKey = _rsa.ToXmlString(true);
			_publicKey = _rsa.ToXmlString(false);
			
			InitLocalKey();

//			DontDestroyOnLoad(gameObject);
		}
	}
	
	public static void InitLocalKey()
	{
		if(secret!=null)
			return;
		MD5 md5Hash = new MD5CryptoServiceProvider();  
		secret = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(CV_Encryption.SecretKey)); 
	}
	
	public string GetHash(PostDataSet[] datas)
	{
		StringBuilder sb=new StringBuilder(datas[0].Value);
		for(int i=1;i<datas.Length;i++)
		{
			sb.Append("|");
			sb.Append(datas[i].Value);
		}
		
		string result=Md5Sum(sb.ToString()); //Get MD5.
		
		sb.Remove(0,sb.Length);
		
		sb.Append(datas[0].Param);
		sb.Append("=");
		sb.Append(datas[0].Value);
		
		for(int i=1;i<datas.Length;i++)
		{
			sb.Append("&");
			sb.Append(datas[i].Param);
			sb.Append("=");
			sb.Append(datas[i].Value);
		}
		
		sb.Append("&hash=");
		sb.Append(result);
		
		result=Aes.Encrypt(sb.ToString());//Get ASE.
		result=WWW.EscapeURL(result);
		
		return result;
	}
	
	public string GetDecodeData(string data)
	{
		string result="";
		
		try
		{
			string TmpStr="";
			TmpStr=WWW.UnEscapeURL(data);
			TmpStr=Aes.Decrypt(TmpStr);
			
			string[] splits = TmpStr.Split('&');
			
			string TmpValue="";
			string TmpData="";
			string TmpMD5="";
			
			for(int i=0;i<splits.Length;i++)
			{
				string variable=splits[i].Split('=')[0];
				string val=splits[i].Split('=')[1];
				
				if(!string.Equals(variable,"hash"))
				{
					if(i>0)
						TmpValue+="|";
					TmpValue+=val;
					
					if(string.Equals(variable,"data"))
						TmpData=val;
				}
				else
					TmpMD5=val;
			}
			
			TmpValue=Md5Sum(TmpValue);
			if(string.Equals(TmpValue,TmpMD5))
				result=TmpData;
			else
				result="";
		}
		catch
		{
			result="";
		}
		
		return result;
	}
	
	public string URLEncoding(string data)
	{
		return WWW.EscapeURL(data);
	}
	
	public string Md5Sum(string strToEncrypt)
	{
		UTF8Encoding ue = new UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
	
	RSACryptoServiceProvider _rsa;
	string _privateKey;
	string _publicKey;
	
	public string Decrypt(string ciphertext, string privateKey_)
	{
		if (string.IsNullOrEmpty(privateKey_))
		{
			return DecryptToBytes(ciphertext, _privateKey);
		}
		else
		{
			return DecryptToBytes(ciphertext, privateKey_);
		}
	}
	
	private string DecryptToBytes(string ciphertext, string privateKey)
	{
		if (string.IsNullOrEmpty(privateKey))
		{
			throw new ArgumentNullException("Error: No key provided.");
		}
		if (ciphertext.Length<=0)
		{
			throw new ArgumentNullException("Error: No message to decrypt.");
		}
		
		byte[] plaintext;
		byte[] ciphertext_Bytes = Encoding.Unicode.GetBytes(ciphertext);
		_rsa.FromXmlString(privateKey);
		
		plaintext = _rsa.Decrypt(ciphertext_Bytes, false);
		
		return Encoding.Unicode.GetString(plaintext);
	}

	public string Compression(string str)
	{
		var rowData = Encoding.UTF8.GetBytes(str);
		byte[] compressed = null;
		using (var outStream = new MemoryStream())
		{
			using (var hgs = new GZipStream(outStream, CompressionMode.Compress))
			{
				//outStream에 압축을 시킨다.
				hgs.Write(rowData, 0, rowData.Length);
			}
			compressed = outStream.ToArray();
		}
		
		return Convert.ToBase64String(compressed);
	}
	
	public string DeCompression(string compressedStr)
	{
		string output = null;
		byte[] cmpData = Convert.FromBase64String(compressedStr);
		using (var decomStream = new MemoryStream(cmpData))
		{
			using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
			{
				//decomStream에 압축 헤제된 데이타를 저장한다.
				using (var reader = new StreamReader(hgs))
				{
					output = reader.ReadToEnd();
				}
			}
		}
		
		return output;
	}
}

public class PostDataSet
{
	public string Param;
	public string Value;
	
	public PostDataSet(string P,string V)
	{
		Param=P;
		Value=V;
	}
}

public static class Base64String
{
	public static string Base64Encode(this string src, Encoding enc)
	{
		byte[] arr = enc.GetBytes(src);
		return Convert.ToBase64String(arr);
	}
	
	public static string Base64Decode(this string src, Encoding enc)
	{
		byte[] arr = Convert.FromBase64String(src);
		return enc.GetString(arr);
	}
}

public static class MD5String
{
	public static string MD5Hash(this string src, string salt, Encoding enc)
	{
		byte[] bytes = enc.GetBytes (src+salt);

		// encrypt bytes
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);

		string base64Hash = Convert.ToBase64String (hashBytes);
		return base64Hash;
	}
}


public class SInt
{
	byte[]  buff  = new byte[CV_Encryption.EncryptionByteSize];

	public SInt()
	{
		set(0);
	}

	public SInt(int _defaultValue)
	{
		set(_defaultValue);
	}
	
	public int get()
	{
		byte[] temp = new byte[CV_Encryption.EncryptionByteSize];
		for (int i = 0; i < CV_Encryption.EncryptionByteSize; i++)
			temp[i] = (byte)(buff[i] ^ CV_Encryption.EncryptionKey);
		
		int  val=0;
		for(int i=0;i<CV_Encryption.EncryptionByteSize;i++)
			val+=(temp[i] << CV_Encryption.EncryptionBitInterval*(CV_Encryption.EncryptionByteSize-1-i));
		return val;
	}
	
	public void set(int val)
	{
		if(val<=0)
			val=0;

		for(int i=0;i<buff.Length;i++)
			buff[i]=(byte)((val >> CV_Encryption.EncryptionBitInterval*(CV_Encryption.EncryptionByteSize-1-i)) & 0xFF);
		for (int i = 0; i < CV_Encryption.EncryptionByteSize; i++)
			buff[i] ^= CV_Encryption.EncryptionKey;
	}
	
	public void Add(int arg)
	{
		int val=get();
		val += arg;
		if(val<=0)
			val=0;
		set(val);
	}
	
	public void Sub(int arg)
	{
		int val=get();
		val -= arg;
		if(val<=0)
			val=0;
		set(val);
	}
}

public class WATCrypt
{
	
	byte[] Skey = new byte[8];
	public WATCrypt(string strKey)
	{
		Skey = Encoding.ASCII.GetBytes(strKey);
	}
	
	public string Encrypt(string p_data)
	{
		if (Skey.Length != 8)	
		{
			throw (new Exception("Invalid key. Key length must be 8 byte."));
		}
		
		DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();
		
		rc2.Key = Skey;
		rc2.IV = Skey;
		
		MemoryStream ms = new MemoryStream();
		CryptoStream cryStream = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);
		byte[] data = Encoding.UTF8.GetBytes(p_data.ToCharArray());
		
		cryStream.Write(data, 0, data.Length);
		cryStream.FlushFinalBlock();
		
		return Convert.ToBase64String(ms.ToArray());
	}
	
	
	
	public string Decrypt(string p_data)
	{
		DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();
		
		rc2.Key = Skey;
		rc2.IV = Skey;
		
		MemoryStream ms = new MemoryStream();
		CryptoStream cryStream = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);
		byte[] data = Convert.FromBase64String(p_data);
		
		cryStream.Write(data, 0, data.Length);
		cryStream.FlushFinalBlock();
		
		return Encoding.UTF8.GetString(ms.GetBuffer());	
	}
}

public class DataPack
{
	public string Key;
	public string iTime;
	public WWWForm wwwform;
	public ArrayList Data;
	public GameObject CallBackGameObject;
	public string CallBackFunctionName;
	
	public DataPack (string key, GameObject callbackGO, string callbackFunctionName)
	{
		Key = key;
		
		int ASCIIValue = 65;
		if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
			ASCIIValue = UnityEngine.Random.Range(65, 91);
		else
			ASCIIValue = UnityEngine.Random.Range(97, 123);
		
		char ch = Convert.ToChar (ASCIIValue);
		
		iTime = ch + (Time.time).ToString ("N3");
		
		Data = new ArrayList ();
		CallBackGameObject = callbackGO;
		CallBackFunctionName = callbackFunctionName;
	}
	
	public DataPack ()
	{
	}
}

public class SendDataInfo
{
	public string Key;
	public string iTime;
	
	public SendDataInfo (string k, string i)
	{
		Key = k;
		iTime = i;
	}
}