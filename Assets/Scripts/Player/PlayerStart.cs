using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStart : MonoBehaviour
{
    [SerializeField]
    private string playerSceneName = "Player";

    private void Start()
    {
        // Check if the scene is already loaded.
        Scene playerScene = SceneManager.GetSceneByName(playerSceneName);

        // If the scene is already loaded, teleport the player character to the start position immediately.
        if (playerScene.isLoaded)
        {
            TeleportCharacterToStartPosition();
            return;
        }

        // Load the player scene before teleporting the character.
        StartCoroutine(LoadPlayerScene());
    }

    private IEnumerator LoadPlayerScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(playerSceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        TeleportCharacterToStartPosition();
    }

    private void TeleportCharacterToStartPosition()
    {
        // Teleport the player character to the start position.
        Debug.Assert(PlayerCharacter.Instance, "The player character doesn't exist!");
        CharacterMovement characterMovement = PlayerCharacter.Instance.GetComponent<CharacterMovement>(); // PlayerCharacter should add a public accessor so that other classes can get access to character movement.
        characterMovement.TeleportTo(transform.position, transform.eulerAngles.z);

        // Destroy this game object. It's no longer useful.
        Destroy(gameObject);
    }
}
