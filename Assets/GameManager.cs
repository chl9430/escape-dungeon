using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Bullet")]
    [SerializeField] Transform bulletPoint;
    [SerializeField] Text bulletText;
    int maxBullet = 30;
    int currentBullet = 0;

    [Header("Weapon FX")]
    [SerializeField] GameObject weaponFlashFX;
    [SerializeField] Transform bulletCasePoint;
    [SerializeField] GameObject bulletCaseFX;
    [SerializeField] Transform weaponClipPoint;
    [SerializeField] GameObject weaponClipFX;

    [Header("Enemy")]
    [SerializeField] GameObject[] spawnPoint;

    [Header("BGM")]
    [SerializeField] AudioClip bgmSound;
    AudioSource BGM;

    [Header("Guide")]
    [SerializeField] GameObject guideObj;

    PlayableDirector cut;
    public bool isWatching = false;
    public bool canPlayerMove = false;

    // Start is called before the first frame update
    void Start()
    {
        // 어디서든 접근 가능한 정적 변수
        instance = this;

        // currentShootDelay = 0f;

        // 게임 시작 시, 컷신을 바로 플레이
        //cut = GetComponent<PlayableDirector>();
        //cut.Play();

        

        InitBullet();

        StartCoroutine(EnemySpawn());
    }

    // Update is called once per frame
    void Update()
    {
        if (isWatching || TalkManager.instance.isTalking)
        {
            canPlayerMove = false;
        }
        else
        {
            canPlayerMove = true;
        }

        bulletText.text = currentBullet + " / " + maxBullet;
    }

    public void Shooting(Vector3 targetPosition, Enemy enemy, AudioSource weaponSound, AudioClip shootingSound)
    {
        if (currentBullet <= 0)
            return;

        currentBullet -= 1;

        weaponSound.clip = shootingSound;
        weaponSound.Play();

        Vector3 aim = (targetPosition - bulletPoint.position).normalized;

        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 총구 화염 이펙트가 있는지 확인한다.
        GameObject flashFX = PoolManager.instance.ActiveObj(0);
        SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 탄피 이펙트가 있는지 확인한다.
        GameObject caseFX = PoolManager.instance.ActiveObj(1);
        SetObjPosition(caseFX, bulletCasePoint);

        // 레이캐스트의 충돌
        if (enemy != null && enemy.EnemyCurrentHP > 0)
        {
            enemy.GetDamaged(1);
        }
    }

    // 탄창이 바뀌고 탄창이 채워지는 함수
    public void ReloadClip()
    {
        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 클립 이펙트가 있는지 확인한다.
        GameObject clipFX = PoolManager.instance.ActiveObj(2);
        SetObjPosition(clipFX, weaponClipPoint);

        InitBullet();
    }

    void InitBullet()
    {
        currentBullet = maxBullet;
    }

    void SetObjPosition(GameObject obj, Transform targetTransform)
    {
        obj.transform.position = targetTransform.position;
    }

    IEnumerator EnemySpawn()
    {
        if (PoolManager.instance != null)
        {
            GameObject enemy = PoolManager.instance.ActiveObj(3);
            SetObjPosition(enemy, spawnPoint[Random.Range(0, spawnPoint.Length)].transform);
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
        GameObject enemy = PoolManager.instance.ActiveObj(3);
        SetObjPosition(enemy, spawnPoint[Random.Range(0, spawnPoint.Length)].transform);
        // StartCoroutine(EnemySpawn());
    }

    public void ShowGuide(string guideText)
    {
        guideObj.transform.GetComponentInChildren<Text>().text = guideText;
        guideObj.SetActive(true);
    }

    public void HideGuide()
    {
        guideObj.SetActive(false);
    }
}