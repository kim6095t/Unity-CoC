using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//�����ٴ� ���
//��Ÿ���� �ٸ��ɷ�
//���� ���忡�� ����

public class Unit : MonoBehaviour
{
    public static string KEY_NAME = "Name";
    public static string KEY_POWER = "Power";
    public static string KEY_ATKSPD = "AttackRate";
    public static string KEY_ATKRANGE = "AttackRadius";
    public static string KEY_MOVESPEED = "MoveSpeed";
    public static string KEY_PRICE = "Price";

    public enum Unit_TYPE
    {
        None = -1,

        Goblin,
        Archer,
        Warrier,
        Giant,

        Count,
    }

    [Header("Search Tower")]
    [SerializeField] float searchTowerRadius;
    [SerializeField] LayerMask searchTowerMask;

    [Header("Search Wall")]
    [SerializeField] float searchWallDistance;
    [SerializeField] LayerMask searchWallMask;

    [Header("Attack")]
    [SerializeField] float attackRadius;
    [SerializeField] float attackRate;
    [SerializeField] float attackPower;

    [Header("etc")]
    [SerializeField] float Hp;




    private Tower targetTower = null;
    private Wall targetWall = null;
    private float nextAttackTime = 0.0f;
    float maxSearchRadius;
    float distanceBetween;
    float speed;            //�ʱⰪ ����

    Animator anim;
    NavMeshAgent navi;
    NavMeshPath path;
    LineRenderer line;

    bool isMove;
    bool destTower;
    bool destWall;


    private void Start()
    {
        anim = GetComponent<Animator>();
        navi = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        path = new NavMeshPath();

        navi.enabled = false;
        navi.enabled = true;

        RePathManager.Instance.RegestedPath(DlRePath);

        maxSearchRadius = 100f;
        distanceBetween = maxSearchRadius;
        speed = navi.speed;
    }

    private void OnDestroy()
    {
        RePathManager.Instance.RemovePath(DlRePath);
    }
    void Update()
    {
        //���̳� Ÿ������ �Ÿ� ����
        if (targetTower != null)
        {
            if (destWall == true){
                if (targetWall != null)
                    distanceBetween = Vector3.Distance(targetWall.transform.position, transform.position);
            }
            else
                distanceBetween = Vector3.Distance(targetTower.transform.position, transform.position);
        }


        //Ÿ��Ž��
        if (targetTower == null)
        {
            destWall = false;
            destTower = false;
            SearchTower();
        }
        //���� �˻��Ǿ��� �� �� Ž���Ҽ� �ְ� ����, Ÿ���� null�� �ƴҶ� ���±濡 ���� �ִ��� Ȯ��
        //Length 1�̻��̶� ���� �ּ��� ������ġ�� �������� ������ ��
        else if (navi.path.corners.Length > 1 && targetTower!=null)
        {
            SearchWall();
        }


        //���ݹ������� ���ݴ���� ���� ��
        if (distanceBetween <= attackRadius)
        {
            if (destTower)
                AttackTower();
            else if (destWall)
                AttackWall();
            //�ƹ��� �������� ���������ʾ����� ���Ƿ� ������ ����
            else
            {
                if (destWall == false)
                    destWall = true;
                else if (destTower == false)
                    destTower = true;
            }
        }
        //���ݹ����� �ƴ� ���� Ž���� Ÿ���� �����̰� �Ѵ�
        //���� �μ��������� RePathManager�� �̿��Ͽ� ��Ž��
        //���� 1���϶�� ���� ���� �˻� ���� �ʾҴٴ� ��, �� �˻��� �Ǹ� �� ���� ���� ���� ������ 
        else if(targetTower!=null && navi.path.corners.Length <=1)
        {
            MoveTo();
        }
    }

    private void SearchTower()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, searchTowerRadius, searchTowerMask);

        //���� ������ �ִ� Ÿ���� Ÿ������ ����
        if (targets.Length > 0)
        {
            Collider pick = targets[0];
            Tower tower = pick.GetComponent<Tower>();
            if (tower != null)
            {
                targetTower = tower;
                destTower = true;
                destWall = false;
                RePathManager.Instance.RePath();
            }
        }
        else
        {
            searchTowerRadius = Mathf.Clamp(searchTowerRadius *= 1.2f, attackRate, maxSearchRadius);
            if(searchTowerRadius>=maxSearchRadius && targetTower==null)
            {
                navi.speed = 0;
                isMove = false;
                anim.SetBool("isMove", isMove);
            }
        }
    }

    private void SearchWall()
    {
        Vector3 rot;
        Vector3 pivot;
        RaycastHit hit;

        //���±� ��ü ���̸� �߻��Ͽ� ���θ��� �� Ž��
        for(int i=0; i< navi.path.corners.Length - 1; i++)
        {
            rot = navi.path.corners[i+1] - navi.path.corners[i];
            pivot = navi.path.corners[i];
            float pathDistance = Vector3.Distance(navi.path.corners[i + 1], navi.path.corners[i]);

            //���� �˻��Ǿ��� ���� ����Ÿ���� ������ ����
            if (Physics.Raycast(pivot, rot.normalized, out hit, pathDistance, searchWallMask))
            {
                Wall wall = hit.collider.gameObject.GetComponent<Wall>();
                if (wall != null)
                {
                    targetWall = wall;
                    destWall = true;
                    destTower = false;
                }
                break;
            }
            //�˻��ȵ� �� ���� ���� ������ �Ǵ� 
            else
            {
                destTower = true;
                destWall = false;
            }
            Debug.DrawRay(pivot, rot.normalized * pathDistance, Color.red);
        }
    }

    //Ÿ���� �̵��ϱ� ���� �Լ�
    private void MoveTo()
    {
        isMove = true;
        anim.SetBool("isMove", isMove);
        navi.speed = speed;

        navi.CalculatePath(targetTower.transform.position, path);
        navi.SetPath(path);

        DrawLine();
    }

    //���̳� Ÿ���� �μ������� �� �� Ž��
    private void DlRePath()
    {
        isMove = true;
        anim.SetBool("isMove", isMove);
        navi.speed = speed;

        destTower = true;
        destWall = false;

        if (targetTower != null)
        {
            navi.CalculatePath(targetTower.transform.position, path);
            navi.SetPath(path);
        }

        DrawLine();
    }

    // ���� ������ �׸���.  
    private void DrawLine()
    {
        line.positionCount = navi.path.corners.Length;
        line.SetPositions(navi.path.corners);
        line.startColor = (targetTower == null) ? Color.green : Color.red;
        line.endColor = (targetTower == null) ? Color.green : Color.red;
    }

    private void AttackTower()
    {
        isMove = false;
        anim.SetBool("isMove", isMove);
        navi.speed = 0f;

        searchTowerRadius = 5f;

        if (nextAttackTime <= Time.time && targetTower != null)
        {
            anim.SetTrigger("onAttack");
            nextAttackTime = Time.time + attackRate;
            destTower = targetTower.OnDamaged(attackPower);
            if (!destTower)
                navi.speed = speed;
        }

    }
    private void AttackWall()
    {
        isMove = false;
        anim.SetBool("isMove", isMove);
        navi.speed = 0f;

        searchTowerRadius = 5f;

        if (nextAttackTime <= Time.time && targetWall != null)
        {
            anim.SetTrigger("onAttack");
            nextAttackTime = Time.time + attackRate;
            destWall = targetWall.OnDamaged(attackPower);
            if (!destWall)
                navi.speed = speed;
        }
    }

    public void OnDamaged(float damaged)
    {
        Hp -= damaged;

        if (Hp <= 0)
            OnDead();
    }

    private void OnDead()
    {
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, searchTowerRadius);

        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, attackRadius);
    }
#endif
}