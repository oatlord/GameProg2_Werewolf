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

    // Time tracking
    [HideInInspector] public float elapsedTime = 0f;
    [HideInInspector] public float bestTime = Mathf.Infinity; // stores best time across runs

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isGameOver)
            elapsedTime += Time.deltaTime;
    }

    // Call this when the werewolf eats a human
    [HideInInspector] public int humansEaten = 0;
    public void HumanEaten()
    {
        humansEaten++;
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

        // Update best time if this run was faster
        if (elapsedTime < bestTime)
            bestTime = elapsedTime;

        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene("Gameover");
    }

    public void CheckWinCondition()
    {
        if (GameObject.FindGameObjectsWithTag("HumanNPC").Length == 0)
        {
            Debug.Log("All humans eaten! You win!");

            // Update best time if this run was faster
            if (elapsedTime < bestTime)
                bestTime = elapsedTime;

            SceneManager.LoadScene("GameWin");
        }
    }

    // Call this when starting a new game
    public void ResetStats()
    {
        humansEaten = 0;
        elapsedTime = 0f;
        isGameOver = false;
    }

    // For UI summary access
    public string GetFormattedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
