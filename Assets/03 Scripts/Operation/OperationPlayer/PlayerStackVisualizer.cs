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

    [Header("물건이 쌓이 리스트")]
    [SerializeField]
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

    public int RemoveManyById(string itemId, int amount)
    {
        if (amount <= 0) return 0;
        if (string.IsNullOrEmpty(itemId)) return 0;

        int removed = 0;

        // 뒤에서부터 제거(최근에 쌓인 것부터)
        for (int i = visualStack.Count - 1; i >= 0 && removed < amount; i--)
        {
            var obj = visualStack[i];
            if (obj == null)
            {
                visualStack.RemoveAt(i);
                continue;
            }

        }

        if (removed > 0) Reposition();
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
