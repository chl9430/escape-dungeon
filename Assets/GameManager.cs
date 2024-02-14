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
        // ��𼭵� ���� ������ ���� ����
        instance = this;

        currentShootDelay = 0f;

        // ���� ���� ��, �ƽ��� �ٷ� �÷���
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
        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) �ѱ� ȭ�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
        GameObject flashFX = PoolManager.instance.ActiveObj(1);
        SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // Instantiate(bulletCaseFX, bulletCasePoint);
        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) ź�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
        GameObject caseFX = PoolManager.instance.ActiveObj(2);
        SetObjPosition(caseFX, bulletCasePoint);

        // ��ǥ���� �������� �Ѿ��� ȸ��Ű�Ŵ�.
        // Instantiate(bulletObj, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) �Ѿ��� �ִ��� Ȯ���Ѵ�.
        GameObject prefabToSpawn = PoolManager.instance.ActiveObj(0);
        SetObjPosition(prefabToSpawn, bulletPoint);
        prefabToSpawn.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // ����ĳ��Ʈ�� �浹
        //if (enemy != null && enemy.enemyCurrentHP > 0)
        //{
        //    enemy.enemyCurrentHP -= 1;
        //}
    }

    // źâ�� �ٲ�� źâ�� ä������ �Լ�
    public void ReloadClip()
    {
        // Instantiate(weaponClipFX, weaponClipPoint);
        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) Ŭ�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
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
        isReady = false;
        PlayBGMSound();
        StartCoroutine(EnemySpawn());
    }
}