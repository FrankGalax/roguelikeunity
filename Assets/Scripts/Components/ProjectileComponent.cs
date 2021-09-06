using UnityEngine;
using System;

public class ProjectileComponent : MonoBehaviour
{
    public float Speed;
    public int Damage;
    public DamageType DamageType = DamageType.Cold;

    public GameObject Instigator { get; set; }
    public GameObject Target { get; set; }
    public Action Callback { get; set; }

    private Vector3 m_Direction;

    private void Start()
    {
        m_Direction = (Target.transform.position - transform.position).normalized;
    }

    private void Update()
    {
        Vector3 diff = Target.transform.position - transform.position;

        if (Vector3.Dot(diff, m_Direction) <= 0)
        {
            Hit();
            return;
        }

        float maxDistance = diff.magnitude;
        Vector3 direction = diff.normalized;

        float distance = Speed * Time.deltaTime;
        if (distance > maxDistance)
        {
            distance = maxDistance;
        }

        transform.position += direction * distance;
    }

    private void Hit()
    {
        DamageComponent damageComponent = Target.GetComponent<DamageComponent>();
        if (damageComponent != null)
        {
            damageComponent.TakeDamage(Instigator, Damage, DamageType);
        }

        if (Callback != null)
        {
            Callback();
        }

        Destroy(gameObject);
    }
}