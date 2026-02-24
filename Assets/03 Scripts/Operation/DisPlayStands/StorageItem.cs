using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageItem : MonoBehaviour
{
    [Header("아이콘이 쌓일 부모 (objOutputGroup)")]
    [SerializeField] private RectTransform outputGroup;

    [Header("Icon 프리팹 (OutputIcon 포함)")]
    [SerializeField] private OutputIcon iconPrefab;

    [Header("표시 제한")]
    [SerializeField] private int maxVisible = 10;

    [Header("배치(그리드)")]
    [SerializeField] private int columns = 5;
    [SerializeField] private Vector2 cellSize = new Vector2(80, 80);
    [SerializeField] private Vector2 startOffset = Vector2.zero;

    private readonly Stack<OutputIcon> pool = new Stack<OutputIcon>();
    private readonly List<OutputIcon> active = new List<OutputIcon>();

    private OutputIcon Get()
    {
        OutputIcon icon = pool.Count > 0 ? pool.Pop() : Instantiate(iconPrefab, transform);
        icon.gameObject.SetActive(true);
        icon.transform.SetParent(outputGroup, false);
        return icon;
    }

    private void Release(OutputIcon icon)
    {
        icon.gameObject.SetActive(false);
        icon.transform.SetParent(transform, false);
        pool.Push(icon);
    }

    public void ClearAll()
    {
        for (int i = 0; i < active.Count; i++)
            Release(active[i]);

        active.Clear();
    }

    // outputCount 만큼 쌓기(보이는 건 maxVisible까지)
    public void Show(ItemSO item, int count)
    {
        ClearAll();

        if (item.icon == null || count <= 0) return;

        int visibleCount = Mathf.Min(count, maxVisible);

        for (int i = 0; i < visibleCount; i++)
        {
            var icon = Get();
            icon.SetSprite(item.icon);
            active.Add(icon);
        }

        RefreshLayout();
    }

    private void RefreshLayout()
    {
        for (int i = 0; i < active.Count; i++)
        {
            int x = i % columns;
            int y = i / columns;

            var rt = active[i].transform as RectTransform;
            if (rt != null)
                rt.anchoredPosition = startOffset + new Vector2(x * cellSize.x, -y * cellSize.y);
        }
    }
}
