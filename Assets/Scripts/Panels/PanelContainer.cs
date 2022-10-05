using UnityEngine;

public class PanelContainer : MonoBehaviour
{
	[Header("Paneler:")]
	[SerializeField] public RectTransform creativePanel;
	[SerializeField] public RectTransform programmingPanel;
	[SerializeField] public RectTransform moreInfoPanel;

	public GameObject CreativePanelGO => creativePanel.gameObject;
	public GameObject ProgrammingPanelGO => programmingPanel.gameObject;
	public GameObject MoreInfoPanelGO => moreInfoPanel.gameObject;
	
	public void HideAllPanels()
	{
		creativePanel.gameObject.SetActive(false);
		programmingPanel.gameObject.SetActive(false);
		moreInfoPanel.gameObject.SetActive(false);
	}
}