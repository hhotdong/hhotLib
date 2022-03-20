//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

using UnityEngine;

public class QueryManagerTestProvider : MonoBehaviour
{

	private void Awake()
	{
		QueryManager.RegisterProvider<TestRequest, GameObject>(TestProvider);
	}

	private GameObject TestProvider(TestRequest request)
	{
		// Log the parameters just to show that they are passed
		Debug.Log("intParam: " + request.IntParam);
		Debug.Log("stringParam: " + request.StringParam);

		return this.gameObject;
	}

}