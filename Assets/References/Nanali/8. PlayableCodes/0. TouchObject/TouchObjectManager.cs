//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Nanali;
//using Random = UnityEngine.Random;
//using ScriptableObjectArchitecture;
//using deVoid.Utils;

//public class TouchObjectManager : MonoBehaviour
//{
//	private static TouchObjectManager _instance;
//	public static TouchObjectManager Instance
//	{
//		get
//		{
//			if (_instance == null)
//				_instance = FindObjectOfType(typeof(TouchObjectManager)) as TouchObjectManager;
//			return _instance;
//		}
//	}

//	public int MaxSpawnCount = 3; //최대 3개까지 생성.
//	public int SpawnInterval = 10; //N초 간격으로 항목 생성.

//	public TouchObjectPosition[] Positions;

//	public Action<TouchObjectPosition, bool> OnCreate; //생성 콜백.
//	public Action<float> OnUpdate; //생성 프로그래스 콜백.

//	public bool IsInitialized { get; private set; } //초기화 여부.
//    public bool IsMaxCount { get { return GetAppearedCount >= MaxSpawnCount || GetRandomSpawnPosition() == null; } } //생성 가능 여부.

//	private bool IsTimerProgress;
//	[SerializeField] private List<TouchObjectPosition> UsablePositions = new List<TouchObjectPosition>();
//	public int GetAppearedCount //노출중인 갯수.
//    {
//		get
//		{
//			if (IsInitialized)
//			{
//				int val = 0;
//				for (int i = 0; i < UsablePositions.Count; i++)
//				{
//					if (UsablePositions[i].IsUsed)
//						val++;
//				}

//				return val;
//			}
//			else
//            {
//				return 0;
//            }
//        }
//    }


//    //저장 데이터.
//	private string JSON
//	{
//		get { return PlayerPrefs.GetString("BMKTOPJSON", ""); }
//		set { PlayerPrefs.SetString("BMKTOPJSON", value); }
//	}

//    [Header("SO Events")]
//	[SerializeField] private IntGameEvent _OnProgressQuest = default(IntGameEvent);
//	[SerializeField] private IntGameEvent _OnProgressAchievement = default(IntGameEvent);

//	/// <summary>
//	/// 터치 오브젝트 위치정보를 초기화합니다. 게임 중 한번만 수행 할 수 있습니다. 현재 내 랜드마크 레벨 상태가 필요합니다.
//	/// </summary>
//	/// <param name="myLevel"> 랜드마크 레벨 </param>
//	public void Initialize(int myLevel)
//    {
//		if (IsInitialized)
//			return;
//		IsInitialized = true;

//		for (int i = 0; i < Positions.Length; i++)
//        {
//			Positions[i].SetCode(i);
//			if (Positions[i].Level <= myLevel)
//            {
//				UsablePositions.Add(Positions[i]);
//            }
//        }
//    }

//	/// <summary>
//	/// 저장된 오브젝트의 위치값을 가져오고 생성 콜백을 등록합니다.
//	/// </summary>
//	/// <param name="_onCreate"> 객체가 생성되어야할 때 반환됩니다. </param>
//	/// <param name="_onUpdate"> 0~1 사이의 값이 반환됩니다. </param>
//	/// <param name="callback"> 바로 생성되어야 할 위치와, 남은 시간을 반환합니다. </param>
//	public void Load(Action<TouchObjectPosition, bool> _onCreate, Action<float> _onUpdate, Action<TouchObjectPosition[], int> callback)
//    {
//		OnCreate = _onCreate;
//		OnUpdate = _onUpdate;

//		List<TouchObjectPosition> returnList = new List<TouchObjectPosition>();
//		if (!IsInitialized || string.IsNullOrEmpty(JSON))
//		{
//			Save(); //저장.
//			StartCoroutine(ProcessCoroutine(SpawnInterval));
//			callback?.Invoke(returnList.ToArray(), 0);
//			return;
//        }

//		//저장된 값 불러옴. 불러온 값이 있을경우 반환값에 추가.
//		Debug.Log(JSON);
//		TOPData data = JsonUtility.FromJson<TOPData>(JSON);
//		DateTime SpawnStartTime = DateTime.ParseExact(data.SpawnStartTime, "yyyyMMddHHmmss", null);

//		List<TOP_SaveCode> codes = data.codeList;
//		for (int i = 0; i < codes.Count; i++)
//		{
//			for (int j = 0; j < UsablePositions.Count; j++)
//            {
//				if (Equals(codes[i].code, UsablePositions[j].Code))
//                {
//					returnList.Add(UsablePositions[j]);
//					break;
//                }
//            }
//		}

//		//최대 갯수에 도달하지 않았다면 = 마지막에 저장된 획득하지 않은 보상이 MaxCount보다 적은 경우
//		int remainSeconds = 0;
//		if (returnList.Count < MaxSpawnCount)// && Utilities.IsConnectedInternet && BackendManager.IsInitialized) //차후 백엔드 init이 먼저 선행되는 경우 이 부분을 추가하여 시간조작방지에 대한 안정성을 확보하세요.
//		{
//			//오프라인 보상 체크.
//			DateTime now = BackendManager.DateTimeNow;
//			DateTime lastStart = SpawnStartTime;
//            Debug.Log(lastStart);
//            Debug.Log(now);

//            TimeSpan span = now - lastStart;

//			int offlineTotalSeconds = (int)span.TotalSeconds;
//            Debug.Log(offlineTotalSeconds);
//            //생성 될 갯수는 최대 수를 넘을 수 없음.
//            int createdCount = Mathf.Clamp(offlineTotalSeconds / SpawnInterval, 0, MaxSpawnCount - returnList.Count);
//            Debug.Log(createdCount);
//            //남은 시간.
//            remainSeconds = createdCount + returnList.Count >= MaxSpawnCount ? 0 : SpawnInterval - offlineTotalSeconds % SpawnInterval;
//            Debug.Log(remainSeconds);

//			int _checker = createdCount;
//			while (_checker > 0)
//            {
//				TouchObjectPosition newPosition = GetRandomSpawnPosition();
//				returnList.Add(newPosition);
//				_checker--;
//			}
//		}

//		callback(returnList.ToArray(), remainSeconds);

//        if (remainSeconds > 0 && returnList.Count < MaxSpawnCount)
//		    StartCoroutine(ProcessCoroutine(remainSeconds));
//	}

//    /// <summary>
//    /// 현재 노출중인 오브젝트의 코드와 마지막 획득 시간을 저장합니다.
//    /// </summary>
//	public void Save()
//    {
//		if (!IsInitialized)
//			return;

//		//AppDisableTime = BackendManager.DateTimeNow;

//		List<TOP_SaveCode> codes = new List<TOP_SaveCode>();

//		for (int i = 0; i < UsablePositions.Count; i++)
//        {
//			if (UsablePositions[i].IsUsed)
//				codes.Add(new TOP_SaveCode(UsablePositions[i].Code));

//            //최대 저장 수량을 초과하는 경우 break.
//			if (codes.Count >= MaxSpawnCount)
//				break;
//        }

//		TOPData newData = new TOPData(BackendManager.DateTimeNow.ToString("yyyyMMddHHmmss"), new ListSerialization<TOP_SaveCode>(codes).ToList());

//		JSON = JsonUtility.ToJson(newData);
//		Debug.Log(JSON);
//    }

//	IEnumerator ProcessCoroutine(float remainSeconds)
//	{
//		if (!IsTimerProgress)
//        {
//			IsTimerProgress = true;
//			float time = Time.time;
//			float _interval = SpawnInterval; //캐스팅.

//			while (Time.time - time < remainSeconds)
//			{
//				float percent = 1 - (remainSeconds / _interval) + ((Time.time - time) / _interval);
//				OnUpdate?.Invoke(percent);
//				yield return null;
//			}

//			IsTimerProgress = false;

//			//생산.
//			OnCreate?.Invoke(GetRandomSpawnPosition(), true);
//		}
//	}

//	/// <summary>
//	/// 터치오브젝트를 등록합니다.
//	/// </summary>
//	/// <param name="code"> TouchObjectPosition 의 Code 값입니다. </param>
//	/// <param name="obj"> 실제 오브젝트입니다. </param>
//	public void SetObject(string code, UIGen_TouchObject obj, bool autoCreate = false)
//    {
//		if (!IsInitialized)
//			return;

//		for (int i = 0; i < UsablePositions.Count; i++)
//        {
//			if (Equals(UsablePositions[i].Code, code))
//			{
//				UsablePositions[i].UsingTarget = obj;
//				Save();
//				if (!IsMaxCount && autoCreate)
//					StartCoroutine(ProcessCoroutine(SpawnInterval));
//				break;
//			}
//		}
//	}

//	/// <summary>
//	/// 터치 오브젝트가 생성되어야 할 위치를 랜덤으로 반환합니다.
//	/// </summary>
//	public TouchObjectPosition GetRandomSpawnPosition()
//	{
//        //사용되지 않는 오브젝트 검색.
//		List<TouchObjectPosition> emptyList = new List<TouchObjectPosition>();
//		for (int i = 0; i < UsablePositions.Count; i++)
//		{
//			if (!UsablePositions[i].IsUsed) {
//				emptyList.Add(UsablePositions[i]);
//			}
//		}

//        //검색된 수량이 있을 경우 랜덤 반환. 없을경우 Null.
//		if (emptyList.Count > 0)
//		{
//			int randomIndex = Mathf.Clamp(Random.Range(0, emptyList.Count), 0, emptyList.Count);
//			return emptyList[randomIndex];
//		}
//		else
//        {
//			return null;
//        }
//	}

//	/// <summary>
//	/// 유저 터치. 즉 보상 획득 이므로, 해당 위치에 대한 초기화 진행.
//	/// </summary>
//	/// <param name="code"> TouchObjectPosition의 Code값 입니다. </param>
//	public void Touch(string code)
//    {
//		if (!IsInitialized)
//			return;

//		_OnProgressQuest.Raise((int)QuestType.GET_TOUCH_REWARD);
//		_OnProgressAchievement.Raise((int)AchievementType.GET_TOUCH_REWARD);
//		GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Balance.NATURAL_PURIFICATION);

//		for (int i = 0; i < UsablePositions.Count; i++)
//		{
//			if (Equals(UsablePositions[i].Code, code) && UsablePositions[i].IsUsed)
//			{
//				UsablePositions[i].UsingTarget = null;
//				Save();
//				if (!IsMaxCount)
//					StartCoroutine(ProcessCoroutine(SpawnInterval));
//				break;
//			}
//		}
//	}

//	/// <summary>
//	/// 랜드마크 레벨 업 등, 노출되어야 할 포지션 정보가 변경되는 경우 호출됩니다.
//	/// </summary>
//	/// <param name="myLevel"> 랜드마크 레벨 </param>
//	public void RefreshPositions(int myLevel)
//	{
//        if (!IsInitialized)
//            return;

//        //사용중인 포지션의 정보 추출.
//        List<TouchObjectPosition> usableInfo = new List<TouchObjectPosition>();
//        for (int i = 0; i < UsablePositions.Count; i++)
//        {
//            if (UsablePositions[i].IsUsed)
//				usableInfo.Add(UsablePositions[i]);
//        }

//        //리스트 초기화.
//        UsablePositions.Clear();

//        //레벨에 맞는 리스트 추가.
//        for (int i = 0; i < Positions.Length; i++)
//        {
//            if (Positions[i].Level <= myLevel)
//            {
//                UsablePositions.Add(Positions[i]);

//                //코드값을 통해 사용중이던 위치는 다시 사용중으로 변경.
//                TouchObjectPosition pos = UsablePositions[UsablePositions.Count - 1];
//                for (int j = 0; j < usableInfo.Count; j++)
//                {
//                    if (Equals(pos.Code, usableInfo[j].Code))
//                    {
//						pos = usableInfo[j];
//						break;
//                    }
//                }
//            }
//        }

//		if (!IsMaxCount)
//			StartCoroutine(ProcessCoroutine(SpawnInterval));
//	}

//	public void ForceCreate()
//	{
//		if (!IsMaxCount)
//			OnCreate?.Invoke(GetRandomSpawnPosition(), false);
//	}
//}

//public enum LandType
//{
//    Ground,
//    Water
//}

//[Serializable]
//public class TouchObjectPosition
//{
//	public LandType type;
//	public int Level;
//	public Transform transform;

//    //On live.
//    [HideInInspector]
//    public string Code;
//	[HideInInspector]
//	public UIGen_TouchObject UsingTarget; //사용중인 대상.

//	public bool IsUsed { get { return UsingTarget != null; } }

//	public void SetCode(int arrayIndex) //펫을 처음 생성할때, 사용되는 함수.
//	{
//		string guid = PlayerPrefs.GetString(string.Format("TOPCODE{0}", arrayIndex), Guid.NewGuid().ToString("N"));

//		Code = guid;
//		PlayerPrefs.SetString(string.Format("TOPCODE{0}", arrayIndex), guid);
//	}
//}

//[Serializable]
//public class TOPData
//{
//	[SerializeField] public string SpawnStartTime;
//	[SerializeField] public List<TOP_SaveCode> codeList;

//	public TOPData(string _LastSpawnTime, List<TOP_SaveCode> _codeList)
//	{
//		SpawnStartTime = _LastSpawnTime;
//		codeList = _codeList;
//	}
//}

//[Serializable]
//public class TOP_SaveCode
//{
//	[SerializeField] public string code;

//	public TOP_SaveCode(string _code) {
//		code = _code;
//    }
//}

//public struct OfflineRewardInformation
//{
//	public int CreatedCount;
//	public int RemainSeconds;

//	public OfflineRewardInformation(int _createdCount, int _remainSeconds)
//    {
//		CreatedCount = _createdCount;
//		RemainSeconds = _remainSeconds;
//    }
//}
