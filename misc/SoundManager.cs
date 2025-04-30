using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
	[System.Serializable]
	public class SoundAudioClip
	{
		public string sound;
		public AudioClip audioClip;
	}

	public SoundAudioClip[] soundAudioClips;
    [SerializeField] AudioMixerGroup SoundMixer;

    public void PlaySound(string sound)
	{
		GameObject soundGameObject = new GameObject("Sound");
		AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
		audioSource.clip = GetAudioClip(sound);
        audioSource.outputAudioMixerGroup = SoundMixer;
        audioSource.Play();
		Destroy(soundGameObject, audioSource.clip.length);
	}

	private AudioClip GetAudioClip(string sound)
	{
		foreach (SoundAudioClip soundAudioClip in soundAudioClips)
		{
			if (soundAudioClip.sound == sound)
				return soundAudioClip.audioClip;
		}
		return null;
	}
}
