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
    [SerializeField] PostProcessVolume damagedEffect;
    float currentHP;
    float maxHP;

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

    public bool IsInteracting { get { return isInteracting; } }

    public Pistol Pistol { get { return pistol; } }

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

    void Start()
    {
        RefreshHP();
    }

    void Update()
    {
        Talk();

        ShowInventory();

        ShowQuestBox();

        AimCheck();

        ActivateTool();
    }

    // �÷��̾��� HP�� ���õ� �Լ�
    IEnumerator ShowDamagedEffect()
    {
        damagedEffect.weight = 1;

        yield return new WaitForSeconds(3f);

        damagedEffect.weight = 0;
    }
    void RefreshHP()
    {
        HPBarObj.GetComponent<Slider>().value = currentHP / maxHP;
    }

    public void RestoreHP(int _value)
    {
        currentHP += _value;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        RefreshHP();
    }

    public void GetDamaged(float _damage, Vector3 _monPos)
    {
        if (!GameManager.instance.IsWatching && !GameManager.instance.IsClear && !GameManager.instance.IsDead && !isTalking)
        {
            // ������ ���¶��
            if (isReloading)
            {
                // ������ ���·� �����Ѵ�.
                anim.SetLayerWeight(1, 0);
                isReloading = false;
            }

            currentHP -= _damage;
            RefreshHP();

            // ���� ü���� ����ϴٸ�
            if (currentHP > _damage)
            {
                StartCoroutine(ShowDamagedEffect());
            }
            else // ���� ü���� ������� �ʴٸ�
            {
                anim.SetTrigger("Dead");
                GetComponent<PlayerInput>().enabled = false;
                GameManager.instance.SetGameOverUI();
            }
        }
    }

    // �÷��̾��� ���۰� ���õ� �Լ�
    void ShowQuestBox()
    {
        if (!GameManager.instance.IsInputLock() && input.showQuest)
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
                if (!GameManager.instance.IsInputLock())
                {
                    isInventory = true;
                    inventory.SetIsShowingInven(true);
                }
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

            if (!GameManager.instance.IsInputLock() && !isInteracting)
            {
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
            else
            {
                StoryManager.instance.Talk(scanedQuestNPC);
            }
        }
    }

    void AimCheck()
    {
        // ��ϵ� reload ��ư�� ������ ��
        if (input.reload)
        {
            input.reload = false;

            if (!GameManager.instance.IsInputLock() && !isReloading)
            {
                // ������ ���¶�� ������ �� �� ���� �Ѵ�.
                if (isReloading)
                {
                    return;
                }

                pistol.SetAim(false);

                isAiming = false;
                anim.SetLayerWeight(1, 1);
                anim.SetTrigger("Reload");
                isReloading = true;
                return;
            }
            // ������ ���¶�� ������ �� �� ���� �Ѵ�.
        }

        // ��ϵ� aim ��ư�� ������ ��
        if (!isReloading)
        {
            if (input.aim)
            {
                isAiming = true;

                if (!GameManager.instance.IsInputLock() && !isReloading)
                {
                    // ���� ī�޶�� ��ȯ
                    pistol.SetAim(true);

                    anim.SetLayerWeight(1, 1);

                    Vector3 targetAim = pistol.GetTargetPos();
                    targetAim.y = transform.position.y;
                    Vector3 aimDir = (targetAim - transform.position).normalized;

                    // �÷��̾��� ������ �׻� Ÿ������ ������Ų��.
                    transform.forward = Vector3.Lerp(
                        transform.forward, aimDir, Time.deltaTime * 50f
                    );

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
            }
            else
            {
                pistol.SetAim(false);
                isAiming = false;
                anim.SetLayerWeight(1, 0);
                anim.SetBool("Shoot", false);
            }
        }
    }

    void ActivateTool()
    {
        // ��ü�� ��ȣ�ۿ� ��ư�� ������ ��
        if (input.interact)
        {
            input.interact = false;

            if (!GameManager.instance.IsInputLock() && !isInteracting && isReloading)
            {
                if (scanedToolObj != null)
                {
                    isInteracting = true;

                    scanedToolObj.GetComponent<Tool>().InteractObject();
                }

                // Tool���� ��ȣ�ۿ��� ������ �ð��� �ٸ� �� �ֱ� ������
                // ���߿��� Tool�ʿ��� isInteracting�� �������� ����� �� ���� �ִ�.
                isInteracting = false;
            }
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
        GetDamaged(20f, other.transform.position);
    }
}