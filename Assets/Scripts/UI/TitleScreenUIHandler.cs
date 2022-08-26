using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleScreenUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;

    public void OnNameInputValueChanged()
    {
        DataManager.Instance.PlayerName = _nameInput.text;
    }

    public void StartGame()
    {
        //Debug.Log("Start game with player's name: " + DataManager.Instance.PlayerName);
        SceneManager.LoadScene(1);
    }
}
