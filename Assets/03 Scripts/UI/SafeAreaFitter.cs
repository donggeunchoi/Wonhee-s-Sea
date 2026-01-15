using UnityEngine;

namespace _03_Scripts.UI
{
    /// <summary>
    /// UI 사이즈 조정
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        RectTransform _rect;
        Rect _lastSafeArea;
        Vector2Int _lastScreenSize;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void Update()
        {
            if (_lastScreenSize.x != Screen.width ||
                _lastScreenSize.y != Screen.height ||
                _lastSafeArea != Screen.safeArea)
                ApplySafeArea();
        }
        
        void ApplySafeArea()
        {
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            _lastSafeArea = Screen.safeArea;
            
            Rect safe = Screen.safeArea;
            
            Vector2 anchorMin = safe.position;
            Vector2 anchorMax = safe.position + safe.size;
            
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
        }
    }
}
