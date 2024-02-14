using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        // 어디서든 접근 가능한 정적 변수
        instance = this;

        currentShootDelay = 0f;

        InitBullet();
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text = currentBullet + " / " + maxBullet;
    }

    public void Shooting(Vector3 targetPosition)
    {
        currentShootDelay += Time.deltaTime;

        if (currentShootDelay < maxShootDelay || currentBullet <= 0)
            return;

        currentBullet -= 1;
        currentShootDelay = 0f;

        Instantiate(weaponFlashFX, bulletPoint);
        Instantiate(bulletCaseFX, bulletCasePoint);

        Vector3 aim = (targetPosition - bulletPoint.position).normalized;

        // 목표물의 방향으로 총알을 회전키신다.
        Instantiate(bulletObj, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
    }

    // 탄창이 바뀌고 탄창이 채워지는 함수
    public void ReloadClip()
    {
        Instantiate(weaponClipFX, weaponClipPoint);
        InitBullet();
    }

    void InitBullet()
    {
        currentBullet = maxBullet;
    }
}