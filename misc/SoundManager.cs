using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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
	AudioSource _audioSource;


    public SoundAudioClip[] soundAudioClips;
    [SerializeField] AudioMixerGroup SoundMixer;
	[SerializeField] float minSoundInterval = 0.2f;
	bool hasPassMinInterval = true;
	//string _lastPlayedSound;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string sound)
	{
		if (!hasPassMinInterval) return;
        _audioSource.clip = GetAudioClip(sound);
        _audioSource.outputAudioMixerGroup = SoundMixer;
        _audioSource.Play();
		hasPassMinInterval = false;
		StartCoroutine(ExtendIEnumerator.DelayAction(minSoundInterval, () => hasPassMinInterval = true));
		//_lastPlayedSound = sound;
    }

	public void PlayExtraSound(string sound)
	{
		GameObject soundObj = new GameObject("Sound");
        soundObj.transform.position = gameObject.transform.position;
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.outputAudioMixerGroup = SoundMixer;
		audioSource.maxDistance = 20f;
        audioSource.spatialBlend = 1f; // make the sound 3d
		audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.Play();
		Destroy(soundObj, audioSource.clip.length);
    }

	public void StopSound()
	{
        _audioSource.Stop();
    }

	public void Mute(bool b)
	{
        _audioSource.mute = b;
	}

	private AudioClip GetAudioClip(string sound)
	{
		foreach (SoundAudioClip soundAudioClip in soundAudioClips)
		{
			if (soundAudioClip.sound == sound)
				return soundAudioClip.audioClip;
		}
		Debug.LogWarning("Sound " + sound + " not found in SoundAudioClips list");
		return null;
	}
}
