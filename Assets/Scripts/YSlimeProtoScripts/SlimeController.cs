using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    // Jewel Launch 
    public bool deBone = false; // turn into off taking input???
    public float chargeDelta;
    public float chargeMax;
    public float currCharge;

    public float chargeFrequency;
    public float shootFrequency;
    public float shootMultiplier;

    public bool ChargeHeld; //was launchHeld
    public bool ChargeUp; //was releasingLaunch
    public float LaunchSpeed; //unused

    // Jewel Movement Stats
    public float jewelAirDeceleration, jewelFallAcceleration, jewelGroundDeceleration;

    // Scaling stuff
    public float distanceCovered;
    public float lossMult, scaleMult;

    public Vector3[][] _connectedAnchors;
    public Vector3[][] _anchors;
    public float[][] _distance; 
    
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
    }

    // --------------------------UPDATE METHODS------------------
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > .5f && takingInput)
        {
            GatherInput();
        }
        UpdateAnchors();

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
    }


    void ChargeShot()
    {
        Vector2 moveDir = _frameInput.ChargeDir.normalized;
        currCharge += chargeDelta * Time.deltaTime;
        Debug.Log($"adding force = {moveDir * currCharge}");
        gameObject.GetComponent<Rigidbody2D>().AddForce(moveDir * currCharge);
        Debug.Log($"charge = {currCharge}");
        if(currCharge >= 200)
        {
            Debug.Log("Wowzers");
        }
    }

    void Shoot()
    {
        Vector2 ShotForce = new Vector2();
        
        for (int i = 0; i < points.Count; i++)
        {
            ShotForce += points[i].GetComponents<SpringJoint2D>()[0].reactionForce;
        }
        DeBone();
        Debug.Log($"currcharge is {currCharge}");
        //gameObject.GetComponent<Rigidbody2D>().AddForce(ShotForce * currCharge, ForceMode2D.Impulse);
        _frameVelocity += ShotForce * shootMultiplier;
        currCharge = 0;
        deBone = true;
        ChargeUp = false;
        
    }


    void DeBone()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Debug.Log($"(i = {i}) debone reforce is {points[i].GetComponents<SpringJoint2D>()[0].reactionForce}");
            points[i].GetComponents<SpringJoint2D>()[0].enabled = false;
        }
        gameObject.transform.DetachChildren();
    }

    //Scale multiplier for each joint
    void ConfigureAnchors()
    {
        _connectedAnchors = new Vector3[points.Count][];
        _anchors = new Vector3[points.Count][];
        _distance = new float[points.Count][];
        for (int i = 0; i < points.Count; i++)
        {
            _connectedAnchors[i] = new Vector3[points[i].GetComponents<SpringJoint2D>().Length];
            _anchors[i] = new Vector3[points[i].GetComponents<SpringJoint2D>().Length];
            _distance[i] = new float[points[i].GetComponents<SpringJoint2D>().Length];
            for (int j = 0; j < points[i].GetComponents<SpringJoint2D>().Length; j++)
            {
                _connectedAnchors[i][j] = points[i].GetComponents<SpringJoint2D>()[j].connectedAnchor;
                _anchors[i][j] = points[i].GetComponents<SpringJoint2D>()[j].anchor;
                _distance[i][j] = points[i].GetComponents<SpringJoint2D>()[j].distance;
            }
        }
    }

    //Reset anchors when scaling?
    void UpdateAnchors()
    {
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points[i].GetComponents<SpringJoint2D>().Length; j++)
            {
                Debug.Log("Connected anchor is ");
                points[i].GetComponents<SpringJoint2D>()[j].connectedAnchor = _connectedAnchors[i][j];
                points[i].GetComponents<SpringJoint2D>()[j].anchor = _anchors[i][j];
                points[i].GetComponents<SpringJoint2D>()[j].distance = _distance[i][j];
            }
        }
    }

    void WalkSlimeLoss()
    {
        // parent scaling breaks spring joints
        
        //--------------------Okay chucklefucks we got a new plan
        //----First off we need three arrays of size 
        
        Vector3 loss = new Vector3(0.001f, 0.001f, 0.001f);
        gameObject.transform.localScale -= distanceCovered * loss;

    }

    // | ||
    // | |_

    //keep track of horizontal distance covered while grounded 
    void CheckWalkLoss()
    {
        if (_grounded)
        {
            distanceCovered = Mathf.Abs(_frameVelocity.x * Time.fixedDeltaTime);
            Debug.Log($"distanceCovered = {distanceCovered}");
        }
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

        
    }

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;
        Vector2 origin = _col.bounds.center + Vector3.up * 0.5f;
        bool groundHit = Physics2D.CircleCast
            (
            origin,
            _col.radius,
            Vector2.down,
            GroundDistance,
            ~groundCheckIgnoreLayers.value
            );

        // Landed on the Ground
        if (!_grounded && groundHit) 
        {
            _grounded = true;
            //_bufferedJumpUsable = true;
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
        if (deBone)
        {
            Debug.Log($"_grounded is {_grounded}");
            var deceleration = _grounded ? jewelGroundDeceleration : jewelAirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            Debug.Log($"_framevelocity is {_frameVelocity}");    
        }
        else
        {
            if (_frameInput.Move.x == 0)
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
    }
    private void ApplyMovement()
    {
        _rb.linearVelocity = _frameVelocity;
        if (!deBone) 
        {
            foreach (GameObject child in points)
            {
                child.GetComponent<Rigidbody2D>().linearVelocity = _frameVelocity;
            }
        }
    }

    private void Gravity()
    {


        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = GroundedGravity;
        }
        else if (deBone && !_grounded)
        {
            var inAirGravity = jewelFallAcceleration;
            GForce = Mathf.MoveTowards(_frameVelocity.y, -MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            _frameVelocity.y = GForce;

        }
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
