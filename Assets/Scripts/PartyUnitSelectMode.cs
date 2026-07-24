using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyUnitSelectMode : MonoBehaviour
{
    public PartyUnitData[] UnitDataArr { get; private set; }

    void Awake()
    {
        LoadAllUnits();
    }

    private void LoadAllUnits()
    {
        UnitDataArr = Resources.LoadAll<PartyUnitData>("UnitData");
    }
}
