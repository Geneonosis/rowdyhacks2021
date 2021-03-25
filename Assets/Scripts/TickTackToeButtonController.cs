using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TickTackToeButtonController : MonoBehaviour
{
    public int buttonIndex = 0;
    private AudioSource audioSource = null;

    //comment out line 20 and un-comment lines 12-15 to see how unity tries to handle a constructor in a monobehavior.
    //private tickTackToeButtonController()
    //{
    //    audioSource = this.gameObject.GetComponent<AudioSource>();
    //}

    //can be used like a constructor
    private void Awake()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    /// determines what happens when the player selects one of the squares
    /// </summary>
    /// <param name="isHumanPlayer">asking if the human is the one who made the move</param>
    public void onClickSquare(bool isHumanPlayer)
    {
        GameManager.Instance.moveCount++;

        if (isHumanPlayer)
        {
            audioSource.PlayOneShot(audioSource.clip); //play the selection choice sound if the human made the move

            this.ConductMove(GameManager.Instance.playerX, GameManager.Instance.playerO, 0);

            //IF there was a winning state, end the game
            if (GameManager.Instance.checkForWinningState().Item1)
                return;

            //NO MORE MOVES
            if (GameManager.Instance.GetBoardSquares().Count.Equals(0))
            {
                GameManager.Instance.DisableAllButtons();
                return;
            }

            //force the computer to make a selection and place their move
            int selectedNumber = 0;
            GameObject selectedBoardSquare = null;
            Debug.Log(GameManager.Instance.moveCount);

            //Computer AI for making a selection begins here

            if (GameManager.Instance.moveCount.Equals(1) && GameManager.Instance.favoredSquareButton.IsInteractable())
            {
                selectedBoardSquare = GameManager.Instance.favoredSquareButton.gameObject;
            }
            else if(GameManager.Instance.moveCount.Equals(1))
            {
                int[] chooseOne = { 0, 2, 6, 8 }; //strategically better to choose a corner peice if the favored middle is taken by the user.
                System.Random random = new System.Random();
                int choice = chooseOne[random.Next(0, chooseOne.Length)]; //randomly selects a number between 0 inclusive and 4 exclusive, uses that as the index to choose a corner peice

                //find the corner peice and temporarily store it
                List<GameObject> findFavoredButton = GameManager.Instance.GetBoardSquares().Where(
                    boardSquare => boardSquare.GetComponent<TickTackToeButtonController>().buttonIndex.Equals(choice)
                    ).ToList();

                selectedBoardSquare = findFavoredButton[0];
            }
            else if( GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerO).Item2 )
            {
                Debug.Log($"the winning move is {GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerO).Item1}");

                selectedNumber = GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerO).Item1;

                List<GameObject> findFavoredButton = GameManager.Instance.GetBoardSquares().Where(
                    boardSquare => boardSquare.GetComponent<TickTackToeButtonController>().buttonIndex.Equals(selectedNumber)
                    ).ToList();

                selectedBoardSquare = findFavoredButton[0];
            }
            else if( GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerX).Item2 )
            {
                Debug.Log($"the winning move is {GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerX).Item1}");

                selectedNumber = GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerX).Item1;

                List<GameObject> findFavoredButton = GameManager.Instance.GetBoardSquares().Where(
                    boardSquare => boardSquare.GetComponent<TickTackToeButtonController>().buttonIndex.Equals(selectedNumber)
                    ).ToList();

                selectedBoardSquare = findFavoredButton[0];
            }
            else
            {
                selectedNumber = Random.Range(0, GameManager.Instance.GetBoardSquares().Count);
                selectedBoardSquare = GameManager.Instance.GetBoardSquares()[selectedNumber];
            }

            //make the move
            selectedBoardSquare.GetComponent<TickTackToeButtonController>().onClickSquare(false);

            //Computer AI ends here
            return;
        }

        //if the computer is the one that made the move do the following
        this.ConductMove(GameManager.Instance.playerO, GameManager.Instance.playerX, 1);
        if (GameManager.Instance.checkForWinningState().Item1)
            return;
    }

    /// <summary>
    /// Conducts the selected move made by both the player and the computer
    /// </summary>
    /// <param name="player"> the currently active player </param>
    /// <param name="opposingPlayer"> the player opposing the active player </param>
    /// <param name="playerID"> the Identification Number integer representation of the player </param>
    public void ConductMove(int [] player, int [] opposingPlayer, int playerID)
    {
        player[buttonIndex] = 1;
        opposingPlayer[buttonIndex] = 2;
        GameManager.Instance.GetBoardSquares().Remove(this.gameObject);
        this.gameObject.GetComponent<Button>().interactable = false;
        DisplayMove(playerID);
    }


    /// <summary>
    /// goes through the children game objects in this parent game object to display the appropriate move back to the user
    /// </summary>
    /// <param name="player">the player that made the move</param>
    public void DisplayMove(int player)
    {
        foreach (Transform child in this.gameObject.transform)
        {
            DetermainXOrOToDisplay(player, child);
        }
    }

    /// <summary>
    /// Determines weather or not display an X or an O back to the user
    /// </summary>
    /// <param name="player">the player that made the move</param>
    /// <param name="child">the associated button's child object transform</param>
    private static void DetermainXOrOToDisplay(int player, Transform child)
    {
        if (player.Equals(0))
        {
            ToggleGameObject(child.gameObject, "Image_X");
            return;
        }

        ToggleGameObject(child.gameObject, "Image_O");
        return;
    }

    /// <summary>
    /// Toggles the game object on or off depending on if it matches the associated name
    /// </summary>
    /// <param name="child"></param>
    /// <param name="name"></param>
    private static void ToggleGameObject(GameObject child, string name)
    {
        if (child.name.Equals(name))
        {
            child.SetActive(true);
            return;
        }
        child.SetActive(false);
    }
}
