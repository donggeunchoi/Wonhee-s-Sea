using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Container : MonoBehaviour
{
    [Header("표시 루트")]
    [SerializeField] private Transform worldStackRoot;

    [Header("월드 표시 옵션")]
    [SerializeField] private bool useWorldStack;
    [SerializeField] private int columnCount;
    [SerializeField] private float worldStackOffsetX;
    [SerializeField] private float worldStackOffsetY;
    [SerializeField] private int worldMaxVisible;

    [Header("보관 옵션")]
    [SerializeField] private int maxCapacity;

    [Header("프리팹 조회")]
    [SerializeField] private ItemAssetManager itemAssetManager;

    [Header("UI프리펩")]
    [SerializeField]
    FishSlotUI fishSlotUI;
    [Header("UI생성위치")]
    [SerializeField]
    Transform content;

    private string currentFishId;
    private int currentFishCount = 0;
    private readonly List<GameObject> pooled = new List<GameObject>();
    private GameObject cachedPrefab;


    // Start is called before the first frame update
    private void Awake()
    {
        //구독해놓고 던전에서 넘겨주면 받아서 표출하기.
        BackendManager.Ins.OnTossFish.AsObservable().Subscribe(_toss =>
        {
            if (_toss == null)
                return;

            if (string.IsNullOrEmpty(_toss))
                return;

            InitData(_toss);

        }).AddTo(this);

        FirstData();
    }

    public void FirstData()
    {
        RefreshVisual();
    }

    public void InitData(string _fishID)
    {
        //던전에서 넘겨주는 데이터로 초기화하기.
        for(int i = 0; i < UserManager.Ins.userInfo.fishContainer.Count; i++)
        {

        }
        currentFishCount = UserManager.Ins.userInfo.GetFishCount(_fishID);

        currentFishCount = Mathf.Clamp(currentFishCount, 0, maxCapacity);

        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (!useWorldStack)
            return;

        if (worldStackRoot == null)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (string.IsNullOrEmpty(currentFishId) || currentFishCount <= 0)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (itemAssetManager == null)
        {
            Debug.LogWarning("[Container] itemAssetManager가 연결되지 않았습니다.", this);
            SetPoolActiveCount(0);
            return;
        }

        GameObject prefab = itemAssetManager.GetPrefab(currentFishId);
        if (prefab == null)
        {
            Debug.LogWarning($"[Container] prefab 없음. fishID={currentFishId}", this);
            SetPoolActiveCount(0);
            return;
        }

        if (cachedPrefab != prefab)
        {
            ClearPool();
            cachedPrefab = prefab;
        }

        int visibleCount = Mathf.Clamp(currentFishCount, 0, worldMaxVisible);

        EnsurePool(prefab, visibleCount);
        SetPoolActiveCount(visibleCount);
        LayoutPoolGrid(visibleCount);
    }

    public void Interact(Inventory inv)
    {
        if (inv == null)
            return;

        if (string.IsNullOrEmpty(currentFishId))
            return;

        if (currentFishCount <= 0)
            return;

        while (currentFishCount > 0)
        {
            bool ok = inv.TryAdd(currentFishId, 1);
            if (!ok)
                break;

            currentFishCount--;

            // userInfo에서도 같이 차감
            UserManager.Ins.userInfo.RemoveFish(currentFishId, 1);
        }

        RefreshVisual();
    }

    private void EnsurePool(GameObject prefab, int needCount)
    {
        if (pooled.Count >= needCount)
            return;

        int createCount = needCount - pooled.Count;

        for (int i = 0; i < createCount; i++)
        {
            GameObject go = Instantiate(prefab, worldStackRoot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);
            pooled.Add(go);
        }
    }

    private void SetPoolActiveCount(int activeCount)
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            bool active = i < activeCount;

            if (pooled[i] != null && pooled[i].activeSelf != active)
                pooled[i].SetActive(active);
        }
    }

    private void LayoutPoolGrid(int visibleCount)
    {
        if (columnCount <= 0)
            columnCount = 5;

        for (int i = 0; i < visibleCount; i++)
        {
            GameObject go = pooled[i];
            if (go == null)
                continue;

            int col = i % columnCount;
            int row = i / columnCount;

            go.transform.localPosition = new Vector3(
                col * worldStackOffsetX,
                row * worldStackOffsetY,
                0f
            );

            go.transform.localRotation = Quaternion.identity;
        }
    }

    private void ClearPool()
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] != null)
                Destroy(pooled[i]);
        }

        pooled.Clear();
    }
} 
