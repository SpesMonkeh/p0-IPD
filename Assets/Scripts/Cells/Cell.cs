
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CellType
{
	Empty,
	Type1,
	Type2,
	Type3,
	Type4
}

public class Cell : MonoBehaviour
{
	[SerializeField] int number;
	[SerializeField] CellType type = CellType.Empty;

	[SerializeField] public Color defaultColor = Color.white;
	[SerializeField] public Color color;
	[SerializeField] public Vector2 coordinates;
	[SerializeField] public Material material;

	[SerializeField] public int lifeTime;
	
	[Space, Header("Bordering Cells:")]
	[SerializeField] BorderCells borderCells;
	[SerializeField] public List<Cell> affectedByCells = Enumerable.Empty<Cell>().ToList();
	
	public int MaxAge { get; set; }
	public int X => (int)coordinates.x;
	public int Y => (int)coordinates.y;
	public int Number => number;
	public bool IsEmpty => type == CellType.Empty;
	public bool IsType1 => type == CellType.Type1;
	public bool IsType2 => type == CellType.Type2;
	public bool IsType3 => type == CellType.Type3;
	public bool IsType4 => type == CellType.Type4;

	public bool HasNorth => borderCells.n != null;
	public bool HasWest => borderCells.w != null;
	public bool HasEast => borderCells.e != null;
	public bool HasSouth => borderCells.s != null;
	public bool HasNorthWest => borderCells.nW != null;
	public bool HasNorthEast => borderCells.nE != null;
	public bool HasSouthWest => borderCells.sW != null;
	public bool HasSouthEast => borderCells.sE != null;
	public Cell N => borderCells.n;
	public Cell W => borderCells.w;
	public Cell E => borderCells.e;
	public Cell S => borderCells.s;
	public Cell NW => borderCells.nW;
	public Cell NE => borderCells.nE;
	public Cell SW => borderCells.sW;
	public Cell SE => borderCells.sE;
	
	public Color Color { get => material.color; set => material.color = value; }
	public BorderCells BorderCells => borderCells;
	public CellType Type { get => type; set => type = value; }

	public void InitializeCell(Vector2 vectorCoordinates, int cellNumber)
	{
		gameObject.name = $"Cell #{cellNumber} at {(int)vectorCoordinates.x},{(int)vectorCoordinates.y}";
		coordinates = vectorCoordinates;
		number = cellNumber;
		material = GetComponent<MeshRenderer>().material;
		SetDefaultColor();
	}

	public void SetDefaultColor()
	{
		Color = defaultColor;
	}

	public void SetColor(Color cellColor)
	{
		Color = cellColor;
	}

	public void SetBorderCells(BorderCells bordering) => borderCells = bordering;
}

[Serializable]
public struct BorderCells
{
	public Cell n;
	public Cell s;
	public Cell e;
	public Cell w;

	public Cell nE;
	public Cell nW;
	
	public Cell sE;
	public Cell sW;

	public List<Cell> sides;
    public List<Cell> corners;
    public List<Cell> northern;
    public List<Cell> southern;
    public List<Cell> western;
    public List<Cell> eastern;
    public List<Cell> all;

    Dictionary<Vector2, Cell> cellDict;
    Vector2 invalidCell;
	
    
    public BorderCells(int resolution, Cell root, Dictionary<Vector2, Cell> dict)
    {
	    sides = Enumerable.Empty<Cell>().ToList();
    	corners = Enumerable.Empty<Cell>().ToList();
        northern = Enumerable.Empty<Cell>().ToList();
        southern = Enumerable.Empty<Cell>().ToList();
        western = Enumerable.Empty<Cell>().ToList();
        eastern = Enumerable.Empty<Cell>().ToList();
    	all = Enumerable.Empty<Cell>().ToList();

        cellDict = dict;
	    invalidCell = new Vector2(-1, -1);

	    var nV2 = root.Y + 1 >= resolution ? invalidCell : new Vector2(root.X, root.Y + 1);
	    var nWv2 = root.Y + 1 >= resolution ? invalidCell : root.X - 1 < 0 ? invalidCell : new Vector2(root.X - 1, root.Y + 1);
	    var nEv2 = root.Y + 1 >= resolution ? invalidCell : root.X + 1 >= resolution ? invalidCell : new Vector2(root.X + 1, root.Y + 1);

	    var wV2 = root.X - 1 < 0 ? invalidCell : new Vector2(root.X - 1, root.Y);
	    var eV2 = root.X + 1 >= resolution ? invalidCell : new Vector2(root.X + 1, root.Y);

	    var sV2 = root.Y - 1 < 0 ? invalidCell : new Vector2(root.X, root.Y - 1);
	    var sWv2 = root.Y - 1 < 0 ? invalidCell : root.X - 1 < 0 ? invalidCell : new Vector2(root.X - 1, root.Y - 1);
	    var sEv2 = root.Y - 1 < 0 ? invalidCell : root.X + 1 >= resolution ? invalidCell : new Vector2(root.X + 1, root.Y - 1);

	    n = ValidateBorderCell(nV2, cellDict);
	    s = ValidateBorderCell(sV2, cellDict);
	    e = ValidateBorderCell(eV2, cellDict);
	    w = ValidateBorderCell(wV2, cellDict);
	    
	    nW = ValidateBorderCell(nWv2, cellDict);
	    nE = ValidateBorderCell(nEv2, cellDict);
	    sW = ValidateBorderCell(sWv2, cellDict);
	    sE = ValidateBorderCell(sEv2, cellDict);

	    Cell[] northernArray = { nW, n, nE };
	    Cell[] southernArray = { sW, s, sE };
	    Cell[] easternArray = { nE, e, sW };
	    Cell[] westernArray = { nW, w, sW };

	    Cell[] sideArray = { n, e, s, w };
	    Cell[] cornerArray = { nW, nE, sW, sE };

	    AddToList(northern, northernArray);
	    AddToList(southern, southernArray);
	    AddToList(eastern, easternArray);
	    AddToList(western, westernArray);
	    
		AddToList(sides, sideArray);
	    AddToList(corners, cornerArray);
    }

    static Cell ValidateBorderCell(Vector2 coordinates, Dictionary<Vector2, Cell> dict)
    {
	    var invalidVector = new Vector2(-1, -1);
	    var cell = coordinates != invalidVector ? dict[coordinates] : null;
	    return cell;
    }

    void AddToList(ICollection<Cell> toList, IEnumerable<Cell> fromArray)
    {
	    foreach (var cell in fromArray)
	    {
		    if (cell == null) continue;
		    toList.Add(cell);
		    all.Add(cell);
	    }
    }
}
