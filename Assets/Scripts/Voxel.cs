
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VoxelType
{
	Open,
	Root,
	Branch,
	Junction,
	Tip
}

public class Voxel : MonoBehaviour
{
	[SerializeField] int number;
	[SerializeField] VoxelType type = VoxelType.Open;

	[SerializeField] public bool isApplied;
	[SerializeField] bool wasModifiedByUser;
	[SerializeField] public Color defaultColor = Color.white;
	[SerializeField] public Color color;
	[SerializeField] public Vector2 coordinates;
	[SerializeField] public Material material;
	
	[Space, Header("Surrounding Voxels:")]
	[SerializeField] SurroundingVoxels surroundingVoxels;
	[SerializeField] public List<Voxel> affectedByVoxels = Enumerable.Empty<Voxel>().ToList();
	
	public int X => (int)coordinates.x;
	public int Y => (int)coordinates.y;
	public int Number => number;
	public bool IsOpen => type == VoxelType.Open;
	public bool IsRoot => type == VoxelType.Root;
	public bool IsBranch => type == VoxelType.Branch;
	public bool IsJunction => type == VoxelType.Junction;
	public bool IsTip => type == VoxelType.Tip;

	public Voxel UpLeftVoxel { get; set; }
	public Voxel UpVoxel { get; set; }
	public Voxel UpRightVoxel {get; set;}
	public Voxel LeftVoxel {get; set;}
	public Voxel RightVoxel {get; set;}
	public Voxel DownLeftVoxel {get; set;}
	public Voxel DownVoxel { get; set; }
	public Voxel DownRightVoxel { get; set; }
	
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

	public bool WasModifiedWithoutUser()
	{
		if (IsRoot) return false;
		return color != defaultColor;
	}

	public void SetSurroundingVoxels(SurroundingVoxels surrounding) => surroundingVoxels = surrounding;
}

[Serializable]
public struct SurroundingVoxels
{
	public Voxel northWestVoxel;
	public Voxel northVoxel;
	public Voxel northEastVoxel;
	public Voxel leftVoxel;
	public Voxel rightVoxel;
	public Voxel downLeftVoxel;
	public Voxel downVoxel;
	public Voxel downRightVoxel;

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

	    northVoxel = uV2 != invalidVector ? voxelDict[uV2] : null;
	    northWestVoxel = uLv2 != invalidVector ? voxelDict[uLv2] : null;
	    northEastVoxel = uRv2 != invalidVector ? voxelDict[uRv2] : null;
	    leftVoxel = lV2 != invalidVector ? voxelDict[lV2] : null;
	    rightVoxel = rV2 != invalidVector ? voxelDict[rV2] : null;
	    downVoxel = dv2 != invalidVector ? voxelDict[dv2] : null;
	    downLeftVoxel = dLv2 != invalidVector ? voxelDict[dLv2] : null;
	    downRightVoxel = dRv2 != invalidVector ? voxelDict[dRv2] : null;

	    AddToStraightsList(northVoxel);
	    AddToStraightsList(downVoxel);
	    AddToStraightsList(leftVoxel);
	    AddToStraightsList(rightVoxel);
	    AddToDiagonalsList(northWestVoxel);
	    AddToDiagonalsList(northEastVoxel);
	    AddToDiagonalsList(downLeftVoxel);
	    AddToDiagonalsList(downRightVoxel);
    }
    
    void AddToStraightsList(Voxel v)
    {
	    if (v == null) return;
	    straights.Add(v);
	    all.Add(v);
    }

    void AddToDiagonalsList(Voxel v)
    {
	    if (v == null) return;
	    diagonals.Add(v);
	    all.Add(v);
    }
}
