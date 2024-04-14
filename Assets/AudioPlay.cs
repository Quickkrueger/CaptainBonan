using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public AudioSource _audioSource;

    public AudioClip[] audioClips;

    public void PlayAudio(string clipName)
    {
        for (int i = 0; i < audioClips.Length; i++)
        {
            if (audioClips[i].name.Contains(clipName))
            {
                _audioSource.clip = audioClips[i];
                _audioSource.Play();
                break;
            }
        }
    }
}