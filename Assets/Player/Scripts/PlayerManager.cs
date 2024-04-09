using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] GameObject HPBarObj;
    [SerializeField] GameObject damagedEffectObj;
    float currentHP;
    float maxHP;
    bool isDamaged;
    bool isInvincible;
    bool isDead;

    [Header("Inventory")]
    [SerializeField] Inventory inventory;
    bool isInventory;

    [Header("Quest")]
    [SerializeField] GameObject questBox;
    bool isTalking;
    bool isQuestBox;

    [Header("Weapon")]
    GameObject weaponObj;
    Pistol pistol;
    bool isReloading;
    bool isAiming;

    bool isInteracting;

    public Inventory Inventory { get { return inventory; } }
    public bool IsInventory { get { return isInventory; } }
    public bool IsTalking { get { return isTalking; } set { isTalking = value; } }
    public bool IsAiming { get { return isAiming; } set { isAiming = value; } }
    public bool IsReloading { get { return isReloading; } }
    public bool IsInvincible { get { return isInvincible; } }
    public bool IsDamaged { get { return isDamaged; } }

    public bool IsInteracting { get { return isInteracting; } }
    public bool IsAlive
    {
        get
        {
            if (isDead)
                return false;
            else
                return true;
        }
    }

    public Pistol Pistol { get {  return pistol; } }

    StarterAssetsInputs input;
    Animator anim;
    QuestNPC scanedQuestNPC;
    GameObject scanedToolObj;

    void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        anim = GetComponent<Animator>();
        weaponObj = FindObjectOfType<Pistol>().gameObject;
        pistol = weaponObj.GetComponent<Pistol>();

        currentHP = 100;
        maxHP = 100;
    }

    void Update()
    {
        Talk();

        ShowInventory();

        ShowQuestBox();

        AimCheck();

        ActivateTool();

        HPBarObj.GetComponent<Slider>().value = currentHP / maxHP;
    }

    void ActivateTool()
    {
        // ��ü�� ��ȣ�ۿ� ��ư�� ������ ��
        if (input.interact && !isInventory && !isTalking 
            && !isDamaged && !isReloading && !isInteracting
            && !GameManager.instance.IsWatching)
        {
            input.interact = false;

            isInteracting = true;

            scanedToolObj.GetComponent<Tool>().InteractObject();

            // Tool���� ��ȣ�ۿ��� ������ �ð��� �ٸ� �� �ֱ� ������
            // ���߿��� Tool�ʿ��� isInteracting�� �������� ����� �� ���� �ִ�.
            isInteracting = false;
        }
    }

    // ������ �ִϸ��̼� �������� ȣ��
    public void UnsetIsDamaged()
    {
        isDamaged = false;
    }

    IEnumerator SetInvincibleState()
    {
        isInvincible = true;
        isDamaged = true;
        damagedEffectObj.GetComponent<PostProcessVolume>().weight = 1;
        anim.SetTrigger("Damage");

        yield return new WaitForSeconds(3f);

        damagedEffectObj.GetComponent<PostProcessVolume>().weight = 0;
        isInvincible = false;
    }

    public void GetDamaged(float _damage, Vector3 _monPos)
    {
        if (!isInvincible && !isDead && !GameManager.instance.IsWatching)
        {
            // ������ ���¶��
            if (isReloading)
            {
                // ������ ���·� �����Ѵ�.
                anim.SetLayerWeight(1, 0);
                isReloading = false;
            }

            // ���� ü���� ����ϴٸ�
            if (currentHP > _damage)
            {
                StartCoroutine(SetInvincibleState());
                transform.LookAt(new Vector3(_monPos.x, transform.position.y, _monPos.z));
                currentHP -= _damage;
            }
            else // ���� ü���� ������� �ʴٸ�
            {
                isDead = true;
                anim.SetTrigger("Dead");
                currentHP -= _damage;
                GetComponent<PlayerInput>().enabled = false;
                GameManager.instance.SetGameOverUI();
            }
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
                inventory.SetIsShowingInven(true);
            }
            else
            {
                isInventory = false;
                inventory.SetIsShowingInven(false);
            }
        }
    }

    void Talk()
    {
        if (input.talk)
        {
            input.talk = false;

            if (scanedQuestNPC != null)
            {
                if (scanedQuestNPC.QuestState != QuestState.NONE)
                {
                    isTalking = true;
                    GameManager.instance.ClearGameLogInTheList();
                    StoryManager.instance.Talk(scanedQuestNPC);
                }
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
        if (input.aim && !isInventory && !isTalking 
            && !isDamaged && !isReloading && !isInteracting 
            && !GameManager.instance.IsWatching)
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

    // ������ �ִϸ��̼� �ʹݿ� ȣ��
    public void ReloadWeaponClip()
    {
        pistol.ReloadClip();
        pistol.PlayReloadingSound(0);
    }

    // ������ �ִϸ��̼� �߹ݿ� ȣ��
    public void ReloadInsertClip()
    {
        pistol.PlayReloadingSound(1);
    }

    // ������ �ִϸ��̼� �Ĺݿ� ȣ��
    public void Reload()
    {
        isReloading = false;
        anim.SetLayerWeight(1, 0);
        pistol.PlayReloadingSound(2);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            GameManager.instance.ShowGuide("NPC�� ��ȭ�Ϸ��� T��ư�� �����ʽÿ�.");
            scanedQuestNPC = other.GetComponent<QuestNPC>();
        }

        if (other.gameObject.CompareTag("Tool"))
        {
            GameManager.instance.ShowGuide("������ ��ȣ�ۿ� �Ϸ��� CTRL��ư�� �����ʽÿ�.");
            scanedToolObj = other.gameObject;
        }

        if (other.gameObject.CompareTag("AttackRange"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            enemy.CanAttack = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            GameManager.instance.HideGuide();
            scanedQuestNPC = null;
        }

        if (other.gameObject.CompareTag("Tool"))
        {
            GameManager.instance.HideGuide();
        }

        if (other.gameObject.CompareTag("AttackRange"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            enemy.CanAttack = false;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        // �÷��̾ Beholder�� �������� �¾��� ��
        GetDamaged(30f, other.transform.position);
    }
}