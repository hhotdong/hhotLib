/// 
/// File:				RSDAsset.cs
/// 
/// System:				Rapid Sheet Data (RSD) Unity3D client library
/// Version:			1.0.0
/// 
/// Language:			C#
/// 
/// License:				
/// 
/// Author:				Tasos Giannakopoulos (tasosg@voidinspace.com)
/// Date:				08 Mar 2017
/// 
/// Description:		The RSDAsset can be created as an asset in the editor and configured in the inspector.
///                     It provides an interface to pull data from the backend and query that data, getting them
///                     as class instances in run time.
///                     It also caches a version of the data that can be used in a final build, or as fallback data.
/// 


using System;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Lib.RapidSheetData
{
    /// 
    /// Class:       RSDObject
    /// Description: Defines a class where sheet data can be deserialized to
    /// 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RSDObject : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public RSDObject() { }
    }

    /// 
    /// Class:       RSDSheetDesc
    /// Description: Defines how a sheet will be pulled and deserialized to a target class
    ///
    [Serializable]
    public class RSDSheetDesc
    {
        /// 
        /// Enum:           Dimension
        /// Description:    How the sheet data will be read
        ///
        public enum Dimension
        {
            Rows = 0,
            Columns
        }

        // 
        public string SheetName { get { return _sheetName; } }
        public string TargetClass { get { return _targetClass; } }
        public Dimension MajorDimension { get { return _majorDimension; } }

        // 
        [SerializeField]
        private string _sheetName = "";

        [SerializeField]
        private string _targetClass = null;

        [SerializeField]
        private Dimension _majorDimension = Dimension.Rows;
    }

    /// 
    /// Class:       RSDAsset
    /// Description: 
    ///
    [CreateAssetMenu(fileName = "New Rapid Sheet Data Asset")]
    public class RSDAsset : ScriptableObject
    {
        // 
        [SerializeField]
        private string _serverURL = "";

        [SerializeField]
        private string _spreadsheetId = "";

        // List of sheets to pull from the RSD service
        [SerializeField, HideInInspector]
        private List<RSDSheetDesc> _sheets = null;

        // 
        [SerializeField, HideInInspector]
        private string _cachedJsonData = null;

        // Deserialized cached data
        private Dictionary<string, Dictionary<string, object>> _cachedData = null;

        // MonoBehaviour parent to execute fetch requests asynchronously
        private MonoBehaviour _parent = null;

        // RSD Serializer
        private IRSDSerializer _serializer = null;

        private const string DataIdScriptFolder = "Assets/2_Scripts/DataIds.cs";

        //[SerializeField] private GameManager _GameManager;

        /// 
        /// Public Interface
        /// 

        #region Public Interface

        /// <summary>
        /// Initializes the Rapid Sheet Data asset.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public bool Init(MonoBehaviour parent = null, IRSDSerializer serializer = null)
        {
            _parent = parent;

            _serializer = serializer;
            if (_serializer == null)
            {
                // Default serializer
                Debug.LogFormat("[RSDAsset] Init : Initializing default RSD serializer 'RSDSerializerDefaultLit'");
                _serializer = new RSDSerializerDefaultLit();
            }

            return DeserializeData(_cachedJsonData);
        }

        /// <summary>
        /// Pull data and deserialize without caching them in the asset.
        /// Used in runtime mode.
        /// </summary>
        /// <param name="onCompleted"></param>
        /// <returns>true on success</returns>
        public bool PullData(Action<bool> onCompleted)
        {
            return PullData(false, true, onCompleted);
        }

        /// <summary>
        /// Pull data and cache them in the asset. 
        /// Used in the RSDAssetInspector.cs in edit mode
        /// </summary>
        /// <returns>true on success</returns>
        public bool PullDataAndCache()
        {
            return PullData(true, false);
        }

        /// <summary>
        /// Returns all sheet data as a list of instances of class T
        /// </summary>
        /// <typeparam name="T">Target class type</typeparam>
        /// <param name="sheet">Sheet name</param>
        /// <returns></returns>
        public List<T> GetSheet<T>(string sheet)
        {
            List<T> dataObjects = new List<T>();

            if (_cachedData != null)
            {
                Dictionary<string, object> data = null;
                if (_cachedData.TryGetValue(sheet, out data))
                {
                    foreach (var kvp in data)
                    {
                        dataObjects.Add((T)kvp.Value);
                    }
                }
                else
                {
                    Debug.LogWarningFormat("[RSDAsset] GetSheet : Cannot to find '{0}' in cached data ...", sheet);
                }
            }

            return dataObjects;
        }

        /// <summary>
        /// Get data from the sheet by index
        /// </summary>
        /// <typeparam name="T">Target class type</typeparam>
        /// <param name="index">Index in the row or column</param>
        /// <param name="sheet">Sheet name</param>
        /// <returns></returns>
        public T GetFromSheet<T>(int index, string sheet)
        {
            var sheetData = GetSheet<T>(sheet);
            if ((index >= 0) && (index < sheetData.Count))
            {
                return sheetData[index];
            }

            return default(T);
        }

        /// <summary>
        /// Get data from the sheet by ID
        /// </summary>
        /// <typeparam name="T">Target class type</typeparam>
        /// <param name="id">Unique ID of the instance</param>
        /// <param name="sheet"Sheet name></param>
        /// <returns></returns>
        public T GetFromSheet<T>(string id, string sheet)
        {
            if (_cachedData != null)
            {
                Dictionary<string, object> sheetData = null;
                if (_cachedData.TryGetValue(sheet, out sheetData))
                {
                    object instance = null;
                    if (sheetData.TryGetValue(id, out instance))
                    {
                        return (T)instance;
                    }
                    else
                    {
                        Debug.LogWarningFormat("[RSDAsset] GetFromSheet : Cannot to find '{0}' in 'sheet' ...", id, sheet);
                    }
                }
                else
                {
                    Debug.LogWarningFormat("[RSDAsset] GetFromSheet : Cannot to find '{0}' in cached data ...", sheet);
                }
            }

            return default(T);
        }

        #endregion /// Public Interface

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="deserialize"></param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        private bool PullData(bool cache = true, bool deserialize = true, Action<bool> onCompleted = null)
        {
            Debug.LogFormat("[RSDAsset] PullData : {0}, {1}",
                (cache ? "Cache" : "Do not cache"),
                (deserialize ? "Deserialize" : "Do not deserialize"));

            // Create fetch request
            string requestData = "";
            {
                for (int idx = 0; idx < _sheets.Count; ++idx)
                {
                    requestData += string.Format("{0}:{1}", _sheets[idx].SheetName, _sheets[idx].MajorDimension);
                    if (idx < (_sheets.Count - 1))
                    {
                        requestData += ",";
                    }
                }
            }

            // Fetch
            RSDFetchRequest request = RSDFetchRequest.Create(_serverURL, _spreadsheetId, requestData, _parent);
            if (request != null)
            {
                return request.Fetch((bool success, string szData) =>
                {
                    if (success)
                    {
                        if (cache) { _cachedJsonData = szData; }
                        if (deserialize) { DeserializeData(szData); }
                    }

                    if (onCompleted != null)
                    {
                        onCompleted(success);
                    }
                });
            }

            return false;
        }

        /// <summary>
        /// Deserializes the cached data
        /// </summary>
        /// <returns></returns>
        private bool DeserializeData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (_serializer != null)
                {
                    List<SheetSerializeDesc> sheetConfig = new List<SheetSerializeDesc>();
                    {
                        foreach (var sheet in _sheets)
                        {
                            var type = Type.GetType(sheet.TargetClass);
                            sheetConfig.Add(new SheetSerializeDesc(sheet.SheetName, type));
                        }
                    }

                    _cachedData = _serializer.Deserialize(sheetConfig, data);

                    return true;
                }
                else
                {
                    Debug.LogWarning("[RSDAsset] DeserializeData : _serialized is null");
                }
            }

            return false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// (Custom) Update data Ids.
        /// </summary>
        public void UpdateDataIds()
        {
            if (_serializer == null)
            {
                _serializer = new RSDSerializerDefaultLit();
            }

            PullData(true, true, (success) =>
            {
                if (success)
                {
                    if (_cachedJsonData != null)
                    {
                        Dictionary<string, string> allIds = new Dictionary<string, string>();
                        for (int idx = 0; idx < _sheets.Count; ++idx)
                        {
                            string sheetName = _sheets[idx].SheetName;
                            Dictionary<string, object> sheetData = null;
                            if (_cachedData != null)
                            {
                                if (_cachedData.TryGetValue(sheetName, out sheetData))
                                {
                                    foreach (var item in sheetData)
                                    {
                                        string id = item.Key.Replace(" ", string.Empty);
                                        if (id.Any(char.IsDigit))
                                        {
                                            string textsWithoutNum = new String(id.Where((c) =>
                                            {
                                                return char.IsLetter(c) || c == '_';
                                            }).ToArray());

                                            if (textsWithoutNum[textsWithoutNum.Length - 1] == '_' && textsWithoutNum[textsWithoutNum.Length - 2] == '_')
                                            {
                                                textsWithoutNum = textsWithoutNum.Substring(0, textsWithoutNum.Length - 1);
                                            }

                                            if (!allIds.ContainsKey(textsWithoutNum))
                                                allIds.Add(textsWithoutNum, textsWithoutNum);
                                        }

                                        allIds.Add(id, id);
                                    }
                                }

                                string sheetName_ = "SHEET_NAME_" + sheetName.Replace(" ", "_");
                                allIds.Add(sheetName_.ToUpper(), sheetName);
                            }
                            else
                                Debug.Log("CachedData is null!");
                        }

                        WriteIdClass(allIds, DataIdScriptFolder);
                        UnityEditor.AssetDatabase.Refresh();
                    }
                    else
                        Debug.Log("There is no cachedJsonData!");
                }
                else
                    Debug.Log("Fetching failed!");
            });
        }

        private static void WriteIdClass(Dictionary<string, string> idPaths, string filePath)
        {
            var targetUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace("ForrestIsland");
            var targetClass = new CodeTypeDeclaration("DataIds")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };

            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);

            foreach (var idPathPair in idPaths)
            {
                var idField = new CodeMemberField(typeof(string), idPathPair.Key)
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    InitExpression = new CodePrimitiveExpression(idPathPair.Value)
                };

                targetClass.Members.Add(idField);
            }

            GenerateCSharpCode(targetUnit, filePath);
        }

        private static void GenerateCSharpCode(CodeCompileUnit targetUnit, string fileName)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();

            using (var sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
                Debug.Log("Data Ids are updated!");
            }
        }

        public void FetchRemoteInitializeData()
        {
            //_GameManager.FetchAndUpdateData();
        }
#endif
    }
} /// Lib.RapidSheetData