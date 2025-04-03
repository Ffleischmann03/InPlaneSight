using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gm;
    [SerializeField] private UIManager uiman;

    private GameObject playerObj;
    private Dictionary<int, float> playerPotential = new Dictionary<int, float>();
    private Dictionary<int, float> playerScores = new Dictionary<int, float>();
    private int currentPlayer;
    private Vector3 playerStartLocation;
    private float currentDistance;


    public void OnGameStart(int numOfPlayers)
    {
        for(int i = 0; i < numOfPlayers; i++)
        {
            playerScores.Add(i, 0);
            playerPotential.Add(i, 0);
        }
    }

    public void PlayStart(int playerNum, GameObject _playerObj)
    {
        currentPlayer = playerNum;
        playerObj = _playerObj;
        playerScores[currentPlayer] += playerPotential[currentPlayer];

        playerStartLocation = playerObj.transform.position;
    }

    public void PlayEnd()
    {
        playerObj = null;
        playerPotential[currentPlayer] = currentDistance;
    }

    public void PlayerFound(int playerNum)
    {
        Debug.Log($"Player {playerNum + 1} just lost their {Mathf.Round(playerPotential[playerNum])} points");
        playerPotential[playerNum] = 0;
    }

    private void Update()
    {
        if(playerObj)
        {
            currentDistance = Vector3.Distance(playerStartLocation, playerObj.transform.position);

            uiman.DistanceUpdate(currentDistance);
        }
    }

    public Dictionary<int, float> GetScores()
    {
        return playerScores;
    }
}
