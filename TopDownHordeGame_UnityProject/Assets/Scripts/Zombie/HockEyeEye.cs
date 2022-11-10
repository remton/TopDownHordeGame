using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HockEyeEye : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private float balanceDamage;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;
    private GameObject owner;

    private static bool pingPong = false;
    private static float pingPongKnockback = 0;
    public static void SetPingPong(float knockback) {
        if (knockback > 0)
            pingPong = true;
        else
            pingPong = false;
        pingPongKnockback = knockback;
    }

    [ClientRpc]
    public void Init(Vector2 movementDir, float damage, float speed)
    {
        moveDir = movementDir;
        balanceDamage = damage;
        flySpeed = speed;
        rb.velocity = moveDir * flySpeed;
        if (!isServer) {
            this.enabled = false;
            return;
        }
        rotationTemp.z += Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(rotationTemp);
        GetComponent<HitBoxController>().EventObjEnter += Impact;
    }

    [Server]
    public void Impact(GameObject player)
    {
        //Debug.Log("Impacting player.");
        if (player.CompareTag("Player"))
        {
            //Debug.Log("Hurting player.");
            GetComponent<HitBoxController>().EventObjEnter -= Impact;
            player.GetComponent<PlayerHealth>().DamageCMD(balanceDamage);
            if (pingPong)
            {
                player.GetComponent<PlayerMovement>().KnockbackCMD(Mathf.Max(pingPongKnockback, rb.velocity.magnitude * 60), moveDir); // change the first argument to adjust the knockback
            }
        }
        NetworkServer.Destroy(gameObject);
    }

    private void OnPause(bool pause) {
        if (pause) {
            rb.velocity = Vector2.zero;
        }
        else {
            rb.velocity = moveDir * flySpeed;
        }
    }

    private void Start() {
        PauseManager.instance.EventPauseStateChange += OnPause;
    }
    private void OnDestroy() {
        PauseManager.instance.EventPauseStateChange -= OnPause;
    }

}