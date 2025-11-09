using UnityEngine;

public class Audio : MonoBehaviour
{
    private static Audio instance;

    void Awake()
    {
        // If there's already an AudioManager, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Keep this one and make it persist between scenes
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
