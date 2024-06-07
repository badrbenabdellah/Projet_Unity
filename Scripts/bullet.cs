using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Update()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si le Collider avec lequel nous sommes en collision est celui du joueur
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("enemy detected");
            // Si c'est le cas, détruis ce GameObject (la pièce)
            Destroy(gameObject);
        }
    }
}
