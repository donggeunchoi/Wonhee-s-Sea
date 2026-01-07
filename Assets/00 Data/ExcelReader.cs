using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

public static class ExcelReader
{
    public static string EXCEL_PATH = Path.Combine(Application.dataPath, "Excel");

    public static T[] Load<T>(string fileName, Func<string[], T> map, string sheetName = null)
    {
        string fullPath = Path.Combine(EXCEL_PATH, fileName + ".xlsx");

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Can't Find ExcelFile : {fullPath}");
            return null;
        }
        
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var list = new List<T>();

        using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
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
}
