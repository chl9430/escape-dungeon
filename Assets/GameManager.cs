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
    [SerializeField] GameObject bulletObj;
    [SerializeField] float maxShootDelay = 0.2f;
    [SerializeField] float currentShootDelay = 0.2f;
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

    PlayableDirector cut;
    public bool isReady = true;

    // Start is called before the first frame update
    void Start()
    {
        // 어디서든 접근 가능한 정적 변수
        instance = this;

        currentShootDelay = 0f;

        // 게임 시작 시, 컷신을 바로 플레이
        cut = GetComponent<PlayableDirector>();
        cut.Play();

        InitBullet();
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text = currentBullet + " / " + maxBullet;
    }

    public void Shooting(Vector3 targetPosition, Enemy enemy, AudioSource weaponSound, AudioClip shootingSound)
    {
        currentShootDelay += Time.deltaTime;

        if (currentShootDelay < maxShootDelay || currentBullet <= 0)
            return;

        currentBullet -= 1;
        currentShootDelay = 0f;

        weaponSound.clip = shootingSound;
        weaponSound.Play();

        Vector3 aim = (targetPosition - bulletPoint.position).normalized;

        // Instantiate(weaponFlashFX, bulletPoint);
        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 총구 화염 이펙트가 있는지 확인한다.
        GameObject flashFX = PoolManager.instance.ActiveObj(1);
        SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // Instantiate(bulletCaseFX, bulletCasePoint);
        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 탄피 이펙트가 있는지 확인한다.
        GameObject caseFX = PoolManager.instance.ActiveObj(2);
        SetObjPosition(caseFX, bulletCasePoint);

        // 목표물의 방향으로 총알을 회전키신다.
        // Instantiate(bulletObj, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 총알이 있는지 확인한다.
        GameObject prefabToSpawn = PoolManager.instance.ActiveObj(0);
        SetObjPosition(prefabToSpawn, bulletPoint);
        prefabToSpawn.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // 레이캐스트의 충돌
        //if (enemy != null && enemy.enemyCurrentHP > 0)
        //{
        //    enemy.enemyCurrentHP -= 1;
        //}
    }

    // 탄창이 바뀌고 탄창이 채워지는 함수
    public void ReloadClip()
    {
        // Instantiate(weaponClipFX, weaponClipPoint);
        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 클립 이펙트가 있는지 확인한다.
        GameObject clipFX = PoolManager.instance.ActiveObj(3);
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
        // Instantiate(enemy, spawnPoint[Random.Range(0, spawnPoint.Length)].transform.position, Quaternion.identity);
        if (PoolManager.instance != null)
        {
            GameObject enemy = PoolManager.instance.ActiveObj(4);
            SetObjPosition(enemy, spawnPoint[Random.Range(0, spawnPoint.Length)].transform);
        }

        yield return new WaitForSeconds(2f);

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
        isReady = false;
        PlayBGMSound();
        StartCoroutine(EnemySpawn());
    }
}