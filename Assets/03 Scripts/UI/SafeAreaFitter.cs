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

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
        
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
        
            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;
        
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
        }
    }
}
