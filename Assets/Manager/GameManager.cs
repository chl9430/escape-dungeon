using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Enemy")]
    [SerializeField] GameObject[] spawnPoint;

    [Header("BGM")]
    [SerializeField] AudioClip bgmSound;
    AudioSource BGM;

    [Header("Guide")]
    [SerializeField] GameObject guideObj;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverUIObj;

    PlayerManager playerManager;
    PlayableDirector cut;
    public bool isWatching = false;

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
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        // currentShootDelay = 0f;

        // ���� ���� ��, �ƽ��� �ٷ� �÷���
        //cut = GetComponent<PlayableDirector>();
        //cut.Play();

        StartCoroutine(EnemySpawn());
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

    IEnumerator EnemySpawn()
    {
        if (PoolManager.instance != null)
        {
            PoolManager.instance.ActiveObj(3, spawnPoint[Random.Range(0, spawnPoint.Length)].transform.position);
        }

        yield return new WaitForSeconds(10f);

        // 2�� �ڿ� �ش� �Լ��� ���ȣ���Ѵ�.
        StartCoroutine(EnemySpawn());
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
        GameObject enemy = PoolManager.instance.ActiveObj(3, spawnPoint[Random.Range(0, spawnPoint.Length)].transform.position);
        // StartCoroutine(EnemySpawn());
    }

    public void ShowGuide(string _guideText)
    {
        guideObj.transform.GetComponentInChildren<Text>().text = _guideText;
        guideObj.SetActive(true);
    }

    public void HideGuide()
    {
        guideObj.SetActive(false);
    }
}