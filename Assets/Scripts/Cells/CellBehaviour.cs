using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
	[Header("Data:")]
	[SerializeField] CellColorData colorData;
	[SerializeField] CellMapData mapData;
	[SerializeField] CellMovementData movementData;
	[SerializeField] CellTypeData typeData;

	void UpdateCell(Cell cell)
	{
		SetMaxAge(cell);
		SetColor(cell);
		SetCellAffections(cell);
		UpdateAge(cell, 1);
	}
	
	public void SetCellAffections(Cell cell)
	{
		AffectBorderCells(cell);
	}

	public void InitializeCells(Dictionary<Vector2, Cell> cellDict)
	{
		colorData.SetColorValues();
		
		foreach (var cell in cellDict.Values)
		{
			cell.SetBorderCells(new BorderCells(mapData.CellResolution, cell, cellDict));
			UpdateCell(cell);
		}
	}

	public void UpdateCells(Dictionary<Vector2, Cell> cellDict)
	{
		foreach (var cell in cellDict.Values)
		{
			UpdateCell(cell);
		}
	}
	
	void SetMaxAge(Cell cell)
	{
		var isNewCycle = mapData.TimesRefreshed == 0;
		var isNewCell = cell.lifeTime == 0;
		
		if(isNewCycle)
			typeData.GetDefaultMaxAge(cell);
		else if(isNewCell)
			typeData.GetCellMaxAge(cell);
	}

	void UpdateAge(Cell cell, int ageChange)
	{
		if (cell.lifeTime >= cell.MaxAge)
			ResetCell(cell, CellType.Empty);
		else
			cell.lifeTime = cell.IsEmpty ? 0 : cell.lifeTime + ageChange;
	}
	
	void SetColor(Cell cell)
	{
		var isNewCycle = mapData.TimesRefreshed == 0;
		var isNewCell = cell.lifeTime == 0;
		
		if (isNewCycle)
			colorData.GetDefaultCellColor(cell);
		else if(isNewCell)
			colorData.GetCellColor(cell);
		else
			cell.Color -= new Color(.05f, .05f, .05f);
	}

	public void ApplyNewType(Cell cell, CellType type)
	{
		if (cell.Type == type) return;
		
		cell.Type = type;
		cell.lifeTime = 0;
		UpdateCell(cell);
	}

	void ResetCell(Cell cell, CellType type)
	{
		cell.Type = type;
		ResetCell(cell);
	}

	void ResetCell(Cell cell)
	{
		cell.lifeTime = 0;
		cell.affectedByCells.Clear();
		SetMaxAge(cell);
		SetColor(cell);
	}
	
	public void AffectBorderCells(Cell cell)
	{
		if (cell.Type == CellType.Empty) return;

		foreach (var bCell in cell.BorderCells.all.Where(bC => !bC.affectedByCells.Contains(cell)))
		{
			switch (cell.Type)
            {
            	case CellType.Type1:
            		if (!cell.HasNorth) break;
            		if(cell.N.IsEmpty)
            		{
            			ApplyNewType(cell.N, CellType.Type3);
            			cell.N.Color += cell.Color * 0.2f;
            		}
            		else
            		{
            			ApplyNewType(cell.N, CellType.Type4);
            			cell.N.Color += cell.Color * 0.2f;
            		}
            		break;
            	case CellType.Type2:
            		if (!cell.HasSouth) break;
            		if (cell.S.IsEmpty)
            		{
            			ApplyNewType(cell.S, CellType.Type4);
            			cell.S.Color += new Color(cell.Color.r * 0.1f, cell.Color.g, cell.Color.b);
            		}
            		else
            		{
            			ApplyNewType(cell.S, CellType.Type3);
            			cell.S.Color += new Color(cell.Color.r, cell.Color.g, cell.Color.b * 0.1f);
            		}
            		break;
            	case CellType.Type3:
            		if (!cell.HasEast) break;
            		if(cell.E.IsEmpty)
            		{
            			ApplyNewType(cell.E, CellType.Type2);
            			cell.E.Color += new Color(cell.Color.r, cell.Color.g * 0.1f, cell.Color.b);
            		}
            		else
            		{
            			ApplyNewType(cell.E, CellType.Type1);
            			cell.E.Color += new Color(cell.Color.r, cell.Color.g * 0.1f, cell.Color.b * 0.1f);
            		}
            		break;
            	case CellType.Type4:
            		if (!cell.HasWest) break;
            		if(cell.W.IsEmpty)
            		{
            			ApplyNewType(cell.W, CellType.Type1);
            			cell.W.Color -= cell.Color * 0.1f;
            		}
            		else
            		{
            			ApplyNewType(cell.W, CellType.Type2);
            			cell.W.Color += cell.Color * 0.2f;
            		}
            		break;
            }
			bCell.affectedByCells.Add(cell);
		}
	}
}