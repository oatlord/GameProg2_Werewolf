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

    private string currentForm = "Human";
    private bool isTransitioning = false;

    void Start()
    {
        // Start as human
        currentForm = "Human";
        gameObject.tag = humanTag;
        LogTagState();

        // Subscribe to timer events
        humanTimer.OnMessageTime += StartTransition;
        werewolfTimer.OnMessageTime += StartTransition;

        humanTimer.OnTimerEnd += () => EndTransition(werewolfTag);
        werewolfTimer.OnTimerEnd += () => EndTransition(humanTag);
    }

    private void StartTransition()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        gameObject.tag = transitionTag;
        currentForm = "Transforming";
        LogTagState();
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
            Destroy(other.gameObject);
            GameManager.Instance.CheckWinCondition();
        }
    }
}
