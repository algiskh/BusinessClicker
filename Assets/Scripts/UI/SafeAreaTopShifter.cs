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
		var scale = canvas ? canvas.scaleFactor : 1f;

		var insetTop = (Screen.height - (safe.y + safe.height)) / scale;

		Vector2 min = rt.offsetMin;
		Vector2 max = rt.offsetMax;
		min.y -= insetTop;
		max.y -= insetTop;
		rt.offsetMin = min;
		rt.offsetMax = max;


		var offset = _scrollView.offsetMin;
		offset.y = 0;
		_scrollView.offsetMin = offset;

		offset = _scrollView.offsetMax;
		offset.y = -rt.rect.height - insetTop;
		_scrollView.offsetMax = offset;
	}
}