// Credit: https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/
using UnityEngine;
using hhotLib.Common;

public class QueryManagerTestProvider : MonoBehaviour
{
	private void Awake()
	{
		QueryManager.RegisterProvider<QueryTestData, GameObject>(TestProvider);
	}

	private GameObject TestProvider(QueryTestData request)
	{
		// Log the parameters just to show that they are passed.
		Debug.Log($"intParam: {request.intParam}, stringParam: {request.stringParam}");
		return gameObject;
	}

}