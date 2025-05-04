using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public event Action<Player> OnPlayerSpawned;

    private Player _player;

    private GameObject playerPrefab;

    private MenuController MenuController;

    [Header("Game Events")]
    public UnityEvent<int> OnLifeValueChanged;
    public UnityEvent<int> OnScoreGained;

    [Header("Game Stats")]
    [SerializeField] private int _score = 0;
    [SerializeField] private int maxHP = 5;
    [SerializeField] private int _playerHealth = 5;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            OnScoreGained?.Invoke(_score);
        }
    }
    
    public int PlayerHealth
    {
        get => _playerHealth;
        set
        {
            _playerHealth = value;
            if (_playerHealth > maxHP) _playerHealth = maxHP;
            OnLifeValueChanged?.Invoke(_playerHealth);

            if (_playerHealth <= 0 && _player != null)
            {
                _player.die();
                StartCoroutine(playerDeath(3f));
                Debug.Log("Player dead!");
                _playerHealth = maxHP;
            }
        }
    }
    public void SetMenuController(MenuController menuController) => this.MenuController = menuController;
    protected override void Awake()
    {
        base.Awake();



        playerPrefab = Resources.Load<GameObject>("Player");

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not found in Resources Folder");
        }
    }

    public void SpawnPlayer(Transform spawnPoint)
    {
        if (playerPrefab != null)
        {
            GameObject playerObject = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
            _player = playerObject.GetComponent<Player>();
            OnPlayerSpawned?.Invoke(_player);
        }
        else
        {
            Debug.LogError("Player prefab is not set.");
        }
    }

    IEnumerator playerDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(playerPrefab);
        SceneManager.LoadScene("Game");
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
