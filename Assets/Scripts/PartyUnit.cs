using UnityEngine;

[System.Serializable]
public class PartyUnit
{
    public PartyUnitData UnitData { get; private set; }
    public PartyUnit(PartyUnitData data)
    {
        UnitData = data;
    }
}
