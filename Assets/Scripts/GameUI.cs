using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _rootButton;
    [SerializeField] private Button _defenseButton;
    [SerializeField] private Button _treasureButton;
    [SerializeField] private TextMeshProUGUI _mpTMP;
    
    void Start()
    {
        _rootButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
            = "expand (R)oots - " + GameManager.RootCost + "mp";
        _defenseButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
            = "build (D)efences - " + GameManager.DefenseCost + "mp";
        _treasureButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
            = "place (T)reasure - " + GameManager.TreasureCost + "mp";

        _rootButton.onClick.AddListener( 
            delegate{ ButtonPress(BuildingType.Root); } );
        _defenseButton.onClick.AddListener( 
            delegate{ ButtonPress(BuildingType.Defense); } );
        _treasureButton.onClick.AddListener( 
            delegate{ ButtonPress(BuildingType.Treasure); } );

        MpUpdate(GameManager.Mp);
        GameManager.onMpChange += MpUpdate;
        GameManager.onRootPriceChange += RootPriceUpdate;
        GameManager.onDefensePriceChange += DefensePriceUpdate;
        GameManager.onTreasurePriceChange += TreasurePriceUpdate;
    }

    private void ButtonPress(BuildingType buildingType)
    {
        RootSystem.ClickCooldown();
        RootSystem.StartBuildingMode(buildingType);
    }

    private void MpUpdate(int newValue)
    {
        _mpTMP.text = "Magic power (mp): " + newValue;
    }
    private void RootPriceUpdate(int newValue)
    {
        _rootButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
            = "expand (R)oots - " + newValue + "mp";
    }
    private void DefensePriceUpdate(int newValue)
    {
        _defenseButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
            = "build (D)efences - " + newValue + "mp";
    }
    private void TreasurePriceUpdate(int newValue)
    {
        _treasureButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
            = "place (T)reasure - " + newValue + "mp";
    }
}
