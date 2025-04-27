using UnityEngine;

public class LevelStart : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    void Start() => GameManager.Instance.SpawnPlayer(startPos);
}
