using UnityEngine;

public class VoxelStencil
{
	int xFill;
	int yFill;
	bool fillType;

	public void Init(bool fill)
	{
		fillType = fill;
	}

	public void SetCenter(Vector2 center)
	{
		xFill = (int)center.x;
		yFill = (int)center.y;
	}
	
	public bool Apply(int x, int y)
	{
		return fillType;
	}
}
