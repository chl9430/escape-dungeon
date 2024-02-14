using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : MonoBehaviour
{
    StarterAssetsInputs input;
    ThirdPersonController controller;
    Animator anim;

    [Header("Aim")]
    [SerializeField] CinemachineVirtualCamera aimCam;
    [SerializeField] GameObject aimImage;
    [SerializeField] GameObject aimObj;
    [SerializeField] float aimObjDis = 10f;
    [SerializeField] LayerMask targetLayer;

    [Header("IK")]
    [SerializeField] Rig handRig;
    [SerializeField] Rig aimRig;

    [Header("Weapon Sound Effect")]
    [SerializeField] AudioClip shootingSound;
    [SerializeField] AudioClip[] reloadSound;
    AudioSource weaponSound;

    Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
        weaponSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // �ƽ��� �������� ���
        if (GameManager.instance.isReady)
        {
            AimControll(false);
            SetRigWeight(0);
            return;
        }

        AimCheck();
    }

    void AimCheck()
    {
        // ��ϵ� reload ��ư�� ������ ��
        if (input.reload)
        {
            input.reload = false;

            if (controller.isReload)
            {
                return;
            }

            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Reload");
            controller.isReload = true;
        }

        // ������ ���¶�� ������ �� �� ���� �Ѵ�.
        if (controller.isReload)
        {
            return;
        }

        // ��ϵ� aim ��ư�� ������ ��
        if (input.aim)
        {
            // ���� ī�޶�� ��ȯ
            AimControll(true);

            anim.SetLayerWeight(1, 1);

            Vector3 targetPosition = Vector3.zero;
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

            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            // �÷��̾��� ������ �׻� Ÿ������ ������Ų��.
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            SetRigWeight(1);

            // ��ϵ� shoot ��ư�� ������ ��
            if (input.shoot)
            {
                anim.SetBool("Shoot", true);
                GameManager.instance.Shooting(targetPosition, enemy, weaponSound, shootingSound);
            }
            else
            {
                anim.SetBool("Shoot", false);
            }
        }
        else
        {
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    void AimControll(bool isCheck)
    {
        aimCam.gameObject.SetActive(isCheck);
        aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    // ������ ���¸� �����ϴ� �Լ�(������ �ִϸ��̼��� �� �κп� ȣ��ȴ�.)
    public void Reload()
    {
        controller.isReload = false;
        SetRigWeight(1);
        anim.SetLayerWeight(1, 0);
        PlayWeaponSound(reloadSound[2]);
    }

    void SetRigWeight(float weight)
    {
        aimRig.weight = weight;
        handRig.weight = weight;
    }

    public void ReloadWeaponClip()
    {
        GameManager.instance.ReloadClip();
        PlayWeaponSound(reloadSound[0]);
    }

    public void ReloadInsertClip()
    {
        PlayWeaponSound(reloadSound[1]);
    }

    void PlayWeaponSound(AudioClip sound)
    {
        weaponSound.clip = sound;
        weaponSound.Play();
    }
}
