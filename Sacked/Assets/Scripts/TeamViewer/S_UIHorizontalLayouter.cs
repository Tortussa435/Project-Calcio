using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_UIHorizontalLayouter
{
    public static void LayoutItems(List<RectTransform> items, float maxSize, bool centerAnchor = true, float customY = -1)
    {
        float itemsSize = 0;
        


        foreach(RectTransform item in items) itemsSize += item.rect.width;
        
        float distance = (maxSize - itemsSize) / items.Count;

        for(int i=0;i<items.Count;i++)
        {
            float position = distance * i ;

            float y = customY == -1 ? items[i].anchoredPosition.y : customY;

            items[i].anchoredPosition = new Vector2( position + items[i].rect.width , y);
        }
    }
}
