using UnityEngine;
using TMPro;

public class NPCCounter : MonoBehaviour
{
    public static NPCCounter instance;

    [Header("Counter Settings")]
    public int totalNPCs;
    [SerializeField] private TMP_Text counterText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Count all NPCs tagged "HumanNPC" at start
        totalNPCs = GameObject.FindGameObjectsWithTag("HumanNPC").Length;
        UpdateUI();
    }

    public void NPCeaten()
    {
        totalNPCs--;
        UpdateUI();

        Debug.Log($"[COUNTER] NPCs left: {totalNPCs}");

        if (totalNPCs <= 0)
        {
            Debug.Log("[COUNTER] All humans eaten!");
        }
    }

    private void UpdateUI()
    {
        if (counterText != null)
            counterText.text = $"Humans Left: {totalNPCs}/30";
    }
}
