using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _roundSignalAudio;

    [SerializeField] private AudioSource _flickerSignalAudio;

    [SerializeField] private AudioSource _triggerSuccessAudio;

    [SerializeField] private AudioSource _triggerFailAudio;

    [SerializeField] private AudioSource _countdownAudio;

    [SerializeField] private AudioSource _startRoundAudio;

    [SerializeField] private AudioSource _clearRoundAudio;

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

    public void PlayFlickerSignalAudio()
    {
        _flickerSignalAudio.Play();
    }

    public void PlayTriggerSuccessAudio()
    {
        _triggerSuccessAudio.Play();
    }

    public void PlayTriggerFailAudio()
    {
        _triggerFailAudio.Play();
    }

    public void OnPlayTriggerAudio(GameObject tileObject, bool isValid)
    {
        if (isValid)
        {
            PlayTriggerSuccessAudio();
        }
        else
        {
            PlayTriggerFailAudio();
        }
    }

    public void PlayCountdownAudio()
    {
        _countdownAudio.Play();
    }

    public void PlayStartRoundAudio()
    {
        _startRoundAudio.Play();
    }

    public void PlayClearRoundAudio()
    {
        _clearRoundAudio.Play();
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
