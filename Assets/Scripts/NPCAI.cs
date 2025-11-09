using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private Transform eyes;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float calmDelay = 3f;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Vector3 startPos;

    private bool isPanicking = false;
    private bool playerInSight = false;
    private Coroutine calmRoutine;
    private Coroutine returnToWalkRoutine;

    // 🔹 Remember last seen player tag
    private string lastSeenTag = null;

    private bool IsGameOver => GameManager.Instance.isGameOver;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPos = transform.position;
        Wander();
    }

    void Update()
    {
        if (player == null) return;

        // Detect the player
        bool canSee = CanSeePlayer(out string playerTag);

        // --- When the player enters vision ---
        if (canSee && !playerInSight)
        {
            playerInSight = true;
            lastSeenTag = playerTag; // 🔹 store last seen tag
            OnPlayerSpotted(playerTag);
        }

        // --- When the player leaves vision ---
        else if (!canSee && playerInSight)
        {
            playerInSight = false;
            lastSeenTag = null; // 🔹 forget last seen tag
            OnPlayerLost();
        }

        // --- Continue wandering only when calm ---
        if (!agent.pathPending && agent.remainingDistance < 0.2f && !isPanicking && !playerInSight)
        {
            Wander();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (eyes == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyes.position, sightRange);

        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfView / 2f, 0) * eyes.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfView / 2f, 0) * eyes.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eyes.position, leftLimit * sightRange);
        Gizmos.DrawRay(eyes.position, rightLimit * sightRange);
    }

    bool CanSeePlayer(out string tag)
    {
        tag = null;

        Vector3 dirToPlayer = player.transform.position - eyes.position;
        float angle = Vector3.Angle(eyes.forward, dirToPlayer);

        if (dirToPlayer.magnitude <= sightRange && angle < fieldOfView / 2f)
        {
            if (Physics.Raycast(eyes.position, dirToPlayer.normalized, out RaycastHit hit, sightRange))
            {
                tag = hit.collider.tag;
                if (hit.collider.CompareTag("Human") || hit.collider.CompareTag("Werewolf") || hit.collider.CompareTag("Transition"))
                    return true;
            }
        }
        return false;
    }

    void OnPlayerSpotted(string playerTag)
    {
        // Stop moving immediately and play standing animation
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetBool("isWalking", false);
        animator.SetBool("isStanding", true);

        if (playerTag == "Werewolf")
        {
            ReactToWerewolf();
        }
        else if (playerTag == "Transition")
        {
            // 🔹 Only trigger game over if still in sight
            StartCoroutine(TriggerGameOverAfterDelay());
        }
    }

    IEnumerator TriggerGameOverAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // small delay

        // 🔹 Only trigger if still in sight and tag is Transition
        if (playerInSight && lastSeenTag == "Transition" && !IsGameOver)
        {
            StopAllActions();
            GameManager.Instance.GameOver();
            Debug.Log("[ALERT] Human spotted the player in Transition form! Game Over triggered.");
        }
    }

    void OnPlayerLost()
    {
        // Wait a short delay before returning to walking
        if (returnToWalkRoutine != null)
            StopCoroutine(returnToWalkRoutine);
        returnToWalkRoutine = StartCoroutine(ReturnToWalkingAfterDelay());
    }

    IEnumerator ReturnToWalkingAfterDelay()
    {
        yield return new WaitForSeconds(1.5f); // delay before walking again

        if (IsGameOver) yield break;

        if (!isPanicking && !playerInSight)
        {
            agent.isStopped = false;
            agent.ResetPath();

            animator.SetBool("isStanding", false);
            animator.SetBool("isWalking", true);

            Debug.Log("[NPC] Player lost from sight — resuming normal behavior.");
            Wander();
        }
    }

    void ReactToWerewolf()
    {
        if (isPanicking || IsGameOver) return;
        isPanicking = true;

        agent.isStopped = false;
        agent.speed = runSpeed;

        animator.SetBool("isStanding", false);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isRunning");

        Vector3 fleeDir = (transform.position - player.transform.position).normalized;
        Vector3 fleePos = transform.position + fleeDir * 10f;
        agent.SetDestination(fleePos);

        if (calmRoutine != null) StopCoroutine(calmRoutine);
        calmRoutine = StartCoroutine(CalmDown());
    }

    IEnumerator CalmDown()
    {
        yield return new WaitForSeconds(calmDelay);
        isPanicking = false;
        animator.ResetTrigger("isRunning");
        animator.SetBool("isWalking", true);
        Wander();
    }

    void Wander()
    {
        if (isPanicking || IsGameOver) return;

        agent.speed = walkSpeed;
        agent.isStopped = false;

        animator.SetBool("isStanding", false);
        animator.SetBool("isWalking", true);

        float rangeX = 8.5f;
        float rangeZ = 13f;

        Vector3 randomPos = startPos + new Vector3(
            Random.Range(-rangeX, rangeX),
            0,
            Random.Range(-rangeZ, rangeZ)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void StopAllActions()
    {
        if (calmRoutine != null)
            StopCoroutine(calmRoutine);
        calmRoutine = null;

        Debug.Log("[GAME OVER] Human NPC stopping all actions.");
        isPanicking = false;
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetBool("isWalking", false);
        animator.SetBool("isStanding", true);
    }
}
