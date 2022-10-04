using TMPro;
using UnityEngine;

public class PanelModeDebugGUI : DebugGUIBase
{
	[SerializeField] TextMeshProUGUI panelModeTMP;

	public string PanelModeText { get => panelModeTMP.text; set => panelModeTMP.text = value; }

	void Update()
	{
		panelModeTMP.text = SetPanelModeText();
	}

	string SetPanelModeText()
	{
		string text = mapData.CurrentMode switch
		{
			PanelMode.None => "Front Page",
			PanelMode.Creative => "Creative",
			PanelMode.Programming => "Programming",
			PanelMode.MoreInfo => "More Info",
			_ => "ERROR"
		};
		return text;
	}
}
