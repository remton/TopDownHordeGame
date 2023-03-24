

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour {
    public static CameraController instance;
   
    [SerializeField] private Camera cam;
    private float recenterSpeed = 1f;

    private Vector3 nextMovementPos;
    bool isShaking = false;
    private Timer timer;
    private System.Guid shakeTimerID;

    [Tooltip("units between screen edge and players when scaling the camera size")]
    [SerializeField]
    float edgeBufferSize;

    [Tooltip("size is vertical units from center of camera")]
    [SerializeField]
    private float minSize;

    [Tooltip("size is vertical units from center of camera")]
    [SerializeField]
    private float maxSize;

    private List<GameObject> players;

    public bool fixPosition = false;
    public Vector3 fixedPos = new Vector3();

    private void Update() {
        if(players.Count > 0) {
            MoveToNextPos();
            ChangeSize();
        }
    }

    //Sets the position of the camera
    //If you want it to stay there consider disabling follow players
    public void FixPos(Vector3 location) {
        fixedPos = location;
        fixPosition = true;
    }
    public void UnfixPos() {
        fixPosition = false;
    }

    public void MoveUpdate(Vector3 location, float speed) {

        if (isShaking) {
            // We are shaking
            Vector2 dir = new Vector3(location.x - transform.position.x, location.y - transform.position.y, 0);
            Vector3 newPos = transform.position + new Vector3(dir.x * speed * Time.deltaTime, dir.y * speed * Time.deltaTime, 0);
            transform.position = newPos;
        }
        else {
            // We arent shaking
            transform.position = location;
        }

    }

    public void Shake(float intensity) {
        intensity = intensity / 35;
        isShaking = true;
        Vector2 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        offset.Normalize();
        offset *= intensity;
        Vector3 targetPos;
        if (fixPosition)
            targetPos = fixedPos;
        else
            targetPos = FindMidpointOfPlayers();
        Vector3 newpos = targetPos + new Vector3(offset.x, offset.y, 0);

        float shakeTime = Vector3.Distance(targetPos, newpos) / recenterSpeed;
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
        if(!fixPosition)
            MoveUpdate(FindMidpointOfPlayers(), recenterSpeed);
        else
            MoveUpdate(fixedPos, recenterSpeed);
    }

    private void ChangeSize() {
        if (fixPosition || players.Count == 0)
            return;

        float maxX = players[0].transform.position.x;
        float minX = players[0].transform.position.x;
        float maxY = players[0].transform.position.y;
        float minY = players[0].transform.position.y;
        for (int i = 0; i < players.Count; i++) {
            if (maxX < players[i].transform.position.x) {
                maxX = players[i].transform.position.x;
            }
            if (minX > players[i].transform.position.x) {
                minX = players[i].transform.position.x;
            }
            if (maxY < players[i].transform.position.y) {
                maxY = players[i].transform.position.y;
            }
            if (minY > players[i].transform.position.y) {
                minY = players[i].transform.position.y;
            }
        }

        float needSizeForY = (edgeBufferSize + maxY - minY) / 2;
        float needSizeForX = (edgeBufferSize + maxX - minX) / (2 * cam.aspect); // since size deals with verical units we need to devide the x's by the aspect ratio
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
        PlayerManager.instance.EventActiveLocalPlayersChange += OnPlayersChanged;
        players = PlayerManager.instance.GetActiveLocalPlayers();
        MoveToNextPos();
    }

    public void OnPlayersChanged(List<GameObject> newPlayers) {
        players = newPlayers;
        if(players.Count == 0) {
            players = PlayerManager.instance.GetActivePlayers();
        }
    }

    private void Awake() {
        timer = GetComponent<Timer>();
        if (instance != null)
            Destroy(gameObject);
        else {
            instance = this;
        }
    }
    private void OnDestroy() {
        PlayerManager.instance.EventActiveLocalPlayersChange -= OnPlayersChanged;
    }
}
