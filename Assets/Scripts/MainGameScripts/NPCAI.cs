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
    private Coroutine calmRoutine;

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

        Vector3 dirToPlayer = player.transform.position - eyes.position;
        Debug.DrawRay(eyes.position, dirToPlayer.normalized * sightRange, Color.red);

        if (CanSeePlayer(out string playerTag))
        {
            if (playerTag == "Werewolf")
            {
                ReactToWerewolf();
            }
            else if (playerTag == "Transition")
            {
                StopAllActions();
                GameManager.Instance.GameOver();
                Debug.Log("[ALERT] Human spotted the player in Transition form! Game Over triggered.");
            }
        }

        if (!agent.pathPending && agent.remainingDistance < 0.2f && !isPanicking)
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

    void ReactToWerewolf()
    {
        if (isPanicking || IsGameOver) return;
        isPanicking = true;
        agent.speed = runSpeed;
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
        Wander();
    }

    void Wander()
    {
        if (isPanicking || IsGameOver) return;

        agent.speed = walkSpeed;

        // Half the floor width and length
        float rangeX = 8.5f;
        float rangeZ = 13f;

        // Pick a random point near the start position
        Vector3 randomPos = startPos + new Vector3(
            Random.Range(-rangeX, rangeX),
            0,
            Random.Range(-rangeZ, rangeZ)
        );

        // Snap the random point to the nearest valid NavMesh spot
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void StopAllActions()
    {
        if (calmRoutine != null)
        {
            StopCoroutine(calmRoutine);
        }
        else
        {
            calmRoutine = null;
        }
        Debug.Log("[GAME OVER] Human NPC stopping all actions.");
        isPanicking = false;
        agent.isStopped = true;
        agent.ResetPath();
    }
}
