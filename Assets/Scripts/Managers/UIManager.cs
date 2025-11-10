using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gm;

    [Header("Turn Prompt Screen")]
    [SerializeField] private GameObject turnPromptScreen;
    [SerializeField] private TMP_Text PlayerTurnPrompt;
    [SerializeField] private TMP_Text roundCounter;
    [HideInInspector] public int maxRound;
    [HideInInspector] public int currentRound;

    [Header("In Game UI")]
    [SerializeField] private GameObject inGameUi;
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text timerText;

    [Header("End game screen")]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text winnerText;

    //round data
    [HideInInspector] public float roundLength;
    private float timeLeft;

    private void Start()
    {
        NextPlayer();
    }

    public void NextPlayer()
    {
        //enable the switch screen
        turnPromptScreen.SetActive(true);
        inGameUi.SetActive(false);

        int currentPlayer = gm.currentPlayer + 1;

        PlayerTurnPrompt.text = $"Player {currentPlayer}'s turn";

        currentRound = gm.currentRound;

        roundCounter.text = $"Round {currentRound}/{maxRound}";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void StartButtonPressed()
    {
        turnPromptScreen.SetActive(false);
        inGameUi.SetActive(true);
        gm.BeginTurn();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        timeLeft = roundLength;
    }

    public void DistanceUpdate(float _distance)
    {
        float distance = _distance;

        distance = Mathf.Round(distance);

        distanceText.text = $"Distance: {distance}";
    }

    public void UpdateTimer(float _timeLeft)
    {
        timeLeft = Mathf.Round(_timeLeft);

        timerText.text = $"Time Left: {timeLeft}";
    }

    public void EndGame(int winner, float winnerVal)
    {
        endScreen.SetActive(true);
        winnerText.text = $"The winner is {winner} with {winnerVal} points!";
    }

    public void Restart()
    {
        
        SceneManager.LoadScene("Gameplay");
    }
}
