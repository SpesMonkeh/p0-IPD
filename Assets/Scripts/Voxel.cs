
using UnityEngine;

public class Voxel : MonoBehaviour
{
	[SerializeField] public bool isApplied;
	[SerializeField] bool editedByUser;
	[SerializeField] public Color defaultColor = Color.white;
	[SerializeField] public Color color;
	[SerializeField] public Vector2 coordinate;
	[SerializeField] public Material material;
	
	public int X => (int)coordinate.x;
	public int Y => (int)coordinate.y;
	public bool UserEdited { get => editedByUser; set => editedByUser = value; }
	public bool Modified => color != defaultColor;
	public Color Color { get => material.color; set => material.color = value; }
	
	public void InitVoxel()
	{
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
}
