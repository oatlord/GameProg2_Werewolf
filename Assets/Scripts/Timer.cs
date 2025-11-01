using System.Collections;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startingTime = 30f;

    [Header("Message Animation")]
    [SerializeField] private Animator messageAnim;
    [SerializeField] private string messageAnimName = "ShowMessage";
    [SerializeField] private float messageAnimDuration = 1.5f;

    private float remainingTime;
    private bool messageTriggered = false;
    private bool isRunning = false;

    // Events for controller
    public System.Action OnTimerEnd;
    public System.Action OnMessageTime;

    void Start()
    {
        // make sure message starts disabled
        if (messageAnim != null)
            messageAnim.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        if (isRunning)
        {
        remainingTime -= Time.deltaTime;

            // Clamp to 0 so it never goes negative
        remainingTime = Mathf.Max(remainingTime, 0f);

            // Trigger message at 5s left
        if (!messageTriggered && remainingTime <= 5f)
            {
                messageTriggered = true;
                OnMessageTime?.Invoke();
            }

            // Update timer text safely
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            // Stop and trigger when time runs out
            if (remainingTime <= 0f)
            {
                isRunning = false;
                OnTimerEnd?.Invoke();
            }
        }

    }

    public void StartTimer()
    {
        remainingTime = startingTime;
        messageTriggered = false;
        isRunning = true;

        // hide message at start
        if (messageAnim != null)
            messageAnim.gameObject.SetActive(false);
    }

    public void PlayMessageAnimation()
    {
        if (messageAnim != null)
        {
            messageAnim.gameObject.SetActive(true);
            messageAnim.Play(messageAnimName);
            StartCoroutine(HideMessageAfterAnim());
        }
    }

    private IEnumerator HideMessageAfterAnim()
    {
        yield return new WaitForSeconds(messageAnimDuration);
        if (messageAnim != null)
            messageAnim.gameObject.SetActive(false);
    }
}
