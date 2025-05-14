using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private Transform target;
    private Camera mainCamera;
    private EnemyContext enemyContext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Initialize(Transform target, EnemyContext context)
    {
        this.target = target;
        mainCamera = Camera.main;

        slider.minValue = 0;
        slider.maxValue = context.maxHealth;
    }

    public void SetHealth(int current, int max)
    {
        slider.value = current;
        slider.maxValue = max;
    }

    private void LateUpdate()
    {
        if (target == null || mainCamera == null) return;
        Debug.DrawLine(target.position, transform.position, Color.red);
        transform.position = target.position + offset;

        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
