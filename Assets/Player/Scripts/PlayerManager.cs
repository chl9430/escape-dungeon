using Cinemachine;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] CinemachineVirtualCamera aimCam;
    [SerializeField] GameObject aimImage;
    [SerializeField] GameObject aimObj;
    [SerializeField] float aimObjDis = 10f;
    [SerializeField] float maxShootDelay = 1f;
    [SerializeField] float currentShootDelay = 0;
    [SerializeField] LayerMask targetLayer;

    [Header("Weapon Sound Effect")]
    [SerializeField] AudioClip shootingSound;
    [SerializeField] AudioClip[] reloadSound;
    AudioSource weaponSound;

    [Header("Inventory")]
    [SerializeField] GameObject inventory;
    bool isInventory;

    [Header("Quest")]
    [SerializeField] GameObject questBox;
    int currentQuest;
    bool isTalking;
    bool isQuestBox;

    public int CurrentQuest { get { return currentQuest; } set { currentQuest = value; } }
    public bool IsTalking { get { return isTalking; } set { isTalking = value; } }

    List<GameObject> items;
    public List<GameObject> ItemList { get { return items; } }

    Enemy enemy;
    StarterAssetsInputs input;
    ThirdPersonController controller;
    Animator anim;
    GameObject scanedNPCObj;

    void Start()
    {
        items = new List<GameObject>();
        currentShootDelay = 0f;
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
        weaponSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        Talk();
        ShowInventory();
        ShowQuestBox();
        ActiveUI();

        // �ƽ��� �������� ���
        if (!GameManager.instance.canPlayerMove)
        {
            AimControll(false);
            return;
        }

        AimCheck();
    }

    public void AddItem(GameObject item)
    {
        items.Add(item);
    }

    public void ActiveUI()
    {
        if (isInventory)
        {
            inventory.SetActive(true);
        }
        else
        {
            inventory.SetActive(false);
        }

        if (isQuestBox)
        {
            questBox.SetActive(true);
        }
        else
        {
            questBox.SetActive(false);
        }
    }

    void ShowQuestBox()
    {
        if (input.showQuest)
        {
            input.showQuest = false;

            if (isQuestBox == false)
            {
                isQuestBox = true;
            }
            else
            {
                isQuestBox = false;
            }
        }
    }

    void ShowInventory()
    {
        if (input.showInventory)
        {
            input.showInventory = false;

            if (isInventory == false)
            {
                isInventory = true;
            }
            else
            {
                isInventory = false;
            }
        }
    }

    void Talk()
    {
        if (input.talk)
        {
            input.talk = false;

            if (scanedNPCObj != null)
            {
                TalkManager.instance.Talk(scanedNPCObj);
            }
        }
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

            // ��ϵ� shoot ��ư�� ������ ��
            if (input.shoot)
            {
                if (currentShootDelay == 0)
                {
                    currentShootDelay += Time.deltaTime;

                    anim.SetBool("Shoot", true);

                    GameManager.instance.Shooting(targetPosition, enemy, weaponSound, shootingSound);
                }
                else
                {
                    anim.SetBool("Shoot", false);

                    currentShootDelay += Time.deltaTime;

                    if (currentShootDelay > maxShootDelay)
                    {
                        currentShootDelay = 0;
                    }
                }
            }
            else
            {
                anim.SetBool("Shoot", false);

                if (currentShootDelay != 0)
                {
                    currentShootDelay += Time.deltaTime;

                    if (currentShootDelay > maxShootDelay)
                    {
                        currentShootDelay = 0;
                    }
                }
            }
        }
        else
        {
            AimControll(false);
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
        anim.SetLayerWeight(1, 0);
        PlayWeaponSound(reloadSound[2]);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            GameManager.instance.ShowGuide("NPC�� ��ȭ�Ϸ��� T��ư�� �����ʽÿ�.");
            scanedNPCObj = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            GameManager.instance.HideGuide();
            scanedNPCObj = null;
        }
    }
}