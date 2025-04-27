using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BooEnemy : EnemyBaseClass
{
    public Transform rayOrigin;

    [Header("Boo Settings")]
    private Transform player;
    public float fadeSpeed = 2f;
    public float detectionAngle = 60f;

    private MeshRenderer rend;
    private Color originalColor;
    private float targetAlpha = 1f;

    #region player spawning
    private void OnEnable()
    {
        GameManager.Instance.OnPlayerSpawned += OnPlayerSpawnedCallback;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawnedCallback;
    }
    private void OnPlayerSpawnedCallback(Player playerObj)
    {
        player = playerObj.transform;
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        rend = GetComponent<MeshRenderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null || rend == null) return;

        if (IsPlayerLookingAtMe())
        {
            targetAlpha = 0f;
        }
        else
        {
            targetAlpha = 1f;
        }

        Color color = rend.material.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        rend.material.color = color;
    }

    private bool IsPlayerLookingAtMe()
    {
        Vector3 toEnemy = (transform.position - player.position).normalized;
        Vector3 playerForward = player.forward;

        float angle = Vector3.Angle(playerForward, toEnemy);
        return angle < detectionAngle;
    }
    protected override void HandleMovement()
    {
        if (IsPlayerLookingAtMe()) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < followRange)
        { 
            Vector3 direction = (player.position - transform.position);
            direction.y = 0f;

            transform.position += moveSpeed * Time.deltaTime * direction;

            RotateTowardsPlayer();
         }

        Hover();
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            targetRotation *= Quaternion.Euler(0, 90f, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    private void Hover()
    {
        Ray ray = new Ray(rayOrigin.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f))
        {
            float desiredHeight = hitInfo.point.y + 1.5f; //Setting height enemy 'floats' above the ground

            float hoverHeight = Mathf.Lerp(transform.position.y, desiredHeight, Time.deltaTime * 5f); //smoothly floating enemy to desired hover height

            hoverHeight = Mathf.Clamp(hoverHeight, desiredHeight - 0.25f, desiredHeight + 0.5f); //Clamping hover height so enemy doesn't go too high or low

            //Applying the oscillation offset for hovering
            Vector3 pos = transform.position;
            //pos.y = hoverHeight + Mathf.Sin((float)(Time.time % (0.5f * Mathf.PI))) * 0.1f;
            pos.y = hoverHeight + Mathf.Sin(Time.time * 5f) * 0.01f; //apply an up and down bob to the enemy, first float effects speed, second float effects amount.
            transform.position = pos;
        }
        else
        {
            //if raycast fails go to a default height
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(transform.position.y, 1f, Time.deltaTime * 5f);
            transform.position = pos;
        }
    }

    protected override void HandleAttack()
    {
        //if (player == null) return;

        //float distance = Vector3.Distance(transform.position, player.position);
        //if (distance <= atkRange)
        //{
        //    //attack logic
        //    return;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player touched");
            Player playerObj = other.GetComponent<Player>();
            if (playerObj != null)
            {
                playerObj.ReceiveDmg(meleeDmg, Player.DamageType.Melee);
            }
        }
    }
}
