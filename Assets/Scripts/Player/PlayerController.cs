using Assets.Scripts.Player;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Player.PlayerData;

public partial class PlayerController : MonoBehaviour
{
    private PlayerData pData;
    public PlayerInput pInput;

    public AudioSource audioSource;

    private SpriteRenderer spriteRenderer;
    private float playtime;
    private bool invinceble = false;

    #region States

    public IdleState IdleState { get; private set; }
    public RunningState RunningState { get; private set; }
    public FallingState FallingState { get; private set; }
    public JumpState JumpState { get; private set; }
    public StartFallingState StartFallingState { get; private set; }
    public WallSlideState WallSlideState { get; private set; }
    public DashingState DashingState { get; private set; }
    public PrimaryAttackState PrimaryAttackState { get; private set; }
    public GroundedSecondaryAttackState GroundedSecondaryAttackState { get; private set; }
    public GroundedDownwardAttackState GroundedDownwardAttackState { get; private set; }
    public DeathState DeathState { get; private set; }
    public CheckPointState CheckPointState { get; private set; }
    public PlayerState WallJumpState { get; internal set; }

    #endregion States

    public PlayerState currentState { get; private set; }

    private string selectedProfile;
    private string selectedProfilePath = @"selectedProfile.txt";
    private string saveDataPath = @"playerdata";

    private void Start()
    {
        playtime = 0;
        pData = GetComponent<PlayerData>();
        if (File.Exists(selectedProfilePath))
        {
            selectedProfile = File.ReadAllText(selectedProfilePath);
            saveDataPath += selectedProfile + ".json";
            if (File.Exists(saveDataPath))
            {
                string json = File.ReadAllText(saveDataPath);
                var dto = JsonConvert.DeserializeObject<PlayerDTO>(json);
                pData.serializeData = dto.playerSerializeData;
                playtime = dto.Playtime;
                if (pData.HasCheckPoint)
                {
                    transform.position = new Vector2(pData.PosX, pData.PosY);
                }
            }
        }

        pInput = new PlayerInput(pData);
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        #region Initialize States

        IdleState = new IdleState(this, pInput, pData, "Idle_Animation");
        RunningState = new RunningState(this, pInput, pData, "Running_Animation");
        FallingState = new FallingState(this, pInput, pData, "Falling_Animation");
        JumpState = new JumpState(this, pInput, pData, "Jump_Animation");
        StartFallingState = new StartFallingState(this, pInput, pData, "Start_Falling_Animation");
        WallSlideState = new WallSlideState(this, pInput, pData, "Wall_slide");
        DashingState = new DashingState(this, pInput, pData, "Dash_Animation");
        PrimaryAttackState = new PrimaryAttackState(this, pInput, pData, "Left_swing_attack");
        GroundedSecondaryAttackState = new GroundedSecondaryAttackState(this, pInput, pData, "Right_swing_attack");
        GroundedDownwardAttackState = new GroundedDownwardAttackState(this, pInput, pData, "Downward"); ;
        DeathState = new DeathState(this, pInput, pData, "Death_Animation");
        CheckPointState = new CheckPointState(this, pInput, pData, "CheckPoint_Animation");
        WallJumpState = new WallJumpState(this, pInput, pData, "Jump_Animation");

        currentState = IdleState;

        #endregion Initialize States
    }

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().path == @"Assets/Scenes/UI/UIMenu.unity")
        {
            return;
        }
        PlayerDTO playerDTO = new PlayerDTO();
        playerDTO.playerSerializeData = pData.serializeData;
        playerDTO.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        playerDTO.Playtime = playtime;

        string json = JsonConvert.SerializeObject(playerDTO, Formatting.Indented);

        using (StreamWriter writer = new StreamWriter(saveDataPath))
        {
            writer.Write(json);
        }
    }

    private void Update()
    {
        playtime += Time.deltaTime;
        if (Time.timeScale == 0)
        {
            return;
        }
        if (currentState != currentState.GetChangedState())
        {
            currentState = currentState.GetChangedState();
            currentState.Enter();
        }
        currentState.LogicUpdate();
        currentState.PlayAnimation();
        CanDash();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(pData.WallDetector.position, new Vector3(pData.WallDetector.position.x + pData.WallDetectorLength * pData.WallDetectorDir.x, pData.WallDetector.position.y, pData.WallDetector.position.z));
    //}

    private void FixedUpdate()
    {
        currentState.PhysicUpdate();
    }

    public void CheckPrimaryAttackHit()
    {
        List<Collider2D> hit = Physics2D.OverlapCapsuleAll(pData.PrimaryHitboxPos.position, pData.PrimaryHitboxSize, pData.PrimaryHitboxDirection, 0, pData.EntityMask).ToList();

        pData.CollidedObjects = hit;
    }

    public void CheckInAirPrimaryAttackHit()
    {
        List<Collider2D> hit = Physics2D.OverlapCapsuleAll(pData.InAirPrimaryHitboxPos.position, pData.InAirPrimaryHitboxSize, pData.InAirPrimaryHitboxDirection, 0, pData.EntityMask).ToList();

        pData.CollidedObjects = hit;
    }

    public void CheckGroundedUpwardAttackHit()
    {
        List<Collider2D> hit = Physics2D.OverlapCapsuleAll(pData.GroundedUpwardHitboxPos.position, pData.GroundedUpwardHitboxSize, pData.GroundedUpwardHitboxDirection, 0, pData.EntityMask).ToList();

        pData.CollidedObjects = hit;
    }

    public void CheckInAirUpwardAttackHit()
    {
        List<Collider2D> hit = Physics2D.OverlapCapsuleAll(pData.InAirUpwardHitboxPos.position, pData.InAirUpwardHitboxSize, pData.InAirUpwardHitboxDirection, 0, pData.EntityMask).ToList();

        pData.CollidedObjects = hit;
    }

    public void TakeDamage(object[] package)
    {
        if (invinceble)
        {
            return;
        }
        else
        {
            invinceble = true;
        }
        // trừ máu
        pData.currentHealth -= Convert.ToSingle(package[0]);
        if (pData.currentHealth <= 0)
        {
            currentState.SetNewState(DeathState);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        StartCoroutine(Invinceble());
    }

    private IEnumerator Invinceble()
    {
        yield return new WaitForSeconds(0.5f);
        invinceble = false;
    }

    public void CanDash()
    {
        if (pData.currentMana < pData.maxMana)
        {
            pData.manaGenerationTimer -= Time.deltaTime;
            if (pData.manaGenerationTimer <= 0)
            {
                pData.currentMana += 2;
                pData.manaGenerationTimer = 0.25f;
            }
        }
        if (pData.currentMana - pData.manaCost <= 0)
        {
            pData.canDash = false;
        }
        else
        {
            pData.canDash = true;
        }
    }
}