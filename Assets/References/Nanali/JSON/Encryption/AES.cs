using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class AES : MonoBehaviour {

	private byte[] AES_Key;
	private byte[] AES_IV;
	public int KEY_Size;
	public int IV_Size;
	public string KEY;
	public string IV;
	public string KEY64Str;
	public string IV64Str;
	
	public AES(string key,string iv){
		Setting(key,iv);
	}
	
	public void Setting(string key,string iv){

		KEY_Size = key.Length * 8;
		IV_Size = iv.Length * 8;
		KEY = key;
		IV = iv;
		AES_Key = Encoding.ASCII.GetBytes(KEY);
	    AES_IV = Encoding.ASCII.GetBytes(IV);
		KEY64Str = System.Convert.ToBase64String(AES_Key);
		IV64Str = System.Convert.ToBase64String(AES_IV);
		
	}
	
	void Start () {
		Setting(KEY,IV);
	}
	
	public string Encrypt(string text)
	{
		Setting(KEY,IV);
		if((KEY.Length == 16 || KEY.Length == 32) && (IV.Length == 16 || IV.Length == 32)){
    		var aes = new RijndaelManaged();
    		aes.KeySize = KEY_Size;
    		aes.BlockSize = IV_Size;
    		aes.Padding = PaddingMode.PKCS7;
    		aes.Key = AES_Key;
    		aes.IV = AES_IV;
    		var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
    		byte[] byteBuffer = null;
    		using (var ms = new MemoryStream())
    		{
        		using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
        		{
            		byte[] byteXml = Encoding.UTF8.GetBytes(text);
            		cs.Write(byteXml, 0, byteXml.Length);
        		}

        		byteBuffer = ms.ToArray();
    		}
    		return System.Convert.ToBase64String(byteBuffer);
		}else{
			return "Key and Iv must be 16 or 32 character";	
		}
	}
	
	
	string ReadByte(byte[] bytearray){
		 StringBuilder text = new StringBuilder();
         foreach (byte item in bytearray)
         {
             text.Append(item.ToString("X2") + " ");
         }
         return text.ToString();	
	}
	
	
	public string Decrypt(string text)
	{
	    Setting(KEY,IV);
		
        RijndaelManaged aes = new RijndaelManaged();
        aes.KeySize = KEY_Size;
        aes.BlockSize = IV_Size;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = AES_Key;
        aes.IV = AES_IV;

        var decrypt = aes.CreateDecryptor();
        byte[] byteBuffer = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
            {
                byte[] byteXml = System.Convert.FromBase64String(text);
                cs.Write(byteXml, 0, byteXml.Length);
            }

            byteBuffer = ms.ToArray();
        }
        return Encoding.UTF8.GetString(byteBuffer);
	}
}
