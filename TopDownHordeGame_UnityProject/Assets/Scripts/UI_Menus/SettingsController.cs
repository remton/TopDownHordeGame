using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsController : Menu
{
    public Menu mainMenu;
    public Slider SFXSlider;
    public Slider MusicSlider;
    public AudioClip sfxDemoClip;

    public void OpenMenu() {
        gameObject.SetActive(true);
        SFXSlider.normalizedValue = SaveData.instance.settings_volumeSFX;
        MusicSlider.normalizedValue = SaveData.instance.settings_volumeMusic;
    }
    public void CloseMenu() {
        gameObject.SetActive(false);
        SaveData.Save();
        EventSystem.current.SetSelectedGameObject(mainMenu.defaultSelectedObject);
    }
    public void SFXSliderRelease(float value) {
        AudioManager.instance.PlaySound(sfxDemoClip);
    }
    public void UpdateSFXVolume() {
        SaveData.instance.settings_volumeSFX = SFXSlider.normalizedValue;
    }
    public void UpdateMusicVolume() {
        SaveData.instance.settings_volumeMusic = MusicSlider.normalizedValue;
    }
    public void ResetSaveData() {
        SaveData.DeleteSave();
        SaveData.ClearData();
        SaveData.Save();
    }
    public override void OnCancel() {
        CloseMenu();
    }

    private void Awake() {
        SFXSlider.gameObject.GetComponent<SliderRelease>().EventOnRelease += SFXSliderRelease;
    }
    private void OnDestroy() {
        SFXSlider.gameObject.GetComponent<SliderRelease>().EventOnRelease -= SFXSliderRelease;
    }
}
