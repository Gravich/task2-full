using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Левел Менеджер, реализован синглтоном, дабы быть во всех сценах единственным
//Контроллирует состояние игрока
//Управляет загрузкой/сохранением номера последней сцены (сцены берутся из билда)
//Управляет переходами между сценами




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
        //синглтон
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);
        GameObject.DontDestroyOnLoad(this);
        //

        //прежде чем начать работу - проверяем наличие сохранений в реестре. Если да, и это не текущая открытая сцена - грузим ее
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

        LevelCount = SceneManager.sceneCountInBuildSettings;//дергаем количество сцен
        ActorStatus = ActorStatuses.Despawned;//устанавливаем игрока в состояние деспавна. Дальше работаем от состояний
    }

    private void Update()
    {
        //проверяем наличие всех нужных компонентов на сцене. Ищем если не достает (нужно при переходе на новую сцену). Если хоть одного компонента нет - спавна не будет
        if (ActorStatus == ActorStatuses.Despawned || !Spawn || !End || !MainCam)
        {
            SpawningElapsedTime = 0;//обнулим счетчик перед спавном
            Spawn = GameObject.FindObjectOfType<SpawnArea>();
            End = GameObject.FindObjectOfType<EndArea>();
            MainCam = GameObject.FindObjectOfType<CamOrto>();
            if (End)
                End.MissionCompleteEvent.AddListener(ActorWin);//сразу цепляем к событию финишной зоны обработчик
            ActorStatus = ActorStatuses.Spawning;//после сбора всех ключевых элементов можно спавнить игрока.
        }
        //

        if (ActorStatus == ActorStatuses.Spawning)//если актер в состоянии спавнинга - можно отсчитывать время и спавнить
        {
            if (SpawningElapsedTime <= SpawningTime)//отсчет таймера
                SpawningElapsedTime += Time.deltaTime;
            else
            {
                Actor = Spawn.Spawn(MainCam);//вызываем спавнинг в зоне спавна
                MainCam.FindActor();//центрируем
                MainCam.TransalePoint.AddListener(Actor.MoveToPoint);//привязываем RTS-управление к игроку
                Instantiate<ParticleSystem>(Actor.SpawnEffect, Actor.transform.position, Quaternion.Euler(-90,0,0));//Эффект при спавне
                ActorStatus = ActorStatuses.Spawned;//теперь можно не тикать таймер и не спавнить кучу игроков. Один уже есть
            }
        }
        //
    }
    public void ActorCatched()//функция-обработчик события поимки игрока. Сейчас - она его безжалостно деспавнит
    {
        if (ActorStatus != ActorStatuses.Winner)//проверка, не прошел ли игрок уровень. Нужно, чтобы не словили на финиш-зоне.
        {
            ActorStatus = ActorStatuses.Dead;
            Instantiate<ParticleSystem>(Instanse.Actor.DespawnEffect, Instanse.Actor.transform.position, Quaternion.Euler(-90, 0, 0));//пуфф
            Destroy(Instanse.Actor.gameObject);
            SceneManager.LoadSceneAsync(GameOverSceneIndex, LoadSceneMode.Additive);//загрузка геймоверного меню
        }
    }

    public void ActorWin()//обработчик входа в финишную зону. Меняем состояние игрока, чтобы не сцапали
    {
        if (!(ActorStatus == ActorStatuses.Winner))//чтобы не выигрывать несколько раз, входя повтороно в зону
        {
            ActorStatus = ActorStatuses.Winner;
            SceneManager.LoadSceneAsync(WinSceneIndex, LoadSceneMode.Additive);//подгружаем финишное меню
        }
    }

    public void JumpToLevel()//циклические прыжки по уровням в обход менюшных сцен. Индексы сцен в билде нужно предварительно задать
    {                        //функция дергается кнопкой из другой сцены, поэтому работаем с инстансом. Иначе будем ссылаться на префаб, заданный кнопке
        Instanse.NextLevel++;
        while (Instanse.NextLevel == Instanse.WinSceneIndex || Instanse.NextLevel == Instanse.GameOverSceneIndex || Instanse.NextLevel == Instanse._preloadSceneIndex)//обход менюшных сцен
            Instanse.NextLevel++;
        
        //зацикливаем
        if (Instanse.NextLevel == Instanse.LevelCount)
            Instanse.NextLevel = 0;
        //

        ActorStatus = ActorStatuses.Despawned;//готовим игрока к повторному спавну (при переходе на сцену состояние сохраняется)
        SceneManager.LoadScene(Instanse.NextLevel, LoadSceneMode.Single);//грузим следующую сцену

        //кидаем в реестр текущий уровень (сохранение)
        PlayerPrefs.SetInt("Level", Instanse.NextLevel);
        PlayerPrefs.Save();
        //
    }
    public void Restart()//обработчик для кнопок рестарта. Просто повторно загружает текущую сцену
    {
        ActorStatus = ActorStatuses.Despawned;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

}
