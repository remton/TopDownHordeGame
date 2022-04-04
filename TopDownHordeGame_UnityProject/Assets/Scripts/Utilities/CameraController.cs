
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private void Awake() {
        timer = GetComponent<Timer>();
        if (instance != null)
            Destroy(gameObject);
        else {
            instance = this;
        }
    }

    [SerializeField] private Camera cam;
    private float recenterSpeed = 1f;

    private Vector3 nextMovementPos;
    bool isShaking = false;
    private Timer timer;
    private System.Guid shakeTimerID;

    [Tooltip("units between screen edge and players when scaling the camera size")][SerializeField] 
    float edgeBufferSize; 

    [Tooltip("size is vertical units from center of camera")][SerializeField] 
    private float minSize;

    [Tooltip("size is vertical units from center of camera")][SerializeField] 
    private float maxSize;

    private List<GameObject> players;

    private void Update() {
        MoveToNextPos();
        ChangeSize();
    }


    public void MoveUpdate(Vector3 location, float speed) {
        
        if(isShaking) {
            // We are shaking
            Vector2 dir = new Vector3(location.x - transform.position.x, location.y - transform.position.y, 0);
            Vector3 newPos = transform.position + new Vector3(dir.x * speed*Time.deltaTime, dir.y * speed*Time.deltaTime, 0);
            transform.position = newPos;
        }
        else{
            // We arent shaking
            transform.position = location;
        }
        
    }

    public void Shake(float intensity) {
        isShaking = true;
        Vector2 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        offset.Normalize();
        offset *= intensity;
        Vector3 midpoint = FindMidpointOfPlayers();
        Vector3 newpos = midpoint + new Vector3(offset.x, offset.y, 0);

        float shakeTime = Vector3.Distance(midpoint, newpos)/recenterSpeed;
        transform.position = newpos;

        timer.KillTimer(shakeTimerID);
        shakeTimerID = timer.CreateTimer(shakeTime, StopShake);
    }

    private void StopShake() {
        isShaking = false;
    }

    private Vector3 FindMidpointOfPlayers() {
        if (players.Count == 0)
            return transform.position;

        //get midpoint of all players
        float sumX = 0;
        float sumY = 0;
        for (int i = 0; i < players.Count; i++) {
            sumX += players[i].transform.position.x;
            sumY += players[i].transform.position.y;
        }
        float avrX = sumX / players.Count;
        float avrY = sumY / players.Count;
        return new Vector3(avrX, avrY, transform.position.z);
    }
    public void MoveToNextPos() {
        if (players.Count == 0)
            return;
        MoveUpdate(FindMidpointOfPlayers(), recenterSpeed);
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
        MoveToNextPos();
    }
    
    public void OnPlayersChanged(List<GameObject> newPlayers) {
        players = newPlayers;
        //        players = PlayerManager.instance.GetActivePlayers();
//        Debug.Log("Camera OnPlayersChanged was called.");

    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= OnPlayersChanged;
    }
}
