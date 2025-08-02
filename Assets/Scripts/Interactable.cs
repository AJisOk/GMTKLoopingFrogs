using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private LineRenderer _rope;
    [SerializeField] private Rigidbody2D _hatch;
    [SerializeField] private SpringJoint2D _spring;

    private Rigidbody2D _rb;

    private bool _handleGrabbed = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rope.SetPosition(0, _hatch.transform.position);
        _rope.SetPosition(1, transform.position);

        _spring.connectedBody = _rb;
        _spring.distance = (transform.position - _hatch.transform.position).magnitude;

    }

    private void Update()
    {
        _rope.SetPosition(0, _hatch.transform.position);
        _rope.SetPosition(1, transform.position);
        
    }


    public void OnGrapple()
    {
        if (_handleGrabbed) return;

        _handleGrabbed = true;

        _hatch.bodyType = RigidbodyType2D.Dynamic;

        
    }
}
