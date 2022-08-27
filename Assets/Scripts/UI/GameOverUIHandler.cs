using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIHandler : MonoBehaviour
{
    [SerializeField] private Text _gameOverText;

    [SerializeField] private Text _congratsText;

    public void EnableUiWithSuccess(bool isSuccess = true)
    {
        _gameOverText.gameObject.SetActive(!isSuccess);
        _congratsText.gameObject.SetActive(isSuccess);

        if (isSuccess)
        {
            _congratsText.text = $"Congratulation, {DataManager.Instance.PlayerName}";
        }

        gameObject.SetActive(true);
    }

    public void DisableUi()
    {
        gameObject.SetActive(false);
    }
}
