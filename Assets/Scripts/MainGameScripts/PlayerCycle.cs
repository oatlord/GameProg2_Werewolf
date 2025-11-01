using System.Collections;
using UnityEngine;

public class PlayerCycle : MonoBehaviour
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

        // Listen to message time (5s left)
        humanTimer.OnMessageTime += StartTransition;
        werewolfTimer.OnMessageTime += StartTransition;

        // Listen to timer end (switch form)
        humanTimer.OnTimerEnd += () => EndTransition(werewolfTag);
        werewolfTimer.OnTimerEnd += () => EndTransition(humanTag);
    }

    private void StartTransition()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        gameObject.tag = transitionTag;
        Debug.Log("Transforming...");
    }

    private void EndTransition(string newTag)
    {
        isTransitioning = false;
        gameObject.tag = newTag;
        currentForm = newTag == werewolfTag ? "Werewolf" : "Human";
        Debug.Log("Now in form: " + currentForm);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentForm == "Werewolf" && other.CompareTag("HumanNPC"))
        {
            // Eat the human
            Destroy(other.gameObject);
            GameManager.Instance.CheckWinCondition();
        }
    }
}
