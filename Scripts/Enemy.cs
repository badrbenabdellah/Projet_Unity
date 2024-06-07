using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2 SpawnPosition;
    GameObject enemyPrefab;

    public void Initialize(Vector2 SpawnPosition, GameObject enemyPrefab) {
   
        this.SpawnPosition = SpawnPosition;
        this.enemyPrefab = enemyPrefab;
    }

    public void SpawnEnemy()
    {
        Instantiate(enemyPrefab, SpawnPosition, Quaternion.identity);
    }
}
