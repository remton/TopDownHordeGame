using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeMenu : Menu
{
    public GameObject infoBox;
    public GameObject ChallengeHolder;
    public GameObject challengeItemPrefab;
    public GameObject challengeItemHolder;
    private List<GameObject> challengeItems = new List<GameObject>();
    public override void Open() {
        base.Open();
        foreach (GameObject item in challengeItems) {
            Destroy(item);
        }
        challengeItems.Clear();


        Challenge[] challenges = ChallengeHolder.GetComponentsInChildren<Challenge>();
        foreach (Challenge challenge in challenges) {
            if (challenge.unlocked) {
                GameObject item = Instantiate(challengeItemPrefab, challengeItemHolder.transform);
                item.GetComponent<ChallengeItem>().Init(challenge, infoBox);
                challengeItems.Add(item);
            }
        }
    }
}
