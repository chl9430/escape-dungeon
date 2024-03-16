using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("BGM")]
    [SerializeField] AudioClip bgmSound;
    AudioSource BGM;

    [Header("Guide")]
    [SerializeField] GameObject guideObj;
    GameObject currentGuideObj;

    [Header("Game Log")]
    [SerializeField] GameObject gameLogObj;
    List<GameObject> gameLogObjList;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverUIObj;

    PlayerManager playerManager;
    PlayableDirector cut;

    [SerializeField] GameObject gameUIObj;
    bool isWatching = false;

    public bool IsWatching { set { isWatching = value; } get { return isWatching; } }

    void Awake()
    {
        // 어디서든 접근 가능한 정적 변수
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameLogObjList = new List<GameObject>();
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        // currentShootDelay = 0f;

        // 게임 시작 시, 컷신을 바로 플레이
        //cut = GetComponent<PlayableDirector>();
        //cut.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddGameLog("아이템을 획득하였습니다.");
        }
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(sceneIndex);
    }

    public void SetGameOverUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverUIObj.SetActive(true);
    }

    void PlayBGMSound()
    {
        BGM = GetComponent<AudioSource>();
        BGM.clip = bgmSound;
        BGM.loop = true;
        BGM.Play();
    }

    // 컷신이 끝나고 호출 될 함수. 게임 매니저의 타임라인 끝부분에 리시버를 통해 호출된다.
    public void StartGame()
    {
        isWatching = false;
        PlayBGMSound();
        // GameObject enemy = PoolManager.instance.ActiveObj(3, spawnPoint[Random.Range(0, spawnPoint.Length)].transform.position);
        // StartCoroutine(EnemySpawn());
    }

    public void ShowGuide(string _guideText)
    {
        if (currentGuideObj != null)
        {
            Destroy(currentGuideObj);
        }

        currentGuideObj = Instantiate(guideObj);
        currentGuideObj.transform.SetParent(gameUIObj.transform, false);
        currentGuideObj.GetComponentInChildren<Text>().text = _guideText;
    }

    public void HideGuide()
    {
        Destroy(currentGuideObj);
    }

    public void AddGameLog(string _logContent)
    {
        GameObject logObj = Instantiate(gameLogObj);
        logObj.transform.SetParent(gameUIObj.transform, false);
        logObj.GetComponent<Text>().text = _logContent;

        for (int i = 0; i < gameLogObjList.Count; i++)
        {
            RectTransform logRectTransform = gameLogObjList[i].GetComponent<RectTransform>();

            Vector2 logPos = logRectTransform.anchoredPosition;
            logPos.y += logRectTransform.sizeDelta.y;
            logRectTransform.anchoredPosition = logPos;
        }

        gameLogObjList.Add(logObj);
    }

    public void RemoveGameLogInTheList(GameObject _logObj)
    {
        gameLogObjList.Remove(_logObj);
    }
}