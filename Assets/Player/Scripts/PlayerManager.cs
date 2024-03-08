using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] GameObject HPBarObj;
    float currentHP;
    float maxHP;
    bool isInvincible;

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
    bool isReloading;
    bool isAiming;

    public GameObject Inventory { get { return inventory; } }
    public int CurrentQuest { get { return currentQuest; } set { currentQuest = value; } }
    public bool IsInventory { get { return isInventory; } }
    public bool IsTalking { get { return isTalking; } set { isTalking = value; } }
    public bool IsAiming { get { return isAiming; } }
    public bool IsReloading { get { return isReloading; } }
    public bool IsInvincible { get { return isInvincible; } }

    public Pistol Pistol { get {  return pistol; } }

    StarterAssetsInputs input;
    Animator anim;
    GameObject scanedNPCObj;

    void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        anim = GetComponent<Animator>();
        weaponObj = FindObjectOfType<Pistol>().gameObject;
        pistol = weaponObj.GetComponent<Pistol>();

        currentHP = 100;
        maxHP = 100;
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
        
        AimCheck();

        // �÷��̾ �ִϸ��̼� ���൵�� ���� �������·� �ѵ�ų� �����Ѵ�.
        CheckInvincible();

        HPBarObj.GetComponent<Slider>().value = currentHP / maxHP;

        // ���¿� ���� �ʱ�ȭ �� �׸���� �ʱ�ȭ�Ѵ�.
        if (isTalking || isInventory)
        {
            input.move = Vector2.zero;
            input.look = Vector2.zero;
            input.sprint = false;
            input.aim = false;
            input.shoot = false;
        }

        if (isAiming)
        {
            input.sprint = false;
        }

        if (isReloading)
        {
            input.sprint = false;
            input.aim = false;
            input.shoot = false;
        }

        if (isInvincible)
        {
            input.sprint = false;
            input.aim = false;
            input.shoot = false;
        }
    }

    void CheckInvincible()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Damage") && stateInfo.normalizedTime < 1.0f)
        {
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
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
                questBox.SetActive(true);
            }
            else
            {
                isQuestBox = false;
                questBox.SetActive(false);
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
                inventory.GetComponent<Inventory>().SetIsShowingInven(true);
            }
            else
            {
                isInventory = false;
                inventory.GetComponent<Inventory>().SetIsShowingInven(false);
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

            pistol.SetAim(false);

            isAiming = false;
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Reload");
            isReloading = true;
            return;
        }

        // ������ ���¶�� ������ �� �� ���� �Ѵ�.
        if (isReloading)
        {
            return;
        }

        // ��ϵ� aim ��ư�� ������ ��
        if (input.aim)
        {
            // ���� ī�޶�� ��ȯ
            pistol.SetAim(true);
            isAiming = true;

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
            isAiming = false;
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
        isReloading = false;
        anim.SetLayerWeight(1, 0);
        pistol.PlayReloadingSound(2);
    }

    public void GetDamaged(float _damage, Vector3 _monPos)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            GetComponent<CharacterController>().Move(20f * Time.deltaTime * Vector3.back);
            transform.LookAt(new Vector3(_monPos.x, transform.position.y, _monPos.z));
            anim.SetLayerWeight(1, 0);
            anim.SetTrigger("Damage");
            currentHP -= _damage;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            GameManager.instance.ShowGuide("NPC�� ��ȭ�Ϸ��� T��ư�� �����ʽÿ�.");
            scanedNPCObj = other.gameObject;
        }

        if (other.gameObject.CompareTag("AttackRange"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            enemy.PlayerManager = GetComponent<PlayerManager>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            GameManager.instance.HideGuide();
            scanedNPCObj = null;
        }

        if (other.gameObject.CompareTag("AttackRange"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            enemy.PlayerManager = null;
        }
    }
}