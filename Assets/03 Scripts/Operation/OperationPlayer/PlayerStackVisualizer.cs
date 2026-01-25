using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStackVisualizer : MonoBehaviour
{
    [Header("머리 위 기준점")]
    [SerializeField]
    private Transform stackRoot;

    private float offsetY = 0.3f;

    private int maxStack = 5; // 이후 유저매니저에서 받아오도록 변경예정

    private List<GameObject> visualStack = new List<GameObject>();

    public int Count => visualStack.Count;


    // 아이템 스택에 아이템 추가
    public bool Add(GameObject itemPrefab)
    {
        if(Count >= maxStack)
        {
            return false;
        }

        GameObject obj = Instantiate(itemPrefab, stackRoot);

        obj.transform.localRotation = Quaternion.identity;

        visualStack.Add(obj);
        Reposition();
        return true;
    }

    // 아이템 스택에서 아이템 제거
    public GameObject Remove()
    {
        if(Count == 0)
        {
            return null;
        }

        int lastIndex = Count - 1;

        GameObject obj = visualStack[lastIndex];
        visualStack.RemoveAt(lastIndex);
        if (obj != null) Destroy(obj);

        Reposition();
        return obj;
    }

    //여러개 들고 있는경우 없애주는 코드.
    public int RemoveMany(int amount)
    {
        int removed = 0;

        for (int i = 0; i < amount; i++)
        {
            if (Count == 0) break;

            int lastIndex = Count - 1;
            GameObject obj = visualStack[lastIndex];
            visualStack.RemoveAt(lastIndex);

            if (obj != null) Destroy(obj);  
            removed++;
        }

        Reposition();
        return removed;
    }



    public void Reposition()
    {
        for(int i = 0; i < visualStack.Count; i++)
        {
            var obj = visualStack[i];
            if (obj == null)
                continue;

            obj.transform.localPosition = Vector3.up * (offsetY * i);
        }
    }
    public void SetMax(int newMax) => maxStack = newMax;
}
