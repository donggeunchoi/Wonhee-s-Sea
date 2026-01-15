using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonManager : Singleton<JsonManager>
{
   // 사용하는 방법은 JsonManager.Ins.fishData.Id 이런 식으로 사용합니다.
   List<FishData> fishData = new List<FishData>();
   List<SashimiData> sashimiData = new List<SashimiData>();
   List<SushiData> sushiData = new List<SushiData>();
   List<DpStandData> dpStandData = new List<DpStandData>();
   List<ItemChart> itemChartData = new List<ItemChart>();
   List<EquipmentData> equipmentData = new List<EquipmentData>();

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
   
   public List<T> GetItems<T>(string fileName)
   {
      string cacheKey = $"ITEMS:{typeof(T).FullName}:{fileName}";

      // 1. 캐시 체크
      if (jsonCache.TryGetValue(cacheKey, out var cached))
         return (List<T>)cached;

      string path = GetPath(fileName);
      List<T> list;

      // 2. 파일 없으면 Resources에서 읽어서 저장
      if (!File.Exists(path))
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
 
}
