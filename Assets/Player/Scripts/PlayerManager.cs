using Cinemachine;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [Header("Inventory")]
    [SerializeField] GameObject inventory;
    bool isInventory;

    [Header("Quest")]
    [SerializeField] GameObject questBox;
    int currentQuest;
    bool isTalking;
    bool isQuestBox;

    [Header("Weapon")]
    GameObject weaponObj;
    Pistol pistol;

    public int CurrentQuest { get { return currentQuest; } set { currentQuest = value; } }
    public bool IsTalking { get { return isTalking; } set { isTalking = value; } }

    List<GameObject> items;
    public List<GameObject> ItemList { get { return items; } }

    StarterAssetsInputs input;
    ThirdPersonController controller;
    Animator anim;
    GameObject scanedNPCObj;

    void Awake()
    {
        items = new List<GameObject>();
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
        weaponObj = FindObjectOfType<Pistol>().gameObject;
        pistol = weaponObj.GetComponent<Pistol>();
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>().gameObject;
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
            pistol.SetAim(false);
            controller.isAimMove = false;
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
            inventory.GetComponent<Inventory>().SetIsShowingInven(true);
        }
        else
        {
            inventory.GetComponent<Inventory>().SetIsShowingInven(false);
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

            pistol.SetAim(false);
            controller.isAimMove = false;
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
            pistol.SetAim(true);
            controller.isAimMove = true;

            anim.SetLayerWeight(1, 1);

            Vector3 targetAim = pistol.GetTargetPos();
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            // �÷��̾��� ������ �׻� Ÿ������ ������Ų��.
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            // ��ϵ� shoot ��ư�� ������ ��
            if (input.shoot)
            {
                if (pistol.Shoot(targetAim))
                {
                    anim.SetBool("Shoot", true);
                }
                else
                {
                    anim.SetBool("Shoot", false);
                }
            }
            else
            {
                anim.SetBool("Shoot", false);

                pistol.ResetShootDelay();
            }
        }
        else
        {
            pistol.SetAim(false);
            controller.isAimMove = false;
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    public void ReloadWeaponClip()
    {
        pistol.ReloadClip();
        pistol.PlayReloadingSound(0);
    }

    public void ReloadInsertClip()
    {
        pistol.PlayReloadingSound(1);
    }

    // ������ ���¸� �����ϴ� �Լ�(������ �ִϸ��̼��� �� �κп� ȣ��ȴ�.)
    public void Reload()
    {
        controller.isReload = false;
        anim.SetLayerWeight(1, 0);
        pistol.PlayReloadingSound(2);
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