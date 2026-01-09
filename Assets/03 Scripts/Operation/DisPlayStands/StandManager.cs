using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StandManager : MonoBehaviour
{// 매대를 전체적으로 관리하기 위한 .cs
    //스텐드타입 
    public enum StandType
    {
        Sashimi,
        Rice,
        Assemble,
        Plate,
        Sell,
        Cashier
    }
    //매대 인터페이스
    public interface IStand
    {
        StandType Type { get; }
        bool IsBusy { get; }
        float progress { get; }
        void Interact(StandContext context);
    }
    public static StandManager Ins { get; private set; }
   
    private readonly List<IStand> _stands = new List<IStand>();

    private void Awake()
    {
        Ins = this;
    }

    public void Register(IStand stand)
    {
        if (_stands == null)
            return;
        if (_stands.Contains(stand))
            return;
        _stands.Add(stand);
    }

    public void Unregister(IStand stand)
    {
        _stands.Remove(stand);
    }
   
    public IStand GetNearest(Vector3 pos, float range)
    {
        IStand best = null;
        float bestSqr = range * range;

        foreach (var s in _stands)
        {
            var mb = s as MonoBehaviour;
            if (mb == null) continue;

            float sqr = (mb.transform.position - pos).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = s;
            }
        }
        return best;
    }

    public class StandContext
    {
        
    }
}