using UnityEngine;
using TMPro;

public class ScoreBoardController : MonoBehaviour
{
    [Tooltip("Text Mesh Pro reference for human player score")]
    public TextMeshProUGUI playerXScore;
    [Tooltip("Text Mesh Pro reference for computer score")]
    public TextMeshProUGUI playerOScroe;

    public void Awake()
    {
        this.playerXScore.text = "0";
        this.playerOScroe.text = "0";
    }

    /// <summary>
    /// Human player score update function
    /// </summary>
    public void updatePlayerXScore()
    {
        this.playerXScore.text = GameManager.Instance.getPlayerXPoints().ToString();
    }

    /// <summary>
    /// Computer score update function
    /// </summary>
    public void updatePlayerOScore()
    {
        this.playerOScroe.text = GameManager.Instance.getPlayerOPoints().ToString();
    }
}
