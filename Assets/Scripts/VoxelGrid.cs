using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    public DisplayManager displayManager;
    
    [SerializeField] GameObject voxelPrefab;
    
    [Space, Header("Map Data:")]
    [SerializeField] VoxelMapData mapData;

    float voxelSize;
    Voxel[] voxels;
    Dictionary<Vector2, Voxel> voxDict = new();
    Dictionary<int, Vector2> v2Dict = new();
    
    Vector3 VoxelScale => Vector3.one * voxelSize * .9f;

    public void InitializeVoxelGrid(VoxelMapData data)
    {
        mapData = data;
        voxelSize = mapData.Size / mapData.VoxelResolution;
        voxels = new Voxel[mapData.VoxelResolution * mapData.VoxelResolution];

        for (int i = 0, y = 0; y < mapData.VoxelResolution; y++)
        {
            for (int x = 0; x < mapData.VoxelResolution; x++, i++)
                CreateVoxel(i, x, y);
        }
        foreach (var vox in voxDict.Select(pair => pair.Value))
        {
            vox.SetSurroundingVoxels(new SurroundingVoxels(mapData.VoxelResolution, vox, voxDict));
            SetVoxelColors(vox);
        }
        
        mapData.refreshDisplay += UpdateGrid;
    }

    void CreateVoxel(int i, int x, int y)
    {
        var voxelGO = Instantiate(voxelPrefab, transform, true);
        voxelGO.transform.localPosition = new Vector3((x + .5f) * voxelSize, (y + .5f) * voxelSize);
        voxelGO.transform.localScale = VoxelScale;
        var voxelCoordinates = new Vector2(x, y);
        
        voxels[i] = voxelGO.GetComponent<Voxel>();
        voxels[i].InitVoxel(voxelCoordinates, i);
        voxDict.Add(voxelCoordinates, voxels[i]);
        v2Dict.Add(i, voxelCoordinates);
    }

    public void Apply(Vector2 voxelVector, VoxelStencil stencil)
    {
        var doApply = stencil.Apply((int)voxelVector.x, (int)voxelVector.y);
        if (doApply && voxDict[voxelVector].Type == VoxelType.Empty)
            voxDict[voxelVector].Type = VoxelType.Type1;
        if(doApply && voxDict[voxelVector].Type == VoxelType.Type4)
            voxDict[voxelVector].Type = VoxelType.Type2;
        
        SetVoxelColors(voxDict[voxelVector]);
    }
    
    void SetVoxelColors(Voxel voxel)
    {
        HandleRootVoxel(voxel);
        HandleBranchVoxel(voxel);
        HandleTipVoxel(voxel);
    }

    void MakeVoxelBranch(Voxel voxel)
    {
        voxel.Type = VoxelType.Type2;
        voxel.Color = Color.yellow;
    }

    void GrowVoxelBranch(Voxel voxel, Voxel affectedBy)
    {
        if (voxel.affectedByVoxels.Contains(affectedBy)) return;
        MakeVoxelBranch(voxel);
        voxel.affectedByVoxels.Add(affectedBy);
    }
    
    void MakeVoxelTip(Voxel voxel)
    {
        voxel.Type = VoxelType.Type4;
        voxel.Color = Color.blue;
    }

    void GrowVoxelTip(Voxel voxel, Voxel affectedBy)
    {
        if (!affectedBy) return;
        if (voxel.affectedByVoxels.Contains(affectedBy)) return;
        MakeVoxelTip(voxel);
        voxel.affectedByVoxels.Add(affectedBy);
    }

    void UpdateGrid()
    {
        foreach (var voxel in voxDict.Values)
        {
            SetVoxelColors(voxel);
            HandleLifeTime(voxel, 1);
        }
    }
    
    void HandleLifeTime(Voxel voxel, int ageAmount)
    {
        voxel.lifeTime = voxel.IsEmpty ? 0 : voxel.lifeTime + ageAmount;
    }

    void HandleRootVoxel(Voxel vox)
    {
        if (!vox.IsType1) return;

        vox.SetColor(Color.black);

        foreach (var sV in vox.SurroundingVoxels.straights.Where(sV => !sV.affectedByVoxels.Contains(vox)))
        {
            
            if (sV.IsEmpty)
            {
                GrowVoxelTip(sV, vox);
            }
            else if (sV.IsType4)
            {
                GrowVoxelBranch(sV, vox);
            }
            else if (sV.IsType2)
            {
                sV.Type = VoxelType.Type3;
                sV.Color = Color.red;
            }
            else if (sV.IsType1)
            {
                sV.Color += new Color(.5f, 0f, 0f);
            }

            sV.affectedByVoxels.Add(vox);
        }
    }

    void HandleTipVoxel(Voxel vox)
    {
        if (!vox.IsType4) return;
        vox.SetColor(Color.green);

        if (vox.lifeTime == 3)
            vox.Type = VoxelType.Type2;
    }
    
    void HandleBranchVoxel(Voxel vox)
    {
        if (!vox.IsType2) return;
        var brown = new Color(.5f, .2f, .2f);
        
        vox.SetColor(brown);

        var west = vox.HasWest && vox.West.IsEmpty;
        var east = vox.HasEast && vox.East.IsEmpty;
        var south = vox.HasSouth && vox.South.IsEmpty;
        var north = vox.HasNorth && vox.North.IsEmpty;
        
        if (!west && !east || south || !north) return;

        GrowVoxelTip(vox.North, vox);
    }

    void OnDisable()
    {
        mapData.refreshDisplay -= UpdateGrid;
    }
}