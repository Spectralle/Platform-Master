using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : Singleton<AudioSystem>
{
#pragma warning disable CS0649
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _voiceoverSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _uiSource;
    [Space]
    [SerializeField] private Vector2 _randomVolumeRange;
    [SerializeField] private Vector2 _randomPitchRange;
#pragma warning restore CS0649


    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        _musicSource.clip = musicClip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void PlayVoiceClip(AudioClip voiceClip, bool randomizeVolumeAndPitch)
    {
        if (randomizeVolumeAndPitch)
        {
            _voiceoverSource.volume = Random.Range(_randomVolumeRange.x, _randomVolumeRange.y);
            _voiceoverSource.pitch = Random.Range(_randomPitchRange.x, _randomPitchRange.y);
        }
        _voiceoverSource.PlayOneShot(voiceClip);
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        _sfxSource.volume = Random.Range(_randomVolumeRange.x, _randomVolumeRange.y);
        _sfxSource.pitch = Random.Range(_randomPitchRange.x, _randomPitchRange.y);
        _sfxSource.PlayOneShot(sfxClip);
    }

    public void PlayUI(AudioClip uiClip) => _uiSource.PlayOneShot(uiClip);
}
