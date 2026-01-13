using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JsonManager.Ins.Delete("SushiData");

        var wrapper = JsonManager.Ins.GetItems<SushiData>("SushiData");
        Debug.Log(wrapper.Count);

        foreach (var item in wrapper)
        {
            Debug.Log(item.id);
            Debug.Log(item.name);
        }

    }

}
