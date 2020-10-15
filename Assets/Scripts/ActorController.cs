using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActorController : MonoBehaviour
{
    private Vector3 TargetPos;
    public ParticleSystem SpawnEffect;
    public ParticleSystem DespawnEffect;

    public void MoveToPoint(Vector3 position)//������ ���������� �������, ������� ������� ������ ��� ���
    {
        
        //�������� ����������� - �������, ���� ��� ���� ������
        TargetPos = position;
        GetComponent<NavMeshAgent>().destination = TargetPos;
    }
}
