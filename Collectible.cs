using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static int score = 0; // Keep it public static to access globally
    public GameObject[] collectibles;

    private void OnTriggerEnter(Collider other)
    {
        // When the object collides with the collectible, increment score and destroy the collectible
        if (other.CompareTag("Collectible"))
        {
            IncrementScore();
            Destroy(other.gameObject);
        }
    }

    public void IncrementScore()
    {
        score++;
        Debug.Log("The score incremented: " + score);
    }
}
