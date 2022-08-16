using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChallengeInfobox : MonoBehaviour
{
    public Text title;
    public Text description;
    public Text progress;

    public void SetChallenge(Challenge challenge) {
        title.text = challenge.title;
        description.text = challenge.description;
        int progressVal = Mathf.FloorToInt(challenge.progress * 100);
        progress.text = progressVal.ToString() + "%";
    }

    public void SetLocation(Vector3 location) {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 corner = corners[3];
        Vector3 diff = location - corner;
        transform.position += diff;
    }
}
