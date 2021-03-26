using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TickTackToeButtonController : MonoBehaviour
{
    public int buttonIndex = 0;
    private AudioSource audioSource = null;

    private const string IMAGE_X = "Image_X";
    private const string IMAGE_O = "Image_O";

    //un-comment lines 17-19 to see how unity tries to handle a constructor in a monobehavior.
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
    public void OnClickSquare(bool isHumanPlayer)
    {
        GameManager.Instance.moveCount++;

        if (isHumanPlayer)
        {
            audioSource.PlayOneShot(audioSource.clip); //play the selection choice sound if the human made the move

            this.ConductMove(GameManager.Instance.playerX, GameManager.Instance.playerO, 0);

            //IF there was a winning state, end the game
            if (GameManager.Instance.CheckForWinningState().Item1)
                return;

            //NO MORE MOVES
            if (GameManager.Instance.GetBoardSquares().Count.Equals(0))
            {
                GameManager.Instance.DisableAllButtons();
                return;
            }

            //let the computer make its move, then initiate the onclick event.
            ChooseComputerSquare().GetComponent<TickTackToeButtonController>().OnClickSquare(false);
            return;
        }

        //if the computer is the one that made the move do the following
        this.ConductMove(GameManager.Instance.playerO, GameManager.Instance.playerX, 1);
        if (GameManager.Instance.CheckForWinningState().Item1)
            return;
    }

    /// <summary>
    /// let the computer make its move
    /// </summary>
    /// <returns>the game object reflecting the computers choice</returns>
    private GameObject ChooseComputerSquare() {

        if (GameManager.Instance.moveCount.Equals(1) && GameManager.Instance.favoredSquareButton.IsInteractable())
            return GameManager.Instance.favoredSquareButton.gameObject;

        if (GameManager.Instance.moveCount.Equals(1))
        {
            int[] chooseOne = { 0, 2, 6, 8 }; //strategically better to choose a corner peice if the favored middle is taken by the user.
            System.Random random = new System.Random();
            int choice = chooseOne[random.Next(0, chooseOne.Length)]; //randomly selects a number between 0 inclusive and 4 exclusive, uses that as the index to choose a corner peice
            return ChooseBoardSquare(choice);
        }

        if (GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerO).Item2)
            return ChooseBoardSquare(GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerO).Item1);

        if (GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerX).Item2)
            return ChooseBoardSquare(GameManager.Instance.DoesWinningMoveExist(GameManager.Instance.playerX).Item1);

        return GameManager.Instance.GetBoardSquares()[Random.Range(0, GameManager.Instance.GetBoardSquares().Count)];

    }

    /// <summary>
    /// the computer chooses a board square based off the selected number
    /// </summary>
    /// <param name="selectedNumber">the index representation of the selected button</param>
    /// <returns>the gameobject representing the computers chosen board square</returns>
    private static GameObject ChooseBoardSquare(int selectedNumber)
    {
        return GameManager.Instance.GetBoardSquares().Where(
            boardSquare => boardSquare.GetComponent<TickTackToeButtonController>().buttonIndex.Equals(selectedNumber)
            ).FirstOrDefault();
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
            DetermainXOrOToDisplay(player, child);
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
            ToggleGameObject(child.gameObject, IMAGE_X);
            return;
        }

        ToggleGameObject(child.gameObject, IMAGE_O);
        return;
    }

    /// <summary>
    /// Toggles the game object on or off depending on if it matches the associated name
    /// </summary>
    /// <param name="child"> the child game object </param>
    /// <param name="playerIdentifier">the name of the game object owned by the player</param>
    private static void ToggleGameObject(GameObject child, string playerIdentifier)
    {
        child.SetActive(child.name.Equals(playerIdentifier));
    }
}
