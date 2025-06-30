using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;
using static Unity.VisualScripting.Metadata;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class SlimeController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private float _time;
    public List<GameObject> points;

    //-------------Editor Interface---------------------------

    // Horizontal Movement
    public bool takingInput = true;
    public float MaxSpeed = 14f;
    public float Acceleration = 0.5f;
    public float GroundDeceleration = 60f;
    public float AirDeceleration = 30f;

    // Vertical Movement
    public bool _grounded;
    public bool _endedJumpEarly;
    private bool _bufferedJumpUsable, _jumpToConsume;
    private float _timeJumpWasPressed;
    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + JumpBuffer;
    public float GroundedGravity = -1.5f;
    public float FallAcceleration = 50f;
    public float JumpEndEarlyGravityModifier = 3f;
    public float MaxFallSpeed = 40.0f;
    public float JumpBuffer = .2f;
    public float JumpPower = 36;
    public float GForce;

    public bool levelEnding;
    

    // Jewel Launch 
    public bool deBone; // turn into off taking input???
    public float chargeDelta;
    public float chargeMax;
    public float currCharge;

    //public float chargeFrequency;
    //public float shootFrequency;
    public float shootMultiplier;

    public bool ChargeHeld; //was launchHeld
    public bool ChargeUp; //was releasingLaunch
    public float LaunchSpeed; //unused

    // Jewel Movement Stats
    public float jewelAirDeceleration, jewelFallAcceleration, jewelGroundDeceleration;

    // Scaling stuff
    public float distanceCovered;
    public float lossMult, scaleMult;

    public float _proportion, maxProp;
    float startSize;

    Vector3[] _hingeConnectedAnchors, _hingeAnchors;
    Vector3[][] _connectedAnchors, _anchors;
    float[][] _distance;

    //Rebounding Stuff
    public Vector3[] _pointTargets;
    public float reboundStrength, joinCrossStrength, expelStrength;
    private bool _currHit, _nextHit;
    
    // Collison
    private bool _cachedQueryStartInColliders;
    public LayerMask groundCheckIgnoreLayers;
    public CircleCollider2D _col;
    public float GroundDistance;
    public struct FrameInput
    {
        public Vector2 Move;
        public bool JumpDown;
        public bool JumpHeld;
 
        //Jewel shoot stuff
        public Vector2 ChargeDir; //was chargeVector
        
    }

    // Put interface for player here:

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

        points = new List<GameObject>();
        // exclude last child which doesnt have rigidbody
        for(int i = 0; i < transform.childCount - 1; i++)
        {
            if(transform.GetChild(i).TryGetComponent<Rigidbody2D>(out _))
                points.Add(transform.GetChild(i).gameObject);
        }

        ConfigureAnchors();
        GetReboundTargets();

        startSize = transform.localScale.x;

    }

    // --------------------------UPDATE METHODS------------------
    public GameObject text;
    bool RDown;
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > .5f && takingInput)
        {
            GatherInput();
        }

        _proportion = transform.localScale.x / startSize;
        if (_proportion < 0.5f) {SlimeDeath(); text.SetActive(true);}
        RDown = Input.GetKeyDown("r");
        if (deBone && !levelEnding)
        {
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        Debug.Log($"press r? RDown = {RDown}");
        if (RDown) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            JumpDown = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump"),
            
            ChargeDir = new Vector2(Input.GetAxisRaw("Horizontal2"), Input.GetAxisRaw("Vertical2")),
        };

        
        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        //--------------------------JASPER WUZ HERE--------------

        if (_frameInput.ChargeDir != Vector2.zero) 
        {
            ChargeHeld = true;
        }
        else if (ChargeHeld && _frameInput.ChargeDir == Vector2.zero)
        {
            ChargeHeld = false;
            ChargeUp = true;
        }

    }

    //-------------------------------DEBONE LATEUPDATE--------------


    //-------------------------------JEWEL SHOOTING SHIT--------------
    private void HandleShoot()
    {

        if (ChargeUp)
        {
            Shoot();
            //StartCoroutine(EndOfFrameDeBone());
            //LaunchJewel();
        }
        else if (ChargeHeld)
        {
            // Show path of jewel when launched, start forward and up and reacts to mouse movement
            ChargeShot();
            
        } 
    }

    void OnDrawGizmos()
    {
        //Gather Component Vectors
        if (ChargeHeld)
        {
            //Debug.Log($"");
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameObject.transform.position,
                new Vector3(gameObject.transform.position.x - _frameInput.ChargeDir.x, gameObject.transform.position.y - _frameInput.ChargeDir.y, gameObject.transform.position.z));
        }

        for(int i = 0; i < _pointTargets.Length; i++)
        {
            //ReShaping Force
            Gizmos.color = Color.red;
            Vector3 target = gameObject.transform.position + (_pointTargets[i] * gameObject.transform.localScale.x);
            Gizmos.DrawLine(points[i].transform.position, target);

            //connecting lines
            Gizmos.color = Color.blue;


         
            if(i == points.Count - 1) Gizmos.DrawLine(points[i].transform.position, points[0].transform.position);
            else Gizmos.DrawLine(points[i].transform.position, points[i + 1].transform.position);
        }
    }

    void ChargeShot()
    {
        Vector2 moveDir = _frameInput.ChargeDir.normalized;
        currCharge += chargeDelta * Time.deltaTime;
        //Debug.Log($"adding force = {moveDir * currCharge}");
        gameObject.GetComponent<Rigidbody2D>().AddForce(moveDir * currCharge);
        //Debug.Log($"charge = {currCharge}");
        if(currCharge >= 200)
        {
            //Debug.Log("Wowzers");
        }
    }

    void Shoot()
    {
        Vector2 ShotForce = new Vector2();
        
        for (int i = 0; i < points.Count; i++)
        {
            ShotForce += points[i].GetComponents<SpringJoint2D>()[0].reactionForce;
        }
        //Debug.Log($"currcharge is {currCharge}");
        _frameVelocity.y = 0f;
        _frameVelocity += ShotForce * shootMultiplier;
        currCharge = 0;
        ChargeUp = false;  
    }

    void DeBone()
    {

        deBone = true;
        takingInput = false;

        /* used to detach and despring
        for (int i = 0; i < points.Count; i++)
        {
            Debug.Log($"(i = {i}) debone reforce is {points[i].GetComponents<SpringJoint2D>()[0].reactionForce}");
            points[i].GetComponents<SpringJoint2D>()[0].enabled = false;
        }
        gameObject.transform.DetachChildren();
    
        */
    }

    //Scale multiplier for each joint
    void ConfigureAnchors()
    {
        _hingeAnchors = new Vector3[points.Count];
        _hingeConnectedAnchors = new Vector3[points.Count];
        _connectedAnchors = new Vector3[points.Count][];
        _anchors = new Vector3[points.Count][];
        _distance = new float[points.Count][];
        for (int i = 0; i < points.Count; i++)
        {
            _hingeConnectedAnchors[i] = points[i].GetComponent<HingeJoint2D>().connectedAnchor;
            _hingeAnchors[i] = points[i].GetComponent<HingeJoint2D>().anchor;
            points[i].GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
            _connectedAnchors[i] = new Vector3[points[i].GetComponents<SpringJoint2D>().Length];
            _anchors[i] = new Vector3[points[i].GetComponents<SpringJoint2D>().Length];
            _distance[i] = new float[points[i].GetComponents<SpringJoint2D>().Length];
            for (int j = 0; j < points[i].GetComponents<SpringJoint2D>().Length; j++)
            {
                _connectedAnchors[i][j] = points[i].GetComponents<SpringJoint2D>()[j].connectedAnchor;
                _anchors[i][j] = points[i].GetComponents<SpringJoint2D>()[j].anchor;
                points[i].GetComponent<SpringJoint2D>().autoConfigureConnectedAnchor = false;
                _distance[i][j] = points[i].GetComponents<SpringJoint2D>()[j].distance;
                points[i].GetComponent<SpringJoint2D>().autoConfigureDistance = false;
            }
        }
    }

    //Reset anchors when scaling?
    void UpdateAnchors()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponent<HingeJoint2D>().connectedAnchor = _hingeConnectedAnchors[i];
            points[i].GetComponent<HingeJoint2D>().anchor = _hingeAnchors[i];

            for (int j = 0; j < points[i].GetComponents<SpringJoint2D>().Length; j++)
            {
                points[i].GetComponents<SpringJoint2D>()[j].connectedAnchor = _connectedAnchors[i][j];
                points[i].GetComponents<SpringJoint2D>()[j].anchor = _anchors[i][j];
                points[i].GetComponents<SpringJoint2D>()[j].distance = _distance[i][j];
            }
        }
    }

    void WalkSlimeLoss()
    {
        Vector3 loss = new Vector3(0.001f, 0.001f, 0.001f);
        gameObject.transform.localScale -= distanceCovered * loss;
        UpdateAnchors();
    }

    //keep track of horizontal distance covered while grounded 
    void CheckWalkLoss()
    {
        if (_grounded)
        {
            distanceCovered = Mathf.Abs(_frameVelocity.x * Time.fixedDeltaTime);
            //Debug.Log($"distanceCovered = {distanceCovered}");
        }
    }

    //slime realigning force
    void GetReboundTargets()
    {
        _pointTargets = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            _pointTargets[i] = points[i].transform.localPosition;
        }
    }
    
    // add multiplier and call when situations get dire?
    void ApplyReboundForce(float reboundMult)
    {
        for (int i = 0;i < _pointTargets.Length; i++)
        {
            Vector3 target = gameObject.transform.position + (_pointTargets[i] * gameObject.transform.localScale.x);
            Vector3 targetDir = target - points[i].transform.position;
            points[i].GetComponent<Rigidbody2D>().AddForce(targetDir * reboundMult, ForceMode2D.Impulse);
        }
    }

    void CheckBetween()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Transform currP, nextP;

            if (i == points.Count - 1)
            {
                currP = points[i].transform;
                nextP = points[0].transform;
            }
            else
            {
                currP = points[i].transform;
                nextP = points[i + 1].transform;
            }

            Vector3 targetDir = nextP.position - currP.position;
            //Debug.DrawRay(currP.position, targetDir, Color.yellow);

            RaycastHit2D hit = Physics2D.Raycast(currP.position, targetDir.normalized, targetDir.magnitude, ~groundCheckIgnoreLayers.value);
            if (hit)
            {
                //Debug.Log($"hit {hit.transform.gameObject}");
                //Debug.Log($"Raycast hit between {currP} and {nextP}");
                Vector2 perpForce = Vector2.Perpendicular(targetDir);

                //Debug.DrawRay(currP.position, perpForce, Color.black);
                //Debug.DrawRay(nextP.position, perpForce, Color.black);

                currP.GetComponent<Rigidbody2D>().AddForce(perpForce * joinCrossStrength, ForceMode2D.Impulse);
                nextP.GetComponent<Rigidbody2D>().AddForce(perpForce * joinCrossStrength, ForceMode2D.Impulse);

                Vector3 currToCore = gameObject.transform.position - currP.position;
                Vector3 nextToCore = gameObject.transform.position - nextP.position;

                RaycastHit2D currHit = Physics2D.Raycast(currP.position, currToCore.normalized, currToCore.magnitude, ~groundCheckIgnoreLayers.value);
                _currHit = currHit;
                if (currHit)
                {
                    //Debug.DrawRay(currP.position, currToCore, Color.green);
                    currP.GetComponent<Collider2D>().isTrigger = true;
                    currP.GetComponent<Rigidbody2D>().AddForce(currToCore * expelStrength, ForceMode2D.Impulse);
                }
                else currP.GetComponent<Collider2D>().isTrigger = false;
                RaycastHit2D nextHit = Physics2D.Raycast(nextP.position, nextToCore.normalized, nextToCore.magnitude, ~groundCheckIgnoreLayers.value);
                _nextHit = nextHit;
                if (nextHit)
                { 
                    //Debug.DrawRay(nextP.position, nextToCore, Color.green);
                    nextP.GetComponent<Rigidbody2D>().AddForce(nextToCore * expelStrength, ForceMode2D.Impulse);
                    nextP.GetComponent<Collider2D>().isTrigger = true;
                }
                else nextP.GetComponent<Collider2D>().isTrigger = false; 
            }

            else 
            {
                if (!_currHit && currP.GetComponent<Collider2D>().isTrigger) currP.GetComponent<Collider2D>().isTrigger = false;
                if (!_nextHit && nextP.GetComponent<Collider2D>().isTrigger) nextP.GetComponent<Collider2D>().isTrigger = false;

            }
        }
    }

    public float AddSlime(float slime, float drainRate)
    {
        if (slime > 0 && _proportion  < maxProp)
        {
            slime -= drainRate * Time.deltaTime;
            gameObject.transform.localScale += new Vector3(0.25f,0.25f,0.25f) * drainRate * Time.deltaTime; 
            //Debug.Log($"slime = {slime}");
        }
        return slime;
    }

    void SlimeDeath()
    {
        DeBone();
    }



    //-------------------------------END JASPER CONTAMINATED ZONE-----------------
    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocityY > 0)
            _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump)
            return;

        if (_grounded)
            ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _frameVelocity.y = JumpPower;
        //Jumped?.Invoke();
    }

    // -------------------------------FIXED UPDATE METHODS--------------
    private void FixedUpdate()
    {
        CheckCollisions();
        HandleShoot();
        HandleJump();
        HandleHorizontal();

        Gravity();
        
        ApplyMovement();

        ApplyReboundForce(reboundStrength); 
        CheckBetween();    
    }

    bool groundHit;
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;
        //Vector2 origin = _col.bounds.center + Vector3.up * 0.5f;

        /*
        bool groundHit = Physics2D.CircleCast
            (
            origin,
            _col.radius,
            Vector2.down,
            GroundDistance,
            ~groundCheckIgnoreLayers.value
            );
        */
        int groundedCount = 0;
        //We are gonna try and check ground collision for each particle
        for (int i = 0; i < points.Count; i++)
        {
            //Debug.DrawRay(points[i].transform.position, transform.localScale.x * Vector3.down * points[i].transform.localScale.x, Color.yellow);
            bool groundRay = Physics2D.Raycast(points[i].transform.position, 
                Vector3.down, 
                transform.localScale.x * points[i].transform.localScale.x, 
                ~groundCheckIgnoreLayers.value);
            groundedCount += groundRay ? 1 : 0;
        }
        if (groundedCount > 0) groundHit = true;
        else groundHit = false;


        // Landed on the Ground
        if (!_grounded && groundHit) 
        {
            _grounded = true;
            _bufferedJumpUsable = true;
            //_endedJumpEarly = false;
            // Can call grounded change event
        }

        // Left the Ground
        else if(_grounded && !groundHit)
        {
            _grounded = false;
            //_frameLeftGrounded = _time;
            //GroundedChanged?.Invoke(false, 0);
        }

        // Set the default where raycasts return true if they start in their colliders
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }
    private void HandleHorizontal()
    {
        if (_frameInput.Move.x == 0 || deBone)
        {
            var deceleration = _grounded ? GroundDeceleration : AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            CheckWalkLoss();
            WalkSlimeLoss();
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * MaxSpeed, Acceleration * Time.fixedDeltaTime);
            CheckWalkLoss();
            WalkSlimeLoss();
        }

    }
    private void ApplyMovement()
    {
        _rb.linearVelocity = _frameVelocity;

        foreach (GameObject child in points)
        {
            child.GetComponent<Rigidbody2D>().linearVelocity = _frameVelocity;
        }

    }

    private void Gravity()
    {


        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = GroundedGravity;
        }
        /*
        else if (deBone && !_grounded)
        {
            var inAirGravity = jewelFallAcceleration;
            GForce = Mathf.MoveTowards(_frameVelocity.y, -MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            _frameVelocity.y = GForce;

        }
        */
        else
        {
            var inAirGravity = FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= JumpEndEarlyGravityModifier;
            //falling
            //else if(frameVelocity.y < 0) Falling?.Invoke();
            GForce = Mathf.MoveTowards(_frameVelocity.y, -MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            _frameVelocity.y = GForce;
        }
    }

}
