using System;
using UnityEngine;

[CreateAssetMenu(menuName = "P0/Creative/Cell Color Data")]
public class CellColorData : ScriptableObject
{
	[Space, Header("Cell Colors:")]
	[SerializeField] Color emptyColor;
	[SerializeField] Color t1Color;
	[SerializeField] Color t2Color;
	[SerializeField] Color t3Color;
	[SerializeField] Color t4Color;
	
	[Space, Header("Default Cell Colors:")]
	[SerializeField] Color defaultEmptyColor = new (.08f, 0.05f, .15f);
	[SerializeField] Color defaultT1Color = Color.cyan;
	[SerializeField] Color defaultT2Color = Color.magenta;
	[SerializeField] Color defaultT3Color = Color.blue;
	[SerializeField] Color defaultT4Color = Color.red;
	
	public Color EmptyColor { get; set; }
	public Color T1Color { get; set; }
	public Color T2Color { get; set; }
	public Color T3Color { get; set; }
	public Color T4Color { get; set; }

	public Color DefaultEmptyColor => defaultEmptyColor;
	public Color DefaultT1Color => defaultT1Color;
	public Color DefaultT2Color => defaultT2Color;
	public Color DefaultT3Color => defaultT3Color;
	public Color DefaultT4Color => defaultT4Color;

	public void GetDefaultCellColor(Cell c)
	{
		switch (c.Type)
		{
			case CellType.Empty:
				c.Color = defaultEmptyColor;
				break;
			case CellType.Type1:
				c.Color = defaultT1Color;
				break;
			case CellType.Type2:
				c.Color = defaultT2Color;

				break;
			case CellType.Type3:
				c.Color = defaultT3Color;

				break;
			case CellType.Type4:
				c.Color = defaultT4Color;
				break;
			default:
				c.Type = CellType.Empty;
				c.Color = defaultEmptyColor;
				break;
		}
	}

	public void GetCellColor(Cell cell)
	{
		switch (cell.Type)
		{
			case CellType.Empty:
				cell.Color = emptyColor;
				break;
			case CellType.Type1:
				cell.Color = t1Color;
				break;
			case CellType.Type2:
				cell.Color = t2Color;

				break;
			case CellType.Type3:
				cell.Color = t3Color;

				break;
			case CellType.Type4:
				cell.Color = t4Color;
				break;
			default:
				cell.Type = CellType.Empty;
				cell.Color = emptyColor;
				break;
		}
	}

	public void SetColorValues()
	{
		emptyColor = defaultEmptyColor;
		t1Color = defaultT1Color;
		t2Color = defaultT2Color;
		t3Color = defaultT3Color;
		t4Color = defaultT4Color;
	}
}