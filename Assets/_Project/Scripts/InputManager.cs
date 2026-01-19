using UnityEngine;

/// <summary>
/// Manages user input through a singleton pattern.
/// </summary>
public class InputManager : MonoBehaviour
{
    // --- Singleton Setup ---
    // Private static instance
    private static InputManager instance;
    // Public static instance for global access from other scripts
    public static InputManager Instance => instance;

    void Awake()
    {
        // --- Initialize Singleton and Prevent Duplicates ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject); return;
        }
    }
}
