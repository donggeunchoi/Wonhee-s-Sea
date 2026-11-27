using UnityEngine;
using UnityEngine.UI;

public class DungeonCombatInput : MonoBehaviour
{
    [SerializeField] private GameObject _attackButtonObj;
    [SerializeField] private GameObject _aimJoystickObj;

    private PlayerRoot _owner;
    private DungeonAttackButton _holdButton;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        if (_aimJoystickObj != null) _aimJoystickObj.SetActive(false);
        if (_attackButtonObj == null) return;

        _attackButtonObj.SetActive(true);
        if (!_attackButtonObj.TryGetComponent<Button>(out var button)) return;

        _holdButton = _attackButtonObj.AddComponent<DungeonAttackButton>();
        _holdButton.Init(button, this);
        _holdButton.enabled = isActiveAndEnabled;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) || (_holdButton != null && _holdButton.IsHeld))
            OnAttackPressed();
    }

    public void OnAttackPressed()
    {
        if (isActiveAndEnabled && Time.timeScale > 0f && _owner != null)
            _owner.Attack?.OnAttack();
    }

    private void OnEnable()
    {
        if (_holdButton != null)
            _holdButton.enabled = true;
    }

    private void OnDisable()
    {
        if (_holdButton != null)
            _holdButton.enabled = false;
    }
}
