using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    private static LevelManager instance = null;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }

            return instance;
        }
    }

    [SerializeField] private Transform towerUIParent;
    [SerializeField] private GameObject towerUIPrefab;

    [SerializeField] private Tower[] towerPrefabs;

    [SerializeField] private int maxLives = 3;
    [SerializeField] private int totalEnemy = 15;

    private int currentLives;
    private int enemyCounter;

    private List<Tower> spawnedTowers = new List<Tower>();

    private List<Bullet> _spawnedBullets = new List<Bullet>();

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Text statusInfo;
    [SerializeField] private Text livesInfo;
    [SerializeField] private Text totalEnemyInfo;

    public int EnemyCounter { get => enemyCounter;  set => enemyCounter = value; }
    public int CurrentLives { get => currentLives; private set => currentLives = value; }

    private void Start()
    {
        SetCurrentLives(maxLives);
        SetTotalEnemy(totalEnemy);

        InstantiateAllTowerUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && PlayerData.IS_OVER)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && PlayerData.IS_OVER)
        {
            Application.Quit();
        }

        if (PlayerData.IS_OVER)
        {
            return;
        }

        foreach (Tower tower in spawnedTowers)
        {
            tower.CheckNearestEnemy(SpawnManager.Instance.SpawnedEnemies);
            tower.SeekTarget();
            tower.ShootTarget();
        }
    }

    public void ReduceLives(int value)
    {
        SetCurrentLives(currentLives - value);
        if (currentLives <= 0)
        {
            SetGameOver(false);
        }
    }

    public void SetCurrentLives(int _currentLives)
    {
        currentLives = Mathf.Max(_currentLives, 0);
        livesInfo.text = $"Lives: {currentLives}";
    }

    public void SetTotalEnemy(int totalEnemy)
    {
        enemyCounter = totalEnemy;
        totalEnemyInfo.text = $"Total Enemy: {Mathf.Max(enemyCounter, 0)}";
    }

    public void SetGameOver(bool isWin)
    {
        PlayerData.IS_OVER = true;

        statusInfo.text = isWin ? "You Win!" : "You Lose!";
        panel.gameObject.SetActive(true);
    }

    private void InstantiateAllTowerUI()
    {
        foreach (var tower in towerPrefabs)
        {
            var newTowerUIObj = Instantiate(towerUIPrefab.gameObject, towerUIParent);
            var newTowerUI = newTowerUIObj.GetComponent<TowerUI>();

            newTowerUI.SetTowerPrefab(tower);

            newTowerUI.transform.name = tower.name;
        }
    }

    public Bullet GetBulletFromPool(Bullet prefab)
    {
        GameObject newBulletObj = _spawnedBullets.Find(
            b => !b.gameObject.activeSelf && b.name.Contains(prefab.name)
        )?.gameObject;

        if (newBulletObj == null)
        {
            newBulletObj = Instantiate(prefab.gameObject);
        }

        Bullet newBullet = newBulletObj.GetComponent<Bullet>();
        if (!_spawnedBullets.Contains(newBullet))
        {
            _spawnedBullets.Add(newBullet);
        }

        return newBullet;
    }

    public void ExplodeAt(Vector2 point, float radius, int damage)
    {
        foreach (Enemy enemy in SpawnManager.Instance.SpawnedEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                if (Vector2.Distance(enemy.transform.position, point) <= radius)
                {
                    enemy.ReduceEnemyHealth(damage);
                }
            }
        }
    }

    public void RegisterSpawnedTower(Tower tower)
    {
        spawnedTowers.Add(tower);
    }

}
