using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    public DisplayManager displayManager;
    
    [SerializeField] GameObject cellPrefab;

    [Space, Header("Data:")]
    [SerializeField] CellColorData colorData;
    [SerializeField] CellMapData mapData;
    [SerializeField] CellBehaviour behaviour;
    
    float cellSize;
    Cell[] cells;
    Dictionary<Vector2, Cell> cellDict = new();
    Dictionary<int, Vector2> v2Dict = new();
    
    Vector3 VoxelScale => Vector3.one * cellSize * .9f;

    public void InitializeCellGrid(CellMapData map, CellColorData color)
    {
        mapData = map;
        colorData = color;
        
        cellSize = mapData.Size / mapData.CellResolution;
        cells = new Cell[mapData.CellResolution * mapData.CellResolution];

        for (int i = 0, y = 0; y < mapData.CellResolution; y++)
        {
            for (int x = 0; x < mapData.CellResolution; x++, i++)
                CreateCell(i, x, y);
        }
        
        behaviour.InitializeCells(cellDict);
        
        mapData.refreshDisplay += UpdateGrid;
    }

    void CreateCell(int i, int x, int y)
    {
        var cellGO = Instantiate(cellPrefab, transform, true);
        cellGO.transform.localPosition = new Vector3((x + .5f) * cellSize, (y + .5f) * cellSize);
        cellGO.transform.localScale = VoxelScale;
        var cellCoordinates = new Vector2(x, y);
        
        cells[i] = cellGO.GetComponent<Cell>();
        cells[i].InitializeCell(cellCoordinates, i);
        cellDict.Add(cellCoordinates, cells[i]);
        v2Dict.Add(i, cellCoordinates);
    }

    public void Apply(Vector2 cellVector, VoxelStencil stencil)
    {
        var doApply = stencil.Apply((int)cellVector.x, (int)cellVector.y);
        var cell = cellDict[cellVector];
        
        switch (doApply)
        {
            case true when cell.IsEmpty:
                behaviour.ApplyNewType(cell, CellType.Type1);
                break;
            case true when cell.IsType1:
                behaviour.ApplyNewType(cell, CellType.Type2);
                break;
            case true when cell.IsType2:
                behaviour.ApplyNewType(cell, CellType.Type3);
                break;
            case true when cell.IsType3:
                behaviour.ApplyNewType(cell, CellType.Type4);
                break;
            case true when cell.IsType4:
                behaviour.ApplyNewType(cell, CellType.Empty);
                break;
        }
    }
    
    void MakeVoxelBranch(Cell cell)
    {
        cell.Type = CellType.Type2;
        cell.Color = Color.yellow;
    }

    void GrowVoxelBranch(Cell cell, Cell affectedBy)
    {
        if (cell.affectedByCells.Contains(affectedBy)) return;
        MakeVoxelBranch(cell);
        cell.affectedByCells.Add(affectedBy);
    }
    
    void MakeVoxelTip(Cell cell)
    {
        cell.Type = CellType.Type4;
        cell.Color = Color.blue;
    }

    void GrowVoxelTip(Cell cell, Cell affectedBy)
    {
        if (!affectedBy) return;
        if (cell.affectedByCells.Contains(affectedBy)) return;
        MakeVoxelTip(cell);
        cell.affectedByCells.Add(affectedBy);
    }

    void UpdateGrid()
    {
        behaviour.UpdateCells(cellDict);
    }
    
    void HandleRootVoxel(Cell c)
    {
        if (!c.IsType1) return;

        c.SetColor(Color.black);

        foreach (var sV in c.BorderCells.sides.Where(sV => !sV.affectedByCells.Contains(c)))
        {
            if (sV.IsEmpty)
            {
                GrowVoxelTip(sV, c);
            }
            else if (sV.IsType4)
            {
                GrowVoxelBranch(sV, c);
            }
            else if (sV.IsType2)
            {
                sV.Type = CellType.Type3;
                sV.Color = Color.red;
            }
            else if (sV.IsType1)
            {
                sV.Color += new Color(.5f, 0f, 0f);
            }

            sV.affectedByCells.Add(c);
        }
    }

    void HandleTipVoxel(Cell vox)
    {
        if (!vox.IsType4) return;
        vox.SetColor(Color.green);

        if (vox.lifeTime == 3)
            vox.Type = CellType.Type2;
    }
    
    void HandleBranchVoxel(Cell vox)
    {
        if (!vox.IsType2) return;
        var brown = new Color(.5f, .2f, .2f);
        
        vox.SetColor(brown);

        var west = vox.HasWest && vox.W.IsEmpty;
        var east = vox.HasEast && vox.E.IsEmpty;
        var south = vox.HasSouth && vox.S.IsEmpty;
        var north = vox.HasNorth && vox.N.IsEmpty;
        
        if (!west && !east || south || !north) return;

        GrowVoxelTip(vox.N, vox);
    }

    void OnDisable()
    {
        mapData.refreshDisplay -= UpdateGrid;
    }
}