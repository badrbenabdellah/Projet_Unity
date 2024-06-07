using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    // Cette méthode est appelée lorsqu'un autre Collider entre en collision avec le Collider de ce GameObject
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si le Collider avec lequel nous sommes en collision est celui du GameObject "coin"
        if (other.CompareTag("Coin"))
        {
            Debug.Log("coin detected");
            // Si c'est le cas, détruis le GameObject "coin"
            Destroy(other.gameObject);
        }
    }
}
