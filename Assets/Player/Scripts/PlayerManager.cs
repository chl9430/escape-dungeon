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
        // 물체와 상호작용 버튼을 눌렀을 때
        if (input.interact && !isInventory && !isTalking 
            && !isDamaged && !isReloading && !isInteracting
            && !GameManager.instance.IsWatching)
        {
            input.interact = false;

            isInteracting = true;

            scanedToolObj.GetComponent<Tool>().InteractObject();

            // Tool마다 상호작용이 끝나는 시간이 다를 수 있기 때문에
            // 나중에는 Tool쪽에서 isInteracting을 거짓으로 만들어 줄 수도 있다.
            isInteracting = false;
        }
    }

    // 데미지 애니메이션 마지막에 호출
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
            // 재장전 상태라면
            if (isReloading)
            {
                // 재장전 상태롤 해제한다.
                anim.SetLayerWeight(1, 0);
                isReloading = false;
            }

            // 남은 체력이 충분하다면
            if (currentHP > _damage)
            {
                StartCoroutine(SetInvincibleState());
                transform.LookAt(new Vector3(_monPos.x, transform.position.y, _monPos.z));
                currentHP -= _damage;
            }
            else // 남은 체력이 충분하지 않다면
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
        // 등록된 reload 버튼을 눌렀을 때
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

        // 재장전 상태라면 조준을 할 수 없게 한다.
        if (isReloading)
        {
            return;
        }

        // 등록된 aim 버튼을 눌렀을 때
        if (input.aim && !isInventory && !isTalking 
            && !isDamaged && !isReloading && !isInteracting 
            && !GameManager.instance.IsWatching)
        {
            // 에임 카메라로 전환
            pistol.SetAim(true);
            isAiming = true;

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
            isAiming = false;
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    // 재장전 애니메이션 초반에 호출
    public void ReloadWeaponClip()
    {
        pistol.ReloadClip();
        pistol.PlayReloadingSound(0);
    }

    // 재장전 애니메이션 중반에 호출
    public void ReloadInsertClip()
    {
        pistol.PlayReloadingSound(1);
    }

    // 재장전 애니메이션 후반에 호출
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
            GameManager.instance.ShowGuide("NPC와 대화하려면 T버튼을 누르십시오.");
            scanedQuestNPC = other.GetComponent<QuestNPC>();
        }

        if (other.gameObject.CompareTag("Tool"))
        {
            GameManager.instance.ShowGuide("도구와 상호작용 하려면 CTRL버튼을 누르십시오.");
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
        // 플레이어가 Beholder의 레이저에 맞았을 때
        GetDamaged(30f, other.transform.position);
    }
}