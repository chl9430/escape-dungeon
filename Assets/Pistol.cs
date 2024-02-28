using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Pistol : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] CinemachineVirtualCamera aimCam;
    [SerializeField] GameObject aimImage;
    [SerializeField] GameObject aimObj;
    [SerializeField] float aimObjDis = 10f;
    [SerializeField] float maxShootDelay = 1f;
    [SerializeField] int maxShootLimit;
    [SerializeField] LayerMask targetLayer;

    [Header("Weapon Sound Effect")]
    [SerializeField] AudioClip shootingSound;
    [SerializeField] AudioClip[] reloadSound;

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

    float currentShootDelay = 0;

    AudioSource weaponSound;
    Enemy enemy = null;

    void Awake()
    {
        weaponSound = GetComponent<AudioSource>();
        InitBullet();
    }

    void Update()
    {
        bulletText.text = currentBullet + " / " + maxBullet;
    }

    // 탄창이 바뀌고 탄창이 채워지는 함수
    public void ReloadClip()
    {
        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 클립 이펙트가 있는지 확인한다.
        GameObject clipFX = PoolManager.instance.ActiveObj(2);
        GameManager.instance.SetObjPosition(clipFX, weaponClipPoint);

        InitBullet();
    }

    void InitBullet()
    {
        currentBullet = maxBullet;
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
        GameManager.instance.SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // 오브젝트 풀에서 사용 가능한(비활성화 상태) 탄피 이펙트가 있는지 확인한다.
        GameObject caseFX = PoolManager.instance.ActiveObj(1);
        GameManager.instance.SetObjPosition(caseFX, bulletCasePoint);

        // 레이캐스트의 충돌
        if (enemy != null && enemy.EnemyCurrentHP > 0)
        {
            enemy.GetDamaged(1);
        }
    }

    public void ResetShootDelay()
    {
        if (currentShootDelay != 0)
        {
            currentShootDelay += Time.deltaTime;

            if (currentShootDelay > maxShootDelay)
            {
                currentShootDelay = 0;
            }
        }
    }

    public bool Shoot(Vector3 _targetPos)
    {
        if (currentShootDelay == 0)
        {
            currentShootDelay += Time.deltaTime;

            Shooting(_targetPos, enemy, weaponSound, shootingSound);

            return true;
        }
        else
        {
            currentShootDelay += Time.deltaTime;

            if (currentShootDelay > maxShootDelay)
            {
                currentShootDelay = 0;
            }

            return false;
        }
    }

    public Vector3 GetTargetPos()
    {
        Vector3 targetPosition;
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        // 카메라 전방 방향으로 레이캐스트 발사
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer))
        {
            targetPosition = hit.point;
            aimObj.transform.position = hit.point;

            enemy = hit.collider.gameObject.GetComponent<Enemy>();
        }
        else
        {
            // 조준 된 게 없을때는 타겟을 항상 카메라 전방으로 설정
            targetPosition = camTransform.position + camTransform.forward * aimObjDis;
            aimObj.transform.position = camTransform.position + camTransform.forward * aimObjDis;
        }

        return targetPosition;
    }

    public void SetAim(bool _isCheck)
    {
        aimCam.gameObject.SetActive(_isCheck);
        aimImage.SetActive(_isCheck);
    }

    public void PlayReloadingSound(int _reloadSoundNum)
    {
        weaponSound.clip = reloadSound[_reloadSoundNum];
        weaponSound.Play();
    }
}
