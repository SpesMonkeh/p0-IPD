using TMPro;
using UnityEngine;

public class CoordinatesDebugGUI : DebugGUIBase
{
	[SerializeField] TextMeshProUGUI voxelCoordinatesText;
	[SerializeField] TextMeshProUGUI chunkCoordinatesText;

	public string VoxelCoordinates { get => voxelCoordinatesText.text; set => voxelCoordinatesText.text = value; }
	public string ChunkCoordinates { get => chunkCoordinatesText.text; set => chunkCoordinatesText.text = value; }
}