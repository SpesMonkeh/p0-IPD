using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelMap : MonoBehaviour
{
	[SerializeField] int voxelResolution = 8;
	[SerializeField] int chunkResolution = 2;
	[SerializeField] float size = 2f;
	[SerializeField] Camera mainCam;
	[SerializeField] VoxelGrid voxelGridPrefab;
	[SerializeField] DisplayManager displayManager;
	[SerializeField] CoordinatesDebugGUI coordinatesGUI;

	float chunkSize;
	float halfSize;
	float voxelSize;
	VoxelGrid[] chunks;
	Vector3 cachedCoordinateTextPosition;

	public bool IsUsingFill { get; set; }
	Vector3 MousePosition => Mouse.current.position.ReadValue();
	
	void Awake()
	{
		halfSize = size * .5f;
		chunkSize = size / chunkResolution;
		voxelSize = chunkSize / voxelResolution;
		chunks = new VoxelGrid[chunkResolution * chunkResolution];
		
		for (int i = 0, y = 0; y < chunkResolution; y++)
		{
			for (int x = 0; x < chunkResolution; x++, i++)
				CreateChunk(i, x, y);
		}
		
		var box = gameObject.AddComponent<BoxCollider>();
		box.size = new Vector3(size, size);
		cachedCoordinateTextPosition = coordinatesGUI.transform.position;
	}

	void FixedUpdate()
	{
		if (!Physics.Raycast(mainCam.ScreenPointToRay(MousePosition), out var hitInfo))
		{
			HandleVoxelCoordinatesText(false, Vector2.zero, Vector2.zero);
			return;
		}

		if (displayManager.currentPanelMode != PanelMode.Creative) return;

		if (hitInfo.collider.gameObject == gameObject)
			EditVoxels(transform.InverseTransformPoint(hitInfo.point));
	}

	void CreateChunk(int i, int x, int y)
	{
		VoxelGrid chunk = Instantiate(voxelGridPrefab, transform, true);
		chunk.InitializeVoxelGrid(voxelResolution, chunkSize);
		chunk.displayManager = displayManager;
		chunk.transform.localPosition = new Vector3(x * chunkSize - halfSize, y * chunkSize - halfSize);
		chunks[i] = chunk;
	}

	void EditVoxels(Vector3 point)
	{
		var bottomLeftX = point.x + halfSize;
		var bottomLeftY = point.y + halfSize;
		
		int centerX = (int)(bottomLeftX / voxelSize);
		int centerY = (int)(bottomLeftY / voxelSize);
		Vector2 centerVector = new Vector2(centerX, centerY);
		
		int chunkX = centerX / voxelResolution;
		int chunkY = centerY / voxelResolution;
		Vector2 chunkVector = new Vector2(chunkX, chunkY);

		HandleVoxelCoordinatesText(true,  centerVector, chunkVector);
		
		int chunk = chunkY * chunkResolution + chunkX;
		VoxelStencil activeStencil = new();
		activeStencil.Init(IsUsingFill);
		activeStencil.SetCenter(centerVector);
		
		centerX -= chunkX * voxelResolution;
		centerY -= chunkY * voxelResolution;
		centerVector = new Vector2(centerX, centerY);
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