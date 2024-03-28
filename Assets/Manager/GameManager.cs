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

    [SerializeField] GameObject gameUIObj;
    [SerializeField] GameObject instUIObj;

    [Header("Guide")]
    [SerializeField] GameObject gameGuideObj;
    GameObject currentGameGuideObj;

    [Header("Game Log")]
    [SerializeField] GameObject gameLogObj;
    List<GameObject> gameLogObjList;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverUIObj;

    PlayerManager playerManager;
    PlayableDirector cut;

    bool isWatching = false;

    public bool IsWatching { 
        set 
        { 
            isWatching = value;

            if (isWatching)
            {
                ClearGameLogInTheList();

                gameUIObj.SetActive(false);
            }
            else
            {
                gameUIObj.SetActive(true);
            }
        } 
        get 
        { 
            return isWatching;
        } 
    }

    void Awake()
    {
        // ��𼭵� ���� ������ ���� ����
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

        // ���� ���� ��, �ƽ��� �ٷ� �÷���
        //cut = GetComponent<PlayableDirector>();
        //cut.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddGameLog("�������� ȹ���Ͽ����ϴ�.");
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

    // �ƽ��� ������ ȣ�� �� �Լ�. ���� �Ŵ����� Ÿ�Ӷ��� ���κп� ���ù��� ���� ȣ��ȴ�.
    public void StartGame()
    {
        isWatching = false;
        PlayBGMSound();
        // GameObject enemy = PoolManager.instance.ActiveObj(3, spawnPoint[Random.Range(0, spawnPoint.Length)].transform.position);
        // StartCoroutine(EnemySpawn());
    }

    public void ShowGuide(string _guideText)
    {
        if (currentGameGuideObj != null)
        {
            Destroy(currentGameGuideObj);
        }

        currentGameGuideObj = Instantiate(gameGuideObj);
        currentGameGuideObj.transform.SetParent(instUIObj.transform, false);
        currentGameGuideObj.GetComponentInChildren<Text>().text = _guideText;
    }

    public void HideGuide()
    {
        Destroy(currentGameGuideObj);
    }

    public void AddGameLog(string _logContent)
    {
        GameObject logObj = Instantiate(gameLogObj);
        logObj.transform.SetParent(instUIObj.transform, false);
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

    public void ClearGameLogInTheList()
    {
        // ���� ���̵�, ���� �α׸� ��� �����Ѵ�.
        foreach (Transform ui in instUIObj.transform)
        {
            Destroy(ui.gameObject);
        }
        gameLogObjList.Clear();
    }
}