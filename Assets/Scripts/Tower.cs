using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Search")]
    [SerializeField] float searchRadius;
    [SerializeField] LayerMask searchMask;


    [Header("Bullet")]
    [SerializeField] float attackPower;
    [SerializeField] float moveSpeed;
    [SerializeField] Bullet bullet;
    [SerializeField] Transform bulletPivot;

    [Header("etc")]
    [SerializeField] float attackRate;
    [SerializeField] Transform pivot;
    [SerializeField] float Hp;

    private Unit target = null;
    private float nextAttackTime = 0.0f;


    void Update()
    {
        if (target == null)
            SearchEnemy();
        else
            AttackEnemy();
    }

    private void SearchEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, searchRadius, searchMask);
        if (targets.Length > 0)
        {
            Collider pick = targets[Random.Range(0, targets.Length - 1)];
            Unit enemy = pick.GetComponent<Unit>();

            if (enemy != null)
                target = enemy;
            Debug.Log($"Ÿ��: {target}");
        }
    }

    private void AttackEnemy()
    {
        // ���� ������ Ÿ���� ����(�׾���) ���.
        if (target == null)
            return;

        // ���� ���� ���� ���� �Ÿ��� ���� ��Ÿ����� �� ���.
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > searchRadius)
        {
            target = null;
            return;
        }

        Vector3 direction = target.transform.position - pivot.position;     // (ȸ��) ���Ϸ� ����
        direction.Normalize();                                              // 0.0 ~ 1.0f ���� ������ ����ȭ.
        Quaternion lookAt = Quaternion.LookRotation(direction);             // (ȸ��) ���Ϸ� -> ���ʹϾ�
        lookAt.x = 0f;
        lookAt.z = 0f;

        // Lerp : ���� -> ������ ������ �ð��� ����� ���� ���� ���� �ش�.
        pivot.rotation = Quaternion.Lerp(pivot.rotation, lookAt, 10f * Time.deltaTime);

        // Ÿ���� ����.
        if (nextAttackTime <= Time.time)
        {
            nextAttackTime = Time.time + attackRate;

            Bullet newBullet = Instantiate(bullet);
            newBullet.transform.position = bulletPivot.position;
            newBullet.transform.rotation = bulletPivot.rotation;
            newBullet.Shoot(target, moveSpeed, attackPower);
        }
    }
    public void OnDamaged(float damaged)
    {
        Debug.Log($"���ֵ�����: {damaged}");
        Hp -= damaged;

        if (Hp <= 0)
            OnDestroy();
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, searchRadius);
    }
#endif
}