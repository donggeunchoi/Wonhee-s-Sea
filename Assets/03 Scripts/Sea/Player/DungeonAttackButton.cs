using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DungeonAttackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, ISubmitHandler
{
    private Button _button;
    private DungeonCombatInput _input;
    private int? _pointerId;

    public bool IsHeld => _pointerId.HasValue && isActiveAndEnabled && _button.IsInteractable();

    public void Init(Button button, DungeonCombatInput input)
    {
        _button = button;
        _input = input;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pointerId.HasValue || eventData.button != PointerEventData.InputButton.Left ||
            !_button.IsInteractable() || !_input.isActiveAndEnabled) return;

        _pointerId = eventData.pointerId;
        _input.OnAttackPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_pointerId == eventData.pointerId)
            CancelHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerUp(eventData);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (_button.IsInteractable())
            _input.OnAttackPressed();
    }

    private void CancelHold() => _pointerId = null;

    private void OnDisable() => CancelHold();

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            CancelHold();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            CancelHold();
    }
}
