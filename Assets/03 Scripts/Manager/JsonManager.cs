using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class JsonManager : Singleton<JsonManager>
{
   // 사용하는 방법은 JsonManager.Ins.fishData.Id 이런 식으로 사용합니다.
   FishData fishData = new FishData();
   SashimiData sashimiData = new SashimiData();
   SushiData sushiData = new SushiData();
   DpStandData dpStandData = new DpStandData();
   ItemChart itemChartData = new ItemChart();

   private string basePath;
   
   private Dictionary<Type, object> jsonCache = new Dictionary<Type, object>();
   private void Awake()
   {
      // 플랫폼 안전 경로
      basePath = Path.Combine(Application.persistentDataPath, "JsonData");

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

   #region Util

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
   }

   #endregion
   /// <summary>
   /// 타입 기반 Json 로드 (캐싱)
   /// </summary>
   public T GetJson<T>() where T : new()
   {
      Type type = typeof(T);

      // 이미 로드된 경우
      if (jsonCache.TryGetValue(type, out var cached))
      {
         return (T)cached;
      }

      string fileName = type.Name;
      string path = GetPath(fileName);

      T data;

      if (!File.Exists(path))
      {
         data = new T();
         Save(fileName, data);
      }
      else
      {
         string json = File.ReadAllText(path);
         data = JsonConvert.DeserializeObject<T>(json);
      }

      jsonCache.Add(type, data);
      return data;
   }
   
 
}
