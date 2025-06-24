using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.WSA;

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
    public float GroundedGravity =-1.5f;
    public float FallAcceleration = 50f;
    public float JumpEndEarlyGravityModifier = 3f;
    public float MaxFallSpeed = 40.0f;
    public float JumpBuffer = .2f;
    public float JumpPower = 36;
    public float GForce;

    // Jewel Launch
    public float LaunchSpeed;

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
        public bool ShootUp;
        public bool ShootHeld;
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
    }

    // --------------------------UPDATE METHODS------------------
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > .5f && takingInput)
        {
            GatherInput();
        }
        
        
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            JumpDown = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump"),
            ShootHeld = Input.GetMouseButton(0),
            ShootUp = Input.GetMouseButtonUp(0)
        };

        
        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
        
    }

    private void HandleShoot()
    {
        if (_frameInput.ShootHeld)
        {
            // Show path of jewel when launched, start forward and up and reacts to mouse movement
            DisplayShot();
        }
        if (_frameInput.ShootUp)
        {
            //LaunchJewel();
        }

        else
        {
            //ClearShot();
        }
    }

    private void DisplayShot()
    {

    }
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
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? GroundDeceleration : AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * MaxSpeed, Acceleration * Time.fixedDeltaTime);
        }
    }
    private void ApplyMovement()
    {
        _rb.linearVelocity = _frameVelocity;
        foreach(GameObject child in points)
        {
            child.GetComponent<Rigidbody2D>().linearVelocity = _frameVelocity;
        }
    }

    private void Gravity()
    {
        if(_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = GroundedGravity;
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
