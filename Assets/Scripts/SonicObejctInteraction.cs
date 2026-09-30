using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using MegaFiers;
using UnityEngine.SceneManagement;
using Cinemachine;

public class SonicObejctInteraction : MonoBehaviour {
    private SFXmanager sfx;
    private SonicController sonic;
    private SonicScore score;

    public enum action { none, speedpad, spring, rainbow_ring, dashRamp ,pulley, rail}
    public action current_action;
    public GameObject actionObject;

    private Rigidbody rb;
    public PathFollower pathFollower;

    public float controlTime;
    float timer;
    private bool pulledUp;

    public float SplineSpd;
    bool fall;
    public bool water;

    public float objectIgnoreTime = 0.5f;


    Vector3 vel;
    float rb_vel_y_limit,
    savedspeed;
    void Awake()
    {
        sonic = GetComponent<SonicController>();
        score = GetComponent<SonicScore>();
        rb = GetComponent<Rigidbody>();
        pathFollower = GetComponent<PathFollower>();
        sfx = GetComponent<SFXmanager>();
        
    }

    void Update()
    {
        Debug.Log ("Object timer: " + timer);
        if(timer > 0 )
        {
            timer -= 1 * Time.deltaTime;
            if(timer < 0)
            {
                returnControl();
            }
        }

        /*
        if(sonic.isGrounded && sonic.canControl == false && (current_action == action.spring || current_action == action.rainbow_ring ))
        {
            returnControl();
        }*/
        if(sonic.isGrounded && (current_action == action.rainbow_ring ))
        {
             returnControl();
        }

        if(current_action == action.none){
        SplineSpd = 13 + sonic.speed/2;
        }
        pathFollower.speed = SplineSpd;

        SplinesController();
        if(current_action ==  action.speedpad)
        {
            Vector3 padForward = actionObject.transform.forward;
            padForward.y = 0; // Optional: lock to horizontal plane to prevent launching upward
            padForward.Normalize();
            sonic.movement = Vector3.ProjectOnPlane(padForward, sonic.hit.normal).normalized * sonic.speed;
            rb.velocity = sonic.movement;

        }
      

        // Dont accelerate player on wall collision midair
        if (!sonic.isGrounded && rb.velocity.y < -10)
        {
            RaycastHit[] slopedWallHit = new RaycastHit[3]; // Array to store raycast hits
            if (Physics.Raycast(transform.position,transform.forward, 2f, LayerMask.GetMask("Default") | LayerMask.GetMask("Wall Collisions")) )
            {
                sonic.speed = -1;
                if(Physics.Raycast(sonic.fwd_pos.transform.position, Vector3.down, 1.5f, LayerMask.GetMask("Default") | LayerMask.GetMask("Wall Collisions")) )
                {
                    Vector3 vel = rb.velocity;
                    vel.y = Mathf.Clamp(vel.y, -100, rb_vel_y_limit);
                    rb.velocity = vel;
                    sonic.speed = -2;
                    rb.velocity += Vector3.down * sonic.gravityWhenFall * Time.deltaTime;

                    fall = true;
                }
            }
            else
            {
                rb_vel_y_limit = rb.velocity.y;
            }
        }
        else
        {
            if(fall)
            {
                if (sonic.isGrounded)
                {
                    if(sonic.groundSlopeAngle > 30)
                    {
                    sonic.speed = 0;

                    transform.forward *= -1;
                    rb.velocity = new Vector3 (rb.velocity.x, -0, rb.velocity.z);

                    fall = false;
                    sonic.controlLock = true;
                    }
                }
            }
        }

      if(current_action == action.rail)
        {
            if(Input.GetButtonDown("X"))
            {
                objectIgnoreTime = 0.5f;
                current_action = action.none;
                sonic.sonic_action = SonicController.Actions.none;
                
                pathFollower.enabled = false;
                pathFollower.pathCreator = null;
                pathFollower.distanceTravelled = 0f;
                actionObject = null;
                sonic.canControl = true;
                rb.isKinematic = false;

                rb.velocity  =transform.up * sonic.jumpSpeed;
            }

            
        }else
        {
            if(objectIgnoreTime > 0)
            {
            objectIgnoreTime -= Time.deltaTime;
            }
        }



        
    }

    void SplinesController()
    {
       if(current_action == action.spring || current_action == action.rainbow_ring || current_action == action.pulley || current_action == action.rail)
		{
			VertexPath vertexPath;
			Vector3 point;

			switch(current_action)
			{
				case action.spring:
                if(actionObject.GetComponent<Spring>().spline){
                    vertexPath = new VertexPath(actionObject.GetComponent<Spring>().pathCreator.bezierPath, actionObject.transform);
                    point = vertexPath.GetPointAtTime(1,EndOfPathInstruction.Stop);
                    if(pathFollower.distanceTravelled >= vertexPath.length)
                    {
                        Debug.Log ("true");
                        returnControl();
                    }
                    

                    Quaternion toRotation = Quaternion.LookRotation( (point - transform.position).normalized, Vector3.up);
                    transform.rotation = toRotation;
                }
				break;

				case action.pulley:
				 vertexPath = new VertexPath(actionObject.GetComponent<Pulley>().pathCreator.bezierPath, actionObject.transform);
				// Call the GetPointAtTime method on the instance
				 point = vertexPath.GetPointAtTime(1,EndOfPathInstruction.Stop);
				if(pathFollower.distanceTravelled >= vertexPath.length)
                    {
                        pulledUp = true;
                        Debug.Log ("true");
                        returnControl();
                    }
				break;

                case action.rainbow_ring:
                if(actionObject.GetComponent<DashRainbowRing>().spline){
                    vertexPath = new VertexPath(actionObject.GetComponent<DashRainbowRing>().pathCreator.bezierPath, actionObject.transform);
                    point = vertexPath.GetPointAtTime(1,EndOfPathInstruction.Stop);
                    if(pathFollower.distanceTravelled >= vertexPath.length)
                    {
                        Debug.Log ("true");
                        returnControl();
                    }

                    
                }
				break;
    
                case action.dashRamp:
                if(actionObject.GetComponent<DashRamp>().pathCreator != null){
                    vertexPath = new VertexPath(actionObject.GetComponent<DashRamp>().pathCreator.bezierPath, actionObject.transform);
                    point = vertexPath.GetPointAtTime(1,EndOfPathInstruction.Stop);
                    if(pathFollower.distanceTravelled >= vertexPath.length)
                    {
                        Debug.Log ("true");
                        returnControl();
                    }

                     Quaternion toRotation = Quaternion.LookRotation( (point - transform.position).normalized, Vector3.up);
                    transform.rotation = toRotation;
                }
				break;

                case action.rail:
				vertexPath = new VertexPath(actionObject.GetComponent<PathCreator>().bezierPath, actionObject.transform);
				// Call the GetPointAtTime method on the instance
				 point = vertexPath.GetPointAtTime(1,EndOfPathInstruction.Stop);
				if(pathFollower.distanceTravelled >= vertexPath.length)
                    {
                        Debug.Log ("true");
                        returnControl();
                    }
				break;
            }

			
		}

        // For Spline Check
        if(pulledUp && !sonic.isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x,10,rb.velocity.z);
            pulledUp = false;
        }
       

    }
    public void SetActionObject(GameObject obj)
    {
        actionObject = obj;
    }
     public GameObject GetActionObject()
    {
        return actionObject;
    }


    void returnControl()
    {
        timer = 0f;
        sonic.canControl = true;
        pathFollower.RotateOnSpline = false;

        if(current_action == action.spring)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 116f * Time.deltaTime);
        }
        if(current_action == action.dashRamp)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3 (rb.velocity.x,10,rb.velocity.z), 116f * Time.deltaTime);
        }

        current_action = action.none;
        sonic.sonic_action = SonicController.Actions.none;

        //pathFollower.enabled = false;
        pathFollower.pathCreator = null;
        pathFollower.distanceTravelled = 0f;
        
        
        
        

        //followPlayer_pulley pulleyTemp = actionObject.gameObject.GetComponentInChildren<followPlayer_pulley>();
        if (actionObject != null && actionObject.activeInHierarchy)
        {
            followPlayer_pulley pulleyTemp = actionObject.GetComponentInChildren<followPlayer_pulley>();
            if (pulleyTemp != null && pulleyTemp.pull)
            {
                pulleyTemp.pull = false;
            }
        }


        actionObject = null;
    }

    IEnumerator returnControlCoRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        returnControl();
    }
    /////////////////////Object Interaction//////////////////////
    private void OnTriggerEnter(Collider other)
    {
      
        if(other.gameObject.tag == "deathZone")
        {
            sfx.playActionSound(sfx.dead);
            sonic.dead = true;
        }

        if (other.gameObject.CompareTag("Spring") && sonic.sonic_action != SonicController.Actions.attack)
        {       
            actionObject = other.gameObject;
            SpringInit(actionObject.GetComponent<Spring>());
            sonic.isGrounded = false;
            sonic.lastGrounded = false;
             
        }
        if (other.gameObject.CompareTag("SpeedPad"))
        {
            Vector3 padForward = other.transform.forward;
            padForward.y = 0; // Optional: lock to horizontal plane to prevent launching upward
            padForward.Normalize();
             // Rotate the player to match the speed pad's rotation
            transform.rotation = Quaternion.LookRotation(padForward, Vector3.up);

            if(sonic.boost || sonic.air_boost)
            {
                sonic.boost = true;
                sonic.b_timer = 0f;
            }
            
            

            sonic.movement = Vector3.ProjectOnPlane(padForward, sonic.hit.normal).normalized * sonic.speed;

            sonic.speed = sonic.maxSpeed + 5f;
           // sonic.movement = padForward * sonic.speed;
            rb.velocity = sonic.movement;

                
            sfx.playActionSound(sfx.speedpad);
            actionObject = other.gameObject;
            SpeedPadaction(actionObject);
             current_action = action.speedpad;
  

        }

        if (other.gameObject.CompareTag("DashRamp"))
        {
            
            if (sonic.speed > 15)
            {
                sfx.playActionSound(sfx.dashramp);
                transform.rotation = (other.gameObject.transform.rotation) ;
                actionObject = other.gameObject;
                Rampaction(actionObject);
                current_action = action.dashRamp;
                float dashRmp = actionObject.GetComponent<DashRamp>().ramp_spd;
                SplineSpd = dashRmp > 0 ?  dashRmp : SplineSpd;
            }
        }

        if (other.gameObject.CompareTag("DashRing"))
        {
            sfx.playActionSound(sfx.dash_ring);
            actionObject = other.gameObject;
            RainbowRingaction(actionObject);
            current_action = action.rainbow_ring;
            rb.velocity = Vector3.zero;

            DashRainbowRing ring_object = actionObject.GetComponent<DashRainbowRing>();
            pathFollower.pathCreator = ring_object.pathCreator;
            transform.rotation = ring_object.gameObject.transform.rotation;

            timer = ring_object.lockTime;

            if(ring_object.maintainPlayerSpeed == false)
            {
                sonic.speed =  ring_object.player_spd_set > 0 ?  ring_object.player_spd_set : 2f;
            }
            SplineSpd = ring_object.ramp_spd > 0 ?  ring_object.ramp_spd : SplineSpd;
            pathFollower.distanceTravelled = 0f;

            
        }
         if (other.gameObject.CompareTag("pulley"))
         {
            if(sonic.sonic_action != SonicController.Actions.attack)
            {sfx.playActionSound(sfx.pullley);}

            Pulley pulley = other.gameObject.GetComponent<Pulley>();
            actionObject = pulley.gameObject;

            pulleyOnCollision(pulley);

            sonic.gameObject.transform.rotation = pulley.gameObject.transform.rotation;
            sonic.isGrounded = false;
            sonic.movement = Vector3.zero;
            sonic.boost = false;

            sonic.sonic_action = SonicController.Actions.spline;
            current_action = action.pulley;
            

            actionObject = pulley.pathCreator.gameObject;    
            sonic.speed =  pulley.player_spd_set > 0 ?  pulley.player_spd_set : 8f;
            pulleyAction(actionObject);
            

            
            pathFollower.pathCreator = pulley.pathCreator;
            
            timer = pulley.time;
            pathFollower.distanceTravelled = 0f;
            other.gameObject.GetComponentInChildren<followPlayer_pulley>().pull = true;
            SplineSpd = pulley.splineSpeedOveride > 0 ? pulley.splineSpeedOveride : SplineSpd ;


         }
          


         if(other.gameObject.tag == "goalRing")
         {
            // Get the index of the current active scene
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Load the next scene based on the current scene index + 1
            SceneManager.LoadScene(currentSceneIndex + 1);
         }


         ///// Camera Transitions
         if(other.gameObject.tag == "Camera2DTrigger")
        {
            sonic.twoDmode = sonic.twoDmode ? false : true;
            Camera.main.GetComponent<UnleashedCam>().cam2d.offset2D = other.GetComponent<CameraTrigger>().offsetSet;

            sonic.path = other.gameObject.GetComponent<CameraTrigger>().path;
        }

        // HurtBox (for enemies like spinners)
        if(other.gameObject.tag == "hurtBox")
        {
            score.rings -= 13;
            sonic.hurt = true;
            sonic.sfx.playActionSound(sonic.sfx.hurtVC);
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("box"))
        {
            if(sonic.boost)
            {
                Destroy(other.gameObject);  
            }
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            if((sonic.jumped) || (sonic.boost) || (sonic.sonic_action == SonicController.Actions.stomp) )
            {
                 // Retain the player's forward velocity after collision
                Vector3 forwardDirection = transform.forward.normalized;

                // Set Rigidbody velocity to the stored value or maintain boost speed
                rb.velocity = forwardDirection * sonic.speed;
                
                sonic.speed = sonic.maxSpeed;
                sonic.jumpingOffEnemy = true;
                EnemyHit(other.gameObject);
                score.boostGauge += 0.3f;
            }
        }

       if(other.gameObject.tag == "rail" && objectIgnoreTime < 0.1f)
        {
            
            //transform.rotation = (other.gameObject.transform.rotation) ;
            current_action = action.rail;
            sonic.isGrounded = false;
            sonic.movement = Vector3.zero;
            sonic.sonic_action = SonicController.Actions.spline;
            sonic.boost = false;
            GameObject railObj = other.gameObject.GetComponent<PathCreator>().gameObject;
           
            actionObject = railObj;

            railAction(railObj);

            
            pathFollower.pathCreator = railObj.GetComponent<PathCreator>();
            pathFollower.SnapPlayerToClosestPoint();
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("Water"))
        {
            water = true;
            if(sonic.speed > sonic.maxSpeed_og + 7)
            {
                sonic.maxSpeed =sonic.maximumSpeed;
            }
            
            if(sonic.speed > sonic.maxSpeed_og - 3)
            {
                sonic.isGrounded = true;
                rb.velocity = new Vector3(rb.velocity.x,1,rb.velocity.z);
            }
            else
            {
                rb.velocity = -Vector3.up * sonic.gravity/1.2f;
            }
        }
        else
        {
            water = false;
        }
    }
     private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Water"))
        {
            water = false;
        }
     }

    public void SpringInit(Spring spring)
    {
        transform.rotation = (spring.gameObject.transform.rotation) ;

        sonic.isGrounded = false;
        sonic.boost = false;
        SpringAction(spring.gameObject);
        current_action = action.spring;
        sonic.speed =  spring.player_spd_set > 0 ?  spring.player_spd_set : 2f;


        pathFollower.pathCreator = spring.pathCreator;
        pathFollower.distanceTravelled = 0f;
        sonic.gameObject.transform.rotation = spring.gameObject.transform.rotation;
        timer = spring.time;
        
        if(sonic.sonic_action != SonicController.Actions.attack)
        sfx.playActionSound(sfx.spring);
    }


    public void pulleyOnCollision(Pulley pulley)
    {
        
        actionObject = pulley.gameObject;
        
        sonic.gameObject.transform.rotation = pulley.gameObject.transform.rotation;
        sonic.isGrounded = false;
        sonic.movement = Vector3.zero;
        sonic.boost = false;

        sonic.sonic_action = SonicController.Actions.spline;
        current_action = action.pulley;
        

        actionObject = pulley.pathCreator.gameObject;    
        sonic.speed =  pulley.player_spd_set > 0 ?  pulley.player_spd_set : 8f;
        pulleyAction(actionObject);
        

        
        pathFollower.pathCreator = pulley.pathCreator;
        
        timer = pulley.time;
        pathFollower.distanceTravelled = 0f;
        pulley.gameObject.GetComponentInChildren<followPlayer_pulley>().pull = true;
        SplineSpd = pulley.splineSpeedOveride > 0 ? pulley.splineSpeedOveride : SplineSpd ;
    }



    public void EnemyHit(GameObject enemy)
    {
         
        sonic.enemyExplode.SpawnExplode(enemy.gameObject.transform.position);
        enemy.gameObject.GetComponent<Enemy_Death>().kill_enemy( new Vector2 (40,5 ));
        if(enemy.gameObject.GetComponent<CapsuleCollider>() != null){
        enemy.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        rb.velocity  = new Vector3(rb.velocity.x, 10 ,rb.velocity.z);
        sonic.speed = sonic.maxSpeed;
        score.boostGauge += 0.3f;

        if(sonic.boost)
        {
            
            sonic.b_timer = 0;
        }
        
        if(sonic.sonic_action != SonicController.Actions.attack)
        sfx.playActionSound(sfx.enemyhit);

        if(sonic.boost || sonic.air_boost)
            {
                sonic.boost = true;

                sonic.b_timer = 0;
            }   
        
    }

    ///////////////////////////////////// object behaviour affecting player /////////

    public void pulleyAction(GameObject collided)
    {   
        Pulley spring = collided.GetComponent<Pulley>();
        sonic.canControl = false;
        
        
        sonic.sonic_action = SonicController.Actions.spline;
        pathFollower.pathCreator = spring.pathCreator;  
        

       
        VertexPath vertexPath = new VertexPath(collided.GetComponent<Pulley>().pathCreator.bezierPath, collided.transform);
        Vector3 point = vertexPath.GetPointAtTime(1,EndOfPathInstruction.Stop);
        if(pathFollower.distanceTravelled >= vertexPath.length)
        {
             pulledUp = true;
            returnControl();
        }
    }

    public void SpringAction(GameObject springObj)
    {
        Spring spring = springObj.GetComponent<Spring>();

        SplineSpd = spring.splineSpeedOveride > 0 ? spring.splineSpeedOveride : SplineSpd ;
        sonic.canControl = false;
        
        if (spring.spline)
        {
            sonic.sonic_action = SonicController.Actions.spline;
            pathFollower.pathCreator = spring.pathCreator;  
        }
        else
        {
            rb.velocity = springObj.transform.up * spring.springSpeed;
            sonic.speed = spring.killSpd ? 0f : sonic.speed;
        }
        
    }

    void SpeedPadaction(GameObject collided)
    {
         if(sonic.boost || sonic.air_boost)
        {
            sonic.b_timer = 0;
            sonic.boost = true;

            
        }
        
        SpeedPad speedpadComponent = collided.GetComponent<SpeedPad>();
       
        if (speedpadComponent != null)
        {   
            //rb.velocity = new Vector3 (rb.velocity.x, -10 ,rb.velocity.z); // Add a small upward force to prevent sticking to ground
            //sonic.speed = speedpadComponent.speedAdd;
            if (speedpadComponent.lockDir)
            {
                // Lock the player's movement direction and set a timer
                sonic.canControl = false;
                timer = speedpadComponent.lockTime;
            }
            else
            {
                // Allow control and reset the timer
                sonic.canControl = true;
                timer = 0;
            }
            StartCoroutine(returnControlCoRoutine(0.7f));
            }
       

    }
    void Rampaction(GameObject collided)
    {
        if (sonic.speed > 15)
        {
            sonic.canControl = false;
            DashRamp rampObj = collided.GetComponent<DashRamp>();

            if(rampObj.pathCreator  != null)
            {
                transform.rotation = (rampObj.gameObject.transform.rotation) ;

                sonic.isGrounded = false;
                sonic.boost = false;
            
                pathFollower.pathCreator = rampObj.pathCreator;
                pathFollower.distanceTravelled = 0f;
                sonic.gameObject.transform.rotation = rampObj.gameObject.transform.rotation;
                timer = rampObj.lockTime;

                sonic.speed = rampObj.player_spd_set;
                
            }
            else
            {
                rb.velocity = rampObj.ramp_launch_pos.transform.forward * rampObj.ramp_spd;
                timer = rampObj.lockTime;
            }
        }
    }

    void RainbowRingaction(GameObject collided)
    {
        sonic.canControl = false;
        sonic.sonic_action =SonicController.Actions.none;
        sonic.boost = false;
		DashRainbowRing RingObj = collided.GetComponent<DashRainbowRing>();
        timer = RingObj.lockTime;
		if(RingObj.spline)
		{
            sonic.sonic_action = SonicController.Actions.spline;
            pathFollower.enabled = true;
            pathFollower.pathCreator = RingObj.pathCreator;
            if(pathFollower.pathCreator.path.Equals(  pathFollower.pathCreator.path.GetPointAtDistance(2,EndOfPathInstruction.Stop)))
            {
                rb.velocity = Vector3.zero;
                returnControl();
            }
		}
		else{
        rb.velocity = RingObj.ramp_launch_pos.transform.forward * RingObj.ramp_spd;
		}
        if(sonic.boost || sonic.air_boost)
            {
                sonic.boost = true;

                sonic.b_timer = 0;
            }
		

        if (sonic.isGrounded)
        {
            returnControl();
        }
    }


    void railAction(GameObject rail)
    {
        sonic.canControl = false;
        sonic.boost = false;
        sonic.sonic_action = SonicController.Actions.spline;
        rb.isKinematic = true;
        pathFollower.enabled = true;
        pathFollower.RotateOnSpline = true;
        
    
        if(sonic.boost || sonic.air_boost)
            {
                sonic.boost = true;

                sonic.b_timer = 0;
            }
		

    }
}