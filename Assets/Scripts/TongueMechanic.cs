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


    private SpringJoint2D _joint;
    private Vector2 _hitPoint;
    private Rigidbody2D _hitRB;
    private Vector3 _transformOffset = new Vector3(0f,1f,0f);

    private bool _tongueActive = false;
    private bool _dynamicGrabbed = false;

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

        //fire a false shot on miss
        if (hit.collider == null) return;

        //form a different type of connection if a dynamic object is hit
        if(hit.rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            _tongueActive=true;
            _dynamicGrabbed = true;

            _hitRB = hit.rigidbody;
            _joint.connectedBody = hit.collider.GetComponent<Rigidbody2D>();
            _joint.distance = (new Vector2(_hitRB.transform.position.x, _hitRB.transform.position.y) - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
            _joint.enabled = true;


            _tongueRenderer.SetPosition(0, hit.collider.transform.position);
            _tongueRenderer.SetPosition(1, transform.position + _transformOffset);
            _tongueRenderer.enabled = true;

            if(hit.rigidbody.TryGetComponent<Interactable>(out Interactable interactable))
            {
                interactable.OnGrapple();
                //StartCoroutine(DelayedReleaseTongue());
                ReleaseTongue();
            }


            return;
        }


        _tongueActive = true;

        _hitPoint = hit.point;
        _joint.connectedAnchor = _hitPoint;
        _joint.distance = (_hitPoint - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
        _joint.enabled = true;

        _tongueRenderer.enabled = true;
        _tongueRenderer.SetPosition(0, _hitPoint);
        _tongueRenderer.SetPosition(1, transform.position + _transformOffset);
        
        
    }

    private IEnumerator DelayedReleaseTongue()
    {
        yield return new WaitForSeconds(.5f);

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
    }

    private void FixedUpdate()
    {
        if (_tongueActive && _joint.distance > _lengthOffset)
        {
            if ( _dynamicGrabbed)
            {
                _joint.distance = (new Vector2(_hitRB.transform.position.x, _hitRB.transform.position.y) - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
            }
            else
            {
                _joint.distance = (_hitPoint - new Vector2(transform.position.x, transform.position.y + 1)).magnitude - _lengthOffset;
            }
        }

        if (_tongueRenderer.enabled)
        {
            _tongueRenderer.SetPosition(1, transform.position + _transformOffset);
            
        }

        if( _dynamicGrabbed)
        {
            _tongueRenderer.SetPosition(0, _joint.connectedBody.transform.position);
        }
    }

    public void ReleaseTongue()
    {
        if (!_tongueActive) return;

        _tongueActive = false;
        _dynamicGrabbed = false;
        _joint.connectedAnchor = Vector2.zero;
        _joint.enabled = false;
        _joint.connectedBody = null;
        _tongueRenderer.enabled = false;
    }
}
