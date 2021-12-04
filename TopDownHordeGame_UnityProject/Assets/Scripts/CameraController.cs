using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Tooltip("units between screen edge and players when scaling the camera size")][SerializeField] 
    float edgeBufferSize; 

    [Tooltip("size is vertical units from center of camera")][SerializeField] 
    private float minSize;

    [Tooltip("size is vertical units from center of camera")][SerializeField] 
    private float maxSize;

    private List<GameObject> players;

    private void Update() {
        MoveToMidpointOfPlayers();
        ChangeSize();
    }

    public void MoveToMidpointOfPlayers() {
        if (players.Count == 0)
            return;

        //get midpoint of all players
        float sumX = 0;
        float sumY = 0;
        for (int i = 0; i < players.Count; i++) {
            sumX += players[i].transform.position.x;
            sumY += players[i].transform.position.y;
        }
        float avrX = sumX/players.Count;
        float avrY = sumY/players.Count;
        Vector3 midpoint = new Vector3(avrX,avrY, transform.position.z);
        transform.position = midpoint;
    }

    private void ChangeSize() {
        float maxX = players[0].transform.position.x;
        float minX = players[0].transform.position.x;
        float maxY = players[0].transform.position.y;
        float minY = players[0].transform.position.y;
        for (int i = 0; i < players.Count; i++) {
            if(maxX < players[i].transform.position.x) {
                maxX = players[i].transform.position.x;
            }
            if(minX > players[i].transform.position.x){
                minX = players[i].transform.position.x;
            }
            if(maxY < players[i].transform.position.y) {
                maxY = players[i].transform.position.y;
            }
            if(minY > players[i].transform.position.y) {
                minY = players[i].transform.position.y;
            }
        }

        float needSizeForY = (edgeBufferSize + maxY - minY)/2;
        float needSizeForX = (edgeBufferSize + maxX - minX)/(2*cam.aspect); // since size deals with verical units we need to devide the x's by the aspect ratio
        float size = (needSizeForX > needSizeForY) ? needSizeForX : needSizeForY;
        if (size < minSize)
            size = minSize;
        else if (size > maxSize)
            size = maxSize;

        cam.orthographicSize = size;
    }

    // -- Get players from player manager --
    // We use start instead of awake since playermanager needs to set its instance first
    private void Start() {
        players = PlayerManager.instance.GetActivePlayers();
        PlayerManager.instance.EventActivePlayersChange += OnPlayersChanged;
    }
    
    public void OnPlayersChanged(List<GameObject> newPlayers) {
        players = newPlayers;
    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= OnPlayersChanged;
    }
}
