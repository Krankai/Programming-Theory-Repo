using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _roundSignalAudio;

    private AudioSource _backgroundAudio;

    public void ToggleBGM(bool isOn)
    {
        if (isOn)
        {
            _backgroundAudio.Play();
        }
        else
        {
            _backgroundAudio.Stop();
        }
    }

    public void PlayRoundSignal()
    {
        _roundSignalAudio.Play();
    }

    private void Awake()
    {
        _backgroundAudio = gameObject.GetComponent<AudioSource>();
    }
}
