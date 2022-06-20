using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HockEyeEye : NetworkBehaviour
{
    private float balanceDamage;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;
    private GameObject owner;

    [ClientRpc]
    public void Init(Vector2 movementDir, float damage, float speed)
    {
        if (!isServer) {
            this.enabled = false;
            return;
        }

        rotationTemp.z += Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(rotationTemp);
        moveDir = movementDir;
        balanceDamage = damage;
        flySpeed = speed;
        GetComponent<HitBoxController>().EventObjEnter += Impact;
    }
    private void FixedUpdate()
    {
        if (PauseManager.instance.IsPaused())
            return;
        Move(moveDir);
    }
    [Server]
    public void Impact(GameObject player)
    {
        //Debug.Log("Impacting player.");
        if (player.CompareTag("Player"))
        {
            //Debug.Log("Hurting player.");
            GetComponent<HitBoxController>().EventObjEnter -= Impact;
            player.GetComponent<PlayerHealth>().Damage(balanceDamage);
        }
        NetworkServer.Destroy(gameObject);
    }

    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir)
    {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
}