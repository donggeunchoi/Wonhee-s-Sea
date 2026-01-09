using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outputRenderer;
    [SerializeField] private ItemDatabaseSO db;

    public void ShowOutput(string itemId)
    {
        if (outputRenderer == null) return;

        if (string.IsNullOrEmpty(itemId))
        {
            outputRenderer.enabled = false;
            outputRenderer.sprite = null;
            return;
        }

        outputRenderer.enabled = true;
        outputRenderer.sprite = db != null ? db.GetIcon(itemId) : null;
    }
}
