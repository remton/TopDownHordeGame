using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurpriseMechanic : WeaponShop
{
    //public GameObject[] weaponPrefabList;
    public List<RandomChoice<GameObject>> weaponChoices;
    //private int cost;
    //public int baseCost;
    [SerializeField] SpriteRenderer expressionRenderer;
    [SerializeField] Animator txtAnimator;

    public List<RandomChoice<Sprite>> expressions;
    public Sprite defaultExpression;
    public Sprite blinkExpression;
    public float expressTime;

    private void Start() {
        StartCoroutine(DefaultExpress());
    }

    override public void TryBuyWeapon(GameObject player) {
        StartCoroutine(RandomExpress());
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
        if (playerStats.GetBank() >= cost)
        {
            //SoundPlayer.Play(purchaseSound, transform.position);
            AudioManager.instance.PlaySound(purchaseSound);
            playerStats.TrySpendMoney(cost);
            weaponControl.PickUpWeapon(RandomChoice<GameObject>.ChooseRandom(weaponChoices));
        }
        else
        {
            //SoundPlayer.Play(failPurchaseSound, transform.position);
            AudioManager.instance.PlaySound(failPurchaseSound);
            Debug.Log("Need something. Money, perhaps.");
        }
    }

    override public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyWeapon;
        popupCanvas.SetActive(true);
        popupCanvas.GetComponentInChildren<Text>().text = "Surprise Mechanic" + "\n$" + cost; // "Surprise Mechanic"
        SpriteWriteText(true);
    }

    override public void OnPlayerExit(GameObject player) {
        base.OnPlayerExit(player);
        SpriteWriteText(false);
    }

    private void SpriteWriteText(bool b) {
        txtAnimator.SetBool("showTxt", b);
    }

    private IEnumerator DefaultExpress() {
        while (true) {
            expressionRenderer.sprite = defaultExpression;
            yield return new WaitForSeconds(Random.Range(15, 30));
            yield return new WaitForEndOfFrame();
            expressionRenderer.sprite = blinkExpression;
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator RandomExpress() {
        StopCoroutine(DefaultExpress());
        expressionRenderer.sprite = RandomChoice<Sprite>.ChooseRandom(expressions);
        yield return new WaitForSeconds(expressTime);
        StartCoroutine(DefaultExpress());
    }
}
