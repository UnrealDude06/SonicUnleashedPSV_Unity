using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using MegaFiers;

using UnityEngine.UI;
using System.Data.Common;
using TMPro;

public class SonicController : MonoBehaviour {
    public bool dead,hurt;
	private Camera cam;
    public bool twoDmode;
	[SerializeField] private Rigidbody rb;
    [SerializeField] private Animator anim;
    public Transform fwd_pos;
    [SerializeField] private Image homingAttack;
    

    private LostRingPool ringPool;
    private SonicScore score;
    public SonicObejctInteraction sonic_col;
    public SFXmanager sfx;
    private PathFollower pathFollow;
    [HideInInspector] public Sonic_enemyExplode_launcher enemyExplode;
    CapsuleCollider capsuleCollider;
    Collider[] enemyCheck = new Collider[5]; // Create a fixed-sized array to store the colliders


[Header("Movement")]
    public float speed;
    public float acc,fric,maxSpeed,maximumSpeed,skidLerp,sideway_speed,slopeAngleThreshold;
    bool airMode;
    public float maxUpwardVelocity = 1.0f;

[Range(0,25)]
    public float jump_lerp_start,jump_lerp_apex; 
    public float jump_lerp;

[Header("Turning")]
 [Range(0f,27f)]
    public float turnSpeedHigh;
 [Range(0f,27f)]
    public float turnSpeedLow;
[Range(0f,27f)]
    public float turnSpeedJump;
    public float turnSpeed,drift_turnSpeed;
    public float speedAnalogThreshold,speedAnalogLoss;
[Range(0f,10f)]
    public float slope_gain,slope_drag;

[Header("Jumping")]
    public float jumpSpeed;
    [SerializeField]private float lastGroundedTimerOG
    ,lastGroundedTimer;
    public bool canControl, lastGrounded;
    public bool isGrounded;
	public float groundStickPower,gravity,gravityWhenFall,groundSlopeAngle;

    public float jumpHeight,minimumJumpHeight;
    bool minJumpReached;
    float startPos;
    public bool jumpingOffEnemy;
    private float yOffset = 1;


 [Header("Controls Lock")]   
    public bool controlLock;
    float controlTimer = 0.5f;
    public float slope_controlLockRate;

	[Header("Sonic Actions")]
    public bool boost,air_boost;
    public enum Actions{none,attack, airDash, q_step,drift,spline, stomp,crouch,slide,homing_trick,wall_hit,wall_hit_heavy};
    public Actions sonic_action;


    
[Header(" Homing Attack ")]
    [SerializeField]public GameObject attackObj;
    [SerializeField]private Transform HomingAttackCheck;
    [SerializeField] private float radius, airDashSpeed;
    public int actionchain;
    private float homingAttackfailSafe = 5f;
    float homingTrickRandomiser;
    

    [Header("Drift")]
    
    public float driftSpd;
    public float rotationSpeedDrift = 40;
    public float drift_turnSpeedIncrement = 15;
    float d_horiz;
    public float drift_time,drift_direction;
    public float drift_lerp;
    public ParticleSystem sparks;
    private ParticleSystem.MainModule sparksModule;
    ///drifting delay
    public float driftDelay = 0.5f;
    private float delay_timer = 0.5f;
    public bool canDrift;
    ///Check if inout is zero to leave drfit
     float zeroInputTimer = 0;
     float releaseDelay = 0.09f;
     float old_spd;


    [Header("Quick Step")]
    public float quickstepspd;
    [SerializeField] private float quickstep_time;

    [Header("Boost")]
    public float boost_spd_add;
    public float boost_take_away;
    public GameObject boostModelFX;
    public float boost_timer;
    public float b_timer;



    [Header("Stomp")]
    [SerializeField]private float stomp_speed;
    public float CrouchSpd;
    [SerializeField]private GameObject stompMod;


    [Header("Wall Affect Player")]
    public ParticleSystem sparks_wallhit;
    private ParticleSystem.MainModule mainModule;

    [Header("Landing Force Impact")]
    Vector3 landingBoost = Vector3.zero;
    public float boostStrength,DecaySpeed;
    
    bool done;


    [Header(" Spline Movement ")]
    public MegaShape	path;					// The Shape that will attract the rigid body
    public int			curve		= 0;		// The sub curve of that shape usually 0
		public bool			usealpha	= false;	// Set to true to use alpha value instead of finding the nearest point on the curve.
    public float		impulse		= 10.0f;	// The force that will applied if the rbody is 1 unit away from the curve
		public float		inputfrc	= 10.0f;	// Max forcce for user input
		public bool			align		= true;		// Should rigid body align to the spline direction
		public float		alpha		= 0.0f;		// current position on spline, The alpha value to use is usealpha mode set, allows you to set the point on the curve to attract the rbody (0 - 1)
		public float		delay		= 1.0f;		// how quickly user input gets to max force
		public float		drag		= 0.0f;		// slows object down when moving
		public float		breakforce	= 100.0f;	// force above which the rigidbody will break free from the path
		float				drive		= 0.0f;
		float				vel			= 0.0f;
		float				tfrc		= 0.0f;
		Vector3				nps;

		public MegaAxis		forwardAxis	= MegaAxis.Z;
		public Vector3		rotSpline			= Vector3.zero;
		public float		horizontalPos;



    bool reAlign = false;
    Vector2 input;
    [HideInInspector] public Vector3 movement, localMove,GroundNormal,groundSlopeDir,airVector,forward,right;
    [HideInInspector] public float offset_distance,maxSpeed_og ;
    [HideInInspector] public RaycastHit hit;
    RaycastHit hit_fwd;
    [HideInInspector] public bool jumped = false;
    bool d, shorthop;
    float velocity_mag;
    private Spring attkSpg;
    Quaternion rot;
    float startdir;
    Vector3 slopeForward;
     int input_snap;
    [HideInInspector]public GameObject previousAttackObject; Collider[] hitColliders;
	// Use this for initialization
	void Awake () {
		 cam = Camera.main;
         maxSpeed_og = maxSpeed;
         delay_timer = driftDelay;

         // Grab components
         score = GetComponent<SonicScore>();
         sonic_col = GetComponent<SonicObejctInteraction>();
         enemyExplode = GetComponent<Sonic_enemyExplode_launcher>();
         
         sfx = GetComponent<SFXmanager>();
         capsuleCollider = GetComponent<CapsuleCollider>();

         mainModule = sparks_wallhit.main;
         sparksModule = sparks.main;

         // Check if LostRingPool.instance is set before attempting to access it
        if (LostRingPool.instance != null)
        {
            ringPool = LostRingPool.instance;
            // Now you can safely use ringPool
        }
        else
        {
            Debug.LogWarning("LostRingPool instance is not set.");
        }

	}
	
	void Update () {
        
        Animation();
        EndSonicLife();


		GroundCheckSlope();
        if(sonic_action != Actions.spline)
        {
            HandleGravity();
            SlopePhysics();

            Movement();
            JumpInput();
            
            WallAffects();

            

            HomingAttack();
            Drift();
            if (sonic_action != Actions.homing_trick)
            {
                if(sonic_action != Actions.wall_hit_heavy)
                {
                    QuickStep();
                    Stomp();
                    CrouchSlide();
                }
            }
        }
        else
        {
            SplineAction();
        }
        //JankReuction();
        BOOST();

        SonicHurt();

	}

   

    void JankReuction()
    {
        if(!canControl)
        {
            var lerpAmount = 20f;

            input = Vector2.zero;
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity, lerpAmount * Time.deltaTime);
        }
    }
    void SplineAction()
    {
        if(boost)
        {
            sonic_action = Actions.none;
            //pathFollow.pathCreator = null;
            canControl = true;
            
        }
    }

    void WallAffects()
    {
        if(sonic_action != Actions.q_step) //Also used for quick-stepping, to ensure change fo direction doesnt happen during q_step
        {
            if(input.x > 0)
            {input_snap = 1;}
            else if (input.x < 0)
            {input_snap = -1;}
        }
        
        if(speed > 26 && isGrounded)
        {
            RaycastHit[] hits = new RaycastHit[1]; // Array to store the results of the raycast
            int layerMask = LayerMask.GetMask("Default" , "Wall Collisions") ; // Get the layer mask for "Default" layer
           
            
            

            
            // Perform the raycast
            if (Physics.RaycastNonAlloc(transform.position, input_snap * transform.right, hits, 1.6f) > 0)
            {
                if(speed > 29){
                speed = Mathf.Lerp(speed,speed - 2, 12 * Time.deltaTime);
                }
                sparks_wallhit.gameObject.SetActive(true);
                sparks_wallhit.transform.position = hits[0].point;
                Debug.Log("a wall");

                anim.SetFloat("wall_collide_direction", input_snap);
                anim.SetLayerWeight(anim.GetLayerIndex("Wall_Collide") ,0.6f);

                turnSpeed = 0.2f;

                if(speed < 35)
                {
                    Vector3 analogStickInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    // Determine if the analog stick is held in the opposite direction of the wall
                    if (Vector3.Dot(analogStickInput.normalized, hits[0].normal) < -0.5f)
                    {
                        float rotationAngle = 5f;
                        // Determine the side of the collision
                        float dotProduct = Vector3.Dot(transform.right, hits[0].normal);
                        if (dotProduct > 0)
                        {
                            // Player is on the left side of the wall
                            transform.Rotate(0, rotationAngle, 0);
                        }
                        else
                        {
                            // Player is on the right side of the wall
                            transform.Rotate(0, -rotationAngle, 0);
                        }
                    }
                }
        
            }
            else 
            {          
                sparks_wallhit.gameObject.SetActive(false);
                anim.SetLayerWeight(anim.GetLayerIndex("Wall_Collide") ,0f);

                ZeroSpd_OnCol();
            }   
            
            
            
        }
        else
        {  
            sparks_wallhit.gameObject.SetActive(false);
            anim.SetLayerWeight(anim.GetLayerIndex("Wall_Collide") ,0f);

        }

        if(sonic_action == Actions.wall_hit_heavy)
        {
            speed= 0;
            rb.velocity = Vector3.zero;
            movement = Vector3.zero;
            input = Vector2.zero;
        }
       
    }

	void GroundCheckSlope()
    {
        
        if(Physics.Raycast(transform.position, -transform.up, out hit , 1.8f,LayerMask.GetMask("Default")))
        {
            if (hit.collider != gameObject)
            {
                
                if(controlLock)
                {
                    groundSlopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                    if(groundSlopeAngle < 30 && controlTimer < 0.1f)
                    {
                        
                        LookAt_Slope(true);
                        
                        canControl = false;
                        Vector3 slopeTangent = Vector3.Cross(Vector3.up, hit.normal).normalized;
                        // Calculate the desired forward direction based on the slope tangent
                        Vector3 desiredForward = Vector3.Cross(hit.normal, slopeTangent);
                        // Check if the slope angle is not flat (i.e., not close to 0 degrees)
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-desiredForward), 10 * Time.deltaTime);
                        //speed = 20;
                        
                        StartCoroutine(canControlTrue(0.5f));
                    }
                }
                else
                if (hit.distance <= 1.1f)
                {
                    isGrounded = true;
                    GroundNormal = hit.normal;  
                    reAlign = true;
                }  
                if(reAlign && groundSlopeAngle < 40)
                {
                    Vector3 velocity = rb.velocity;
                    velocity.y = 0; // flatten vertical movement
                    rb.velocity = velocity;

                    Vector3 pos = rb.position;
                    pos.y = hit.point.y + yOffset;
                    rb.position = pos;
                    reAlign = false;
                }
                

                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation =  Quaternion.Lerp(transform.rotation,  targetRotation * transform.rotation , 20 * Time.deltaTime) ;

                groundSlopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            }
        }
        else
        {
            isGrounded = false;
            GroundNormal = Vector3.up;
            transform.rotation =  Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }

///////////////////////////  Physics Control  //////////////////////
    void SlopePhysics()
    {

    // Increase maxSpeed when going downhill
        Vector3 temp = Vector3.Cross(GroundNormal, transform.forward);
        Vector3 myDirection = Vector3.Cross(temp, GroundNormal);
        float normalizedSlope1 = Mathf.InverseLerp(0, 180, groundSlopeAngle);

        if (groundSlopeAngle > 15 && myDirection.y < 0 && isGrounded)
        {
            speed += slope_gain * 23 * Time.deltaTime;
            maxSpeed += slope_gain * Time.deltaTime;

        }

        // Reduce speed going uphill
        if (groundSlopeAngle > 15 && myDirection.y > 0 && isGrounded )
        {
            float normalizedSlope = Mathf.InverseLerp(0, 180, groundSlopeAngle);
            speed -= slope_drag * 6 * normalizedSlope  * Time.deltaTime;
           
            
        }

        if(groundSlopeAngle > 40 )
        {
            if(speed < 4)
            {
                if(isGrounded) transform.forward *= -1;
                transform.rotation =Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
                if(!controlLock)
                {
                    isGrounded = false;
                    lastGrounded = false;
                    controlLock = true;
                }
            }
        }
        else
        {
            if (groundSlopeAngle < 40 && groundSlopeAngle > 20 && speed < -0.1f)
            {
                if(!controlLock)
                {
                    transform.forward *= -1;
                    controlLock = true;
                }
            }
        }    
        if(controlLock)
        {
             
            if (controlTimer < 0 && isGrounded || groundSlopeAngle < 15)
            {
                done = false;
                controlTimer = 0.2f;
                controlLock = false;
                reAlign = true;
            }
             
             
             if (isGrounded)
            {
                controlTimer -= Time.deltaTime;
                turnSpeed = 0.2f;
                Vector3 slopeTangent = Vector3.Cross(Vector3.up, hit.normal).normalized;
                Vector3 desiredForward = Vector3.Cross(hit.normal, slopeTangent);

                // Check if the slope angle is not flat (i.e., not close to 0 degrees)
                if (groundSlopeAngle > 5 ) // Adjust this threshold as needed
                {
                    Vector3 downhill =
                    Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;

                    transform.rotation = Quaternion.LookRotation(downhill, Vector3.up);
                    
                }
                //Apply
                movement = Vector3.ProjectOnPlane(transform.forward, hit.normal) * speed;
                

    
                }
            }
            
        // MaxSpeed reset when too slow
        if(speed > maxSpeed_og)
        {
            if(input.magnitude < 0.1f || speed < maxSpeed_og )
            {
                maxSpeed = maxSpeed_og;
            }
        }
    
    





    }

    void LookAt_Slope(bool smooth)
    {
        Vector3 slopeTangent = Vector3.Cross(Vector3.up, hit.normal).normalized;
        Vector3 desiredForward = Vector3.Cross( hit.normal, slopeTangent);

        // Rotate the player to face the slope
        transform.rotation = smooth ? Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-desiredForward) * transform.rotation, 12 * Time.deltaTime):
        Quaternion.LookRotation(-desiredForward);
    }

/////////////////////////// Gravity Control//////////////////////
    void HandleGravity()
    {
        if (isGrounded)
        {
            // Stick to the ground
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, GroundNormal);
            rb.velocity += -GroundNormal * groundStickPower * Time.deltaTime;
        }
        else
        {
            // Apply gravity
            rb.velocity += !air_boost ? Vector3.down * gravity * Time.deltaTime :Vector3.down * gravity/2 * Time.deltaTime ;
        }
    }

    void ZeroSpd_OnCol()
    {
        if(Physics.Raycast(fwd_pos.position ,transform.forward, out hit_fwd ,0.5f, LayerMask.GetMask("Default", "Wall Collisions")) )
        {
            if(hit_fwd.collider.gameObject != this.gameObject && !(hit_fwd.collider.gameObject.CompareTag("Enemy")))
            {
                if(speed > 32  && groundSlopeAngle < 10 && isGrounded && sonic_action != Actions.wall_hit_heavy)
                {
                    sfx.playActionSound(sfx.collision);   
                    speed = 0f;   
                    boost = false;
                    sonic_action = Actions.wall_hit_heavy;
                    speed =0;
                    rb.velocity = Vector3.zero;
                    canControl = false;
                    StartCoroutine(EndWallHitCooldown(1.9f));
                }
                
                
                if(speed > 8 && speed < 26 && sonic_action != Actions.wall_hit_heavy)
                {
                    if(hit_fwd.collider.gameObject.layer != LayerMask.GetMask("Homing"))
                    {
                        transform.rotation =Quaternion.Lerp(transform.rotation,  Quaternion.LookRotation( Vector3.Reflect(transform.forward,hit_fwd.normal) ) , 10 * Time.deltaTime);
                        speed= -6f;
                        boost = false;
                        
                        
                    }
                }
            }
                // Enemy Collisions
                if(hit_fwd.collider.gameObject.CompareTag("Enemy") && sonic_action != Actions.wall_hit_heavy) 
                {
                    if(sonic_action == SonicController.Actions.none && !boost)
                    {
                        speed=  0f;
                    }
                    if(boost)
                    {
                         // Retain the player's forward velocity after collision
                        Vector3 forwardDirection = transform.forward.normalized;

                        // Set Rigidbody velocity to the stored value or maintain boost speed
                        rb.velocity = forwardDirection * speed;
                        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed , rb.velocity.z); // Maintain vertical velocity
                        
                        speed = speed + 5f; // Increase speed after hitting enemy while boosting
                        jumpingOffEnemy = true;
                        sonic_col.EnemyHit(hit_fwd.collider.gameObject);
                        score.boostGauge += 0.3f;
                    }
                    
                }

                 if(rb.velocity.y < 0 && !isGrounded &&Vector3.Angle( hit_fwd.normal ,Vector3.up) > 90 )
                {
                    rb.velocity += -gravity * transform.up ;
                    Debug.Log("pushin y down");
                }
           

            
            
        }

        
    }
    IEnumerator EndWallHitCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        speed = 0f;
        canControl = true;
        sonic_action = Actions.none;
    }

	void Movement()
    {   
        input = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        input = Vector2.ClampMagnitude(input, 1);
        
        // Camera forward and right vectors:
         forward = cam.transform.forward;
         right = cam.transform.right;

        forward.y = 0f;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

       
        // Calculate the rotation based on the hit normal and input
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        localMove = rotation * (forward * input.y + right * input.x) * speed;

        Vector3 jumpVector = !boost ? 
        ((forward * input.y )+ (right * input.x)/1.5f) * (speed < 10 ? 14 : speed)
        :movement;
        
        //air movement 
        airVector = Vector3.Lerp(rb.velocity,  ( transform.forward * (controlLock ? 0 : speed)) + Vector3.Project(rb.velocity, rb.transform.up), jump_lerp);

        jump_lerp =  Mathf.MoveTowards(jump_lerp, !isGrounded ?  (jumpHeight > 3.5) ? jump_lerp_apex : jump_lerp_start : jump_lerp_start, isGrounded ? 18 * Time.deltaTime : 8 * Time.deltaTime);

        
        // Set fwd movement
        movement = !controlLock ? 
        isGrounded ? 
        ((Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized * speed) + right * input.x * sideway_speed  + new Vector3(landingBoost.x, 0, landingBoost.z)):
        sonic_action == Actions.none ? airVector : movement
        : (Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized * speed);
        
         /////// Add force upon landing (when landing on a slope, it forces the player to fall in direction of slope)
        landingBoost = Vector3.Lerp(landingBoost, Vector3.zero, Time.deltaTime * DecaySpeed);

        
        //air movement 
		if(!isGrounded)
        {
            movement.y = rb.velocity.y;
        }

        if(canControl  )
        {
            if(!controlLock)
             rb.velocity =movement ; 
        }

        //face direction    
         if (input.magnitude > 0 && !(Vector3.Angle(transform.forward, localMove) >= 150 ) && canControl )
        {
            
                if(sonic_action != Actions.airDash){

                    rot = (!controlLock) ?  Quaternion.FromToRotation(transform.up, hit.normal) * Quaternion.LookRotation(forward * input.y + right * input.x) : rot;


                    // Do instant snappy turning if speed is blow 5 (just staring out)
                    transform.rotation = controlLock ? Quaternion.Slerp(transform.rotation, rot.normalized, Time.deltaTime * 1f) :
                    speed > 5 ? Quaternion.Slerp(transform.rotation, rot.normalized, Time.deltaTime * turnSpeed) 
                    : Quaternion.Slerp(transform.rotation, rot.normalized, Time.deltaTime * 9);

                }
            
        }

        float tS = speed / 23; 
        ///// CHANGE 18 TO 9 IF TH TURNSPEED TRANSITION IS TOO HIGH OR TOO FAST

        if(!controlLock)
        {
            if(isGrounded){
            turnSpeed = (hit_fwd.collider != null) ? 1f :  
            Mathf.Lerp(turnSpeedHigh, turnSpeedLow, tS);
            }
            else
            {
            if(jumpHeight > 3) turnSpeed = Mathf.Lerp(turnSpeed, turnSpeedJump, jumpHeight * Time.deltaTime);
            }
        }
        else
        {
            turnSpeed = 0.4f;
        }

        if(twoDmode)
        {
            input = new Vector2(Input.GetAxis("Horizontal"),0);
            Position();
        }


        
        //CHANGE TURNSPED TO 0.4F IF WERE AT maximum possible speed
        if(speed > maximumSpeed - 3)
        {
            turnSpeed = 0.6f;
        }
        Vector2 inputAcc = new Vector2(Input.GetAxis("Horizontal")  /  ( ((sonic_action != Actions.drift) || speed > 17.5f) ? (1.4f) : (0) ),Input.GetAxis("Vertical")) ;


        // Acceleration
        if( ( !controlLock ) || (sonic_action == Actions.none) || (!boost) )
        {
            if(isGrounded){
                if(inputAcc.magnitude > 0)
                {                                               // Dont use magnitude to maxSpeed when drifting casue you lose speed
                    if(speed < maxSpeed/ 3)
                    {
                        speed = Mathf.MoveTowards(speed,maxSpeed * ((sonic_action != Actions.drift) ? inputAcc.magnitude : 1) , acc * 1.3f * Time.deltaTime);
                    }
                    else
                    {
                        if(groundSlopeAngle > 20)
                        {
                            speed = Mathf.MoveTowards(speed,maxSpeed * ((sonic_action != Actions.drift) ? inputAcc.magnitude : 1) , (acc - 2) * Time.deltaTime);
                        }
                        else{
                            speed = Mathf.MoveTowards(speed,maxSpeed * ((sonic_action != Actions.drift) ? inputAcc.magnitude : 1) , acc * Time.deltaTime);
                        }
                    }
                }
                else
                {
                     Vector3 slope = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                    // True downhill direction based on slope normal
                    Vector3 slopeRight = Vector3.Cross(Vector3.up, hit.normal); // Perpendicular to normal
                    Vector3 slopeDownhill = Vector3.Cross(slopeRight, hit.normal).normalized; // Tangent pointing downhill
                    if(!(Vector3.Dot(transform.forward, slopeDownhill) > 0f))
                    {
                        if(sonic_action != Actions.drift || sonic_action != Actions.q_step)
                        {
                            speed = Mathf.MoveTowards(speed,0 , 
                            (groundSlopeAngle > 20 || speed > maxSpeed/2 || isGrounded) ? fric * Time.deltaTime :
                            fric * Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                if(sonic_action != Actions.drift){

                    if(jumpHeight > 3.4f && jumped && !lastGrounded && speed > 10 && !boost)
                    {   if(jumpingOffEnemy == false)
                        {
                            //set velocity magnitude
                            shorthop = false;
                            speed = Mathf.MoveTowards(speed, speed-18 , 25 * Time.deltaTime);
                            

                            //limit speed
                            rb.velocity =new Vector3( Vector3.ClampMagnitude(rb.velocity,speed).x, rb.velocity.y, Vector3.ClampMagnitude(rb.velocity,speed).z);

                            
                        }
                    }
                    if(speed < 4 * input.magnitude)
                    {
                        speed = Mathf.MoveTowards(speed,input.magnitude * 4, 5 * Time.deltaTime);
                    }
                }
            }
        
            speed = isGrounded ? Mathf.Clamp(speed,-maximumSpeed,maximumSpeed) : Mathf.Clamp(speed,0,maximumSpeed);   
        }


        
        ///Reduce speed if trun angle is too harsh
        if(groundSlopeAngle < 30 && !boost && (sonic_action != Actions.drift) &&sonic_col.water == false)
        {
            if(isGrounded)
            {
                if(speed < 9f)
                {
                    if(Vector3.Angle(transform.forward, (forward * input.y + right * input.x)) >= speedAnalogThreshold - 20)
                    {
                        speed = Mathf.MoveTowards(speed,0, 15* Time.deltaTime);
                    }
                } 
                if(speed < 19f)
                {
                    if(Vector3.Angle(transform.forward, (forward * input.y + right * input.x)) >= speedAnalogThreshold)
                    {
                        speed -= (isGrounded ? speedAnalogLoss * 2.5f : speedAnalogLoss * 2.5F) * Time.deltaTime;
                    }
                } 
                if(speed > 19f && (speed < 29.5f))
                {
                    if(Vector3.Angle(transform.forward, (forward * input.y + right * input.x)) >= speedAnalogThreshold/2)
                    {
                        speed -= (isGrounded ? speedAnalogLoss * 1.5F: speedAnalogLoss * 1.5F) * Time.deltaTime;
                    }
                } 

                if(speed > 29.5f)
                {
                    if(Vector3.Angle(transform.forward, (forward * input.y + right * input.x)) >= speedAnalogThreshold)
                    {
                        speed -=  (isGrounded ? speedAnalogLoss  : speedAnalogLoss ) * Time.deltaTime;
                        speed = Mathf.Clamp(speed,0,maxSpeed);
                    }
                }
                
            }
            else
            {
                if(Vector3.Angle(transform.forward, (forward * input.y + right * input.x)) >= speedAnalogThreshold / 2)
                {
                    speed -= (speedAnalogLoss * 2) * Time.deltaTime;
                }
            }
        }


        if(Vector3.Angle(transform.forward, localMove) > 150 && speed > 20 && isGrounded)
        {
            speed -= skidLerp * 10 * Time.deltaTime;
            anim.SetBool("Skid", true );
            if(speed > 19)
            sfx.actionAudioSource.PlayOneShot(sfx.breakSoundClip);
        }
        else
        { 
            anim.SetBool("Skid", false );
        }

        // Skid
		if (Vector3.Angle(transform.forward, localMove) >=120   && speed > 1 && !controlLock && isGrounded)
        {  SkidTurnaround(speed > 30 ? 8 : skidLerp * 15  );}


   
    //////////// DEBUGGIN ///////////////////////////////////////
        Debug.DrawRay(transform.position, localMove, Color.red);
    }

    public void Position()
		{
			if ( path )
			{
				Vector3 p = rb.position;    //transform.position;

				Vector3 tangent = Vector3.zero;
				int kn = 0;

				Vector3 np = Vector3.zero;
				if ( usealpha )
					np = path.transform.TransformPoint(path.InterpCurve3D(curve, alpha, true));
				else
					np = path.FindNearestPointWorldXZ(p, 15, ref kn, ref tangent, ref alpha);

				Vector3 p1 = path.transform.TransformPoint(path.InterpCurve3D(curve, alpha + 0.0001f, true));
				Vector3 cross = Vector3.Cross((p1 - np).normalized, Vector3.up) * horizontalPos;
				np += cross;    //Vector3.Cross((p1 - np).normalized, Vector3.up) * horizontalPos;

				nps = np;
				np.y = p.y;
				Vector3 dir = np - p;
				dir.y = 0.0f;

				Vector3 iforce = dir * impulse;

				float mag = iforce.magnitude / Time.fixedDeltaTime;
				if ( mag > breakforce )
				{
					rb.angularVelocity = Vector3.zero;
				}

					rb.AddForce(iforce, ForceMode.Impulse);

					np.y = p.y;
					rb.MovePosition(np);

					p1 = path.transform.TransformPoint(path.InterpCurve3D(curve, alpha + 0.0001f, true));
					p1 += cross;
					p1.y = p.y;

					if ( align )
					{
						Vector3 ndir = (p1 - np).normalized;
						Vector3 rdir = Vector3.forward; //transform.forward;

						switch ( forwardAxis )
						{
							case MegaAxis.X:
								rdir = transform.right;
								break;
							case MegaAxis.Y:
								rdir = transform.up;
								break;
							case MegaAxis.Z:
								rdir = transform.forward;
								break;
						}
						rdir.y = 0.0f;
						rdir = rdir.normalized;

						float angle = Vector3.Angle(rdir, ndir);

						cross = Vector3.Cross(rdir, ndir);

						if ( cross.y < 0.0f )
							angle = -angle;

						Quaternion erot = Quaternion.Euler(rotSpline);
						Quaternion qrot = rb.rotation;
						Quaternion yrot = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f)); //LookRotation(p1 - np);	//.eulerAngles;

						//rb.MoveRotation(qrot * yrot * erot);
                        float moveDir = ndir.x;

                     


					if ( drag != 0.0f )
						rb.AddForce(-rb.velocity * drag);

					if ( drive != 0.0f )
						rb.AddForce((np - p1).normalized * drive, ForceMode.Force);
				}
			}


           
		}

 

    void JumpInput()
    {      
        if(!isGrounded)
        {   
            // Update jump height if player is still rising
            jumpHeight = Mathf.Max(jumpHeight, transform.position.y - startPos);
            lastGroundedTimer -= Time.deltaTime;
            if(lastGroundedTimer < 0)
            {
                lastGrounded = false;
            }
            if(rb.velocity.y > 0 && jumpHeight > minimumJumpHeight)
            {
                minJumpReached = true;
            }
        }
        else
        {
            startPos = transform.position.y;
            jumpHeight = 0f;
            lastGrounded = true;
            lastGroundedTimer = lastGroundedTimerOG;
            jumpingOffEnemy = false;
            minJumpReached = false;
        }

        if (Input.GetButtonDown("X") && lastGrounded && sonic_col.current_action == SonicObejctInteraction.action.none   )
        {   
            if(!jumped)
            {
                rb.velocity = new Vector3(rb.velocity.x,0 ,rb.velocity.z);
                sfx.playActionSound(sfx.jump);
            }
            if(sonic_action == Actions.slide || sonic_action == Actions.crouch)
            {
                StartCoroutine(leaveCrouchState(0f));
            }
            
            isGrounded = false;
            lastGrounded = false;
            jumped = true;
            
            transform.position = new Vector3(transform.position.x,transform.position.y + 0.7f,transform.position.z);
            
            sfx.PlayJumpSound();
            
            rb.velocity  =transform.up * jumpSpeed;
        }
        
        
        if (Input.GetButtonUp("X") && !isGrounded && sonic_col.current_action == SonicObejctInteraction.action.none && (sonic_action != Actions.attack) && (sonic_action != Actions.homing_trick))
        {
            if(minJumpReached)
            {
                if(!jumpingOffEnemy)
                {
                    if(jumpHeight < 4.5f && !shorthop )
                    {
                        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.1f ,rb.velocity.z);
                        shorthop = true;
                    }
                    if (rb.velocity.y > 0 )
                    {
                        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.3f ,rb.velocity.z);
                    }
                }
            }
            
            
        }

        //set jumps bool etc
        if(isGrounded){
            if(jumped && isGrounded)
            {
                anim.SetLayerWeight(anim.GetLayerIndex("Land_Impact"),  1);
                Vector3 downhillDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                landingBoost = downhillDir * boostStrength;
            }
            anim.SetTrigger("Landing Trigger");
            
            jumped = false;
            shorthop = false;
            velocity_mag = rb.velocity.magnitude;

        }

        //additional gravity when player falling
        if(rb.velocity.y < 0)
        {
            rb.velocity += -transform.up * ((gravityWhenFall) - 1) * Time.deltaTime;
        }    
    }
    void SkidTurnaround(float skidLerp)
    {
        if(!controlLock){
            if (speed < 6    )
            {   
                speed= Mathf.MoveTowards(speed, 0 , 2 * Time.deltaTime);
                if(speed < 2)
                {transform.forward *= -1;    }
            anim.SetBool("Skid", false );}   

            turnSpeed = 0.3f;
            float skidVelocity = (speed * -1);

            if(speed > 10)
            { 
                anim.SetBool("Skid", true );
                speed = Mathf.MoveTowards(speed, -4f , skidLerp * Time.deltaTime);  
            }
            else
            speed = isGrounded ? Mathf.MoveTowards(speed, -4f , (speed > maxSpeed_og - 4 ? skidLerp * maxSpeed : skidLerp) * Time.deltaTime) :
            (speed > speed/2) ? Mathf.MoveTowards(speed, (speed/2), skidLerp * 2* Time.deltaTime) :
            Mathf.MoveTowards(speed, (0), skidLerp * 2* Time.deltaTime);    
        }
              
        
    }


    public void SonicHurt()
    {
        if(hurt)
        {
            ringPool.SpawnRings();
            canControl = false;
            speed = 0f;
            
            rb.velocity = -transform.forward * 10f;
            anim.SetBool("dead", true);
            Invoke("returnControl",0.7f);
        }
        
    }
    void returnControl()
    {
        hurt = false;
       canControl = true ;
       anim.SetBool("dead", false);
    }


//////////// SONIC ACTIONS  ///////////////////////////////////////

    void HomingAttack()
    {
        AirDashing();
        if(sonic_action != Actions.attack){
        // Use OverlapSphere to detect all colliders within the specified radius
        if(!isGrounded){
         hitColliders = Physics.OverlapSphere(HomingAttackCheck.transform.position, radius,  LayerMask.GetMask("Homing"));

         // Iterate through all detected colliders
            foreach (var hitCollider in hitColliders)
            {
                if (hitColliders.Length > 0 && sonic_action != Actions.attack && !isGrounded)
                {
                    attackObj = hitColliders[0].gameObject;
                }
            }
        }
        }

        
        //cancel homing attack if the following occur
        if( sonic_action == Actions.attack)
        {
            if(hurt){
                canControl = true;
                sonic_action = Actions.none;
            }
            if(attackObj == null)
            {
                canControl = true;
                sonic_action = Actions.none;
            }
        }
        


        // start homing attack
        if (!isGrounded && Input.GetButtonDown("X") && attackObj != null && rb.velocity.y < 1 && !d && (sonic_action != Actions.attack) && !boost)
        {
            homingTrickRandomiser = Random.Range(1,6);
            PerformAction (Actions.attack);
            // reset maxSpeeds
            maxSpeed = maxSpeed_og;
            
        }

        // start airdash
        if (!lastGrounded && Input.GetButtonDown("X") && attackObj == null && sonic_action != Actions.attack && rb.velocity.y < -1 && !d)
        {
            rb.velocity = Vector3.zero;
            PerformAction( Actions.airDash);
            sfx.PlayADSound();
            d = true;
        }

        // if homing attack start
        if (sonic_action == Actions.attack)
        {
            Vector3 attackDirection = attackObj.transform.position - transform.position;
            rb.velocity = attackDirection.normalized * 125;

            // KILL ON COLLISION
            float sqrDistance = attackDirection.sqrMagnitude;
            if (sqrDistance < 15)
            {
                HomingAttackAction();
                attackObj = null;
                canControl = true;
                speed = 0;
                rb.velocity = new Vector3(rb.velocity.x/2 ,rb.velocity.y ,rb.velocity.z/2);
                homingAttackfailSafe = 5;
            }
            else if (attackObj == null || isGrounded)
            {
                sonic_action =(Actions.none);
                attackObj = null;
                canControl = true;
                attackObj = null;
            }
            homingAttackfailSafe -= Time.deltaTime;
            if(homingAttackfailSafe < 0 && sqrDistance > 15)
            {sonic_action = Actions.none; homingAttackfailSafe = 5; }

            
        }

        if (sonic_action == Actions.homing_trick)
        {
            
            canControl = false;
            speed = 0;
            if(rb.velocity.y > -1)
            {
                //keeping speed at a low number to prevent sudden forward shift when y < 0
                speed = input.magnitude * 6f;
                rb.velocity = new Vector3(0 ,rb.velocity.y ,0); 
                sonic_action = Actions.homing_trick;       
            } 
            else 
            {
                PerformAction(Actions.none);
                canControl = true;

                //additional gravity when player falling
                rb.velocity += -transform.up * ((gravityWhenFall) ) * Time.deltaTime;
            }
            
        }
    }

    void AirDashing()
    {
        if(sonic_action == Actions.airDash)
        {
            //zero out input
            /*
            speed = 5f;
            maxSpeed = maxSpeed_og;
            input = Vector3.zero;
            Vector3 newForce = transform.forward *  8 * airDashSpeed;
            newForce.y =0;
            rb.velocity = (newForce);
            airDashSpeed -= 12* Time.deltaTime;
            */
            turnSpeed = 0f;
            if(airDashSpeed < 1 || isGrounded)
            {
                PerformAction (Actions.none);
                
                airDashSpeed = 5f;
            }

            //zero out input
            maxSpeed = maxSpeed_og;
            
            rb.velocity = (transform.forward * speed);

            speed = speed > 15 ? speed : 15  + airDashSpeed ;
            airDashSpeed -=  12* Time.deltaTime;

            if(airDashSpeed < 1 || isGrounded)
            {
                sonic_action = (Actions.none);
                
                airDashSpeed = 5f;
            }
        }
        if(isGrounded)
        {
            d = false;
        }
    }
    void HomingAttackAction()
    {
        
        
        if(attackObj.gameObject.CompareTag("Spring"))
        {
            if(attackObj != null){
                sonic_col.actionObject = attackObj.gameObject;
            sonic_col.SetActionObject(attackObj.gameObject); 

            Spring spring = sonic_col.actionObject.GetComponent<Spring>();
            sonic_col.SpringInit(spring);
            }
            attackObj = null;
            sonic_action = Actions.none;

        }
        if(attackObj.gameObject.CompareTag("pulley"))
        {
            sonic_col.pulleyOnCollision(attackObj.gameObject.GetComponent<Pulley>());
            
        }

        if(attackObj.gameObject.CompareTag( "Enemy"))
        {
            rb.velocity  = new Vector3(rb.velocity.x, jumpSpeed + 3,rb.velocity.z);
            sonic_action = Actions.homing_trick;
            

            sfx.playActionSound(sfx.enemyhit);
            enemyExplode.SpawnExplode(attackObj.gameObject.transform.position);
            attackObj.gameObject.GetComponent<Enemy_Death>().kill_enemy( new Vector2 (30,5 ));

            actionchain += 1;

            attackObj.gameObject.layer = LayerMask.GetMask("Ignore Raycast");
            attackObj.GetComponent<CapsuleCollider>().enabled = false;
            anim.SetBool("Homing_Attk", true);
            Invoke("set_anim_false", 0.5f);
            attackObj = null;
        } 

        attackObj = null;
    }
    void set_anim_false()
    {
        anim.SetBool("Homing_Attk", false);
        sfx.playActionSound(sfx.homingtrick);
    }   

    void BOOST()
    {

        if(score.boostGauge > 1)
        {
            if(Input.GetButtonDown("Square") && !boost)
            {
                 sfx.PlayBoostSound();
                score.boostGauge -= (int)boost_take_away;
                if(isGrounded){ boost = true; canControl = true;}
                else 
                {
                    air_boost = true;
                    rb.velocity = new Vector3(rb.velocity.x,0,rb.velocity.y);
                }
                
                speed = maximumSpeed+4;
                sfx.playActionSound(sfx.boost);
                Vector3 flatForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                if (flatForward.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(flatForward);
                }
            }
        }

        if(boost)
        {

            turnSpeed = 0f; 
            speed += 2.5f * Time.deltaTime;
            maxSpeed = maximumSpeed;
            
            if(sonic_col.current_action != SonicObejctInteraction.action.none )
            {
                speed = Mathf.MoveTowards(speed,maxSpeed_og,5 * Time.deltaTime);
                if(speed <= maximumSpeed -10 && Vector3.Angle(transform.forward, localMove) >= 120)
                {
                    boost = false;
                    b_timer = boost_timer;
                    b_timer = 0;
                  
                }
                
            }
            
            b_timer += Time.deltaTime;
            if(b_timer > boost_timer)
            {
                b_timer = 0;
                boost = false;
            }
            if(speed <= maximumSpeed -10 && Vector3.Angle(transform.forward, localMove) >= 120)
            {
                boost = false;
                b_timer = boost_timer;
                b_timer = 0;
                
            }

            if(!isGrounded)
            {
                localMove.x = localMove.x/2;
            }
            
            
        }

        if(!boost)
        {
            b_timer = 0;
        }
        if(air_boost)
        {
            if(!isGrounded)
            {
                speed = maximumSpeed;
                maxSpeed = maximumSpeed;
                score.boostGauge -= boost_take_away * Time.deltaTime;
                
                 
                
                b_timer += Time.deltaTime;
                if(b_timer > boost_timer)
                {
                    b_timer = 0;
                    boost = false;
                    air_boost = false;
                }

                localMove = transform.forward * speed;
                rb.velocity += Vector3.up * gravity/2 * Time.deltaTime;
            }
            
        }
        if(isGrounded && boost || air_boost)
        {
            if(b_timer < boost_timer )
            {
                air_boost = false;
                boost = true;
            }
        }
        //boost model
        boostModelFX.SetActive(boost || air_boost);
    }
    IEnumerator airboost_false(float time)
    {
        yield return new WaitForSeconds (time);
        air_boost = false;
    }
    IEnumerator canControlTrue(float time)
    {
        yield return new WaitForSeconds (time);
        canControl = true;
        controlLock = false;
       
        if (isGrounded)
            {
                
            }
    }
	void Drift()
    {
        d_horiz = Mathf.RoundToInt(input.x);
        float driftSpeedTotal = Mathf.MoveTowards(12 ,driftSpd, 10 * Time.deltaTime);

		// Check if Fire1 button is pressed (you can replace "Fire1" with your desired button input)
		if (Input.GetButtonDown("R") &&  sonic_action != Actions.drift && speed > 10 && canDrift)
		{
            delay_timer = driftDelay;
            startdir =  d_horiz ;
            drift_direction = d_horiz * drift_turnSpeed;
			if (isGrounded && input.x != 0f )
			{          
				PerformAction(Actions.drift);
                old_spd = speed;
				canControl = false;
			}
		}
        if ((Input.GetButtonUp("R") || speed < 14 || !isGrounded  ) && sonic_action == Actions.drift)
        {
            EndDrift();
            sfx.playActionSound(sfx.drift_finish);
        }
        if(sonic_col.current_action != SonicObejctInteraction.action.none && isGrounded && sonic_action == Actions.drift)
        {
            EndDrift();
            sfx.playActionSound(sfx.drift_finish);
        }

        if( !canDrift )
        {
            delay_timer -= 1 * Time.deltaTime;
            if(delay_timer < 0)
            {
               delay_timer = driftDelay;
               canDrift  =true;
            }
        }

		// Enable canControl if drift is false
		if (sonic_action == Actions.drift)
		{
            canControl = false;

			Vector3 driftDir = Vector3.ProjectOnPlane(transform.forward, GroundNormal).normalized;
            rb.velocity = driftDir * speed + cam.transform.right * (-d_horiz * driftSpeedTotal);


            /*

            IF we are drift, 

            drift direction 0 to 30
            move player left and right. can slide but lose speed slowly

            drift 30+
            fast drift and auto add drift to keep siding

            leave drift--
            if sliding, remove speed slowly 

            if not(drift dir above 30) 
            check if player leaves input for like a microsecond and stop drift 

            */


            if(d_horiz > 0)
            {
                if(drift_direction < 30 ){
                drift_direction = Input.GetAxisRaw("Horizontal") * rotationSpeedDrift; 
                //speed = Mathf.MoveTowards(speed, speed -16, fric * Time.deltaTime);
                
                }
                else{
                drift_direction += (input.x * drift_turnSpeedIncrement)  * Time.deltaTime; }

            }
            else if(d_horiz < 0)
            {
                if(drift_direction > -30 ){
                drift_direction = Input.GetAxisRaw("Horizontal") * rotationSpeedDrift; 
                
                }
                else{
                drift_direction += (input.x * drift_turnSpeedIncrement)  * Time.deltaTime; }
            }

            transform.Rotate(0, drift_direction * Time.deltaTime,0);

            if(speed > 20 && notSliding() )
            {
                //maxSpeed+= 4.5f * Time.deltaTime;
                if(speed > maxSpeed/2 && input.y > 0.6f)
                {
                speed = Mathf.MoveTowards(speed, speed -16, speed > 40 ? 9 : 15 * Time.deltaTime);
                }
            }   

            drift_time += 1 *Time.deltaTime;

            //cancel drift if direction change or if drift time has run out
            if(startdir > 0 && drift_direction < -80 )
            {
                if(Input.GetAxis("Horizontal") < 0  )
                {
                    EndDrift();
                }
            }
            if (startdir < 0 && drift_direction > 80 )
            {
                if(Input.GetAxis("Horizontal") > 0  )
                {
                    EndDrift();
                }
            }

            

                // If input is close to zero for releaseDelay seconds, end the drift
                if (Mathf.Abs(input.x) < 0.01f)
                {
                    zeroInputTimer += Time.deltaTime;
                    if (zeroInputTimer >= releaseDelay)
                    {
                        EndDrift();
                    }
                }
            

            sparks.emissionRate = 50;
		}
        sparks.gameObject.SetActive(sonic_action == Actions.drift);

        drift_direction = Mathf.Clamp(drift_direction,-200,200);

	}
    bool notSliding()
    {
        if ((drift_direction > 120 && drift_direction > 0) || (drift_direction < -120 && drift_direction < 0))
            return true;

        return false;
    }


    void EndDrift()
    {
        canDrift = false;
        drift_time = 0f;
        PerformAction(Actions.none);
        canControl  = true;
        if(drift_time > 0.6f)
        {
            speed = maxSpeed;
        }
        drift_direction = 0;
        zeroInputTimer = 0f;
        drift_direction = 0f;
        
    }


    void QuickStep()
    {
         if (Input.GetButtonDown("Triangle") && isGrounded)
        {
            PerformAction(Actions.q_step);
             sfx.PlayQSSound();
        }
        if(sonic_action == Actions.q_step)
        {
            if(input_snap > 0){
            Vector3 forwardVelocity = transform.forward * rb.velocity.magnitude;
            Vector3 sidewaysVelocity = transform.right * (quickstepspd);
            rb.velocity = forwardVelocity + sidewaysVelocity;
            input_snap = 1;
            }
            if(input_snap < 0){
            Vector3 forwardVelocity = transform.forward * rb.velocity.magnitude;
            Vector3 sidewaysVelocity = -transform.right * (quickstepspd);
            rb.velocity = forwardVelocity + sidewaysVelocity;
            input_snap = -1;
            }
            

            StartCoroutine(EndQstep(quickstep_time));
        }
    }
    IEnumerator EndQstep(float time)
    {
        yield return new WaitForSeconds(time);
        sonic_action = (Actions.none);
    }    

    void Stomp()
    {
        if(!lastGrounded && Input.GetButtonDown("Circle") && sonic_action != Actions.stomp && jumpHeight >= 2)
        {
            sfx.playActionSound(sfx.stompClip);
            PerformAction (Actions.stomp);
            
        }
        if(sonic_action == Actions.stomp)
        {
            boost = false;
            if(!isGrounded){
            speed -= 10 * Time.deltaTime;
            rb.velocity -= new Vector3(0,stomp_speed,0) * Time.deltaTime;
            }
            else
            {
                sfx.playActionSound(sfx.stompLandClip);
                PerformAction (Actions.none);
            }

        }   
        stompMod.SetActive(sonic_action == Actions.stomp);
    }

    void CrouchSlide()
    {
       if (isGrounded)
        {
            if (Input.GetButton("Circle") && sonic_action != Actions.crouch)
            {
                PerformAction (Actions.crouch);
                canControl = false;
            }
            else if ( (Input.GetButtonUp("Circle") && (sonic_action == Actions.crouch || sonic_action == Actions.slide) ) || !isGrounded)
            {
                StartCoroutine(leaveCrouchState(0.2f));
            }

            if (sonic_action == Actions.crouch)
            {
                
                if (speed > 2)
                {
                    PerformAction ( Actions.slide );
                    
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
                

                if(input.magnitude != 0 && speed  < 2 ){
                    speed = 12f;
                    sonic_action= ( Actions.slide );
                    sfx.playActionSound(sfx.slide);
                }

                // Adjust the capsule collider for crouching
                capsuleCollider.height = 0.2f; // Set the desired height for the crouched state
                capsuleCollider.center = new Vector3(0f, -0.35f, 0f); // Adjust the center position based on the new height
                
            }
             if (sonic_action == Actions.slide)
                {
                sfx.LoopClip(sfx.slideClip);
                input = Vector2.zero;

                if (speed > 2)
                {
                    speed -= 6.5f * Time.deltaTime;
                    canControl = false;
                    //rb.velocity = transform.forward * speed + Vector3.down * gravity;
                    
                    rb.velocity = Vector3.Lerp(rb.velocity, transform.forward  * speed , 10 * Time.deltaTime);
                    
                    StartCoroutine(LookAtSlopeRoutine(1f));
                }
                else
                {
                    PerformAction ( Actions.crouch );
                    sfx.LoopClipStop();
                }

                
                // Adjust the capsule collider for crouching
                capsuleCollider.height = 0.2f; // Set the desired height for the crouched state
                capsuleCollider.center = new Vector3(0f, -0.35f, 0f); // Adjust the center position based on the new height
                

                if(jumped || !isGrounded)
                {
                    StartCoroutine(leaveCrouchState(0.2f));
                }
            }
        }

        
    }

    IEnumerator leaveCrouchState(float time)
    {
        yield return new WaitForSeconds(time); 

        canControl = true;
        PerformAction ( Actions.none );
        //transform.position = new Vector3(transform.position.x,transform.position.y + 1,transform.position.z);
        capsuleCollider.height = 2f; // Set the original height
        capsuleCollider.radius = 0.5f;
        capsuleCollider.center = Vector3.zero; // Set the original center position
    }
    IEnumerator LookAtSlopeRoutine(float time)
    {
        yield return new WaitForSeconds(time); 
        
        Vector3 slopeTangent = Vector3.Cross(Vector3.up, hit.normal).normalized;
        Vector3 desiredForward = Vector3.Cross( hit.normal, slopeTangent);

        // Rotate the player to face the slope
        if(sonic_action == Actions.slide){
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-desiredForward) * transform.rotation, 12 * Time.deltaTime);
        }
    }

    void EndSonicLife()
    {
        if(score.rings < 0)
        {
            speed = 0f;
            canControl = false;
            StartCoroutine(Ends(1f));
            ///dead////
            anim.SetBool("dead", dead);
        }
    }
    IEnumerator Ends(float time)
    {
        yield return new WaitForSeconds(time);
        dead = true;
    }
	void Animation()
    {
        if(score.rings != -1){ //stop animation shit when dead
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));
        anim.SetFloat("RawSpeed", speed);
        }
        anim.SetBool("isGrounded", lastGrounded);
        anim.SetBool("airDash", sonic_action == Actions.airDash);
        
        anim.SetFloat("driftHoriz",d_horiz );
        anim.SetBool("jumped", jumped);
        anim.SetBool("short_hop", shorthop);
        anim.SetBool("drift", sonic_action == Actions.drift );
        anim.SetBool("q_step", sonic_action == Actions.q_step);
        anim.SetBool("spline",  sonic_action == Actions.spline );
        anim.SetBool("stomp",  sonic_action == Actions.stomp );
        anim.SetBool("slide",  sonic_action == Actions.slide );
        anim.SetBool("crouch",  sonic_action == Actions.crouch);
        anim.SetBool("AttackEnemyHomingAttk",  sonic_action == Actions.attack);
        anim.SetBool("wall_hit_heavy",(sonic_action == Actions.wall_hit_heavy));
        anim.SetFloat("jumpHeight",jumpHeight);
        anim.SetBool("air_boost",air_boost);
        anim.SetBool("controlLock",controlLock);

        anim.SetFloat("abs_Yvel", Mathf.Abs( rb.velocity.y));

        anim.SetFloat("homingTrickRandomiser", (int)homingTrickRandomiser);

        ////Flying upwards animation interferes with homing attack animation.
        // 0'ing the value may work for the time being
        if(sonic_action == Actions.homing_trick && !isGrounded)
        {
            anim.SetFloat("Yvelocity", 0);
        }
        else
        {
            anim.SetFloat("Yvelocity", rb.velocity.y);
        }
       

        /////player object interaction
        anim.SetBool("rainbow_ring", sonic_col.current_action == SonicObejctInteraction.action.rainbow_ring );
        anim.SetBool("dashRamp",  sonic_col.current_action == SonicObejctInteraction.action.dashRamp );
        anim.SetBool("spring",  sonic_col.current_action == SonicObejctInteraction.action.spring );

        anim.SetBool("pulley",  sonic_col.current_action == SonicObejctInteraction.action.pulley);
        anim.SetBool("railGrinding",sonic_col.current_action == SonicObejctInteraction.action.rail);
        anim.SetBool("water",sonic_col.water);
        
        if(isGrounded && !jumped || sonic_action != Actions.none || sonic_col.current_action != SonicObejctInteraction.action.none)
        {
            float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("Land_Impact"));
                currentWeight = Mathf.Lerp(currentWeight, 0, 8 * Time.deltaTime);
                anim.SetLayerWeight(anim.GetLayerIndex("Land_Impact"), currentWeight);
        }
        if(!isGrounded && anim.GetLayerWeight(anim.GetLayerIndex("Land_Impact")) >0)
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Land_Impact"), 0);
        }
        
        
        if(speed > 3 && isGrounded)
        {
            if(speed < 20){
                anim.speed = 0.7f;
            } 
            if(speed > 22)
            {
                anim.speed = 1.4f;
            }
            if(speed > maxSpeed_og)
            {
                anim.speed = 1.8f;
            }
        }
        else
        {
            anim.speed = 1f;
        }

        anim.SetInteger("rings", score.rings);

        //running turn animations
        if(speed > 15 && (sonic_action != Actions.drift  && sonic_action != Actions.q_step ))
        {
            if(input.magnitude !=0 )
            {
                anim.SetFloat("horizontal", Mathf.MoveTowards( anim.GetFloat("horizontal") , Input.GetAxis("Horizontal"), 8f * Time.deltaTime) );
            }
            else
            anim.SetFloat("horizontal", Mathf.MoveTowards( Input.GetAxis("Horizontal") , 0, 2f * Time.deltaTime) );

        }
        else
        {
            anim.SetFloat("horizontal", Mathf.MoveTowards( anim.GetFloat("horizontal") , Input.GetAxis("Horizontal"), 18f * Time.deltaTime) );
        }
        
        
        anim.SetFloat("deadzone",input.magnitude);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HomingAttackCheck.transform.position, radius);
    }




    ///////////////////////STATES WORK/////////////////////////
    public void PerformAction(Actions newState)
    {
        // Check if the transition is valid
        if (CanTransition(sonic_action, newState))
        {
            // Exit current state
            ExitState(sonic_action);

            // Enter new state
            EnterState(newState);

            // Update current state
            sonic_action = newState;
        }
        else
        {
            Debug.Log("Cannot transition from " + sonic_action + " to " + newState);
        }
    }

    private bool CanTransition(Actions sonic_action, Actions nextState)
    {
        // Define the conditions for valid state transitions here
        // Example: Allow any state transition by default
        // Define the conditions for valid state transitions here
        if (sonic_action == Actions.none)
        {
            // Allow transition from Idle to any state
            return true;
        }
        if (sonic_action == Actions.stomp)
        {
             // Allow transition from Stomp to Slide
            return  nextState == Actions.slide || nextState == Actions.none;
        }
        else if (sonic_action == Actions.slide)
        {
            // Allow transition from Slide to Crouch or Stomp
            return nextState == Actions.crouch || nextState == Actions.stomp|| nextState == Actions.none;
        }

        if (sonic_action == Actions.attack)
        {
            // Allow transition from Slide to Crouch or Stomp
            return nextState == Actions.none || nextState == Actions.homing_trick ;
        }
        if (sonic_action == Actions.homing_trick)
        {
            canControl = true;
            // Allow transition from Idle to any state
            return true;
        }

        if (sonic_action == Actions.wall_hit_heavy)
        {
             // Allow transition from Idle to any state
            return false;
        }



        // By default, disallow all other state transitions
        return true;
    }

    private void ExitState(Actions state)
    {
        // Perform any necessary actions when exiting a state
        // Example: Stop any ongoing animations or reset variables
    }

    private void EnterState(Actions state)
    {
        // Perform any necessary actions when entering a state
        // Example: Play the corresponding animation or update variables
        
    }

  

}
