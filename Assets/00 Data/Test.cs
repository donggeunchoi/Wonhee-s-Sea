using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Test : MonoBehaviour
{
    [ContextMenu("Execute Excel To JSON")]
    public void Process()
    {
        var test = ExcelReader.Load<TestExcel>("TestExcel", cols => new TestExcel {
            id = ExcelReader.ParseSafe<int>(cols[0]),
            name = ExcelReader.ParseSafe<string>(cols[1]),
            price = ExcelReader.ParseSafe<int>(cols[2])
        });

        if (test != null)
        {
            ExcelReader.SaveToJson(test, "TestExcel");
        }
    }
}

[Serializable]
public class TestExcel 
{
    public int id;
    public string name;
    public int price;
}
