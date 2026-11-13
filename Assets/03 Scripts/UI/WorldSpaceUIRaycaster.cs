using UnityEngine.UI;

public class WorldSpaceUIRaycaster : GraphicRaycaster
{
    public override int sortOrderPriority => int.MinValue + 1;

}
