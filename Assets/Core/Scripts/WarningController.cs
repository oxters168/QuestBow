using UnityEngine;
using UnityHelpers;

[RequireComponent(typeof(RectTransform))]
public class WarningController : MonoBehaviour
{
    private RectTransform _rectTransform;
    public RectTransform rectTransform { get { if (!_rectTransform) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
    private RectTransform _parentRect;
    public RectTransform parentRect { get { if (!_parentRect) _parentRect = transform.parent.GetComponent<RectTransform>(); return _parentRect; } }

    public RectTransform exclamation;
    public BorderController border;
}
