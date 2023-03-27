using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameUIManager _gameUIManager;
    public Player PlayerCharacter { get; private set; }
    private bool _gameIsOver;

    private void Awake() => PlayerCharacter = FindObjectOfType<Player>();
    private void GameOver() => _gameUIManager.ShowGameOverUI();
    public void GameIsFinished() => _gameUIManager.ShowGameIsFinishedUI();

    void Update()
    {
        if (_gameIsOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            _gameUIManager.TogglePauseUI();

        if (PlayerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            _gameIsOver = true;
            GameOver();
        }
    }

    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
