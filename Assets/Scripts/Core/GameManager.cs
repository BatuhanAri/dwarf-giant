using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameOver = false;

    public void OnPlayerCaught()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Game Over! The Giant caught the Dwarf.");
        
        // Simple UI feedback could be added here
        
        // Reload scene after a short delay
        StartCoroutine(RestartRoutine());
    }

    public void OnPlayerEscaped()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Victory! The Dwarf escaped the house.");
        
        // Reload scene after a short delay
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
