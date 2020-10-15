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

    private void OnTriggerEnter(Collider other)//если игрок зашел в зону финиша - звоним Менеджеру. Вызов меню финиша не компетенция финишной зоны
    {
        if (other.gameObject.GetComponent<ActorController>())
            MissionCompleteEvent.Invoke();
    }
}
