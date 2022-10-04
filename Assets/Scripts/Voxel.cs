
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VoxelType
{
	Empty,
	Type1,
	Type2,
	Type3,
	Type4
}

public class Voxel : MonoBehaviour
{
	[SerializeField] int number;
	[SerializeField] VoxelType type = VoxelType.Empty;

	[SerializeField] public Color defaultColor = Color.white;
	[SerializeField] public Color color;
	[SerializeField] public Vector2 coordinates;
	[SerializeField] public Material material;

	[SerializeField] public int lifeTime;
	
	[Space, Header("Surrounding Voxels:")]
	[SerializeField] SurroundingVoxels surroundingVoxels;
	[SerializeField] public List<Voxel> affectedByVoxels = Enumerable.Empty<Voxel>().ToList();
	
	public int X => (int)coordinates.x;
	public int Y => (int)coordinates.y;
	public int Number => number;
	public bool IsEmpty => type == VoxelType.Empty;
	public bool IsType1 => type == VoxelType.Type1;
	public bool IsType2 => type == VoxelType.Type2;
	public bool IsType3 => type == VoxelType.Type3;
	public bool IsType4 => type == VoxelType.Type4;

	public bool HasNorth => surroundingVoxels.north != null;
	public bool HasWest => surroundingVoxels.west != null;
	public bool HasEast => surroundingVoxels.east != null;
	public bool HasSouth => surroundingVoxels.south != null;
	public bool HasNorthWest => surroundingVoxels.northWest != null;
	public bool HasNorthEast => surroundingVoxels.northEast != null;
	public bool HasSouthWest => surroundingVoxels.southWest != null;
	public bool HasSouthEast => surroundingVoxels.southEast != null;
	public Voxel North => surroundingVoxels.north;
	public Voxel West => surroundingVoxels.west;
	public Voxel East => surroundingVoxels.east;
	public Voxel South => surroundingVoxels.south;
	public Voxel NorthWest => surroundingVoxels.northWest;
	public Voxel NorthEast => surroundingVoxels.northEast;
	public Voxel SouthWest => surroundingVoxels.southWest;
	public Voxel SouthEast => surroundingVoxels.southEast;
	
	public Color Color { get => material.color; set => material.color = value; }
	public SurroundingVoxels SurroundingVoxels => surroundingVoxels;
	public VoxelType Type { get => type; set => type = value; }
	
	public void InitVoxel(Vector2 vectorCoordinates, int voxelNumber)
	{
		gameObject.name = $"Voxel #{voxelNumber} at {(int)vectorCoordinates.x},{(int)vectorCoordinates.y}";
		coordinates = vectorCoordinates;
		number = voxelNumber;
		material = GetComponent<MeshRenderer>().material;
		SetDefaultColor();
	}

	public void SetDefaultColor()
	{
		Color = defaultColor;
	}

	public void SetColor(Color voxelColor)
	{
		Color = voxelColor;
	}

	public void SetSurroundingVoxels(SurroundingVoxels surrounding) => surroundingVoxels = surrounding;
}

[Serializable]
public struct SurroundingVoxels
{
	public Voxel northWest;
	public Voxel north;
	public Voxel northEast;
	public Voxel west;
	public Voxel east;
	public Voxel southWest;
	public Voxel south;
	public Voxel southEast;

	public List<Voxel> straights;
    public List<Voxel> diagonals;
    public List<Voxel> all;

    public SurroundingVoxels(int resolution, Voxel root, Dictionary<Vector2, Voxel> voxelDict)
    {
	    straights = Enumerable.Empty<Voxel>().ToList();
    	diagonals = Enumerable.Empty<Voxel>().ToList();
    	all = Enumerable.Empty<Voxel>().ToList();
    
	    var invalidVector = new Vector2(resolution + 2, resolution + 2);

	    var uV2 = root.Y + 1 >= resolution ? invalidVector : new Vector2(root.X, root.Y + 1);
	    var uLv2 = root.Y + 1 >= resolution ? invalidVector : root.X - 1 < 0 ? invalidVector : new Vector2(root.X - 1, root.Y + 1);
	    var uRv2 = root.Y + 1 >= resolution ? invalidVector : root.X + 1 >= resolution ? invalidVector : new Vector2(root.X + 1, root.Y + 1);

	    var lV2 = root.X - 1 < 0 ? invalidVector : new Vector2(root.X - 1, root.Y);
	    var rV2 = root.X + 1 >= resolution ? invalidVector : new Vector2(root.X + 1, root.Y);

	    var dv2 = root.Y - 1 < 0 ? invalidVector : new Vector2(root.X, root.Y - 1);
	    var dLv2 = root.Y - 1 < 0 ? invalidVector : root.X - 1 < 0 ? invalidVector : new Vector2(root.X - 1, root.Y - 1);
	    var dRv2 = root.Y - 1 < 0 ? invalidVector : root.X + 1 >= resolution ? invalidVector : new Vector2(root.X + 1, root.Y - 1);

	    north = uV2 != invalidVector ? voxelDict[uV2] : null;
	    northWest = uLv2 != invalidVector ? voxelDict[uLv2] : null;
	    northEast = uRv2 != invalidVector ? voxelDict[uRv2] : null;
	    west = lV2 != invalidVector ? voxelDict[lV2] : null;
	    east = rV2 != invalidVector ? voxelDict[rV2] : null;
	    south = dv2 != invalidVector ? voxelDict[dv2] : null;
	    southWest = dLv2 != invalidVector ? voxelDict[dLv2] : null;
	    southEast = dRv2 != invalidVector ? voxelDict[dRv2] : null;

	    Voxel[] northArray = { northWest, north, northEast };
	    Voxel[] southArray = { southWest, south, southEast };
	    Voxel[] westArray = { northWest, west, southWest };
	    Voxel[] eastArray = { northEast, east, southWest };
	    Voxel[] sideArray = { north, east, south, west };
	    Voxel[] cornerArray = { northWest, northEast, southWest, southEast };

	    AddToList(straights, sideArray);
	    AddToList(diagonals, cornerArray);
    }

    void AddToList(ICollection<Voxel> toList, IEnumerable<Voxel> fromArray)
    {
	    foreach (var voxel in fromArray)
	    {
		    if (voxel == null) continue;
		    toList.Add(voxel);
		    all.Add(voxel);
	    }
    }
}
