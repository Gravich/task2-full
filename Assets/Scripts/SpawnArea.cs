using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public ActorController ActorPrefab;
    public Transform SpawnPoint;

    public ActorController Spawn(CamOrto cam)//функция спавна актера. Дергается из Менеджера
    {
        ActorController actor = Instantiate<ActorController>(ActorPrefab, SpawnPoint.position, new Quaternion());
        return actor;
    }
}
