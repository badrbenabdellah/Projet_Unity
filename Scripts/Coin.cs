using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector2 SpawnPosition;
    GameObject coinPrefab;

    public void Initialize(Vector2 SpawnPosition, GameObject coinPrefab)
    {

        this.SpawnPosition = SpawnPosition;
        this.coinPrefab = coinPrefab;
    }

    public void SpawnCoin()
    {
        Instantiate(coinPrefab, SpawnPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si le Collider avec lequel nous sommes en collision est celui du joueur
        if (other.CompareTag("Player"))
        {
            Debug.Log("player detected");
            // Si c'est le cas, détruis ce GameObject (la pièce)
            Destroy(gameObject);
        }
    }

}
