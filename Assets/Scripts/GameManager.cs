using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// GameManger class that controls the state of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    #region SINGLETON
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    #region VARIABLE DECLARATIONS

    [Tooltip("Reference to the GameBoard game object")]
    public GameObject gameBoard = null;

    [Tooltip("Reference for the boardSquare Prefab")]
    public GameObject boardSquare = null;

    [Tooltip("Reference for the boardSquares that get instantiated when the game starts")]
    [SerializeField]
    private List<GameObject> boardSquares = new List<GameObject>();

    [Tooltip("Reference for the buttons that the user can click on the board squares")]
    [SerializeField]
    private List<Button> allBoardSquareButtons = new List<Button>();

    [Tooltip("the controller for the scoreboard, this link is defined in unity")]
    public ScoreBoardController scoreBoardController = null;

    //0,1,2 (2 IS FOR IMPOSSIBLE MOVE)
    [Tooltip("The human player")]
    public int[] playerX = new int[9];
    [Tooltip("The computer player")]
    public int[] playerO = new int[9];

    //the point count for both players
    private int xPoints = 0;
    private int oPoints = 0;

    //the number of moves made so far
    public int moveCount = 0;

    //a constant representing the maximum number of available board squares
    private const int SQUARECOUNT = 9;

    [Tooltip("favored button for the computer to want to use")]
    public Button favoredSquareButton = null;

    #endregion

    #region MonoBehaviour and Unity Lifecycle Hooks
    void Start()
    {
        Transform example = this.gameObject.transform.gameObject.transform;
        //Load the peices of the tictactoe board.
        LoadBoardSquares();
    }

    private void Update()
    {
        // quit the application if the user presses the escape key (only works in a build)
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    /// <summary>
    /// Loads the 9 playable board squares of the tic tac toe board.
    /// </summary>
    void LoadBoardSquares()
    {
        //add the rest of the sq
        for (int i = 0; i < SQUARECOUNT; i++)
        {
            GameObject newBoardSquare = Instantiate<GameObject>(boardSquare, gameBoard.transform);
            newBoardSquare.GetComponent<TickTackToeButtonController>().buttonIndex = i;
            this.CacheBoardSquares(newBoardSquare);
        }

        List<Button> findFavoredButton = allBoardSquareButtons.Where(button => button.gameObject.GetComponent<TickTackToeButtonController>().buttonIndex == 4).ToList();
        favoredSquareButton = findFavoredButton[0];
    }

    /// <summary>
    /// Determine if a winning move exists for the computer
    /// </summary>
    /// <returns></returns>
    internal (int, bool) DoesWinningMoveExist(int [] player)
    {
        int[] playerOCopy = new int[9];

        //fill in player copy with other data
        for (int i = 0; i < playerOCopy.Length; i++)
        {
            playerOCopy[i] = player[i];
        }

        for (int i = 0; i < playerOCopy.Length; i++)
        {
            if (playerOCopy[i].Equals(0))
            {
                playerOCopy[i] = 1;
                bool result = checkForWinningStateRedo(playerOCopy);
                if (result)
                {
                    return (i, result);
                }
                playerOCopy[i] = 0;
            }
        }

        return (0, false);
        throw new NotImplementedException();
    }

    /// <summary>
    /// determine if a winning move exists for the player
    /// </summary>
    /// <returns></returns>
    internal bool DoesWinningMoveExistForPlayer()
    {
        return false;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add the boardSquare and its associated button component to the associated lists 
    /// used to reference available moves and button components
    /// </summary>
    /// <param name="boardSquare">GameObject representing a tictactoe board piece</param>
    void CacheBoardSquares(GameObject boardSquare)
    {
        this.boardSquares.Add(boardSquare);
        this.allBoardSquareButtons.Add(boardSquare.GetComponent<Button>());
    }

    /// <summary>
    /// returns the winning move if it finds one
    /// </summary>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public bool checkForWinningStateRedo(int [] playerData)
    {
        //TODO: check all the squares that X is in
        if (
            playerData[0].Equals(1) && playerData[1].Equals(1) && playerData[2].Equals(1) ||
            playerData[0].Equals(1) && playerData[4].Equals(1) && playerData[8].Equals(1) ||
            playerData[3].Equals(1) && playerData[4].Equals(1) && playerData[5].Equals(1) ||
            playerData[6].Equals(1) && playerData[7].Equals(1) && playerData[8].Equals(1) ||
            playerData[0].Equals(1) && playerData[3].Equals(1) && playerData[6].Equals(1) ||
            playerData[1].Equals(1) && playerData[4].Equals(1) && playerData[7].Equals(1) ||
            playerData[2].Equals(1) && playerData[5].Equals(1) && playerData[8].Equals(1) ||
            playerData[2].Equals(1) && playerData[4].Equals(1) && playerData[6].Equals(1)
        )
        {
            return true;
        }

        return false;
    }

    #endregion

    #region End Game Logic
    /// <summary>
    /// checks for a winning state between X and O
    /// </summary>
    /// <returns>tuple containing a boolean indicating that the game is over and a string indicating the player that won</returns>
    public (bool, string) checkForWinningState()
    {
        //TODO: check all the squares that X is in
        if(anyWinningStatesForPlayer(playerX))
        {
            DisableAllButtons();
            Debug.Log("Player X won");
            this.xPoints++;
            this.scoreBoardController.updatePlayerXScore();
            return (true, "Player X");
        }

        //TODO: check all the squares that Y is in
        if (anyWinningStatesForPlayer(playerO))
        {
            DisableAllButtons();
            Debug.Log("Player O won");
            this.oPoints++;
            this.scoreBoardController.updatePlayerOScore();
            return (true, "Player O");
        }

        //return the state
        return (false, null);
    }

    /// <summary>
    /// checks if there are any winning states for a particular player
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private bool anyWinningStatesForPlayer(int [] player)
    {
        return (player[0].Equals(1) && player[1].Equals(1) && player[2].Equals(1) ||
            player[0].Equals(1) && player[4].Equals(1) && player[8].Equals(1) ||
            player[3].Equals(1) && player[4].Equals(1) && player[5].Equals(1) ||
            player[6].Equals(1) && player[7].Equals(1) && player[8].Equals(1) ||
            player[0].Equals(1) && player[3].Equals(1) && player[6].Equals(1) ||
            player[1].Equals(1) && player[4].Equals(1) && player[7].Equals(1) ||
            player[2].Equals(1) && player[5].Equals(1) && player[8].Equals(1) ||
            player[2].Equals(1) && player[4].Equals(1) && player[6].Equals(1));
    }

    /// <summary>
    /// disables all the buttons so that the user cannot click on a button after the game has been finished
    /// </summary>
    internal void DisableAllButtons()
    {
        foreach (Button button in this.allBoardSquareButtons)
        {
            button.interactable = false;
        }
    }

    /// <summary>
    /// Restart a game or make a new game
    /// </summary>
    public void onClickRestart()
    {
        this.moveCount = 0;

        foreach(Button button in allBoardSquareButtons)
        {
            button.interactable = true;
            foreach(Transform child in button.gameObject.transform)
            {
                child.gameObject.SetActive(false);
            }
            Destroy(button.gameObject);
        }

        for(int i = 0; i < SQUARECOUNT; i++)
        {
            playerX[i] = 0;
            playerO[i] = 0;
        }

        this.allBoardSquareButtons.Clear();
        this.boardSquares.Clear();

        LoadBoardSquares();
    }

    #endregion

    #region Getters and Setters

    /// <summary>
    /// Obtain a list of all available board squares.
    /// </summary>
    public List<GameObject> GetBoardSquares()
    {
        return this.boardSquares;
    }

    /// <summary>
    /// get the points for the human player
    /// </summary>
    /// <returns></returns>
    public int getPlayerXPoints()
    {
        return this.xPoints;
    }

    /// <summary>
    /// get the points for the computer player
    /// </summary>
    /// <returns></returns>
    public int getPlayerOPoints()
    {
        return this.oPoints;
    }

    #endregion
}
