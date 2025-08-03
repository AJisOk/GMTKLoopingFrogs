using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class TongueMechanic : MonoBehaviour
{
    [SerializeField] private float _lengthOffset;
    [SerializeField] private float _tongueRange = 5f;
    [SerializeField] private LayerMask _grappleLayer;
    [SerializeField] private LayerMask _dynamicGrappleLayer;
    [SerializeField] private LineRenderer _tongueRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _tongueOrigin;
    [SerializeField] private Transform _crouchedTongueOrigin;
    [SerializeField] private SpriteRenderer _mouthSprite;
    [SerializeField] private SpriteRenderer _endTongueSprite;

    private FrogCharacterMovement _movement;
    private SpringJoint2D _joint;
    private Vector2 _hitPoint;
    private Vector2 _missHitPoint;
    private Rigidbody2D _hitRB;
    private Vector3 _transformOffset = new Vector3(0f,1f,0f);
    private Vector3 aimDirection;
    private float _dynamicJointDistance;
    private float _newJointDistance;

    private bool _tongueActive = false;
    //private bool _missActive = false;
    private bool _dynamicGrabbed = false;

    public bool CanTongue { get; set; }
    private void Awake()
    {
        CanTongue = true;

        _movement = GetComponent<FrogCharacterMovement>();

        _joint = GetComponent<SpringJoint2D>();
        _tongueRenderer = GetComponentInChildren<LineRenderer>();
        _joint.enabled = false;
        _tongueRenderer.enabled = false;
        _mouthSprite.enabled = false;

    }

    

    public void OnTongueShoot(InputValue value)
    {
        if (!CanTongue) return;

        RaycastHit2D hit = Physics2D.Raycast(_movement.IsCrouched ? _crouchedTongueOrigin.position : _tongueOrigin.position,
            (Camera.main.ScreenToWorldPoint(Input.mousePosition) - (_movement.IsCrouched ? _crouchedTongueOrigin.position : _tongueOrigin.position)),
            _tongueRange,
            _grappleLayer);

        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition) - _tongueOrigin.position);

        //fire a false shot on miss
        if (hit.collider == null) return;
        //if (hit.collider == null)
        //{
        //    aimDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition)-_tongueOrigin.position).normalized;
        //    Debug.Log(aimDirection);

        //    _missHitPoint = new Vector2((aimDirection.x * _tongueRange) + _tongueOrigin.position.x, 
        //        (aimDirection.y * _tongueRange) + _tongueOrigin.position.y);

        //    _tongueActive = true;
        //    _missActive = true;

        //    _tongueRenderer.enabled = true;
        //    _mouthSprite.enabled = true;
        //    _tongueRenderer.SetPosition(0, _missHitPoint);
        //    _tongueRenderer.SetPosition(1, _tongueOrigin.position);
        //    _endTongueSprite.transform.position = _missHitPoint;
        //    _endTongueSprite.enabled = true;

        //    StartCoroutine(DelayedReleaseTongue(.2f));

        //    return;
        //}



        //form a different type of connection if a dynamic object is hit
        if (hit.rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            //_missActive = false;
            _tongueActive=true;
            _dynamicGrabbed = true;

            _hitRB = hit.rigidbody;
            _joint.connectedBody = hit.collider.GetComponent<Rigidbody2D>();
            _joint.distance = (new Vector2(_hitRB.transform.position.x, _hitRB.transform.position.y) - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
            _joint.enabled = true;


            _tongueRenderer.SetPosition(0, hit.collider.transform.position);
            _tongueRenderer.SetPosition(1, _movement.IsCrouched ? _crouchedTongueOrigin.position : _tongueOrigin.position);
            _endTongueSprite.transform.position = hit.collider.transform.position;
            _tongueRenderer.enabled = true;
            _mouthSprite.enabled = true;
            _endTongueSprite.enabled = true;


            if(hit.rigidbody.TryGetComponent<Interactable>(out Interactable interactable))
            {
                interactable.OnGrapple();
                StartCoroutine(DelayedReleaseTongue(.1f));
                //ReleaseTongue();
            }


            return;
        }


        _tongueActive = true;
        //_missActive = false;

        _hitPoint = hit.point;
        _joint.connectedAnchor = _hitPoint;
        _joint.distance = (_hitPoint - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
        _joint.enabled = true;

        _tongueRenderer.enabled = true;
        _mouthSprite.enabled = true;
        _tongueRenderer.SetPosition(0, _hitPoint);
        _tongueRenderer.SetPosition(1, _movement.IsCrouched ? _crouchedTongueOrigin.position : _tongueOrigin.position);
        _endTongueSprite.transform.position = _hitPoint;
        _endTongueSprite.enabled = true;
        
        
    }

    private IEnumerator DelayedReleaseTongue(float delay)
    {
        yield return new WaitForSeconds(delay);

        ReleaseTongue();

        yield return null;
    }

    public void OnTongueRelease(InputValue value)
    {
        if (!_tongueActive) return;

        _tongueActive = false;
        _dynamicGrabbed = false;
        _joint.connectedAnchor = Vector2.zero;
        _joint.enabled = false;
        _joint.connectedBody = null;
        _tongueRenderer.enabled = false;
        _mouthSprite.enabled = false;
        _endTongueSprite.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_tongueActive && _joint.distance > _lengthOffset)
        {
            if ( _dynamicGrabbed)
            {
                _dynamicJointDistance = _joint.distance;
                _newJointDistance = (new Vector2(_hitRB.transform.position.x, _hitRB.transform.position.y) - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
                _joint.distance = _newJointDistance < _dynamicJointDistance ? _newJointDistance : _dynamicJointDistance;
            }
            else
            {
                _joint.distance = (_hitPoint - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
            }
        }

        if (_tongueRenderer.enabled)
        {
            _tongueRenderer.SetPosition(1, _movement.IsCrouched ? _crouchedTongueOrigin.position : _tongueOrigin.position);
            
        }

        //if (_missActive)
        //{
        //    _missHitPoint = new Vector2((aimDirection.x * _tongueRange) + _tongueOrigin.position.x,
        //        (aimDirection.y * _tongueRange) + _tongueOrigin.position.y);

        //    _tongueRenderer.SetPosition(0, _missHitPoint);
        //    _endTongueSprite.transform.position = _missHitPoint;
        //}

        if( _dynamicGrabbed)
        {
            _tongueRenderer.SetPosition(0, _joint.connectedBody.transform.position);
            _endTongueSprite.transform.position = _joint.connectedBody.transform.position;
        }
    }

    private void Update()
    {
        _animator.SetBool("TongueOut", _tongueActive);
    }

    public void ReleaseTongue()
    {
        if (!_tongueActive) return;

        //_missActive = false;
        _tongueActive = false;
        _dynamicGrabbed = false;
        _joint.connectedAnchor = Vector2.zero;
        _joint.enabled = false;
        _joint.connectedBody = null;
        _tongueRenderer.enabled = false;
        _mouthSprite.enabled = false;
        _endTongueSprite.enabled = false;
    }
}
