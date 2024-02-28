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

    // źâ�� �ٲ�� źâ�� ä������ �Լ�
    public void ReloadClip()
    {
        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) Ŭ�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
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

        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) �ѱ� ȭ�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
        GameObject flashFX = PoolManager.instance.ActiveObj(0);
        GameManager.instance.SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        // ������Ʈ Ǯ���� ��� ������(��Ȱ��ȭ ����) ź�� ����Ʈ�� �ִ��� Ȯ���Ѵ�.
        GameObject caseFX = PoolManager.instance.ActiveObj(1);
        GameManager.instance.SetObjPosition(caseFX, bulletCasePoint);

        // ����ĳ��Ʈ�� �浹
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

        // ī�޶� ���� �������� ����ĳ��Ʈ �߻�
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer))
        {
            targetPosition = hit.point;
            aimObj.transform.position = hit.point;

            enemy = hit.collider.gameObject.GetComponent<Enemy>();
        }
        else
        {
            // ���� �� �� �������� Ÿ���� �׻� ī�޶� �������� ����
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
