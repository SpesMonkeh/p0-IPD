using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    public DisplayManager displayManager;
    
    [SerializeField] GameObject voxelPrefab;

    int voxelResolution;
    bool[] voxels;
    float voxelSize;
    Material[] voxelMaterials;

    Vector3 VoxelScale => Vector3.one * voxelSize * .9f;

    public void InitializeVoxelGrid(int resolution, float size)
    {
        voxelResolution = resolution;
        voxelSize = size / resolution;
        voxels = new bool[resolution * resolution];
        voxelMaterials = new Material[voxels.Length];

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
                CreateVoxel(i, x, y);
        }
        SetVoxelColors();
    }

    void CreateVoxel(int i, int x, int y)
    {
        var voxelGO = Instantiate(voxelPrefab, transform, true);
        voxelGO.transform.localPosition = new Vector3((x + .5f) * voxelSize, (y + .5f) * voxelSize);
        voxelGO.transform.localScale = VoxelScale;
        voxelMaterials[i] = voxelGO.GetComponent<MeshRenderer>().material;
    }

    public void Apply(Vector2 voxelVector, VoxelStencil stencil)
    {
        int voxel = (int)(voxelVector.y * voxelResolution + voxelVector.x);
        voxels[voxel] = stencil.Apply((int)voxelVector.x, (int)voxelVector.y);
        SetVoxelColors();
    }

    void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            voxelMaterials[i].color = voxels[i] ? Color.black : Color.white;
        }
    }

    void Update()
    {
        if (displayManager.currentPanelMode != PanelMode.None) return;
        for (int i = 0; i < voxels.Length; i++)
        {
            bool isFirst = i == 0;
            if(!isFirst && voxelMaterials[i - 1].color == Color.black)
                voxelMaterials[i].color = Color.blue;
            else if(voxelMaterials[i].color == Color.blue)
                voxelMaterials[i - 2].color = Color.red;
            else
                voxelMaterials[i].color = voxels[i] ? Color.black : Color.white;
        }
    }
    
}
