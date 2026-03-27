using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        gameManager.OnPlayerWin();
    }
}
