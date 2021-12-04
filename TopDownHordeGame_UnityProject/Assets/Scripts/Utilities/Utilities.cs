using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {
    public static bool CompareTags(GameObject obj, List<string> tags) {
        for (int i = 0; i < tags.Count; i++) {
            if (obj.CompareTag(tags[i]))
                return true;
        }
        return false;
    }

    //CREDIT: https://forum.unity.com/threads/text-at-location-in-2d-world-space.397950/
    public static Vector2 WorldToCanvas(RectTransform canv, Vector3 pos) {

        //Please note that this will only work if the element you're positioning is either a direct child of the main canvas or the child of a transform that stretches to the full extents of the canvas. Otherwise you have to take the offsets of all parent RectTransforms into account.
        if (Camera.main == null)
            return Vector2.zero;

        //This part is a bit complicated, but you have to make sure that you adjust your coordinates by the size of the canvas.
        Vector2 cPos = new Vector2(((Camera.main.WorldToViewportPoint(pos).x * canv.sizeDelta.x) - (canv.sizeDelta.x * .5f)), ((Camera.main.WorldToViewportPoint(pos).y * canv.sizeDelta.y) - (canv.sizeDelta.y * .5f)));

        return cPos;
    }
}
