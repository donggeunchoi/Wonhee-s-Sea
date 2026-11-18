using UnityEngine.UI;

public class JoystickGraphicRaycaster : GraphicRaycaster
{
    public override int sortOrderPriority => int.MinValue;
    public override int renderOrderPriority => int.MinValue;
}
