using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//����� ��������, ���������� ����������, ���� ���� �� ���� ������ ������������
//������������� ��������� ������
//��������� ���������/����������� ������ ��������� ����� (����� ������� �� �����)
//��������� ���������� ����� �������




public class LevelManager : MonoBehaviour
{
    public static LevelManager Instanse = null;

    private CamOrto MainCam;
    private SpawnArea Spawn;
    private EndArea End;
    private ActorController Actor;

    public int GameOverSceneIndex;
    public int WinSceneIndex;
    public int _preloadSceneIndex;

    private int LevelCount;
    private int NextLevel;

    public float SpawningTime;
    private float SpawningElapsedTime;
    private enum ActorStatuses { Spawned, Spawning, Despawned, Dead, Winner};
    ActorStatuses ActorStatus;


    private void Awake()
    {
        //��������
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);
        GameObject.DontDestroyOnLoad(this);
        //

        //������ ��� ������ ������ - ��������� ������� ���������� � �������. ���� ��, � ��� �� ������� �������� ����� - ������ ��
        if (PlayerPrefs.HasKey("Level"))
        {
            Instanse.NextLevel = PlayerPrefs.GetInt("Level");
            if (!(NextLevel == SceneManager.GetActiveScene().buildIndex))
            {
                SceneManager.LoadScene(Instanse.NextLevel, LoadSceneMode.Single);
            }
            else
            {
                if (SceneManager.GetActiveScene().buildIndex == _preloadSceneIndex)
                    JumpToLevel();
            }
            Debug.Log(Instanse.NextLevel);
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex == _preloadSceneIndex)
            JumpToLevel();
        }
        //

        LevelCount = SceneManager.sceneCountInBuildSettings;//������� ���������� ����
        ActorStatus = ActorStatuses.Despawned;//������������� ������ � ��������� ��������. ������ �������� �� ���������
    }

    private void Update()
    {
        //��������� ������� ���� ������ ����������� �� �����. ���� ���� �� ������� (����� ��� �������� �� ����� �����). ���� ���� ������ ���������� ��� - ������ �� �����
        if (ActorStatus == ActorStatuses.Despawned || !Spawn || !End || !MainCam)
        {
            SpawningElapsedTime = 0;//������� ������� ����� �������
            Spawn = GameObject.FindObjectOfType<SpawnArea>();
            End = GameObject.FindObjectOfType<EndArea>();
            MainCam = GameObject.FindObjectOfType<CamOrto>();
            if (End)
                End.MissionCompleteEvent.AddListener(ActorWin);//����� ������� � ������� �������� ���� ����������
            ActorStatus = ActorStatuses.Spawning;//����� ����� ���� �������� ��������� ����� �������� ������.
        }
        //

        if (ActorStatus == ActorStatuses.Spawning)//���� ����� � ��������� ��������� - ����� ����������� ����� � ��������
        {
            if (SpawningElapsedTime <= SpawningTime)//������ �������
                SpawningElapsedTime += Time.deltaTime;
            else
            {
                Actor = Spawn.Spawn(MainCam);//�������� �������� � ���� ������
                MainCam.FindActor();//����������
                MainCam.TransalePoint.AddListener(Actor.MoveToPoint);//����������� RTS-���������� � ������
                Instantiate<ParticleSystem>(Actor.SpawnEffect, Actor.transform.position, Quaternion.Euler(-90,0,0));//������ ��� ������
                ActorStatus = ActorStatuses.Spawned;//������ ����� �� ������ ������ � �� �������� ���� �������. ���� ��� ����
            }
        }
        //
    }
    public void ActorCatched()//�������-���������� ������� ������ ������. ������ - ��� ��� ����������� ���������
    {
        if (ActorStatus != ActorStatuses.Winner)//��������, �� ������ �� ����� �������. �����, ����� �� ������� �� �����-����.
        {
            ActorStatus = ActorStatuses.Dead;
            Instantiate<ParticleSystem>(Instanse.Actor.DespawnEffect, Instanse.Actor.transform.position, Quaternion.Euler(-90, 0, 0));//����
            Destroy(Instanse.Actor.gameObject);
            SceneManager.LoadSceneAsync(GameOverSceneIndex, LoadSceneMode.Additive);//�������� ������������ ����
        }
    }

    public void ActorWin()//���������� ����� � �������� ����. ������ ��������� ������, ����� �� �������
    {
        if (!(ActorStatus == ActorStatuses.Winner))//����� �� ���������� ��������� ���, ����� ��������� � ����
        {
            ActorStatus = ActorStatuses.Winner;
            SceneManager.LoadSceneAsync(WinSceneIndex, LoadSceneMode.Additive);//���������� �������� ����
        }
    }

    public void JumpToLevel()//����������� ������ �� ������� � ����� �������� ����. ������� ���� � ����� ����� �������������� ������
    {                        //������� ��������� ������� �� ������ �����, ������� �������� � ���������. ����� ����� ��������� �� ������, �������� ������
        Instanse.NextLevel++;
        while (Instanse.NextLevel == Instanse.WinSceneIndex || Instanse.NextLevel == Instanse.GameOverSceneIndex || Instanse.NextLevel == Instanse._preloadSceneIndex)//����� �������� ����
            Instanse.NextLevel++;
        
        //�����������
        if (Instanse.NextLevel == Instanse.LevelCount)
            Instanse.NextLevel = 0;
        //

        ActorStatus = ActorStatuses.Despawned;//������� ������ � ���������� ������ (��� �������� �� ����� ��������� �����������)
        SceneManager.LoadScene(Instanse.NextLevel, LoadSceneMode.Single);//������ ��������� �����

        //������ � ������ ������� ������� (����������)
        PlayerPrefs.SetInt("Level", Instanse.NextLevel);
        PlayerPrefs.Save();
        //
    }
    public void Restart()//���������� ��� ������ ��������. ������ �������� ��������� ������� �����
    {
        ActorStatus = ActorStatuses.Despawned;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

}
