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
    public float MaxSpeed = 100.0f;
    public float Acceleration = 0.5f;
    public float GroundDeceleration = 0.5f;
    public float AirDeceleration = 0.25f;

    // Vertical Movement
    public bool _grounded;

    // Collison
    private bool _cachedQueryStartInColliders;
    public LayerMask groundCheckIgnoreLayers;
    public CapsuleCollider2D _col;
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
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

        points = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
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
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C)
        };

        if (_frameInput.JumpDown)
        {
            
        }
    }

    // -------------------------------FIXED UPDATE METHODS--------------
    private void FixedUpdate()
    {
        CheckCollisions();
        HandleHorizontal();
        ApplyMovement();
    }

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        bool groundHit = Physics2D.CapsuleCast
            (
            _col.bounds.center,
            _col.size,
            _col.direction,
            0,
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

}
