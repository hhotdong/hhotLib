//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

using UnityEngine;
using hhotLib.Common;

public class QueryManagerTestRequester : MonoBehaviour
{
	private void Start()
	{
		GameObject result = QueryManager.Query<TestRequest, GameObject>(new TestRequest(77, "Hello Query Manager"));
		Debug.Log("result: " + result.gameObject.name);
	}
}