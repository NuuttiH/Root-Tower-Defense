using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    private List<Mole> _moles;
    [SerializeField] private int _income;
    [HideInInspector] public int Income { 
        get { return _income; } 
    }

    void Start()
    {
        _moles = new List<Mole>();
        GameManager.RegisterTreasure(this);
    }

    void OnDestroy()
    {
        GameManager.UnregisterTreasure(this);
        foreach(Mole mole in _moles) 
            if(mole != null) mole.SetNewTarget();
        RootSystem.BuildBuilding(
            RootSystem.GridLayout.WorldToCell(this.gameObject.transform.position), 
            BuildingType.Destroyed);
    }
    
    public void RegisterMole(Mole mole)
    {
        _moles.Add(mole);
    }
    public void UnregisterMole(Mole mole)
    {
        _moles.Remove(mole);
    }
}
