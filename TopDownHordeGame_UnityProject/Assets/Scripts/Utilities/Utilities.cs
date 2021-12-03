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
}
