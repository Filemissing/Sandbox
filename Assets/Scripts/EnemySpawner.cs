using UnityEngine;

namespace RobotGame
{
    public class EnemySpawner : MonoBehaviour
    {
        void Start()
        {
            Instantiate(GameManager.instance.selectedEnemyPrefab, transform.position, Quaternion.identity);
        }
    }
}