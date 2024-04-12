using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Player Spawn Point")]
    Transform[] playerSpawnPoints;

    [Header("Monster Spawn Point")]
    [SerializeField] Transform[] monSpawnPoints;
    [SerializeField] GameObject[] monObjs;

    [Header("Item Box Spawn Point")]
    [SerializeField] Transform[] itemBoxSpawnPoints;
    [SerializeField] GameObject[] itemBoxObjs;

    [Header("Gate Btn Spawn Point")]
    [SerializeField] Transform[] gateBtnSpawnPoints;
    [SerializeField] GameObject[] gateBtnObjs;

    [Header("Portal Spawn Point")]
    [SerializeField] Transform[] portalSpawnPoints;
    [SerializeField] GameObject[] portalObjs;

    [Header("Score Time")]
    [SerializeField] GameObject gameClearUIObj;
    [SerializeField] Text scoreText;
    [SerializeField] Text finalScoreText;

    float scoreTime;

    PlayerManager playerManager;
    PlayableDirector cut;

    bool isWatching = false;
    bool isClear = false;

    public bool IsWatching
    {
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

    public bool IsClear
    {
        set
        {
            isClear = value;

            if (isClear)
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
            return isClear;
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
        scoreTime = 0f;
        IsWatching = true;
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        // �÷��̾� ��ġ�� ã�´�.
        playerSpawnPoints = FindObjectsOfType<Transform>()
                                                .Where(obj => obj.name == "Player Spawn Point")
                                                .ToArray();

        // ã�� ��ġ �� ������ ��ġ�� �÷��̾ �����Ѵ�.
        // �÷��̾��� ��ġ�� ����Ѵ�.
        playerManager.gameObject.transform.position = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)].position;

        // currentShootDelay = 0f;

        // ���� ���� ��, �ƽ��� �ٷ� �÷���
        //cut = GetComponent<PlayableDirector>();
        //cut.Play();
    }

    void Update()
    {
        if (!isClear)
        {
            scoreTime += Time.deltaTime;

            scoreText.text = scoreTime.ToString("F2");
        }

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

    public void SetGameClearUI()
    {
        IsClear = true;

        // ���콺�� ǥ���Ѵ�.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameClearUIObj.SetActive(true);

        finalScoreText.text = "�ɸ��ð� : " + ((int)scoreTime).ToString() + "��";
    }

    public void SetGameOverUI()
    {
        // ���콺�� ǥ���Ѵ�.
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

    // �ƽ��� ������ ȣ�� �� �Լ�. ���� ��ŸƮ Ÿ�Ӷ��� ���κп� ���ù��� ���� ȣ��ȴ�.
    public void StartGame()
    {
        IsWatching = false;

        // ���� ���
        // ���� ����(���� 5����)
        for (int i = 0; i < 5; i++)
        {
            int spawnPointRandNum = Random.Range(0, monSpawnPoints.Length);

            // �÷��̾��� ��ġ�� ���� ���� ��ġ�� �ʹ� �����ٸ�
            while (Vector3.Distance(playerManager.transform.position, monSpawnPoints[spawnPointRandNum].position)
                <= 16)
            {
                // ���� ���� ��ġ�� �ٽ� �������� ã�´�.
                spawnPointRandNum = Random.Range(0, monSpawnPoints.Length);
            }

            if (i == 0 || i == 1)
            {
                Instantiate(monObjs[0],
                monSpawnPoints[spawnPointRandNum]);
            }
            else if (i == 2 || i == 3 || i == 4)
            {
                Instantiate(monObjs[1],
                monSpawnPoints[spawnPointRandNum]);
            }
        }

        // ������ �ڽ� ����(������ �ڽ� 3��)
        for (int i = 0; i < 3; i++)
        {
            Instantiate(itemBoxObjs[Random.Range(0, itemBoxObjs.Length)],
                itemBoxSpawnPoints[Random.Range(0, itemBoxSpawnPoints.Length)]);
        }
    }

    public void ActivatePortal()
    {
        for (int i = 0; i < portalObjs.Length; i++)
        {
            Instantiate(portalObjs[i], 
                portalSpawnPoints[Random.Range(0, portalSpawnPoints.Length)]);
        }
    }

    public void ActivateGateBtn()
    {
        for (int i = 0; i < gateBtnObjs.Length; i++)
        {
            int gateGroupRanNum = Random.Range(0, gateBtnSpawnPoints.Length);

            int gatePosRanNum = Random.Range(0, gateBtnSpawnPoints[gateGroupRanNum].childCount);

            Instantiate(gateBtnObjs[i],
                gateBtnSpawnPoints[gateGroupRanNum].GetChild(gatePosRanNum));
        }
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