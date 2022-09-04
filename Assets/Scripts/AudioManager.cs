using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _roundSignalAudio;

    [SerializeField] private AudioSource _flickeringSignalAudio;

    [SerializeField] private AudioSource _countdownAudio;

    [SerializeField] private AudioSource _startAudio;

    [SerializeField] private AudioSource _successGameOverAudio;

    [SerializeField] private AudioSource _failGameOverAudio;

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

    public void PlayRoundSignalAudio()
    {
        _roundSignalAudio.Play();
    }

    public void PlayFlickeringSignalAudio()
    {
        _flickeringSignalAudio.Play();
    }

    public void PlayCountdownAudio()
    {
        _countdownAudio.Play();
    }

    public void PlayStartAudio()
    {
        _startAudio.Play();
    }

    public void PlaySuccessGameOverAudio()
    {
        _successGameOverAudio.Play();
    }

    public void PlayFailGameOverAudio()
    {
        _failGameOverAudio.Play();
    }

    private void Awake()
    {
        _backgroundAudio = gameObject.GetComponent<AudioSource>();
    }
}
