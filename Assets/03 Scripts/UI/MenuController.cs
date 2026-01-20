using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _03_Scripts.UI
{
    public class MenuController : MonoBehaviour
    {
        public enum MenuType { Shop, Town, Dungeon }

        [System.Serializable]
        public class MenuVisual
        {
            public MenuType menu;
            public Button button;
            public Image icon;
            public Sprite normalIcon;
            public Sprite selectedIcon;
        }

        [Header("메뉴")]
        [SerializeField] private MenuVisual[] menuBtns;

        [Header("판넬")]
        [SerializeField] private GameObject shopPanel;

        [Header("던전씬")]
        [SerializeField] private string dungeonSceneName = "DungeonScene";

        private MenuType _currentMenu = MenuType.Town;

        void Awake()
        {
            // 각 메뉴버튼 기능 부여
            foreach (var menu in menuBtns)
            {
                if (menu == null || menu.button == null) continue;

                var menuType = menu.menu;
                menu.button.onClick.AddListener(() => OnClickMenu(menuType));
            }

            // 시작 상태는 Town입니당
            SetMenu(MenuType.Town, closeShopPanel: true);
        }
        
        void OnClickMenu(MenuType menu)
        {
            switch (menu)
            {
                // 상점 버튼
                case MenuType.Shop:
                    if (_currentMenu == MenuType.Shop)
                    {
                        SetMenu(MenuType.Town, closeShopPanel: true);
                    }
                    else
                    {
                        SetMenu(MenuType.Shop, closeShopPanel: false);
                        if (shopPanel != null) shopPanel.SetActive(true);
                    }
                    break;

                // 마을 버튼
                case MenuType.Town:
                    SetMenu(MenuType.Town, closeShopPanel: true);
                    break;

                // 바다 버튼
                case MenuType.Dungeon:
                    if (shopPanel != null) shopPanel.SetActive(false);
                    SetMenu(MenuType.Dungeon, closeShopPanel: true);

                    if (!string.IsNullOrEmpty(dungeonSceneName))
                        SceneManager.LoadScene(dungeonSceneName);
                    break;
            }
        }

        // 메뉴 변경 적용
        void SetMenu(MenuType menu, bool closeShopPanel)
        {
            _currentMenu = menu;

            if (closeShopPanel && shopPanel != null)
                shopPanel.SetActive(false);

            ApplyIconVisual(menu);
        }

        // 아이콘 변경
        void ApplyIconVisual(MenuType selected)
        {
            foreach (var menuBtn in menuBtns)
            {
                if (menuBtn == null || menuBtn.icon == null) continue;

                bool isSelected = (menuBtn.menu == selected);

                // 버튼별로 지정한 스프라이트를 사용
                if (isSelected && menuBtn.selectedIcon != null)
                    menuBtn.icon.sprite = menuBtn.selectedIcon;
                else if (!isSelected && menuBtn.normalIcon != null)
                    menuBtn.icon.sprite = menuBtn.normalIcon;
            }
        }

        // Dim 선택용
        public void CloseShopToTown()
        {
            SetMenu(MenuType.Town, closeShopPanel: true);
        }
    }
}
