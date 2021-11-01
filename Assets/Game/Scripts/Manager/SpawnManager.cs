using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private LevelManager lm;
    [SerializeField] private Enemy[] enemyPrefabs;

    [SerializeField] private Transform[] enemyPaths;
    [SerializeField] private float spawnDelay;

    public List<Enemy> spawnedEnemies = new List<Enemy>();

    private float runningSpawnDelay;

    public List<Enemy> SpawnedEnemies { get => spawnedEnemies; set => spawnedEnemies = value; }

    private void Awake()
    {
        lm = LevelManager.Instance;
        Instance = this;
    }

    private void Update()
    {
        if (PlayerData.IS_OVER)
        {
            return;
        }

        runningSpawnDelay -= Time.unscaledDeltaTime;
        if (runningSpawnDelay <= 0f)
        {
            SpawnEnemy();
            runningSpawnDelay = spawnDelay;
        }

        foreach (Enemy enemy in SpawnedEnemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }

            if (Vector2.Distance(enemy.transform.position, enemy.TargetPosition) < 0.1f)
            {
                enemy.SetCurrentPathIndex(enemy.CurrentPathIndex + 1);
                if (enemy.CurrentPathIndex < enemyPaths.Length)
                {
                    enemy.SetTargetPosition(enemyPaths[enemy.CurrentPathIndex].position);
                }
                else
                {
                    lm.ReduceLives(1);
                    enemy.gameObject.SetActive(false);
                }
            }
            else
            {
                enemy.MoveToTarget();
            }
        }

    }

    private void SpawnEnemy()
    {
        lm.SetTotalEnemy(--lm.EnemyCounter);

        if (lm.EnemyCounter < 0)
        {
            bool isAllEnemyDestroy = spawnedEnemies.Find(e => e.gameObject.activeSelf) == null;
            if (isAllEnemyDestroy)
            {
                lm.SetGameOver(true);
            }
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        string enemyIndexString = (randomIndex + 1).ToString();

        var newEnemyObj = SpawnedEnemies.Find(e => !e.gameObject.activeSelf && e.name.Contains(enemyIndexString))?.gameObject;

        if (newEnemyObj == null)
        {
            newEnemyObj = Instantiate(enemyPrefabs[randomIndex].gameObject);
        }

        var newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (!SpawnedEnemies.Contains(newEnemy))
        {
            SpawnedEnemies.Add(newEnemy);
        }

        newEnemy.transform.position = enemyPaths[0].position;
        newEnemy.SetTargetPosition(enemyPaths[1].position);
        newEnemy.SetCurrentPathIndex(1);
        newEnemy.gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < enemyPaths.Length - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(enemyPaths[i].position, enemyPaths[i + 1].position);
        }
    }

}
