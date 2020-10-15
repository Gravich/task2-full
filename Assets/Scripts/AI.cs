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
        //устанавливаем дальность и угол у спотлайта согласно данным из инспектора. Так легче будет определить, куда смотрит бот 
        DangerIndicator.range = Range;
        DangerIndicator.spotAngle = (180 - 180 * FOV);
        //

        Walker = GetComponent<NavMeshAgent>();

        //устанавливаем первую вейпоинт. За нее берем исходную позицию бота на карте
        GameObject basePos = new GameObject();
        basePos.transform.position = transform.position;
        WayPoints.Add(basePos.transform);
        //

        CurrentPoint = 0;

        //ищем левелменеджер на уровне и подвязываем событие поимки игрока. Что делать с игроком дальше - уже не бота забота
        LV = GameObject.FindObjectOfType<LevelManager>();
        Catch = new UnityEvent();
        if (LV)
            Catch.AddListener(LV.ActorCatched);//боту нужно самому привязать событие. Ботов много, а менеджер один. 
                                               //если захотим динамически заспавнить нового бота - он сам автоматом подпишет Менеджера на прослушку
        //

    }


    void Update()
    {
        Walk();
        AlarmCheck();
    }


    void Walk()//функция зацикленного хождения туда-сюда, согласно списку вейпоинтов. Если вейпоинт один (базовый) - будем стоять на месте
    {          //если в инспекторе задан хотя бы один вейпоинт - будем ходить до него и обратно
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


    void AlarmCheck()//функция сканирования поля зрения на наличие игрока
    {                //бот будет чекать, попадает ли игрок в поле зрения. Если да - рейкастит в него. Если луч не встречает препятствий - можно считать, что игрок пойман.
        Actor = GameObject.FindObjectOfType<ActorController>();//заранее проверим, на сцене ли игрок (вдруг не заспавнился еще)
        if (Actor)
        {
            //Расчет попадания в поле зрения
            Vector3 search = Actor.transform.position - transform.position;
            float angle = Vector3.Dot(search.normalized, transform.forward);

            GameObject raycastedObj = null;
            if (angle > FOV)//если игрок в поле зрения, чекаем, не за стеной ли он:ы
            {
                RaycastHit checkedObj;
                if (Physics.Raycast(transform.position, search, out checkedObj, Range))
                {
                    raycastedObj = checkedObj.collider.gameObject;
                    if (raycastedObj == Actor.gameObject)//если зарейкастили игрока - сигналим Менеджеру. Пусть сам решает что с ним делать
                    {
                        DangerIndicator.color = new Color(255, 0, 0);
                        Catch.Invoke();
                    }
                }
            }
        }
    }


}
