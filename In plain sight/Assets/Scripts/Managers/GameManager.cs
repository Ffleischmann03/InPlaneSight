using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Multiplayer variables")]
    [SerializeField] private int numberOfPlayers;
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public int currentPlayer = 0;
    private bool playerActive = false;

    [Header("Round data")]
    public int numOfRounds;
    public float maxPlayTime;
    private float timeLeft;
    public int currentRound = 1;

    [Header("Interface variables")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerObjectHolder;
    [SerializeField] private ThirdPersonCam camController;
    [SerializeField] private PlayerMovement movement;
    


    [Header("Managers")]
    [SerializeField] private PlayableObjectManager PObjM;
    [SerializeField] private UIManager uiMng;
    [SerializeField] private PointsManager pm;

    private void Awake()
    {   
        //initialize the player -> object dictionary
        for(int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(i, null);
        }

        PObjM.SpawnObjects();

        pm.OnGameStart(numberOfPlayers);

        uiMng.NextPlayer();
        uiMng.roundLength = maxPlayTime;
        uiMng.maxRound = numOfRounds;

        
    }

    private void Update()
    {
        if(playerActive)
        {
            timeLeft -= Time.deltaTime;
            uiMng.UpdateTimer(timeLeft);

            if(timeLeft <= 0)
            {
                timeLeft = 0;
                EndTurn();
            }
            
        }
    }

    public void BeginTurn()
    {
        playerActive = true;
        //BroadcastMessage("Turn Started");

        //Find a new object for the player to inhabit
        GameObject playerObject = players[currentPlayer];

        

        //if the player had an object last turn, reset that object to be uninhabited
        if (playerObject != null)
        {
            PObjM.InhabitedObjects.Remove(playerObject);
        }
            

        //initialize the new object
        players[currentPlayer] = PObjM.GetFreeObject();
        playerObject = players[currentPlayer];

        Destroy(playerObject.GetComponent<Rigidbody>());

        playerObject.transform.rotation = Quaternion.Euler(0f, playerObject.transform.rotation.y, 0f);


        camController.InhabitObject(playerObject);
        movement.InhabitObject(playerObject);

        player.transform.position = playerObject.transform.position;
        player.transform.rotation = playerObject.transform.rotation;
        player.transform.localScale = playerObject.transform.localScale;
        playerObject.transform.parent = playerObjectHolder.transform;
        playerObject.tag = "Player";
        playerObject.layer = LayerMask.NameToLayer("Player");


        pm.PlayStart(currentPlayer, playerObject);

        timeLeft = maxPlayTime;
    }    

    private void EndTurn()
    {
        //BroadcastMessage("Turn Ended");

        GameObject playerObject = players[currentPlayer];
        playerObject.transform.parent = null;
        playerObject.tag = "Inactive";
        playerObject.layer = LayerMask.NameToLayer("Inhabitable");
        Rigidbody objRb = playerObject.AddComponent<Rigidbody>();
        objRb.interpolation = RigidbodyInterpolation.Interpolate;
        objRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        movement.movementEnabled = false;
        camController.movementEnabled = false;
        camController.UninhabitObject();

        pm.PlayEnd();

        playerActive = false;

        currentPlayer = (currentPlayer + 1) % numberOfPlayers;
        if (currentPlayer == 0)
        {
            bool gameOver = NextRound();
            if (gameOver)
                return;
        }

        uiMng.NextPlayer();
    }

    public void PlayerFound(GameObject playerObj)
    {
        int playerFound = 0;
        foreach(KeyValuePair<int, GameObject> item in players)
        {
            if(item.Value == playerObj)
            {
                playerFound = item.Key;
            }
        }

        //PLAYER {PLAYERFOUND} WAS FOUND

        pm.PlayerFound(playerFound);
    }

    private bool NextRound()
    {
        currentRound++;
        if(currentRound > numOfRounds)
        {
            EndGame();
            return true;
        }
        return false;
    }

    private void EndGame()
    {
        Dictionary<int, float> playerScores = pm.GetScores();

        int winner = 0;
        float winnerValue = 0;

        foreach(KeyValuePair<int, float> item in playerScores)
        {

            if (item.Value > winnerValue)
            {
                winner = item.Key;
                winnerValue = item.Value;
            }
                
        }

        uiMng.EndGame(winner, winnerValue);

    }





}
