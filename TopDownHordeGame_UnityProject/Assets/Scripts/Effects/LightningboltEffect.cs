using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningboltEffect : MonoBehaviour
{
    private SpriteRenderer sRenderer;

    private GameObject startPosObj;
    private GameObject endPosObj;

    Vector3 botLeft;
    Vector3 topRight;
    Vector3 startPos;
    Vector3 endPos;

    public void SetPos(Vector3 start, Vector3 end) {
        startPos = start;
        endPos = end;
        UpdateTransform();
    }
    //Called at the end of the animation
    public void DestroyEffect() {
        Destroy(gameObject);
    }
    private void Awake() {
        sRenderer = GetComponent<SpriteRenderer>();
    }
    private void UpdateTransform() {
        if(startPosObj!=null)
            startPos = startPosObj.transform.position;
        if(endPosObj!=null)
            endPos = endPosObj.transform.position;

        transform.localScale = new Vector3(1, 1, 1);

        botLeft = sRenderer.transform.TransformPoint(sRenderer.sprite.bounds.min);
        transform.Translate(startPos - botLeft);

        botLeft = sRenderer.transform.TransformPoint(sRenderer.sprite.bounds.min);
        topRight = sRenderer.transform.TransformPoint(sRenderer.sprite.bounds.max);

        float rotateAngle = Vector3.SignedAngle(topRight - botLeft, endPos - botLeft, Vector3.forward);
        transform.RotateAround(botLeft, Vector3.forward, rotateAngle);

        float currDist = Vector3.Distance(topRight, botLeft);
        float targetDist = Vector3.Distance(endPos, botLeft);
        float newScale = targetDist / currDist;
        transform.localScale = new Vector3(newScale, newScale, 1);
    }
}
