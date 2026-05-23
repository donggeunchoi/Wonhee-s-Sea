using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Container : MonoBehaviour
{
    [Header("표시 루트")]
    [SerializeField] 
    Transform worldStackRoot;

    [Header("보관 옵션")]
    [SerializeField] 
    int maxCapacity;

    [Header("아이콘 조회")]
    [SerializeField] 
    ItemAssetManager itemAssetManager;

    [Header("UI프리펩")]
    [SerializeField]
    FishSlotUI fishSlotUI;
    [Header("UI생성위치")]
    [SerializeField]
    Transform content;

    [Header("캔버스 오브젝트")]
    [SerializeField]
    GameObject visualRoot;

    [Header("생성 슬롯 UI")]
    [SerializeField]
    List<FishSlotUI> fishSlotUIs = new List<FishSlotUI>();


    string currentFishId;
    int currentFishCount = 0;
    readonly List<GameObject> pooled = new List<GameObject>();
    GameObject cachedPrefab;


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

    }

    public void InitData(string _fishID)
    {
        //던전에서 넘겨주는 데이터로 초기화하기.
        currentFishId = _fishID;
        currentFishCount = UserManager.Ins.userInfo.GetFishCount(currentFishId);

        currentFishCount = Mathf.Clamp(currentFishCount, 0, maxCapacity);

        Sprite fishSprite = itemAssetManager.GetIcon(currentFishId);

        for (int i = 0; i < fishSlotUIs.Count; i++)
        {
            fishSlotUIs[i].FirstInit(fishSprite, currentFishId, currentFishCount, ReceiveFish);
        }
    }

    public void Interact()
    {
        SetVisualActive(true);
    }

    public void SetVisualActive(bool active)
    {
        if (visualRoot == null)
            return;

        visualRoot.SetActive(active);
    }

    void ReceiveFish(string _id) 
    {
        if (string.IsNullOrEmpty(_id))
        {
            Debug.LogWarning("없다");
            return;
        }

        Debug.LogWarning("버튼눌림"+_id);

    }
} 
