using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 SpawnPosition;
    GameObject playerPrefab;
    private GameObject playerInstance;

    public void Initialize(Vector2 SpawnPosition, GameObject playerPrefab)
    {
        this.SpawnPosition = SpawnPosition;
        this.playerPrefab = playerPrefab;
    }

    public void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab, SpawnPosition, Quaternion.identity);
    }

    public Vector2 GetPlayerPosition()
    {
        if (playerInstance != null)
        {
            //return playerInstance.transform.position;
            return new Vector2(playerInstance.transform.position.x, playerInstance.transform.position.y);
        }
        else
        {
            Debug.LogWarning("Le joueur n'est pas encore apparu dans le monde.");
            return Vector2.zero;
        }
    }

    public void MoveTo(Vector2 newPosition)
    {
        // Déplacement du joueur à la nouvelle position
        if (playerInstance != null)
        {
            playerInstance.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Le joueur n'est pas encore apparu dans le monde.");
        }
    }
}
