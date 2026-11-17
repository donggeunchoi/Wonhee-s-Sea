using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private PlayerRoot _player;
    [SerializeField] private DungeonResultUI _resultUI;

    private List<int> _settledFish;
    private bool _returning;
    private float _previousTimeScale;

    private void Start()
    {
        _player.Stats.OnDied += HandlePlayerDied;
        if (_player.Stats.IsDead)
            HandlePlayerDied();
    }

    public void RequestReturn()
    {
        FinishDive(died: false);
    }

    private void HandlePlayerDied()
    {
        FinishDive(died: true);
    }

    private void FinishDive(bool died)
    {
        if (_settledFish != null) return;

        _settledFish = new List<int>();
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (_player.Input != null) _player.Input.enabled = false;
        if (_player.Controller != null) _player.Controller.enabled = false;
        if (_player.Oxygen != null) _player.Oxygen.enabled = false;
        if (_player.Attack != null)
        {
            _player.Attack.StopSmgFire();
            _player.Attack.StopAllCoroutines();
            _player.Attack.enabled = false;
        }

        var inventory = _player.Inventory;
        if (died)
            inventory.LoseFishPercent(0.9f);

        _settledFish.AddRange(inventory.FishList);
        _resultUI.Show(_settledFish.Count, inventory.CurrentWeight, inventory.MaxWeight);
    }

    public void ReturnToTown()
    {
        if (_settledFish == null || _returning) return;
        _returning = true;

        foreach (int id in _settledFish)
        {
            string fishId = id.ToString();
            UserManager.Ins.userInfo.AddFish(fishId, 1);
            BackendManager.Ins.OnTossFish.OnNext(fishId);
        }

        BackendManager.Ins.SaveData();
        SceneManager.LoadScene("TownScene_Stage1");
    }

    private void OnDestroy()
    {
        if (_player?.Stats != null)
            _player.Stats.OnDied -= HandlePlayerDied;

        if (_settledFish != null)
            Time.timeScale = _previousTimeScale;
    }
}
