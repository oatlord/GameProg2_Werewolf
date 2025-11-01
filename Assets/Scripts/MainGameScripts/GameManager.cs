using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER! You were caught transforming!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheckWinCondition()
    {
        if (GameObject.FindGameObjectsWithTag("NPC").Length == 0)
        {
            Debug.Log("All humans eaten! You win!");
            // Trigger win screen or load next level
        }
    }
}
