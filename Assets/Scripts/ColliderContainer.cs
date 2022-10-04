
using UnityEngine;

public class ColliderContainer : MonoBehaviour
{
	[SerializeField] EdgeCollider2D edgeCollider;
	[SerializeField] Camera mainCam;

	[Space, Header("2D Vektorer:")]
	[SerializeField] Vector2 leftTop;
	[SerializeField] Vector2 leftBottom;
	[SerializeField] Vector2 rightTop;
	[SerializeField] Vector2 rightBottom;
	
	void Start()
	{
		leftBottom = mainCam.ScreenToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
		leftTop = mainCam.ScreenToWorldPoint(new Vector3(0, mainCam.pixelHeight, mainCam.nearClipPlane));
		rightTop = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth, mainCam.pixelHeight, mainCam.nearClipPlane));
		rightBottom = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth, 0, mainCam.nearClipPlane));
		Vector2[] edgePoints = { leftBottom, leftTop, rightTop, rightBottom, leftBottom };
		edgeCollider.points = edgePoints;
	}
}