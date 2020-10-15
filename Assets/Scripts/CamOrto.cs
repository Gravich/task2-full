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
        CameraTransform = MainCam.transform;//�������� ������ �� ��������� �������, � ���� �������� ��������� ����������
    }


    void Update()
    {
        CameraMove();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            MovePointCast();//�������� �� ��� ��� � RTS
    }

    private void CameraMove()
    {

        CameraTransform.position += Input.GetAxis("Mouse ScrollWheel") * CameraTransform.forward * ScrollSpeed * Time.deltaTime * ScrollMultiplier;//����������� ������

        //������ ������� ������ ����� ������� ��� WASD
        Vector3 pos = new Vector3();
        pos -= Input.GetAxis("Horizontal") * CamSpeed * Time.deltaTime * CamMultiplier * transform.right;
        pos -= Input.GetAxis("Vertical") * CamSpeed * Time.deltaTime * CamMultiplier * transform.forward;
        transform.position += pos;
        //

        //������� ������ ������ ����� ������ (�-�� ���������)
        if (Input.GetKey(KeyCode.Mouse2))
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X"), 0) * RotationSpeed * Time.deltaTime * RotationMuliplier;
        //

        //������������� �� ������
        if (Input.GetKey(KeyCode.Space))
            FindActor();
        //

    }


    private void MovePointCast()//������� ���������� � ��������� �����, � ������� ����� ����. �������� ActorController-�
    {
        Ray movePoint = MainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit point;
        if (Physics.Raycast(movePoint, out point, Mathf.Infinity))
        {
            Instantiate<ParticleSystem>(MovePointParticle, point.point + new Vector3(0,1),  Quaternion.Euler(90,0,0));//����
            TransalePoint.Invoke(point.point);//������ ����� ������ ����� ��������� ������� � ����������
        }

    }


    public void FindActor()//������� ������������� �� ������
    {
        ActorController actor = GameObject.FindObjectOfType<ActorController>();
        if (actor)//���� ����� �� ��������� - ������������ �� �� ���
            transform.position = actor.transform.position;
    }


}


