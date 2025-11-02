using System.Collections;
using UnityEngine;

public class PlayerTransformController : MonoBehaviour
{
    [Header("Tag States")]
    [SerializeField] private string humanTag = "Human";
    [SerializeField] private string werewolfTag = "Werewolf";
    [SerializeField] private string transitionTag = "Transition";

    [Header("Timer References")]
    [SerializeField] private Timer humanTimer;
    [SerializeField] private Timer werewolfTimer;

    [Header("Components")]
    [SerializeField] private Collider playerCollider;

    [Header("Camera Animation")]
    [SerializeField] private Animator cameraAnimator; // 🎥 Camera Animator
    [SerializeField] private string raiseUpTrigger = "RaiseUp";       // trigger name in Animator
    [SerializeField] private string backNormalTrigger = "BackNormal"; // trigger name in Animator

    private string currentForm = "Human";
    private bool isTransitioning = false;

    void Start()
    {
        // Start as human
        currentForm = "Human";
        gameObject.tag = humanTag;
        LogTagState();

        // Subscribe to timer events
        humanTimer.OnMessageTime += () => StartTransition(humanTimer);
werewolfTimer.OnMessageTime += () => StartTransition(werewolfTimer);


        humanTimer.OnTimerEnd += () => EndTransition(werewolfTag);
        werewolfTimer.OnTimerEnd += () => EndTransition(humanTag);
    }

    private void StartTransition(Timer sourceTimer)
{
    if (isTransitioning) return;
    isTransitioning = true;

    gameObject.tag = transitionTag;
    currentForm = "Transforming";
    LogTagState();

    if (sourceTimer == humanTimer)
    {
        // Human → Werewolf
        cameraAnimator.SetTrigger(raiseUpTrigger);
        Debug.Log("[TRANSFORM] Human to Werewolf transition started.");
    }
    else if (sourceTimer == werewolfTimer)
    {
        // Werewolf → Human
        cameraAnimator.SetTrigger(backNormalTrigger);
        Debug.Log("[TRANSFORM] Werewolf to Human transition started.");
    }
}


    private void EndTransition(string newTag)
    {
        isTransitioning = false;
        gameObject.tag = newTag;
        currentForm = newTag == werewolfTag ? "Werewolf" : "Human";
        LogTagState();
    }

    private void LogTagState()
    {
        Debug.Log($"[TRANSFORM STATE] → Current Tag: {gameObject.tag} | Current Form: {currentForm}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentForm == "Werewolf" && other.CompareTag("HumanNPC"))
        {
            Debug.Log("[ACTION] Devoured a human!");
            other.gameObject.SetActive(false);
            StartCoroutine(DelayedCheck());
        }
    }

    private IEnumerator DelayedCheck()
    {
        yield return null; // wait one frame
        GameManager.Instance.CheckWinCondition();
    }

}
