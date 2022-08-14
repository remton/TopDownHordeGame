using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


using UnityEngine.InputSystem;

public class MainMenuController : Menu {

    public GameObject zoomImg;
    public float zoomScale;
    public float zoomTime;

    private Timer timer;
    private System.Guid zoomTimerID;

    public void PlayStartAnim() {
        zoomTimerID = timer.CreateTimer(zoomTime, null);
    }

    public void ExitGame() {
        Debug.Log("There is no escape");
        Application.Quit();
    }

    public override void SetInteractable(bool interactable) {
        base.SetInteractable(interactable);
        gameObject.SetActive(interactable);
    }

    protected override void Update() {
        base.Update();
        if (timer.HasTimer(zoomTimerID)) {
            float ratio = timer.TimeLeft(zoomTimerID) / zoomTime;
            float scale = Mathf.Lerp(zoomScale, 1, ratio);
            zoomImg.transform.localScale = new Vector3(scale, scale, 1);
        }

        //if (Keyboard.current.pKey.IsPressed()) {
        //    PlayStartAnim();
        //}
    }
    public override void OnCancel() {
        //Override to do nothing
    }

    private void Awake() {
        timer = GetComponent<Timer>();
    }

}
