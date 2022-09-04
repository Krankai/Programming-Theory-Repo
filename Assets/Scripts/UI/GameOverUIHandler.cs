using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverUIHandler : MonoBehaviour
{
    [SerializeField] private Text _gameOverText;

    [SerializeField] private Text _congratsText;

    [SerializeField] private UnityEvent _gameOverEvent;

    [SerializeField] private UnityEvent _congratsEvent;

    public void EnableUiWithSuccess(bool isSuccess = true)
    {
        _gameOverText.gameObject.SetActive(!isSuccess);
        _congratsText.gameObject.SetActive(isSuccess);

        if (isSuccess)
        {
            _congratsText.text = $"Congratulation, {DataManager.Instance.PlayerName}";
            _congratsEvent.Invoke();
        }
        else
        {
            _gameOverEvent.Invoke();
        }

        gameObject.SetActive(true);
    }

    public void DisableUi()
    {
        gameObject.SetActive(false);
    }
}
