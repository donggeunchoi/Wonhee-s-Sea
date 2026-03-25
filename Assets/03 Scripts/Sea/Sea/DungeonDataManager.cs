using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public static class DungeonDataManager
{
    private static Dictionary<int, FishData> _cache;

    public static FishData Get(int id)
    {
        var dict = LoadById();

        if (dict.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"FishData 없음: {id}");
        return null;
    }

    public static Dictionary<int, FishData> LoadById()
    {
        if (_cache != null)
            return _cache;

        _cache = new Dictionary<int, FishData>();

        var list = Load();

        foreach (var fish in list)
        {
            if (!_cache.ContainsKey(fish.fishId))
                _cache.Add(fish.fishId, fish);
        }

        return _cache;
    }

    private static List<FishData> Load()
    {
        TextAsset asset = Resources.Load<TextAsset>("Json/Fish_Data");

        if (asset == null)
        {
            Debug.LogError("Fish_Data.json 없음");
            return new List<FishData>();
        }

        return Parse(asset.text);
    }

    private static List<FishData> Parse(string json)
    {
        var list = new List<FishData>();

        try
        {
            json = json.Trim();

            if (json.StartsWith("["))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<FishData>>(json);

            var root = JObject.Parse(json);
            var items = root["items"] as JArray;

            if (items == null)
                return list;

            foreach (var row in items)
            {
                try
                {
                    var data = row.ToObject<FishData>();
                    if (data != null)
                        list.Add(data);
                }
                catch { }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파싱 에러: {e}");
        }

        return list;
    }
}
