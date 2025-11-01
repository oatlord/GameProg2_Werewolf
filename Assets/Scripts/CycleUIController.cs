using System.Collections;
using UnityEngine;

public class CycleUIController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject humanUI;
    [SerializeField] private GameObject werewolfUI;

    [Header("Timers")]
    [SerializeField] private Timer humanTimer;
    [SerializeField] private Timer werewolfTimer;

    void Start()
    {
        // 🧍‍♂️ Human starts first
        humanUI.SetActive(true);
        werewolfUI.SetActive(false);

        // Set up event connections
        humanTimer.OnMessageTime += humanTimer.PlayMessageAnimation;
        humanTimer.OnTimerEnd += OnHumanTimerEnd;

        werewolfTimer.OnMessageTime += werewolfTimer.PlayMessageAnimation;
        werewolfTimer.OnTimerEnd += OnWerewolfTimerEnd;

        // Start human first
        humanTimer.StartTimer();
    }

    private void OnHumanTimerEnd()
    {
        StartCoroutine(SwitchToWerewolf());
    }

    private void OnWerewolfTimerEnd()
    {
        StartCoroutine(SwitchToHuman());
    }

    private IEnumerator SwitchToWerewolf()
    {
        yield return new WaitForSeconds(0.5f);
        humanUI.SetActive(false);
        werewolfUI.SetActive(true);
        werewolfTimer.StartTimer();
    }

    private IEnumerator SwitchToHuman()
    {
        yield return new WaitForSeconds(0.5f);
        werewolfUI.SetActive(false);
        humanUI.SetActive(true);
        humanTimer.StartTimer();
    }
}
