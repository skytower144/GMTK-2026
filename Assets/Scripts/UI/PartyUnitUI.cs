using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartyUnitUI : SelectableUI
{
    [SerializeField] private Image unitIcon;
    private PartyUnitData unitData;

    public void SetDisplayingUnit(PartyUnitData data)
    {
        unitData = data;
        unitIcon.sprite = data.IconSprite;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        PartyManager.instance.AddUnit(unitData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }
}
