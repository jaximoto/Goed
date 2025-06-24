using System.Runtime.CompilerServices;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlimeController _slimeController;
    [SerializeField] private Transform _lineSpawnPoint;
    [Header("Trajectory Line Smoothmess/Length")]
    [SerializeField] private int _segmentCount = 50;

    private Vector2[] _segments;
    private LineRenderer _lineRenderer;

    private float _projectileSpeed;
    private float _projectileGravity;
    void Awake()
    {
        // init segments
        _segments = new Vector2[_segmentCount];
       
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _segmentCount;

        // Get speed and gravity from slime
        _projectileSpeed = _slimeController.LaunchSpeed;
        _projectileGravity = _slimeController.GForce;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 startPos = _lineSpawnPoint.position;
        _segments[0] = startPos;
        _lineRenderer.SetPosition(0, startPos);

        //set the starting velocity based on starting velocity of jewel
        
    }
}
