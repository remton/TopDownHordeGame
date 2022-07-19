using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsMenu : Menu
{
    [System.Serializable]
    struct Map {
        [Mirror.Scene] public string scene;
        public string Name;
        [TextArea] public string description;
        public Sprite preview;
    }
    [SerializeField]
    private List<Map> maps;
    private int mapIndex = 0;
    [SerializeField] private Text mapNameTxt;
    [SerializeField] private Text mapDescriptionTxt;
    [SerializeField] private Image mapPreview;

    [SerializeField]
    private Lobby lobby;
    [SerializeField]
    private Menu menuToOpenOnClose;

    public override void Open() {
        base.Open();
        UpdateMapDisplay();
    }

    public override void Close() {
        if (gameObject.activeSelf) {
            base.Close();
            menuToOpenOnClose.Open();
        }
    }

    public void Button_MapLeft() {
        mapIndex = (mapIndex - 1) % maps.Count;
        if (mapIndex < 0)
            mapIndex = maps.Count-1;
        UpdateMapDisplay();
    }
    public void Button_MapRight() {
        mapIndex = (mapIndex + 1) % maps.Count;
        UpdateMapDisplay();
    }

    public void Button_StartGame() {
        lobby.gameSceneName = maps[mapIndex].scene;
        lobby.TryStartGame();
    }

    private void UpdateMapDisplay() {
        Map map = maps[mapIndex];
        mapNameTxt.text = map.Name;
        mapNameTxt.resizeTextForBestFit = true;
        mapDescriptionTxt.text = map.description;
        mapDescriptionTxt.resizeTextForBestFit = true;
        mapPreview.sprite = map.preview;
    }
}
