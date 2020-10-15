using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CamOrto : MonoBehaviour
{
    public Camera MainCam;
    public TranslatePointEvent TransalePoint;
    public ParticleSystem MovePointParticle; 

    public float ScrollSpeed;
    public float RotationSpeed;
    public float CamSpeed;

    private const float ScrollMultiplier = 1000f;
    private const float RotationMuliplier = 100f;
    private const float CamMultiplier = 10f;
    private Transform CameraTransform;

    void Start()
    {
        CameraTransform = MainCam.transform;//схороним ссылку на трансформ заранее, к нему придется постоянно обращаться
    }


    void Update()
    {
        CameraMove();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            MovePointCast();//хождение по ПКМ как в RTS
    }

    private void CameraMove()
    {

        CameraTransform.position += Input.GetAxis("Mouse ScrollWheel") * CameraTransform.forward * ScrollSpeed * Time.deltaTime * ScrollMultiplier;//приближение камеры

        //меняем позицию камеры через стрелки или WASD
        Vector3 pos = new Vector3();
        pos -= Input.GetAxis("Horizontal") * CamSpeed * Time.deltaTime * CamMultiplier * transform.right;
        pos -= Input.GetAxis("Vertical") * CamSpeed * Time.deltaTime * CamMultiplier * transform.forward;
        transform.position += pos;
        //

        //Поворот камеры вокруг точки обзора (а-ля Фростпанк)
        if (Input.GetKey(KeyCode.Mouse2))
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X"), 0) * RotationSpeed * Time.deltaTime * RotationMuliplier;
        //

        //центрирование на игроке
        if (Input.GetKey(KeyCode.Space))
            FindActor();
        //

    }


    private void MovePointCast()//функция нахождения и установки точки, в которую нужно идти. Сигналит ActorController-у
    {
        Ray movePoint = MainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit point;
        if (Physics.Raycast(movePoint, out point, Mathf.Infinity))
        {
            Instantiate<ParticleSystem>(MovePointParticle, point.point + new Vector3(0,1),  Quaternion.Euler(90,0,0));//пуфф
            TransalePoint.Invoke(point.point);//отдаем точку игроку через кастомное событие с параметром
        }

    }


    public void FindActor()//функция центрирования на игроке
    {
        ActorController actor = GameObject.FindObjectOfType<ActorController>();
        if (actor)//пока актер не заспавнен - центрировать не на что
            transform.position = actor.transform.position;
    }


}


