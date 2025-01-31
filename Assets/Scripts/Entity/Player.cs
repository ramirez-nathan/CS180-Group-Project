using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : Entity
{
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform target;


    
    // this holds a lot of necessary logic for throwing projectiles
    private float throwRate = 1.0f;
    private float projectileMoveSpeed = 5.0f;
    [SerializeField] private float throwTimer = 1.0f;
    public bool onThrowCooldown = false;

    Rigidbody2D playerRB;
    private PlayerInput playerInput;
    public Vector2 moveInput;
    public struct PlayerActions 
    {
        public InputAction move; // WASD
        public InputAction throwCard; // left click
        public InputAction playAllHands; // right click
        public InputAction unloadHand; // E
        public InputAction dodge; // space

    } PlayerActions playerControls;
    
    //tracks whether or not we are dodging for update loops 
    public struct DodgeState
    {
        public bool isDodging;
        public int dodgeFramesRemaining;

    } private DodgeState dodgeState;


    private void Awake()
    {
        target = GameObject.Find("Enemy").transform;
        playerInput = GetComponent<PlayerInput>();
        playerRB = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        playerControls.move = playerInput.actions["Move"];
        playerControls.throwCard = playerInput.actions["ThrowCard"];
        playerControls.playAllHands = playerInput.actions["PlayAllHands"];
        playerControls.unloadHand = playerInput.actions["UnloadHand"];
        playerControls.dodge = playerInput.actions["Dodge"];

        playerControls.dodge.started += Dodge;
        playerControls.throwCard.started += ThrowCard;


    }   
    private void OnDisable()
    {
        playerControls.dodge.started -= Dodge;
        playerControls.throwCard.started -= ThrowCard;
    }
    
    
    void Update()
    {
        //added to handle the diagnol speedup problem 
        Vector2 normalizedInput = moveInput.normalized;
        moveInput = playerControls.move.ReadValue<Vector2>();

        UpdateCoolDowns();
    }

    void FixedUpdate()
    {
        if (dodgeState.isDodging)
        {
            //Debug.Log("Dodged in fixed update");
            playerRB.linearVelocity = (moveInput * moveSpeed) * 3;
            if (dodgeState.dodgeFramesRemaining > 0)
            {
                dodgeState.dodgeFramesRemaining--;
            }

            if (dodgeState.dodgeFramesRemaining == 0)
            {
                dodgeState.isDodging = false;
            }
            
        }
        else {
            playerRB.linearVelocity = moveInput * moveSpeed;
        }
        
    }
    // this should handle all cooldowns neatly
    void UpdateCoolDowns()
    {
        if (onThrowCooldown) throwTimer -= Time.deltaTime;
        if (throwTimer <= 0)
        {
            onThrowCooldown = false;
            throwTimer = throwRate; // reset to 
        }
    }
    void ThrowCard(InputAction.CallbackContext context)
    {
        if (!onThrowCooldown)
        {
            Vector3 moveDir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

            // applying 90-degree rotation
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle + 90);

            Projectile projectile = Instantiate(projectilePrefab, transform.position, spawnRotation).GetComponent<Projectile>();
            projectile.InitializeProjectile(target, projectileMoveSpeed);
        } 
    }

    void Dodge(InputAction.CallbackContext context)
    {
        if (!dodgeState.isDodging) //cant dodge while dodging
        {
            //could use a tuple of bool and # frames to increase speed for? 
            dodgeState.isDodging = true;
            dodgeState.dodgeFramesRemaining = 10;
            Debug.Log("Dodged");
        }
    }
}
