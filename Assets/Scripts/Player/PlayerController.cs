using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{

    PlayerData pData;
    PlayerInput pInput;

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
    #endregion

    public PlayerState currentState { get; private set; }


    private void Start()
    {
        pData = GetComponent<PlayerData>();
        pInput = new PlayerInput();

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
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(pData.WallDetector.position, new Vector3(pData.WallDetector.position.x + pData.WallDetectorLength * pData.WallDetectorDir.x, pData.WallDetector.position.y, pData.WallDetector.position.z));
    //}

    private void FixedUpdate()
    {
        currentState.PhysicUpdate();
    }

}
