using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

public static class ExcelReader
{
    public static string EXCEL_PATH = Path.Combine(Application.dataPath, "09 Excel");

    public static T[] Load<T>(string fileName, Func<string[], T> map, string sheetName = null)
    {
        string fullPath = Path.Combine(EXCEL_PATH, fileName + ".xlsx");

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Can't Find ExcelFile : {fullPath}");
            return null;
        }
        
        // 한글 깨짐 방지 : RegisterProvider -> 시스템 메모리에 등록하겠다  :  CodePagesEncodingProvider -> 언어 인코딩 사전  (한번만 호출하면 됩니다)
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        List<T> list = new List<T>();

        // using은 동적할당
        using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // (이 위치에 있는, 파일을 열건데, 읽기전용으로 열거고, 내가 사용하는 도중에 다른사람이 사용하는 것을 허락할거야)
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream)) // CreateReader는 파일 형식을 감지해주고, 데이터를 한줄한줄 스캔할 준비를 해줌
            {
                var result = reader.AsDataSet(); // 데이터를 표 형태로 변환
                DataTable table = (sheetName != null) ? result.Tables[sheetName] : result.Tables[0];

                for (int i = 2; i < table.Rows.Count; ++i)
                {
                    int colCount = table.Columns.Count;
                    string[] cols = new string[colCount];
                    bool isEmptyRow = true;

                    for (int j = 0; j < colCount; ++j)
                    {
                        cols[j] = table.Rows[i][j]?.ToString().Trim() ?? "";
                        if (!string.IsNullOrEmpty(cols[j])) isEmptyRow = false;
                    }

                    if (!isEmptyRow) list.Add(map(cols));
                }

            }
        }
        
        return list.ToArray();
    }

    // 파싱 안전장치
    public static T ParseSafe<T>(string s, T defaultValue = default)
    {
        if (typeof(T) == typeof(int)) return (T)(object)(int.TryParse(s, out var v) ? v : (object)defaultValue);
        if (typeof(T) == typeof(float)) return (T)(object)(float.TryParse(s, out var v) ? v : (object)defaultValue);
        if (typeof(T) == typeof(bool))
        {
            var lower = s.ToLower();
            if (lower == "true" || lower == "1" || lower == "y") return (T)(object)true;
            return (T)(object)false;
        }
        if (typeof(T) == typeof(string)) return (T)(object)(string.IsNullOrEmpty(s) ? defaultValue : (object)s);

        return defaultValue;
    }

    public static T EnumParseSafe<T>(string raw, T @default) where T : struct, Enum =>
        string.IsNullOrWhiteSpace(raw) ? @default : (Enum.TryParse<T>(raw.Trim(), true, out var val) ? val : @default);

    public static void SaveToJson<T>(T[] data, string fileName)
    {
        SerializationWrapper<T> wrapper = new SerializationWrapper<T> {items = data};

        string json = JsonUtility.ToJson(wrapper, true);

        string path = Path.Combine(Application.dataPath, "Resource/Data");

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, fileName +  ".json"), json);

        Debug.Log($"JSON 저장 완료: {path}/{fileName}.json");
    }
}

[System.Serializable]
public class SerializationWrapper<T>
{
    public T[] items;
}
