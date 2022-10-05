using System;
using UnityEngine;



public class DisplayManager : MonoBehaviour
{
    [Space, Header("Buttons:")]
    [SerializeField] ButtonContainer frontPageButtons;
    [SerializeField] ButtonContainer returnButton;
    [Space, Header("Panels:")]
    [SerializeField] PanelContainer panels;

    [Space, Header("Map Data:")]
    [SerializeField] CellMapData mapData;

    void Awake()
    {
        mapData.ResetTimer();
    }

    void Start()
    {
        ShowFrontPageButtons(true);
        HidePanels();
    }

    void Update()
    {
        mapData.RunTimer();
    }

    public void ShowCreativePanel()
    {
        ShowFrontPageButtons(false);
        panels.CreativePanelGO.SetActive(true);
        mapData.CurrentMode = PanelMode.Creative;
    }

    public void ShowProgrammingPanel()
    {
        ShowFrontPageButtons(false);
        panels.ProgrammingPanelGO.SetActive(true);
        mapData.CurrentMode = PanelMode.Programming;
    }

    public void ShowMoreInfoPanel()
    {
        ShowFrontPageButtons(false);
        panels.MoreInfoPanelGO.SetActive(true);
        mapData.CurrentMode = PanelMode.MoreInfo;
    }

    public void HidePanels()
    {
        panels.HideAllPanels();
        ShowFrontPageButtons(true);
    }

    void ShowFrontPageButtons(bool doShow)
    {
        
        frontPageButtons.gameObject.SetActive(doShow);
        returnButton.gameObject.SetActive(!doShow);
        if (!doShow) return;
        mapData.CurrentMode = PanelMode.None;
    }
}
