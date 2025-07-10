using UnityEngine;

/// <summary>
/// Use only for top-strech RectTransform: shifts the whole panel down under the notch (brovka)
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaTopShifter : MonoBehaviour
{
	[SerializeField] private RectTransform _scrollView;

	void Start()
	{
		ApplyTopShift();
	}

	void ApplyTopShift()
	{
		Rect safe = Screen.safeArea;
		RectTransform rt = GetComponent<RectTransform>();
		Canvas canvas = GetComponentInParent<Canvas>();
		float scale = canvas ? canvas.scaleFactor : 1f;

		float insetTop = (Screen.height - (safe.y + safe.height)) / scale;

		Vector2 min = rt.offsetMin;
		Vector2 max = rt.offsetMax;
		min.y -= insetTop;
		max.y -= insetTop;
		rt.offsetMin = min;
		rt.offsetMax = max;

		float topPanelHeight = rt.rect.height;
		var offset = _scrollView.offsetMin;
		offset.y = 0; // bottom
		_scrollView.offsetMin = offset;

		offset = _scrollView.offsetMax;
		offset.y = -topPanelHeight; // top
		_scrollView.offsetMax = offset;
	}
}