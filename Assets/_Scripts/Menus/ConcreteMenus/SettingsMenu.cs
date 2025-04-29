using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : BaseMenu
{
    public AudioMixer mixer;

    public Button backBtn;

    public Slider masterSlider;
    public TMP_Text masterTxt;

    public Slider musicSlider;
    public TMP_Text musicTxt;

    public Slider sfxSlider;
    public TMP_Text sfxTxt;

    public override void Init(MenuController context)
    {
        base.Init(context);
        state = MenuStates.Settings;

        if (backBtn) backBtn.onClick.AddListener(JumpBack);

        LoadVolumeSettings();

        setupSliderInfo(masterSlider, masterTxt, "MasterVol");
        setupSliderInfo(musicSlider, musicTxt, "MusicVol");
        setupSliderInfo(sfxSlider, sfxTxt, "SFXVol");
    }

    void OnSliderValueChanged(float value, Slider slider, TMP_Text sliderText, string parameterName)
    {
        value = (value == 0.0f) ? -80.0f : 20.0f * Mathf.Log10(slider.value);
        sliderText.text = (value == -80.0f) ? "0%" : $"{(int)(slider.value * 100)}%";
        mixer.SetFloat(parameterName, value);

        PlayerPrefs.SetFloat(parameterName, slider.value);
    }
    void setupSliderInfo(Slider slider, TMP_Text sliderText, string parameterName)
    {
        slider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, slider, sliderText, parameterName));
        OnSliderValueChanged(slider.value, slider, sliderText, parameterName);
    }

    void LoadVolumeSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);

        mixer.SetFloat("MasterVol", (masterSlider.value == 0.0f) ? -80.0f : 20.0f * Mathf.Log10(masterSlider.value));
        mixer.SetFloat("MusicVol", (musicSlider.value == 0.0f) ? -80.0f : 20.0f * Mathf.Log10(musicSlider.value));
        mixer.SetFloat("SFXVol", (sfxSlider.value == 0.0f) ? -80.0f : 20.0f * Mathf.Log10(sfxSlider.value));
    }
}