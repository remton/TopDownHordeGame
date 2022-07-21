using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsController : Menu
{
    public Menu mainMenu;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public AudioClip sfxDemoClip;
    public AudioClip masterDemoClip;

    public void OpenMenu() {
        gameObject.SetActive(true);
        sfxSlider.normalizedValue = SaveData.instance.settings_volumeSFX;
        musicSlider.normalizedValue = SaveData.instance.settings_volumeMusic;
    }
    public void CloseMenu() {
        gameObject.SetActive(false);
        SaveData.Save();
        EventSystem.current.SetSelectedGameObject(mainMenu.defaultSelectedObject);
    }
    public void UpdateMasterVolume() {
        SaveData.instance.settings_volumeMaster = masterSlider.normalizedValue;
    }
    public void MasterSliderRelease(float value) {
        AudioManager.instance.PlaySound(masterDemoClip);
    }
    public void UpdateSFXVolume() {
        SaveData.instance.settings_volumeSFX = sfxSlider.normalizedValue;
    }
    public void SFXSliderRelease(float value) {
        AudioManager.instance.PlaySound(sfxDemoClip);
    }
    public void UpdateMusicVolume() {
        SaveData.instance.settings_volumeMusic = musicSlider.normalizedValue;
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
        sfxSlider.gameObject.GetComponent<SliderRelease>().EventOnRelease += SFXSliderRelease;
        masterSlider.gameObject.GetComponent<SliderRelease>().EventOnRelease += MasterSliderRelease;
    }
    private void OnDestroy() {
        sfxSlider.gameObject.GetComponent<SliderRelease>().EventOnRelease -= SFXSliderRelease;
        masterSlider.gameObject.GetComponent<SliderRelease>().EventOnRelease -= MasterSliderRelease;
    }
}
