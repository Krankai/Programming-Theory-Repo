using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleScreenUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;

    [SerializeField] private GameObject _tutorialPanel;

    public void OnNameInputValueChanged()
    {
        DataManager.Instance.PlayerName = _nameInput.text;
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(DataManager.Instance.PlayerName))
        {
            DataManager.Instance.PlayerName = "Anonymous";
        }

        SceneManager.LoadScene(1);
    }

    public void ShowTutorial()
    {
        _tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        _tutorialPanel.SetActive(false);
    }
}
