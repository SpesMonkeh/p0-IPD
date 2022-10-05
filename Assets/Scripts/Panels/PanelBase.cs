using UnityEngine;

public class PanelBase : MonoBehaviour
{
	[SerializeField] protected DisplayManager displayManager;
	[SerializeField] protected CellMapData mapData;

	protected PanelMode CurrentPanelMode => mapData.CurrentMode;
}