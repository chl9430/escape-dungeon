using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Windows;

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

    PlayerManager playerManager;
    PlayableDirector cut;
    public bool isWatching = false;
    public bool canPlayerMove = false;

    void Awake()
    {
        // 어디서든 접근 가능한 정적 변수
        instance = this;
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        // currentShootDelay = 0f;

        // 게임 시작 시, 컷신을 바로 플레이
        //cut = GetComponent<PlayableDirector>();
        //cut.Play();

        StartCoroutine(EnemySpawn());
    }

    void Update()
    {
        if (isWatching || playerManager.IsTalking)
        {
            canPlayerMove = false;
        }
        else
        {
            canPlayerMove = true;
        }
    }

    IEnumerator EnemySpawn()
    {
        if (PoolManager.instance != null)
        {
            PoolManager.instance.ActiveObj(3, spawnPoint[Random.Range(0, spawnPoint.Length)].transform.position);
        }

        yield return new WaitForSeconds(10f);

        // 2초 뒤에 해당 함수를 재귀호출한다.
        StartCoroutine(EnemySpawn());
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