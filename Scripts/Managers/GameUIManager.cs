using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private TMPro.TextMeshProUGUI _coinTxt;
    [SerializeField] private Slider _healthSlider;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinished;

    private enum GameStateUI
    {
        GamePlay, Pause, GameOver, GameIsFinished
    }
    private GameStateUI _currentState;

    private void Start() => SwitchUIState(GameStateUI.GamePlay);

    void Update()
    {
        _healthSlider.value = _gm.PlayerCharacter.GetComponent<Health>().CurrentHealthPercentage;
        _coinTxt.text = _gm.PlayerCharacter.Coin.ToString();
    }

    private void SwitchUIState(GameStateUI state)
    {
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);

        Time.timeScale = 1;

        switch (state)
        {
            case GameStateUI.GamePlay:
                break;
            case GameStateUI.Pause:
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case GameStateUI.GameOver:
                UI_GameOver.SetActive(true);
                break;
            case GameStateUI.GameIsFinished:
                UI_GameIsFinished.SetActive(true);
                break;
        }

        _currentState = state;
    }
    public void TogglePauseUI()
    {
        if (_currentState == GameStateUI.GamePlay)
            SwitchUIState(GameStateUI.Pause);
        else if (_currentState == GameStateUI.Pause)
            SwitchUIState(GameStateUI.GamePlay);
    }
    public void Button_MainMenu() => _gm.ReturnToTheMainMenu();
    public void Button_Restart() => _gm.Restart();
    public void ShowGameOverUI() => SwitchUIState(GameStateUI.GameOver);
    public void ShowGameIsFinishedUI() => SwitchUIState(GameStateUI.GameIsFinished);
}
