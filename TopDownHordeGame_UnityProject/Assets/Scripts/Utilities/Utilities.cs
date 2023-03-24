using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public static class Utilities {

    public static string FormatTime(float time_in_sec) {
        string result = "";
        int sec = Mathf.RoundToInt(time_in_sec);
        int min = sec / 60;
        sec %= 60;
        int hours = min / 60;
        min %= 60;
        if (hours > 0)
            result += hours.ToString("D2") + ":";
        if(min > 0)
            result += min.ToString("D2") + ":";
        result += sec.ToString("D2");
        return result;
    }

    public static Texture2D GetEmptyTexture(int width, int height, Color fillColor) {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Color[] fillPixels = new Color[texture.width * texture.height];
        for (int i = 0; i < fillPixels.Length; i++) {
            fillPixels[i] = fillColor;
        }
        texture.SetPixels(fillPixels);
        return texture; 
    }

    public static Texture2D GetSteamImageAsTexture(int imageID) {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(imageID, out uint width, out uint height);
        if (isValid) {
            byte[] image = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(imageID, image, (int)(width * height * 4));
            if (isValid) {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture;
    }

    /// <summary> Checks if a given GameObject has a specific component </summary>
    public static bool HasComponent<T>(this GameObject obj) where T : Component {
        return obj.GetComponent<T>() != null;
    }

    public static bool CompareTags(GameObject obj, List<string> tags) {
        for (int i = 0; i < tags.Count; i++) {
            if (obj.CompareTag(tags[i]))
                return true;
        }
        return false;
    }

}
