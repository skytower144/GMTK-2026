using UnityEngine;

[CreateAssetMenu(fileName = "PartyUnitData", menuName = "Scriptable Objects/PartyUnitData")]
public class PartyUnitData : ScriptableObject
{
    [field: SerializeField] public string Name;
    [field: SerializeField] public Sprite IconSprite;
    [field: SerializeField, TextArea] public string Description;
    [field: SerializeField] public float MaxHp;
    [field: SerializeField] public float Attack;
}
