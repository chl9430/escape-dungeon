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

        // 컷신이 실행중일 경우
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
        // 등록된 reload 버튼을 눌렀을 때
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

        // 재장전 상태라면 조준을 할 수 없게 한다.
        if (controller.isReload)
        {
            return;
        }

        // 등록된 aim 버튼을 눌렀을 때
        if (input.aim)
        {
            // 에임 카메라로 전환
            AimControll(true);

            anim.SetLayerWeight(1, 1);

            Vector3 targetPosition = Vector3.zero;
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

            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            // 플레이어의 전방을 항상 타겟으로 고정시킨다.
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            // 등록된 shoot 버튼을 눌렀을 때
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

    // 재장전 상태를 해제하는 함수(재장전 애니메이션의 끝 부분에 호출된다.)
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