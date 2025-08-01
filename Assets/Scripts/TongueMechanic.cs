using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TongueMechanic : MonoBehaviour
{
    [SerializeField] private float _lengthOffset;
    [SerializeField] private float _tongueRange = 5f;
    [SerializeField] private LayerMask _grappleLayer;
    [SerializeField] private LineRenderer _tongueRenderer;


    private SpringJoint2D _joint;
    private Vector2 _hitPoint;
    private Vector3 _transformOffset = new Vector3(0f,1f,0f);

    private bool _tongueActive = false;

    private void Awake()
    {
        _joint = GetComponent<SpringJoint2D>();
        _tongueRenderer = GetComponentInChildren<LineRenderer>();
        _joint.enabled = false;
        _tongueRenderer.enabled = false;

    }

    public void OnTongueShoot(InputValue value)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + _transformOffset,
            (Camera.main.ScreenToWorldPoint(Input.mousePosition) - (transform.position + _transformOffset)),
            _tongueRange,
            _grappleLayer);

        if (hit.collider == null) return;

        _tongueActive = true;

        _hitPoint = hit.point;
        _joint.connectedAnchor = _hitPoint;
        _joint.distance = (_hitPoint - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
        _joint.enabled = true;

        _tongueRenderer.enabled = true;
        _tongueRenderer.SetPosition(0, _hitPoint);
        _tongueRenderer.SetPosition(1, transform.position + _transformOffset);
    }

    public void OnTongueRelease(InputValue value)
    {
        if (!_tongueActive) return;

        _tongueActive = false;
        _joint.enabled = false;
        _tongueRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_tongueActive && _joint.distance > _lengthOffset)
        {
            _joint.distance = (_hitPoint - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
        }

        if (_tongueRenderer.enabled)
        {
            _tongueRenderer.SetPosition(1, transform.position + _transformOffset);
            
        }
    }
}
