using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Tooltip("Seconds to wait before showing the Game Over scene")]
    public float gameOverDelay = 2f;

    // Prevent triggering GameOver multiple times
    public bool isGameOver { get; set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        Debug.Log("GAME OVER! You were caught transforming!");
        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene("Gameover");
    }

    public void CheckWinCondition()
    {
        if (GameObject.FindGameObjectsWithTag("HumanNPC").Length == 0)
        {
            Debug.Log("All humans eaten! You win!");
            SceneManager.LoadScene("GameWin");
            // Trigger win screen or load next level
        }
    }
}
