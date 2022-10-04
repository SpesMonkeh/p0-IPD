using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelMap : MonoBehaviour
{
	[SerializeField] Camera mainCam;
	[SerializeField] VoxelGrid voxelGridPrefab;
	[SerializeField] DisplayManager displayManager;
	[SerializeField] CoordinatesDebugGUI coordinatesGUI;

	[SerializeField] VoxelMapData mapData;
	
	float chunkSize;
	float halfSize;
	float voxelSize;
	VoxelGrid[] chunks;
	Vector3 cachedCoordinateTextPosition;

	public bool IsUsingFill { get; set; }
	static Vector3 MousePosition => Mouse.current.position.ReadValue();
	
	void Awake()
	{
		var voxRez = mapData.VoxelResolution;
		var chunkRez = mapData.ChunkResolution;
		var sz = mapData.Size;
		
		halfSize = sz * .5f;
		chunkSize = sz / chunkRez;
		voxelSize = chunkSize / voxRez;
		chunks = new VoxelGrid[chunkRez * chunkRez];
		
		for (int i = 0, y = 0; y < chunkRez; y++)
		{
			for (int x = 0; x < chunkRez; x++, i++)
				CreateChunk(i, x, y);
		}
		
		var box = gameObject.AddComponent<BoxCollider>();
		box.size = new Vector3(sz, sz);
		cachedCoordinateTextPosition = coordinatesGUI.transform.position;
	}

	void FixedUpdate()
	{
		if (!Physics.Raycast(mainCam.ScreenPointToRay(MousePosition), out var hitInfo))
		{
			HandleVoxelCoordinatesText(false, Vector2.zero, Vector2.zero);
			return;
		}

		if (mapData.CurrentMode != PanelMode.None) return;
		if (hitInfo.collider.gameObject != gameObject) return;
		if (!Mouse.current.leftButton.isPressed) return;
		
		EditVoxels(transform.InverseTransformPoint(hitInfo.point));
	}

	void CreateChunk(int i, int x, int y)
	{
		VoxelGrid grid = Instantiate(voxelGridPrefab, transform, true);
		grid.displayManager = displayManager;
		grid.InitializeVoxelGrid(mapData);
		grid.transform.localPosition = new Vector3(x * chunkSize - halfSize, y * chunkSize - halfSize);
		chunks[i] = grid;
	}

	void EditVoxels(Vector3 point)
	{
		var bottomLeftX = point.x + halfSize;
		var bottomLeftY = point.y + halfSize;
		
		int centerX = (int)(bottomLeftX / voxelSize);
		int centerY = (int)(bottomLeftY / voxelSize);
		Vector2 centerVector = new Vector2(centerX, centerY);
		
		int chunkX = centerX / mapData.VoxelResolution;
		int chunkY = centerY / mapData.VoxelResolution;
		Vector2 chunkVector = new Vector2(chunkX, chunkY);

		HandleVoxelCoordinatesText(true,  centerVector, chunkVector);
		
		int chunk = chunkY * mapData.ChunkResolution + chunkX;
		VoxelStencil activeStencil = new();
		activeStencil.Init(IsUsingFill);
		activeStencil.SetCenter(centerVector);
		
		chunks[chunk].Apply(centerVector, activeStencil);
	}

	void HandleVoxelCoordinatesText(bool isActive, Vector2 voxelVector, Vector2 chunkVector)
	{
		var coordinatesTf = coordinatesGUI.transform;
		var vCoordinates = "[-,-]";
		var cCoordinates = vCoordinates;
		switch (isActive)
		{
			case true:
				var textPos = new Vector2(MousePosition.x + 80, MousePosition.y - 40);
				coordinatesTf.position = textPos;
				vCoordinates = $"[{voxelVector.x},{voxelVector.y}]";
				cCoordinates = $"[{chunkVector.x},{chunkVector.y}]";
				break;
			case false when coordinatesTf.position != cachedCoordinateTextPosition:
				coordinatesTf.position = cachedCoordinateTextPosition;
				break;
		}
		coordinatesGUI.VoxelCoordinates = vCoordinates;
		coordinatesGUI.ChunkCoordinates = cCoordinates;
	}
}