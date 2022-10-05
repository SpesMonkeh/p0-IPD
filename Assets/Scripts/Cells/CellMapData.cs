using System;
using UnityEngine;

public enum PanelMode
{
	None,
	Creative,
	Programming,
	MoreInfo
}

[CreateAssetMenu(menuName = "Cell Map Data", order = 0)] 
public class CellMapData : ScriptableObject 
{
	[Header("Cell Grid:")]
	[SerializeField] int cellResolution = 8;
	
	[SerializeField] int chunkResolution = 2;
	[SerializeField] float size = 2f;

	[SerializeField] PanelMode currentPanelMode = PanelMode.None;

	[Space, Header("Display Timer:")]
	[SerializeField] bool doRunTimer = true;
	[SerializeField] float timer;
	[SerializeField] int timesRefreshed;
	[SerializeField, Range(.1f, 10f)] float refreshTimer = 1f;
	
	public Action refreshDisplay;

	public int CellResolution { get => cellResolution; set => cellResolution = value; }
	public int ChunkResolution { get => chunkResolution; set => chunkResolution = value; }
	public int TimesRefreshed => timesRefreshed;
	public float Size { get => size; set => size = value; }
	public float Timer { get => timer; set => timer = value; }
	public float RefreshTimer { get => refreshTimer; set => refreshTimer = value; }
	public PanelMode CurrentMode { get => currentPanelMode; set => currentPanelMode = value; }

	public void RunTimer()
	{
		if (!doRunTimer) return;
		if (currentPanelMode != PanelMode.None) return;
		timer += 1 * Time.deltaTime;
		if (timer < refreshTimer) return;
		refreshDisplay?.Invoke();
		timesRefreshed++;
		timer = 0;
	}

	public void ResetTimer() => timer = 0;

	public void ResetAllTimerData()
	{
		ResetTimer();
		timesRefreshed = 0;
		refreshTimer = 1;
	}
}