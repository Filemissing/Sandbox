using RobotGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void StartLevel(GameObject enemyPrefab)
    {
        GameManager.instance.selectedEnemyPrefab = enemyPrefab;
        SceneManager.LoadScene("Assembly Scene");
    }
}
