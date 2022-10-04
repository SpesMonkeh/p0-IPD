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
        }
        SetVoxelColors();
        
        
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
        if (doApply && voxDict[voxelVector].Type == VoxelType.Open)
            voxDict[voxelVector].Type = VoxelType.Root;
        if(doApply && voxDict[voxelVector].Type == VoxelType.Tip)
            voxDict[voxelVector].Type = VoxelType.Branch;
        SetVoxelColors();
    }

    void SetVoxelColors()
    {
        foreach (var voxel in voxDict.Select(pair => pair.Value))
        {
            if(voxel.IsRoot)
                voxel.SetColor(Color.black);
            if (voxel.IsTip)
                MakeVoxelBranch(voxel);
        }
        
        foreach (var num in v2Dict.Select(number => number.Key))
        {
            var v2 = v2Dict[num];
            var voxel = voxDict[v2];
            if(voxel.IsRoot)
            {
                foreach (var sV in voxel.SurroundingVoxels.straights)
                {
                    if (sV.affectedByVoxels.Contains(voxel)) continue;
                    if (sV.IsOpen)
                    {
                        GrowVoxelTip(sV, voxel);
                    }
                    else if (sV.IsTip)
                    {
                        GrowVoxelBranch(sV, voxel);
                    }
                    else if (sV.IsBranch)
                    {
                        sV.Type = VoxelType.Junction;
                        sV.Color = Color.red;
                    }
                    else if (sV.IsRoot)
                    {
                        sV.Color += new Color(.5f, 0f, 0f);
                    }
                    sV.affectedByVoxels.Add(voxel);
                }
            }

            if (voxel.IsBranch)
            {
                if (voxel.SurroundingVoxels.leftVoxel.IsOpen || voxel.SurroundingVoxels.rightVoxel.IsOpen)
                {
                    if (voxel.SurroundingVoxels.downVoxel.Type != VoxelType.Open)
                    {
                        if (voxel.SurroundingVoxels.northVoxel.Type == VoxelType.Open)
                        {
                            GrowVoxelTip(voxel.SurroundingVoxels.northVoxel, voxel);
                        }
                    }
                }
            }
        }
    }

    void MakeVoxelBranch(Voxel voxel)
    {
        voxel.Type = VoxelType.Branch;
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
        voxel.Type = VoxelType.Tip;
        voxel.Color = Color.blue;
    }

    void GrowVoxelTip(Voxel voxel, Voxel affectedBy)
    {
        if (voxel.affectedByVoxels.Contains(affectedBy)) return;
        MakeVoxelTip(voxel);
        voxel.affectedByVoxels.Add(affectedBy);
    }

    void UpdateGrid()
    {
        SetVoxelColors();
        /*for (int i = 0; i < voxels.Length; i++)
        {
            bool isFirst = i == 0;
            if (!isFirst && voxels[i - 1].wasModifiedByUser)
            {
                voxels[i].color = Color.blue;
            }
            else if (i - 2 >= 0)
            {
                if (voxels[i].color == Color.blue && voxels[i - 2].color == Color.white)
                    voxels[i].color = Color.red;
            }
            else
                voxels[i].color = voxels[i].wasModifiedByUser ? Color.black : Color.white;
        }*/
    }

    void OnDisable()
    {
        mapData.refreshDisplay -= UpdateGrid;
    }
}