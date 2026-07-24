using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager instance { get; private set; }
    public static event Action OnAddUnit = null;

    private readonly List<PartyUnit> partyUnits = new();
    public List<PartyUnit> PartyUnits => partyUnits;

    void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("Detected multiple PartyManagers");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void AddUnit(PartyUnitData data)
    {
        partyUnits.Add(new PartyUnit(data));
        OnAddUnit?.Invoke();
    }
}
