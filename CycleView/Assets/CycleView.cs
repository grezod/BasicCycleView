using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CycleView : MonoBehaviour, IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerDownHandler
{
    
    // Start is called before the first frame update
    Vector2 draggingPoint;
    Vector2 lastDraggingPoint;
    public Camera mainCamera;
    public float speed =0.1f;
    public float edgeDistance = 200;
    public float unitInerval = 50;
    float topEdge;
    float bottomEdge;
    float unitHieght;

    public List<GameObject> scrollUnits;
    public LinkedListNode<GameObject> scrollUnitNodes;
    bool isDirectionUp;

    void Start()
    {
        setTopEdge();
        setBottomEdge();
        setUintHieght();
    }
    private void setUintHieght()
    {
        unitHieght = scrollUnits[0].GetComponent<RectTransform>().rect.height;
    }
    private void setTopEdge()
    {
        topEdge = ((transform as RectTransform).rect.width/2)+edgeDistance;
    }

    private void setBottomEdge()
    {
        bottomEdge = (((transform as RectTransform).rect.width / 2) + edgeDistance) * -1;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        lastDraggingPoint = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        lastDraggingPoint = draggingPoint;
        draggingPoint = eventData.position;
        setIsDirectionUp();
        var diff = getDiff();
        setScrollUnitsRefreshOrder();

        RectTransform overTopUnit = null;
        RectTransform overBottomUnit = null;
        foreach (var scrollUnit in scrollUnits)
        {
            var rect = scrollUnit.transform as RectTransform;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + diff);

            if (overTopUnit != null || overBottomUnit != null) continue;

            if (uintOverTop(rect))
            {
                overTopUnit = rect;
            }
            else if (uniyOverBottom(rect))
            {
                overBottomUnit = rect;
            }
        }

        setOverTop(overTopUnit);
        setOverBottom(overBottomUnit);
    }
    private bool uniyOverBottom(RectTransform rect)
    {
        return rect.anchoredPosition.y < bottomEdge && !isDirectionUp;
    }
    private bool uintOverTop(RectTransform rect)
    {
        return rect.anchoredPosition.y > topEdge && isDirectionUp;
    }
    private void setScrollUnitsRefreshOrder()
    {
        if (isDirectionUp)
        {
            scrollUnits.Sort((u1, u2) => u2.transform.position.y.CompareTo(u1.transform.position.y));

        }
        else
        {
            scrollUnits.Sort((u1, u2) => u1.transform.position.y.CompareTo(u2.transform.position.y));

        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
    }
    private void setOverTop(RectTransform overTopUnit)
    {
        if (overTopUnit == null) return;

        var newYPos = scrollUnits[scrollUnits.Count - 1].GetComponent<RectTransform>().anchoredPosition.y - unitHieght - unitInerval;
        overTopUnit.anchoredPosition = new Vector2(overTopUnit.anchoredPosition.x, newYPos);
    }
    private void setOverBottom(RectTransform overBottomUnit)
    {
        if (overBottomUnit == null ) return;

        var newYPos = scrollUnits[scrollUnits.Count-1].GetComponent<RectTransform>().anchoredPosition.y + unitHieght + unitInerval;
        overBottomUnit.anchoredPosition = new Vector2(overBottomUnit.anchoredPosition.x, newYPos);
    }
    private float getDiff()
    {
        float diff = Mathf.Abs( lastDraggingPoint.y - draggingPoint.y);
        if (isDirectionUp)
        {
            return diff;
        }
        return (diff * -1f);
    }
    private void setIsDirectionUp()
    {
        isDirectionUp = lastDraggingPoint.y - draggingPoint.y < 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastDraggingPoint = eventData.position;
        draggingPoint = eventData.position;
    }
}
