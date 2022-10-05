using UnityEngine;
using UnityEngine.UI;

public class CreativePanel : PanelBase
{
	[SerializeField] CellColorData colorData;

	public CellColorData ColorData => colorData;
}