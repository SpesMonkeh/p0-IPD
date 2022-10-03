using UnityEngine;

public class PanelBase : MonoBehaviour
{
	[SerializeField] protected DisplayManager displayManager;

	protected PanelMode CurrentPanelMode => displayManager.currentPanelMode;
}