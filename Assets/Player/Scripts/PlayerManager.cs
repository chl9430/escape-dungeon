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

        // 컷신이 실행중일 경우
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
        // 등록된 reload 버튼을 눌렀을 때
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

        // 재장전 상태라면 조준을 할 수 없게 한다.
        if (controller.isReload)
        {
            return;
        }

        // 등록된 aim 버튼을 눌렀을 때
        if (input.aim)
        {
            // 에임 카메라로 전환
            pistol.SetAim(true);
            controller.isAimMove = true;

            anim.SetLayerWeight(1, 1);

            Vector3 targetAim = pistol.GetTargetPos();
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            // 플레이어의 전방을 항상 타겟으로 고정시킨다.
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            // 등록된 shoot 버튼을 눌렀을 때
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

    // 재장전 상태를 해제하는 함수(재장전 애니메이션의 끝 부분에 호출된다.)
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
            GameManager.instance.ShowGuide("NPC와 대화하려면 T버튼을 누르십시오.");
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