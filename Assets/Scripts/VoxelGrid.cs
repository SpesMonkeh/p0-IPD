using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    public DisplayManager displayManager;
    
    [SerializeField] GameObject voxelPrefab;

    int voxelResolution;
    float voxelSize;
    Voxel[] voxels;
    Material[] voxelMaterials;
    
    [Space, Header("Timer: ")]
    public float timer;
    public float update = 5;

    Vector3 VoxelScale => Vector3.one * voxelSize * .9f;

    public void InitializeVoxelGrid(int resolution, float size)
    {
        voxelResolution = resolution;
        voxelSize = size / resolution;
        voxels = new Voxel[resolution * resolution];
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
        
        voxels[i] = voxelGO.GetComponent<Voxel>();
        voxels[i].coordinate = new Vector2(x, y);
        voxels[i].InitVoxel();
    }

    public void Apply(Vector2 voxelVector, VoxelStencil stencil)
    {
        int voxel = (int)(voxelVector.y * voxelResolution + voxelVector.x);
        voxels[voxel].UserEdited = stencil.Apply((int)voxelVector.x, (int)voxelVector.y);
        SetVoxelColors();
    }

    void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            var voxel = voxels[i];
            voxels[i].SetColor(voxel.UserEdited ? Color.black : Color.white);
        }
    }

    void Update()
    {
        if (displayManager.currentPanelMode != PanelMode.None) return;

        timer += 1f * Time.deltaTime;
        if (timer < update) return;

        for (int i = 0; i < voxels.Length; i++)
        {
            bool isFirst = i == 0;
            if (!isFirst && voxels[i - 1].UserEdited)
            {
                voxels[i].color = Color.blue;
            }
            else if (i - 2 >= 0)
            {
                if (voxels[i].color == Color.blue && voxels[i - 2].color == Color.white)
                    voxels[i].color = Color.red;
            }
            else
                voxels[i].color = voxels[i].UserEdited ? Color.black : Color.white;
        }
        timer = 0;
    }
}
