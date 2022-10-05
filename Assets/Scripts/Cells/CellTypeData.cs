
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "P0/Cell Type Data")]
public class CellTypeData : ScriptableObject
{
	[SerializeField] int emptyMaxAge = -1;
	[SerializeField] int t1MaxAge = 4;
	[SerializeField] int t2MaxAge = 6;
	[SerializeField] int t3MaxAge = 8;
	[SerializeField] int t4MaxAge = 10;
	
	[SerializeField] int defaultEmptyMaxAge = -1;
	[SerializeField] int defaultT1MaxAge = 4;
	[SerializeField] int defaultT2MaxAge = 6;
	[SerializeField] int defaultT3MaxAge = 8;
	[SerializeField] int defaultT4MaxAge = 10;
	
	[SerializeField] int emptyMaxAffect = -1;
	[SerializeField] int t1MaxAffects = 4;
	[SerializeField] int t2MaxAffects = 6;
	[SerializeField] int t3MaxAffects = 8;
	[SerializeField] int t4MaxAffects = 10;
	
	[SerializeField] int defaultEmptyMaxAffect = 1;
	[SerializeField] int defaultT1MaxAffect = 4;
	[SerializeField] int defaultT2MaxAffect = 6;
	[SerializeField] int defaultT3MaxAffect = 8;
	[SerializeField] int defaultT4MaxAffect = 10;
	
	
	
	public void GetCellMaxAge(Cell cell)
	{
		cell.MaxAge = cell.Type switch
		{
			CellType.Empty => emptyMaxAge,
			CellType.Type1 => t1MaxAge,
			CellType.Type2 => t2MaxAge,
			CellType.Type3 => t3MaxAge,
			CellType.Type4 => t4MaxAge,
			_ => cell.MaxAge
		};
	}

	public void GetDefaultMaxAge(Cell cell)
	{
		cell.MaxAge = cell.Type switch
		{
			CellType.Empty => defaultEmptyMaxAge,
			CellType.Type1 => defaultT1MaxAge,
			CellType.Type2 => defaultT2MaxAge,
			CellType.Type3 => defaultT3MaxAge,
			CellType.Type4 => defaultT4MaxAge,
			_ => cell.MaxAge
		};
	}
}