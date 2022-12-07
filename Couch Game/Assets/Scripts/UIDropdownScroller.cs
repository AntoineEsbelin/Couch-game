using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDropdownScroller : MonoBehaviour, ISelectHandler
{
    private ScrollRect _scrollRect;
    private float scrollPosition = 1;
    
    void Start()
    {
        _scrollRect = GetComponentInParent<ScrollRect>(true);

        int childCount = _scrollRect.content.transform.childCount - 1;
        int childIndex = transform.GetSiblingIndex();

        childIndex = childIndex < ((float) childCount / 2f) ? childIndex - 1 : childIndex;

        scrollPosition = 1 - ((float) childIndex / childCount);

    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_scrollRect)
            _scrollRect.verticalScrollbar.value = scrollPosition;
    }
}
