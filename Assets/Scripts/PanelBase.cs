using UnityEngine;

public class PanelBase : MonoBehaviour
{
	[SerializeField] protected DisplayManager displayManager;
	[SerializeField] protected VoxelMapData mapData;

	protected PanelMode CurrentPanelMode => mapData.CurrentMode;
}