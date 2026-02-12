using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    [SerializeField] private GameObject _returnButton;

    private void Awake()
    {
        if (_returnButton != null)
            _returnButton.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (_returnButton != null)
            _returnButton.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (_returnButton != null)
            _returnButton.SetActive(false);
    }
}
