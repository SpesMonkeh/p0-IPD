using UnityEngine;

public enum PanelMode
{
    None,
    Creative,
    Programming,
    MoreInfo
}

public class DisplayManager : MonoBehaviour
{
    [Space, Header("Knapper:")]
    [SerializeField] ButtonContainer frontPageButtons;
    [SerializeField] ButtonContainer returnButton;
    [Space, Header("Paneler:")]
    [SerializeField] public PanelMode currentPanelMode = PanelMode.None;
    [SerializeField] PanelContainer panels;

    void Start()
    {
        ShowFrontPageButtons(true);
        HidePanels();
    }

    public void ShowCreativePanel()
    {
        ShowFrontPageButtons(false);
        panels.CreativePanelGO.SetActive(true);
        currentPanelMode = PanelMode.Creative;
    }

    public void ShowProgrammingPanel()
    {
        ShowFrontPageButtons(false);
        panels.ProgrammingPanelGO.SetActive(true);
        currentPanelMode = PanelMode.Programming;
    }

    public void ShowMoreInfoPanel()
    {
        ShowFrontPageButtons(false);
        panels.MoreInfoPanelGO.SetActive(true);
        currentPanelMode = PanelMode.MoreInfo;
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
        currentPanelMode = PanelMode.None;
    }
}
