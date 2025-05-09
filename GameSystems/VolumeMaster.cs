using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeMaster : MonoBehaviour
{
    [SerializeField] AudioMixer _mainMixer;
    [SerializeField] float _startVolume = 1f;
    [SerializeField] Slider[] _volumeSliders = new Slider[4];

    private void OnDestroy()
    {
        RemoveLisenersForSliders();
    }

    public void InitializeSliders()
    {
        foreach(Slider slider in _volumeSliders)
        {
            slider.value = _startVolume;
        }
        SetMasterVolume(_startVolume);
        SetButtonVolume(_startVolume);
        SetCharacterVolume(_startVolume);
        SetMusicVolume(_startVolume);
        AddLisenersForSliders();
    }

    public void SetMasterVolume(float sliderValue)
    {
        _mainMixer.SetFloat("MasterVolume", SliderValueToVolume(sliderValue));
    }
    public void SetButtonVolume(float sliderValue)
    {
        _mainMixer.SetFloat("ButtonVolume", SliderValueToVolume(sliderValue));
    }
    public void SetCharacterVolume(float sliderValue)
    {
        _mainMixer.SetFloat("CharacterVolume", SliderValueToVolume(sliderValue));
    }
    public void SetMusicVolume(float sliderValue)
    {
        _mainMixer.SetFloat("MusicVolume", SliderValueToVolume(sliderValue));
    }

    public void RemoveLisenersForSliders()
    {
        foreach(Slider slider in _volumeSliders)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
    }

    void AddLisenersForSliders()
    {
        _volumeSliders[0].onValueChanged.AddListener(SetMasterVolume);
        _volumeSliders[1].onValueChanged.AddListener(SetButtonVolume);
        _volumeSliders[2].onValueChanged.AddListener(SetCharacterVolume);
        _volumeSliders[3].onValueChanged.AddListener(SetMusicVolume);
    }

    public void ResetSliders()
    {
        AddLisenersForSliders();
        float[] volume = new float[4];
        _mainMixer.GetFloat("MasterVolume", out volume[0]);
        _mainMixer.GetFloat("ButtonVolume", out volume[1]);
        _mainMixer.GetFloat("CharacterVolume", out volume[2]);
        _mainMixer.GetFloat("MusicVolume", out volume[3]);
        for (int i = 0; i < 4; i++)
        {
            _volumeSliders[i].value = VolumeToSliderValue(volume[i]);
        }
    }

    float SliderValueToVolume(float sliderValue)
    {
        return Mathf.Log10(sliderValue) * 20;
    }

    float VolumeToSliderValue(float sliderValue)
    {
        return Mathf.Pow(10, sliderValue / 20);
    }
}
