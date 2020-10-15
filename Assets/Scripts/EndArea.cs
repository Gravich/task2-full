using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndArea : MonoBehaviour
{
    public UnityEvent MissionCompleteEvent;
    private void Start()
    {
        MissionCompleteEvent = new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)//���� ����� ����� � ���� ������ - ������ ���������. ����� ���� ������ �� ����������� �������� ����
    {
        if (other.gameObject.GetComponent<ActorController>())
            MissionCompleteEvent.Invoke();
    }
}
