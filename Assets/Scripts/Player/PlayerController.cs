using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerController : MonoBehaviour
{

    PlayerData pData;
    PlayerInput pInput;

    SpriteRenderer spriteRenderer;
    float timer = 0.2f;

    #region States
    public IdleState IdleState { get; private set; }
    public StartRunState StartRunState { get; private set; }
    public RunningState RunningState { get; private set; }
    public FallingState FallingState { get; private set; }
    public JumpState JumpState { get; private set; }
    public LandingState LandingState { get; private set; }
    public StartFallingState StartFallingState { get; private set; }
    public UnhangedState UnhangedState { get; private set; }
    public WallContactState WallContactState { get; private set; }
    public WallJumpState WallJumpState { get; private set; }
    public WallSlideState WallSlideState { get; private set; }
    public DashingState DashingState { get; private set; }
    public StopDashingState StopDashingState { get; private set; }
    public PrimaryAttackState PrimaryAttackState { get; private set; }
    public GroundedSecondaryAttackState GroundedSecondaryAttackState { get; private set; }
    public GroundedUpwardAttackState GroundedUpwardAttackState { get; private set; }
    public GroundedDownwardAttackState GroundedDownwardAttackState { get; private set; }
    public InAirPrimaryAttackState InAirPrimaryAttackState { get; private set; }    
    public InAirSecondaryAttackState InAirSecondaryAttackState { get; private set; }
    public InAirUpwardAttackState InAirUpwardAttackState { get; private set; }
    #endregion

    public PlayerState currentState { get; private set; }


    private void Start()
    {
        pData = GetComponent<PlayerData>();
        pInput = new PlayerInput();
        spriteRenderer = GetComponent<SpriteRenderer>();

        #region Initialize States 
        IdleState = new IdleState( this,  pInput,  pData, "Idle_Animation");
        StartRunState = new StartRunState( this,  pInput,  pData, "Start_Run_Animation");
        RunningState = new RunningState( this,  pInput,  pData, "Running_Animation");
        FallingState = new FallingState( this,  pInput,  pData, "Falling_Animation");
        JumpState = new JumpState( this,  pInput,  pData, "Jump_Animation");
        LandingState = new LandingState( this,  pInput,  pData, "Tiep_dat_Animation");
        StartFallingState = new StartFallingState( this,  pInput,  pData, "Start_Falling_Animation");
        UnhangedState = new UnhangedState( this,  pInput,  pData, "Wallclimb_unhanged");
        WallContactState = new WallContactState( this,  pInput,  pData, "Wall_contact");
        WallJumpState = new WallJumpState( this,  pInput,  pData, "Wallclimbing_jump");
        WallSlideState = new WallSlideState( this,  pInput,  pData, "Wall_slide");
        DashingState = new DashingState( this,  pInput,  pData, "Dash_Animation");
        StopDashingState = new StopDashingState( this,  pInput,  pData, "Stop_Run_Animation");
        PrimaryAttackState = new PrimaryAttackState(this, pInput, pData, "Left_swing_attack");
        GroundedSecondaryAttackState = new GroundedSecondaryAttackState(this, pInput, pData, "Right_swing_attack");
        GroundedUpwardAttackState = new GroundedUpwardAttackState(this, pInput,pData, "Upward_clamped");
        GroundedDownwardAttackState = new GroundedDownwardAttackState(this, pInput, pData, "Downward"); ;
        InAirPrimaryAttackState = new InAirPrimaryAttackState(this, pInput, pData, "Right_swing_jump");
        InAirSecondaryAttackState = new InAirSecondaryAttackState(this, pInput, pData, "Left_swing_jump");
        InAirUpwardAttackState = new InAirUpwardAttackState(this, pInput, pData, "Upward_jump");

        currentState = IdleState;
        #endregion

    }

    private void Update()
    {
        currentState.LogicUpdate();
        if (currentState != currentState.GetChangedState())
        {
            currentState = currentState.GetChangedState();
            currentState.Enter();
        }
        currentState.PlayAnimation();

        InvulnerableEffect();
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

    private void InvulnerableEffect()
    {

        if (pData.invulnerableTimer >= 0)
        {
            pData.invulnerableTimer -= Time.deltaTime;
        }

        var tempColor = spriteRenderer.color;
        
        if (pData.invulnerableTimer <= 0)
        {
            tempColor.a = 255;
            spriteRenderer.color = tempColor;
            return;
        }

        timer -= Time.deltaTime;
        if (timer >= 0.15f)
        {
            //var tempColor1 = spriteRenderer.color;
            tempColor.a = 255;
            spriteRenderer.color = tempColor;
            return;
        }
        else
        {
            //var tempColor1 = spriteRenderer.color;
            tempColor.a = 0;
            spriteRenderer.color = tempColor;
            if (timer<=0f)
            {
                timer = 0.3f;
            }
            return;
        }

    }

    public void TakeDamage(object[] package) {
        if (pData.invulnerableTimer >= 0)
        {
            return;
        }
        pData.invulnerableTimer = pData.invulnerableTime;
        timer = 0f;
        // trừ máu
        pData.currentHealth -= Convert.ToSingle(package[0]);
        //if (pData.currentHealth <= 0)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}
        
    }

    public void CanDash()
    {
        if (pData.currentMana < pData.maxMana)
        {
            pData.canDash = false;
        }
    }
}
