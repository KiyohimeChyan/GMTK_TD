using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


public class Enemy : MonoBehaviour
{
    public float health = 100f;      // 血量
    public float damageThreshold = 0.1f; // 碰到障碍物时的转向灵敏度

    private NavMeshAgent agent;
    private Vector3 lastPosition;

    private List<Collider> targets = new List<Collider>();
    private Collider target;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        
        foreach (Collider collider in FindObjectsOfType<Collider>())
        {
            // 检查物体是否在选择框的范围内
            if (collider.gameObject.layer==3)
            {
                targets.Add(collider);
            }
        }
    }

    void Update()
    {
        target = GetLargestCollider(targets);
        Debug.Log(target.transform.parent.parent.name);
        // 自动寻路到目标
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }

        // 检测是否碰到障碍物并处理转向
        AvoidObstacles();
    }

    void AvoidObstacles()
    {
        Vector3 direction = transform.position - lastPosition;
        lastPosition = transform.position;

        // 如果移动速度很小，说明可能被卡住了
        if (direction.magnitude < damageThreshold)
        {
            // 简单地随机转向，尝试脱离障碍物
            float randomAngle = Random.Range(-90f, 90f);
            transform.Rotate(0, randomAngle, 0);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Enemy died");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 例如碰到障碍物时的逻辑
        if (other.CompareTag("Obstacle"))
        {
            TakeDamage(10f); // 每次撞到障碍物时扣 10 点血量
        }
    }
    
    
    Collider GetLargestCollider(List<Collider> colliders)
    {
        Collider largestCollider = null;
        float largestScale = 0f;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            // 获取 Collider 的总体积（通过三个轴的缩放值乘积）
            Vector3 scale = col.transform.localScale;
            float currentScale = scale.x * scale.y * scale.z;

            // 比较 scale
            if (currentScale > largestScale)
            {
                largestScale = currentScale;
                largestCollider = col;
                closestDistance = Vector3.Distance(transform.position, col.transform.position);
            }
            else if (Mathf.Approximately(currentScale, largestScale))
            {
                // 如果 scale 相同，比较距离
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    largestCollider = col;
                    closestDistance = distance;
                }
            }
        }
        Debug.Log("test");
        return largestCollider;
    }
}