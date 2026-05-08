using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [SerializeField, Tooltip("If true, automatically progresses to next scene in SceneProgressionManager")]
    private bool _useProgression = true;

    [SerializeField, Tooltip("Manual scene name (only used if Use Progression is false)")]
    private string _nextSceneName;

    [Header("Room Clear Requirements")]
    [SerializeField, Tooltip("If true, player must clear the room before transitioning")]
    private bool _requireRoomClear = true;

    [SerializeField, Tooltip("Message shown when trying to transition with enemies remaining")]
    private string _blockedMessage = "Defeat all enemies before proceeding!";

    [Header("Visual Settings")]
    [SerializeField, Tooltip("Color of the trigger zone in the editor (invisible in game)")]
    private Color _gizmoColor = new Color(0f, 1f, 0f, 0.3f);

    private bool _isAlreadyTriggered = false;

    private void OnDrawGizmos()
    {
        // Draw the trigger zone in the editor
        Gizmos.color = _gizmoColor;
        
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
            
            // Draw outline
            Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, 1f);
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }
        else
        {
            // Fallback for other collider types
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isAlreadyTriggered)
            return;

        // Checks if the other object is the player.
        PlayerCharacter playerCharacter = collision.GetComponentInParent<PlayerCharacter>();
        if (!playerCharacter)
            return;

        Debug.Log($"Player entered transition trigger. Require clear: {_requireRoomClear}");

        // Check if room must be cleared first
        if (_requireRoomClear)
        {
            // Check TestCharacter instance count
            int enemyCount = TestCharacter.InstanceCount;
            
            if (enemyCount > 0)
            {
                Debug.Log($"{_blockedMessage} ({enemyCount} enemies remaining)");
                
                // Show splash message to player via UIHandler
                if (UIHandler.instance != null)
                {
                    UIHandler.instance.ShowRoomNotClearedMessage(_blockedMessage);
                }
                else if (SplashMessageUIToolkit.Instance != null)
                {
                    SplashMessageUIToolkit.Instance.ShowMessage(_blockedMessage);
                }
                else if (SplashMessage.Instance != null)
                {
                    SplashMessage.Instance.ShowMessage(_blockedMessage);
                }
                return;
            }
            else
            {
                Debug.Log("Room is cleared! Proceeding with transition.");
            }
        }

        // Determine which scene to load
        string targetScene;
        
        if (_useProgression)
        {
            if (SceneProgressionManager.Instance != null)
            {
                targetScene = SceneProgressionManager.Instance.GetNextScene();
            }
            else
            {
                Debug.LogError("SceneProgressionManager not found! Make sure it exists in your first scene.");
                return;
            }
        }
        else
        {
            targetScene = _nextSceneName;
        }

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("No target scene specified! Check SceneProgressionManager or set a manual scene name.");
            return;
        }

        // Room is cleared (or not required), proceed to next scene
        _isAlreadyTriggered = true;
        Debug.Log($"Transitioning to scene: {targetScene}");
        SceneTransitionManager.Instance.SwitchToScene(targetScene);
    }

    /*
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_nextSceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
    */
}

