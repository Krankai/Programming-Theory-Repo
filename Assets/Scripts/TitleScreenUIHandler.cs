using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreenUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;

    private string _playerName;

    public void OnNameInputValueChanged()
    {
        _playerName = _nameInput.text;
    }

    public void OnStartGame()
    {
        Debug.Log("Start game with player's name: " + _playerName);
    }
}
