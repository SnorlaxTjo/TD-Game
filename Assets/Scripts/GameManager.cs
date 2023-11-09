using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
/*
THE MASTER BECOMES 
A HUMAN FURNACE
THAT MAY SMITE 
AND PUNISH GOD
*/
[System.Serializable]
public class PlayerData
{
    [Header("Static Values")]
    public int PlayerMaxHealth = 10;
    [Header("Dynamic Values")]
    public int PlayerHealth = 10;
    public int PlayerMoney = 0;

    public void Reset()
    {
        PlayerHealth = PlayerMaxHealth;
        PlayerMoney = 0;
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager GlobalGameManager = null;

    [Header("Spawnable Objects")]
    public List<GameObject> SpawnableObjects = new List<GameObject>();
    public Dictionary<string, GameObject> SpawnableObjectsMap = new Dictionary<string, GameObject>();
    
    [Header("Enemies & Tracking")]
    public List<EnemyWaveContainer> EnemyWavesInLevel = new List<EnemyWaveContainer>();
    public int CurrentEnemyWave = 0;
    public Transform EnemyStartingPos = null;
    public int amountOfEnemiesAlive;
    public float timeToWaitUntilWaveSpawn;
    public string nextWaveCountdownText;
    [Header("Static References")]
        public PlayerData CurrentPlayerData = null;
        public TextMeshProUGUI PlayerHealthText = null;
        public TextMeshProUGUI PlayerMoneyText = null;
        public TextMeshProUGUI WaveNumberText = null;
        public TextMeshProUGUI nextWaveCountdownTextObject;
   [Header("Dynamic References")]
    public List<EnemyBase> AllEnemies = new List<EnemyBase>();

    float timeLeftUntilNextWaveSpawn;
    bool wantsToSpawnNewWave;


    private void Awake()
    {
        GlobalGameManager = this;
        foreach (GameObject spawnable in SpawnableObjects)
        {
            var spawnableGO = GameObject.Instantiate(spawnable, this.gameObject.transform);
            spawnableGO.SetActive(false);
            SpawnableObjectsMap.Add(spawnable.name, spawnableGO);
        }
        CurrentPlayerData.Reset();
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        OnSpawnNextWave();

    }
    private void Update()
    {
        PlayerMoneyText.text = "$" + CurrentPlayerData.PlayerMoney.ToString();

        if (wantsToSpawnNewWave) { SpawnNextWaveCountdown(); }
    }
    public void OnSpawnNextWave()
    {
        if (CurrentEnemyWave >= EnemyWavesInLevel.Count) { return; }

        timeLeftUntilNextWaveSpawn = timeToWaitUntilWaveSpawn;
        nextWaveCountdownTextObject.gameObject.SetActive(true);
        wantsToSpawnNewWave = true;
    }

    void SpawnNextWaveCountdown()
    {
        timeLeftUntilNextWaveSpawn -= Time.deltaTime;
        nextWaveCountdownTextObject.text = nextWaveCountdownText + "\n\n" + ((int)timeLeftUntilNextWaveSpawn + 1);

        if (timeLeftUntilNextWaveSpawn <= 0)
        {
            wantsToSpawnNewWave = false;
            timeLeftUntilNextWaveSpawn = timeToWaitUntilWaveSpawn;
            nextWaveCountdownTextObject.gameObject.SetActive(false);
            DoSpawnWave();
        }
    }

    private void DoSpawnWave()   
    {
        foreach(GameObject wavePart in EnemyWavesInLevel[CurrentEnemyWave].WaveKey)
        {
           var enemyWave = SpawnObject(wavePart.name,EnemyStartingPos.position);
            enemyWave.GetComponent<EnemyWave>().MovementDirection = this.transform.position - EnemyStartingPos.position;
        }
        CurrentEnemyWave++;
        if (CurrentEnemyWave > EnemyWavesInLevel.Count)
        { CurrentEnemyWave = 0; }
        WaveNumberText.text = $"Wave {CurrentEnemyWave}/{EnemyWavesInLevel.Count}";
    }

    public GameObject SpawnObject(string key, Vector3 aPostion = new Vector3())
    {
        if (key.Length < 1)
        {
            return null;
        }
        if (SpawnableObjectsMap.ContainsKey(key))
        {
            var GO = Instantiate(SpawnableObjectsMap[key]);
            GO.SetActive(true);
            GO.transform.position = aPostion;
            return GO;
            
        }
        return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        var enemyCollider = other.gameObject.GetComponent<EnemyBase>();
        if (enemyCollider != null)
        {
            CurrentPlayerData.PlayerHealth -= enemyCollider.EnemyData.Damage;
            if(PlayerHealthText != null)
            {
                PlayerHealthText.text = CurrentPlayerData.PlayerHealth.ToString() + " HP";
            }
            GameObject.Destroy(enemyCollider.gameObject);
        }
    }
}
