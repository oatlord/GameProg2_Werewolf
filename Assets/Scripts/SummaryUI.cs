using TMPro;
using UnityEngine;

public class GameSummaryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeTakenText;
    [SerializeField] private TMP_Text bestTimeText;

    private void Start()
    {
        GameManager gm = GameManager.Instance;

        timeTakenText.text = $"Time Taken: {gm.GetFormattedTime(gm.elapsedTime)}";
        bestTimeText.text = $"Best Time: {gm.GetFormattedTime(gm.bestTime)}";
    }
}
