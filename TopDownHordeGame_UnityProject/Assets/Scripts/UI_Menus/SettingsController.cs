using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsController : Menu
{
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public AudioClip sfxDemoClip;
    public AudioClip masterDemoClip;

    public override void Open() {
        base.Open();
        masterSlider.normalizedValue = SaveData.instance.settings_volumeMaster;
        sfxSlider.normalizedValue = SaveData.instance.settings_volumeSFX;
        musicSlider.normalizedValue = SaveData.instance.settings_volumeMusic;
    }
    public override void Close() {
        base.Close();
        SaveData.Save();
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
        UpdateMasterVolume();
        UpdateSFXVolume();
        UpdateMusicVolume();
        SaveData.Save();
    }
    public override void OnCancel() {
        Close();
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
