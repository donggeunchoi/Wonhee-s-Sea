using UnityEngine.UI;
using UnityEngine;

public enum Shop_Category_UIState
{
    Stand,
    Equipment,
    Employee
}

public class Shop_PanelManager : MonoBehaviour
{
    [SerializeField] private Button Category_Ingredient;
    [SerializeField] private Button Category_Equipment;
    [SerializeField] private Button Category_Employee;

    private Shop_Category_StandUI ingredientUI;
    private Shop_Category_EquipmentUI equipmentUI;
    private Shop_Category_EmplyoeeUI emplyoeeUI;

    private Shop_Category_UIState currentState;

    private void Start()
    {
        ingredientUI = GetComponentInChildren<Shop_Category_StandUI>(true);
        ingredientUI.Init(this);
        equipmentUI = GetComponentInChildren<Shop_Category_EquipmentUI>(true);
        equipmentUI.Init(this);
        emplyoeeUI = GetComponentInChildren<Shop_Category_EmplyoeeUI>(true);
        emplyoeeUI.Init(this);

        Category_Ingredient.onClick.AddListener(OnCLickIngredientButton);
        Category_Equipment.onClick.AddListener(OnCLickEquipmentButton);
        Category_Employee.onClick.AddListener(OnCLickEmployeeButton);

        ChangeState(Shop_Category_UIState.Stand);
    }

    public void ChangeState(Shop_Category_UIState state)
    {
        currentState = state;

        ingredientUI.SetActive(currentState);
        equipmentUI.SetActive(currentState);
        emplyoeeUI.SetActive(currentState);
    }

    public void OnCLickIngredientButton()
    {
        ChangeState(Shop_Category_UIState.Stand);
    }
    
    public void OnCLickEquipmentButton()
    {
        ChangeState(Shop_Category_UIState.Equipment);
    }

    public void OnCLickEmployeeButton()
    {
        ChangeState(Shop_Category_UIState.Employee);
    }

}
