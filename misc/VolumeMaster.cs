using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeMaster : MonoBehaviour
{
    [SerializeField] AudioMixer _mainMixer;
    [SerializeField] float _startVolume = 1f;
    [SerializeField] Slider[] _volumeSliders = new Slider[3];

    public void InitializeSliders()
    {
        _volumeSliders[0].value = _startVolume;
        _volumeSliders[1].value = _startVolume;
        _volumeSliders[2].value = _startVolume;
        SetMasterVolume(_startVolume);
        SetButtonVolume(_startVolume);
        SetCharacterVolume(_startVolume);
        _volumeSliders[0].onValueChanged.AddListener(SetMasterVolume);
        _volumeSliders[1].onValueChanged.AddListener(SetButtonVolume);
        _volumeSliders[2].onValueChanged.AddListener(SetCharacterVolume);
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

    public void RemoveLiseners()
    {
        _volumeSliders[0].onValueChanged.RemoveAllListeners();
        _volumeSliders[1].onValueChanged.RemoveAllListeners();
        _volumeSliders[2].onValueChanged.RemoveAllListeners();
    }

    public void ResetSliders()
    {
        _volumeSliders[0].onValueChanged.AddListener(SetMasterVolume);
        _volumeSliders[1].onValueChanged.AddListener(SetButtonVolume);
        _volumeSliders[2].onValueChanged.AddListener(SetCharacterVolume);
        float[] volume = new float[3];
        _mainMixer.GetFloat("MasterVolume", out volume[0]);
        _mainMixer.GetFloat("ButtonVolume", out volume[1]);
        _mainMixer.GetFloat("CharacterVolume", out volume[2]);
        for(int i = 0; i < 3; i++)
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
