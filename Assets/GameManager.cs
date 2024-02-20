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
        // ��𼭵� ���� ������ ���� ����
        instance = this;

        // currentShootDelay = 0f;

        // ���� ���� ��, �ƽ��� �ٷ� �÷���
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

        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) �ѱ� ȭ�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
        GameObject flashFX = PoolManager.instance.ActiveObj(0);
        SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) ź�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
        GameObject caseFX = PoolManager.instance.ActiveObj(1);
        SetObjPosition(caseFX, bulletCasePoint);

        // ����ĳ��Ʈ�� �浹
        if (enemy != null && enemy.EnemyCurrentHP > 0)
        {
            enemy.GetDamaged(1);
        }
    }

    // źâ�� �ٲ�� źâ�� ä������ �Լ�
    public void ReloadClip()
    {
        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) Ŭ�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
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