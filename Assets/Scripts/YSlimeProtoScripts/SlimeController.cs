using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float rolltiplier;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    public List<GameObject> points;

    //-------------Editor Interface---------------------------

    // Horizontal Movement
    public float Speed = 1.0f;
    public float MaxSpeed = 14f;
    public float Acceleration = 0.5f;
    public float GroundDeceleration = 60f;
    public float AirDeceleration = 30f;

    // Vertical Movement
    public bool _grounded;
    public bool _endedJumpEarly;
    public float GroundedGravity =-1.5f;
    public float FallAcceleration = 50f;
    public float JumpEndEarlyGravityModifier = 3f;
    public float MaxFallSpeed = 40.0f;


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
            points.Add(transform.GetChild(i).gameObject);
        }
    }

    // --------------------------UPDATE METHODS------------------
    void Update()
    {
        GatherInput();
        
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            JumpDown = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump")
        };

        /*
        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
        */
    }

    // -------------------------------FIXED UPDATE METHODS--------------
    private void FixedUpdate()
    {
        CheckCollisions();
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
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

}
