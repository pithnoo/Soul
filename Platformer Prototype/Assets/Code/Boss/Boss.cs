using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Boss : MonoBehaviour
{
    #region Variables
    public BossStateMachine stateMachine {get; private set;}
    [SerializeField] private BossData bossData;
    public int facingDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public GameObject aliveGO { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }
    public float currentHealth;
    protected bool isDead;
    private LevelManager levelManager;
    private Player player;
    private Soul soul;
    private Portal portal;
    private bool isInvincible;
    private DamageDetails damageDetails;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Transform originalPoint;
    private bool isWithPlayer, isWithSoul;
    public CinemachineVirtualCamera virtualCamera;

    public enum bossAttackType{
        SOUL,
        PLAYER
    }
    public bossAttackType attackType;
    #endregion

    #region States
    private enum bossCollisionState{
        SOUL,
        PLAYER
    }

    [SerializeField] private bossCollisionState collisionState;
    public BossIdleState bossIdleState {get; private set;}
    public BurstAttack burstAttack {get; private set;}
    public DiskAttack diskAttack {get; private set;}
    public LazerAttack lazerAttack {get; private set;}
    public SpikeAttack spikeAttack {get; private set;}
    public SilhouetteAttackHorizontal silhouetteAttackHorizontal {get; private set;}
    public SilhouetteAttackVertical silhouetteAttackVertical {get; private set;}


    #endregion

    #region Unity Callback Functions
    private void Awake() {
        stateMachine = new BossStateMachine();
        bossIdleState = new BossIdleState(this, stateMachine, bossData, "idle");
        burstAttack = new BurstAttack(this, stateMachine, bossData, "burst");
        diskAttack = new DiskAttack(this, stateMachine, bossData, "disk");
        lazerAttack = new LazerAttack(this, stateMachine, bossData, "lazer");
        spikeAttack = new SpikeAttack(this, stateMachine, bossData, "spike");
        silhouetteAttackHorizontal = new SilhouetteAttackHorizontal(this, stateMachine, bossData, "sHorizontal");
        silhouetteAttackVertical = new SilhouetteAttackVertical(this, stateMachine, bossData, "sVertical");
    }

    private void Start(){
        aliveGO = transform.Find("Alive").gameObject;
        rb = aliveGO.GetComponent<Rigidbody2D>();
        anim = aliveGO.GetComponent<Animator>();
        atsm = aliveGO.GetComponent<AnimationToStateMachine>();
        portal = aliveGO.GetComponent<Portal>();
        levelManager = FindObjectOfType<LevelManager>();
        player = FindObjectOfType<Player>();

        currentHealth = bossData.maxHealth;
        facingDirection = 1;
        isInvincible = false;

        attackType = bossAttackType.PLAYER;
        stateMachine.Initialise(bossIdleState);
    }

    private void Update(){
        stateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate(){
        stateMachine.currentState.PhysicsUpdate();
    }
    #endregion
    
    #region Transform Functions
    
    public void flip(){
        facingDirection *= -1;
        aliveGO.transform.Rotate(0f,180f,0f);
    }

    public void LookTowardsEntity(){
        switch(collisionState){
            case bossCollisionState.PLAYER:
                LookTowardsPlayer();
                break;
            case bossCollisionState.SOUL:
                LookTowardsSoul();
                break;
        }
    }
    private void LookTowardsPlayer(){
        //Debug.Log(player.transform.position.x);
        if(player.transform.position.x > transform.position.x){
            transform.localScale = new Vector3(1,1,1);
        }
        else if(player.transform.position.x < transform.position.x){
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    private void LookTowardsSoul(){
        if(soul.transform.position.x > transform.position.x){
            transform.localScale = new Vector3(1,1,1);
        }
        else if(soul.transform.position.x < transform.position.x){
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    public void ReturnToCentre() => transform.position = originalPoint.position;
    public void BossFlight() => transform.position = originalPoint.position + (Vector3.right * Mathf.Sin(Time.time/2*bossData.flightSpeed)*bossData.xScaleFlight - Vector3.up * Mathf.Sin(Time.time * bossData.flightSpeed)*bossData.yScaleFlight);
    #endregion
    
    #region Other Functions
    public void SpawnGameObject(string entityTag, Transform spawnPosition){
        GameObject item = ObjectPool.SharedInstance.GetPooledObject(entityTag);
        if (item != null)
        {
            item.transform.position = spawnPosition.position;
            item.transform.rotation = spawnPosition.rotation;
            item.SetActive(true);
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            if(!isInvincible){
                DecideSpawn();
            }
            else{
                damageDetails.damageAmount = bossData.collisionDamage;
                other.transform.SendMessage("TakeDamage", damageDetails);
            }
        }    
    }

    private void DecideSpawn(){
        switch (collisionState)
        {
            case bossCollisionState.PLAYER:
                spawnSoul();
                attackType = bossAttackType.SOUL;
                break;
            case bossCollisionState.SOUL:
                spawnPlayer();
                attackType = bossAttackType.PLAYER;
                break;
        }
    }

    private void spawnSoul(){
        if(levelManager.player.isDashing){
            levelManager.player.isDashing = false;
            Destroy(levelManager.player.gameObject);

            Instantiate(bossData.portalParticle, transform.position, transform.rotation);
            Instantiate(bossData.soulToSpawn, transform.position, transform.rotation);

            levelManager.ResetSoulCheck();
            ResetSoulCheck();
            virtualCamera.m_Follow = levelManager.soul.transform;
            levelManager.state = LevelManager.currentPlayerState.SOUL;

            isInvincible = true;
            collisionState = bossCollisionState.SOUL;
        }
    }

    private void spawnPlayer(){
        //source.GenerateImpulse();
        Destroy(levelManager.soul.gameObject);

        Instantiate(bossData.portalParticle, transform.position, transform.rotation);
        Instantiate(bossData.playerToSpawn, transform.position, transform.rotation);

        levelManager.ResetPlayerCheck();
        ResetPlayerCheck();
        virtualCamera.m_Follow = levelManager.player.transform;
        levelManager.state = LevelManager.currentPlayerState.PLAYER;
        //canSpawnPlayer = false;

        isInvincible = true;
        collisionState = bossCollisionState.PLAYER;
    }

    private void ResetPlayerCheck() => player = FindObjectOfType<Player>();
    private void ResetSoulCheck() => soul = FindObjectOfType<Soul>();
    #endregion
}
