using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AI : MonoBehaviour
{
    public float FOV;
    public float Range;
    public Light DangerIndicator;
    public List<Transform> WayPoints;
    
    
    private NavMeshAgent Walker;
    private short CurrentPoint;
    private ActorController Actor;
    private LevelManager LV;
    private UnityEvent Catch;

    void Start()
    {
        //������������� ��������� � ���� � ��������� �������� ������ �� ����������. ��� ����� ����� ����������, ���� ������� ��� 
        DangerIndicator.range = Range;
        DangerIndicator.spotAngle = (180 - 180 * FOV);
        //

        Walker = GetComponent<NavMeshAgent>();

        //������������� ������ ��������. �� ��� ����� �������� ������� ���� �� �����
        GameObject basePos = new GameObject();
        basePos.transform.position = transform.position;
        WayPoints.Add(basePos.transform);
        //

        CurrentPoint = 0;

        //���� ������������� �� ������ � ����������� ������� ������ ������. ��� ������ � ������� ������ - ��� �� ���� ������
        LV = GameObject.FindObjectOfType<LevelManager>();
        Catch = new UnityEvent();
        if (LV)
            Catch.AddListener(LV.ActorCatched);//���� ����� ������ ��������� �������. ����� �����, � �������� ����. 
                                               //���� ������� ����������� ���������� ������ ���� - �� ��� ��������� �������� ��������� �� ���������
        //

    }


    void Update()
    {
        Walk();
        AlarmCheck();
    }


    void Walk()//������� ������������ �������� ����-����, �������� ������ ����������. ���� �������� ���� (�������) - ����� ������ �� �����
    {          //���� � ���������� ����� ���� �� ���� �������� - ����� ������ �� ���� � �������
        if (!Walker.hasPath)
        {
            if (CurrentPoint < WayPoints.Count)
            {
                Walker.destination = WayPoints[CurrentPoint].position;
                CurrentPoint++;
            }
            else
                CurrentPoint = 0;
        }
    }


    void AlarmCheck()//������� ������������ ���� ������ �� ������� ������
    {                //��� ����� ������, �������� �� ����� � ���� ������. ���� �� - ��������� � ����. ���� ��� �� ��������� ����������� - ����� �������, ��� ����� ������.
        Actor = GameObject.FindObjectOfType<ActorController>();//������� ��������, �� ����� �� ����� (����� �� ����������� ���)
        if (Actor)
        {
            //������ ��������� � ���� ������
            Vector3 search = Actor.transform.position - transform.position;
            float angle = Vector3.Dot(search.normalized, transform.forward);

            GameObject raycastedObj = null;
            if (angle > FOV)//���� ����� � ���� ������, ������, �� �� ������ �� ��:�
            {
                RaycastHit checkedObj;
                if (Physics.Raycast(transform.position, search, out checkedObj, Range))
                {
                    raycastedObj = checkedObj.collider.gameObject;
                    if (raycastedObj == Actor.gameObject)//���� ������������ ������ - �������� ���������. ����� ��� ������ ��� � ��� ������
                    {
                        DangerIndicator.color = new Color(255, 0, 0);
                        Catch.Invoke();
                    }
                }
            }
        }
    }


}
