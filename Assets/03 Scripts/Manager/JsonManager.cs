using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonManager : Singleton<JsonManager>
{
    private string basePath;

    [Serializable]
    private class ItemWrapper<TItem>
    {
        public List<TItem> items = new List<TItem>();
    }

    private Dictionary<string, object> jsonCache = new Dictionary<string, object>();
    protected override void Awake()
    {
        base.Awake();
        // 플랫폼 안전 경로
        basePath = Path.Combine(Application.persistentDataPath, "Json");

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
    }


    #region Save

    /// <summary>
    /// 데이터를 JSON 파일로 저장
    /// </summary>
    public void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);

        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[JsonManager] Save Error ({fileName}) : {e}");
        }
    }

    #endregion

    #region Load

    /// <summary>
    /// JSON 파일 로드 (없으면 기본값 반환)
    /// </summary>
    public T Load<T>(string fileName) where T : new()
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
        {
            // 파일이 없으면 기본 객체 생성 후 저장
            T newData = new T();
            Save(fileName, newData);
            return newData;
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[JsonManager] Load Error ({fileName}) : {e}");
            return new T();
        }
    }

    #endregion

    //#region Util

    private string GetPath(string fileName)
    {
        if (!fileName.EndsWith(".json"))
            fileName += ".json";

        return Path.Combine(basePath, fileName);
    }

    /// <summary>
    /// 특정 JSON 파일 삭제
    /// </summary>
    public void Delete(string fileName)
    {
        string path = GetPath(fileName);
        if (File.Exists(path))
            File.Delete(path);

        // 캐시도 같이 날리기(간단 버전)
        jsonCache.Clear();
    }

    private Dictionary<Type, string> fileNameMap = new Dictionary<Type, string>()
    {
        { typeof(FishData), "Fish_Data" },
        { typeof(ItemDataTable), "ItemDataTable" },
        { typeof(IngredientData), "Ingredient_Data" },
        { typeof(EquipmentData), "Equipment_Data" },
        { typeof(StandData), "Stand_Data" },
    };

    private string GetFileNameByType<T>()
    {
        Type type = typeof(T);

        if (fileNameMap.TryGetValue(type, out string fileName))
            return fileName;

        return type.Name;
    }

    public List<T> GetJson<T>()
    {
        string fileName = GetFileNameByType<T>();
        string cacheKey = $"ITEMS:{typeof(T).FullName}:{fileName}";

        // 1. 캐시 체크
        if (jsonCache.TryGetValue(cacheKey, out var cached))
            return (List<T>)cached;

        string path = GetPath(fileName);
        List<T> list;

        bool forceReload = true; // 임시 변수

        // 2. 파일 없으면 Resources에서 읽어서 저장
        if (!File.Exists(path) || forceReload)
        {
            list = LoadItemsFromResources<T>(fileName);
            Save(fileName, new ItemWrapper<T> { items = list });
        }
        else
        {
            string json = File.ReadAllText(path);
            list = ParseItems<T>(json);
        }

        jsonCache[cacheKey] = list;
        return list;
    }

    private List<T> LoadItemsFromResources<T>(string fileName)
    {
        TextAsset asset = Resources.Load<TextAsset>($"Json/{fileName}");
        if (asset == null)
        {
            Debug.LogWarning($"[JsonManager] Resources/Json/{fileName}.json 없음");
            return new List<T>();
        }

        return ParseItems<T>(asset.text);
    }
    private List<T> ParseItems<T>(string json)
    {
        try
        {
            json = json?.Trim();
            if (string.IsNullOrEmpty(json))
                return new List<T>();

            // 배열이면 그대로 시도
            if (json.StartsWith("["))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();

            // 객체면 items 배열 뽑기
            var root = Newtonsoft.Json.Linq.JObject.Parse(json);
            var itemsToken = root["items"] as Newtonsoft.Json.Linq.JArray;

            if (itemsToken == null)
            {
                Debug.LogWarning("[JsonManager] items 키가 없거나 배열이 아님");
                return new List<T>();
            }

            var result = new List<T>();

            // 한 줄씩 안전 파싱(실패한 행은 스킵)
            foreach (var row in itemsToken)
            {
                try
                {
                    var obj = row.ToObject<T>();
                    if (obj != null) result.Add(obj);
                }
                catch
                {
                    // 여기서 "int" 같은 타입행/헤더행이 걸러짐
                    continue;
                }
            }

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonManager] ParseItems Error: {e}");
            return new List<T>();
        }
    }


    ///<summory>
    ///string 키용 딕셔너리
    ///</summory>>
    private Dictionary<string, object> jsonDictionaryCache = new Dictionary<string, object>();
    public Dictionary<string, T> GetJsonDicString<T>(Func<T, string> keySelector)
    {
        string fileName = GetFileNameByType<T>();
        string cacheKey = $"DICT_STRING:{typeof(T).FullName}:{fileName}";

        if (jsonDictionaryCache.TryGetValue(cacheKey, out var cached))
            return (Dictionary<string, T>)cached;

        List<T> lists = GetJson<T>();
        Dictionary<string, T> dict = new Dictionary<string, T>();


        for (int i = 0; i < lists.Count; i++)
        {
            T data = lists[i];
            string key = keySelector(data);

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("JsonManager Key가 없습니다.");
                continue;
            }

            if (dict.ContainsKey(key))
            {
                Debug.LogWarning($"JsonManager 중복된 Key: {key}");
                continue;
            }

            dict.Add(key, data);
        }

        jsonDictionaryCache[cacheKey] = dict;
        return dict;
    }

    public Dictionary<int, T> GetJsonDicInt<T>(Func<T, int> keySelector)
    {
        string fileName = GetFileNameByType<T>();

        string cacheKey = $"DICT_INT:{typeof(T).FullName}:{fileName}";

        if (jsonDictionaryCache.TryGetValue(cacheKey, out var cached))
            return (Dictionary<int, T>)cached;

        List<T> lists = GetJson<T>();
        Dictionary<int, T> dict = new Dictionary<int, T>();

        for (int i = 0; i < lists.Count; i++)
        {
            T data = lists[i];
            int key = keySelector(data);

            if (dict.ContainsKey(key))
            {
                Debug.LogWarning($"[JsonManager] 중복 key 발견: {key}, Type: {typeof(T).Name}");
                continue;
            }

            dict.Add(key, data);
        }

        jsonDictionaryCache[cacheKey] = dict;
        return dict;

    }

    public T GetDataByKey<T>(string key, Func<T, string> keySelector)
    {
        Dictionary<string, T> dict = GetJsonDicString(keySelector);

        if (dict.TryGetValue(key, out T value))
            return value;

        return default;
    }

    public T GetDataByKey<T>(int key, Func<T, int> keySelector)
    {
        Dictionary<int, T> dict = GetJsonDicInt(keySelector);

        if (dict.TryGetValue(key, out T value))
            return value;

        return default;
    }
}
