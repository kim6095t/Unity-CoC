using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTile : MonoBehaviour
{
    private GameObject onObject;        // 타일 위에 존재하는 오브젝트.
    public bool IsOnObject => onObject != null;

    private void Start()
    {
        FindOnObject();
    }

    public void SetUnit(Unit newUnit)
    {
        newUnit.transform.position = transform.position;   // 위치 값 갱신.
        newUnit.transform.rotation = transform.rotation;   // 회전 값 갱신.
    }

    private void FindOnObject()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.up, 1.0f);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject target = hits[i].collider.gameObject;
            ObjectLocationManager.Instance.DataCreate(target);

            //예외처리
            if (MapManager.Instance==null)
                return;

            if (target.CompareTag(MapManager.Instance.TAG_PLAYABLE))
            {
                onObject = target;
                break;
            }
            else
                onObject = null;
        }
    }
}
