using System;
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
    private Dictionary<string, object> jsonDictionaryCache = new Dictionary<string, object>();

    private Dictionary<Type, string> fileNameMap = new Dictionary<Type, string>()
    {
        { typeof(FishData), "Fish_Data" },
        { typeof(ItemDataTable), "ItemDataTable" },
        { typeof(IngredientData), "Ingredient_Data" },
        { typeof(EquipmentData), "Equipment_Data" },
        { typeof(StandData), "Stand_Data" },
    };

    protected override void Awake()
    {
        base.Awake();

        basePath = Path.Combine(Application.persistentDataPath, "Json");

        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
    }

    #region Save / Load

    public void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);

        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonManager] Save Error ({fileName}) : {e}");
        }
    }

    public T Load<T>(string fileName) where T : new()
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
            return new T();

        try
        {
            string json = File.ReadAllText(path);
            T data = JsonConvert.DeserializeObject<T>(json);
            return data == null ? new T() : data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonManager] Load Error ({fileName}) : {e}");
            return new T();
        }
    }

    #endregion

    #region Util

    private string GetPath(string fileName)
    {
        if (!fileName.EndsWith(".json"))
            fileName += ".json";

        return Path.Combine(basePath, fileName);
    }

    public void Delete(string fileName)
    {
        string path = GetPath(fileName);

        if (File.Exists(path))
            File.Delete(path);

        jsonCache.Clear();
        jsonDictionaryCache.Clear();
    }

    private string GetFileNameByType<T>()
    {
        Type type = typeof(T);

        if (fileNameMap.TryGetValue(type, out string fileName))
            return fileName;

        return type.Name;
    }

    private string GetListCacheKey<T>(string fileName)
    {
        return $"ITEMS:{typeof(T).FullName}:{fileName}";
    }

    private string GetStringDictCacheKey<T>(string fileName)
    {
        return $"DICT_STRING:{typeof(T).FullName}:{fileName}";
    }

    private string GetIntDictCacheKey<T>(string fileName)
    {
        return $"DICT_INT:{typeof(T).FullName}:{fileName}";
    }

    #endregion

    #region List

    public List<T> GetJson<T>()
    {
        string fileName = GetFileNameByType<T>();
        string cacheKey = GetListCacheKey<T>(fileName);

        if (jsonCache.TryGetValue(cacheKey, out var cached))
            return (List<T>)cached;

        string path = GetPath(fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<T> list = ParseItems<T>(json);
            jsonCache[cacheKey] = list;
            return list;
        }

        return new List<T>();
    }

    public void SetJson<T>(List<T> list)
    {
        string fileName = GetFileNameByType<T>();
        string cacheKey = GetListCacheKey<T>(fileName);
        string stringDictKey = GetStringDictCacheKey<T>(fileName);
        string intDictKey = GetIntDictCacheKey<T>(fileName);

        if (list == null)
            list = new List<T>();

        jsonCache[cacheKey] = new List<T>(list);

        if (jsonDictionaryCache.ContainsKey(stringDictKey))
            jsonDictionaryCache.Remove(stringDictKey);

        if (jsonDictionaryCache.ContainsKey(intDictKey))
            jsonDictionaryCache.Remove(intDictKey);

        Save(fileName, new ItemWrapper<T> { items = list });
    }

    private List<T> ParseItems<T>(string json)
    {
        try
        {
            json = json?.Trim();
            if (string.IsNullOrEmpty(json))
                return new List<T>();

            if (json.StartsWith("["))
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();

            JObject root = JObject.Parse(json);
            JArray itemsToken = root["items"] as JArray;

            if (itemsToken == null)
            {
                Debug.LogWarning("[JsonManager] items 키가 없거나 배열이 아님");
                return new List<T>();
            }

            List<T> result = new List<T>();

            foreach (var row in itemsToken)
            {
                try
                {
                    T obj = row.ToObject<T>();
                    if (obj != null)
                        result.Add(obj);
                }
                catch
                {
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

    #endregion

    #region Dictionary

    public Dictionary<string, T> GetJsonDicString<T>(Func<T, string> keySelector)
    {
        string fileName = GetFileNameByType<T>();
        string cacheKey = GetStringDictCacheKey<T>(fileName);

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
                Debug.LogWarning($"[JsonManager] Key가 없습니다. Type : {typeof(T).Name}, Index : {i}");
                continue;
            }

            if (dict.ContainsKey(key))
            {
                Debug.LogWarning($"[JsonManager] 중복된 Key : {key}, Type : {typeof(T).Name}");
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
        string cacheKey = GetIntDictCacheKey<T>(fileName);

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
                Debug.LogWarning($"[JsonManager] 중복 key 발견 : {key}, Type : {typeof(T).Name}");
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

    #endregion
}