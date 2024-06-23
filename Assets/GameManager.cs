using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> floorEnemies;
    public List<GameObject> skyEmemies;
    public List<GameObject> spaceEnemies;

    public List<ItemDropChance> items;
    public int enemySpawnRate;
    public int itemSpawnRate;
    [SerializeField] GameObject player;

    [Header("Flag")]
    public bool isInFloorStage;
    public bool isInSkyStage;
    public bool isInSpaceStage;

    ScreenPositions screenPositions;

    private void Awake()
    {
        screenPositions = GameObject.Find("Main Camera").GetComponent<ScreenPositions>();
    }
    private void Start()
    {
        SpawnEnemy();
        SpawnItems();
    }
    private void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3();

        spawnPosition.y = screenPositions.topSide;
        int spawnDirection = Random.Range(0, 2);
        if (spawnDirection == 0)
        {
            //Spawn Left Side
            spawnPosition.x = screenPositions.leftSide;
        }
        else
        {
            //Spawn Right Side
            spawnPosition.x = screenPositions.rightSide;
        }
        GameObject enemy = null;
        if (isInFloorStage)
        {
            var RandomEnemy = Random.Range(0, floorEnemies.Count);
            enemy = Instantiate(floorEnemies[RandomEnemy], spawnPosition, Quaternion.identity);
        }
        else if (isInSkyStage)
        {
            var RandomEnemy = Random.Range(0, skyEmemies.Count);
            enemy = Instantiate(skyEmemies[RandomEnemy], spawnPosition, Quaternion.identity);
        }
        else if (isInSpaceStage)
        {
            var RandomEnemy = Random.Range(0, spaceEnemies.Count);
            enemy = Instantiate(spaceEnemies[RandomEnemy], spawnPosition, Quaternion.identity);
        }
        Debug.Log(enemy);
        if (enemy != null)
        {
            Hazards enemyManager = null;
            if (enemy.transform.parent != null)
            {
                enemyManager = enemy.GetComponentInChildren<Hazards>();
            }
            else
            {
                enemyManager = enemy.GetComponent<Hazards>();
            }

            Debug.Log(enemyManager.gameObject);
            if (enemyManager.usingDropPoint)
            {
                enemy.transform.position = new Vector3(Random.Range(screenPositions.leftSide, screenPositions.rightSide), enemy.transform.position.y, enemy.transform.position.z);
            }
            else if (enemyManager.goingBetweenTwoPoints)
            {
                if (spawnDirection == 0)
                {
                    //Spawn Left Side
                    enemyManager.pointA.transform.position = enemy.transform.position;
                    enemyManager.pointB.transform.position = new Vector3(enemyManager.pointA.transform.position.x + (Random.Range(10, 30)), enemyManager.pointA.transform.position.y, enemyManager.pointA.transform.position.z);

                }
                else
                {
                    enemyManager.pointB.transform.position = enemy.transform.position;
                    enemyManager.pointA.transform.position = new Vector3(enemyManager.pointB.transform.position.x - (Random.Range(10, 30)), enemyManager.pointB.transform.position.y, enemyManager.pointB.transform.position.z);

                }
            }
            else if (enemyManager.usingGlide)
            {
                var chooseDirection = Random.Range(0, 2);
                if (spawnDirection == 0)
                {
                    //Spawn Left Side
                    enemyManager.goLeft = false;
                    enemyManager.GoRight = true;
                    enemy.transform.localScale = new Vector3(-1, enemy.transform.localScale.y, enemy.transform.localScale.z);
                }
                else
                {
                    //Spawn Right Side
                    enemyManager.goLeft = true;
                    enemyManager.GoRight = false;
                    enemy.transform.localScale = new Vector3(1, enemy.transform.localScale.y, enemy.transform.localScale.z);
                }

                if (chooseDirection == 0)
                {
                    //go Down
                    enemyManager.glideUp = false;
                    enemyManager.glideDown = true;
                }
                else
                {
                    //Go Up
                    enemyManager.glideUp = false;
                    enemyManager.glideDown = true;
                }
            }
            else if (enemyManager.goingDirection)
            {
                if (spawnDirection == 0)
                {
                    //Spawn Left Side
                    enemyManager.goLeft = false;
                    enemyManager.GoRight = true;
                    enemy.transform.localScale = new Vector3(-1, enemy.transform.localScale.y, enemy.transform.localScale.z);
                }
                else
                {
                    //Spawn Right Side
                    enemyManager.goLeft = true;
                    enemyManager.GoRight = false;
                    enemy.transform.localScale = new Vector3(1, enemy.transform.localScale.y, enemy.transform.localScale.z);
                }
            }
        }

        Invoke("SpawnEnemy", enemySpawnRate);
    }
    private void SpawnItems()
    {
        Vector3 spawnPosition = new Vector3();

        spawnPosition.y = screenPositions.topSide;
        spawnPosition.x = Random.Range(screenPositions.leftSide, screenPositions.rightSide);
        var RandomItem = Random.Range(0, items.Count);


        foreach (ItemDropChance item in items)
        {
            if (ShouldDropItem(item))
            {
                var spawnedItem = Instantiate(items[RandomItem].item, spawnPosition, Quaternion.identity);
            }
        }
        Invoke("SpawnItems", itemSpawnRate);
    }
    bool ShouldDropItem(ItemDropChance item)
    {
        var ChanceForItemToSpawn = Random.Range(0, 100);
        return ChanceForItemToSpawn <= item.dropChanceOutOf100;
    }

    [System.Serializable]
    public class ItemDropChance
    {
        public GameObject item;
        public int dropChanceOutOf100;
        public ItemDropChance(GameObject _item, int _dropChance)
        {
            item = _item;
            dropChanceOutOf100 = _dropChance;
        }
    }
}
