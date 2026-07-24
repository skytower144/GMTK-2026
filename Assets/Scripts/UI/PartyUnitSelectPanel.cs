using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUnitSelectPanel : MonoBehaviour
{
    private const int INITIAL_ICON_SPAWN = 10;

    [SerializeField] private TMP_Text leftDaysText;
    [SerializeField] private GameObject unitIconPrefab;
    [SerializeField] private Transform unitIconParent;
    private List<Image> iconList = new();

    void Awake()
    {
        PartyManager.OnAddUnit -= DisplayCurrentSelectedUnits;
        PartyManager.OnAddUnit += DisplayCurrentSelectedUnits;

        for (int i = 0; i < INITIAL_ICON_SPAWN; i ++)
            SpawnIcon();
    }

    void OnDestroy()
    {
        PartyManager.OnAddUnit -= DisplayCurrentSelectedUnits;
    }

    void Start()
    {
        leftDaysText.text = GameManager.CurrentCountdown.ToString();
    }

    private void SpawnIcon()
    {
        var unitUI = Instantiate(unitIconPrefab, unitIconParent).GetComponent<Image>();
        iconList.Add(unitUI);
        iconList[^1].gameObject.SetActive(false);
    }

    public void DisplaySelectableUnits()
    {
        
    }

    private void DisplayCurrentSelectedUnits()
    {
        for (int i = 0; i < PartyManager.instance.PartyUnits.Count; i++)
        {
            if (i >= iconList.Count)
                SpawnIcon();
            
            iconList[i].sprite = PartyManager.instance.PartyUnits[i].UnitData.IconSprite;
            iconList[i].gameObject.SetActive(true);
        }

        for (int i = PartyManager.instance.PartyUnits.Count; i < iconList.Count; i++)
            iconList[i].gameObject.SetActive(false);
    }
}
