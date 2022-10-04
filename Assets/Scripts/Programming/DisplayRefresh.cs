using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayRefresh : MonoBehaviour
{
	[SerializeField] VoxelMapData mapData;

	[SerializeField] Slider refreshSlider;
	[SerializeField] TextMeshProUGUI refreshTimeText;

	void Start()
	{
		refreshSlider.value = 1f;
	}

	public void SetRefreshTime()
	{
		refreshTimeText.text = refreshSlider.value.ToString(CultureInfo.InvariantCulture);
		mapData.RefreshTimer = refreshSlider.value;
	}

}
