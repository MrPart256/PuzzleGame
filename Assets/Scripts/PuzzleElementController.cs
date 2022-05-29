using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleElementController : MonoBehaviour, IDragHandler
{
    [HideInInspector] public Vector2 InitialPosition;
    [HideInInspector] public Vector2 CurrentPosition;

    private Canvas canvas;
    private RectTransform rectTransform;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = rectTransform.root.GetComponent<Canvas>();
    }

    public void SetCurrentPosition(Vector2 position)
    {
        CurrentPosition = position;
        rectTransform.anchoredPosition = CurrentPosition;

        if (!IsDraggable())
        {
            PuzzleGameController.Instance.AddCompletedPuzzleElementToList(gameObject);
        }
    }

    public bool IsDraggable()
    {
        if (Vector2.Distance(rectTransform.anchoredPosition, InitialPosition) < 40 && rectTransform.parent.CompareTag("TargetRect"))
        {
            return false;
        }
        return true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggable())
        {
            rectTransform.SetAsLastSibling();
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            PlaceOnTarget(eventData);
        }
        else
        {
            SetCurrentPosition(InitialPosition);
            PuzzleGameController.Instance.AddCompletedPuzzleElementToList(gameObject);
        }
    }

    private void PlaceOnTarget(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 1)
        {

            var item = raycastResults[1].gameObject.GetComponent<RectTransform>();

            if (item.CompareTag("TargetRect"))
            {
                rectTransform.SetParent(item);
            }
        }
    }
}
