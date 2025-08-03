using CharacterMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrogCharacterMovement : CharacterMovement2D
{
    [SerializeField] protected Animator _animator;

    [Header("Crouching")]
    [SerializeField] protected float _crouchHeight;
    [SerializeField] protected float _crouchOffset = 0.75f;
    [SerializeField] protected Sprite _crouchedSprite;
    [SerializeField] protected float _spriteCrouchedScale = 1.5f;
    

    protected float _baseHeight;
    protected float _baseOffset;
    protected float _baseSpriteScale;
    protected Vector3 _baseSpritePosition;
    protected Sprite _baseSprite;
    protected SpriteRenderer _spriteRenderer;

    protected bool _isCrouched = false;
    protected bool _tryingUnCrouch = false;

    public bool IsCrouched { get => _isCrouched; }

    protected override void Awake()
    {
        base.Awake();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_animator == null) _animator = GetComponent<Animator>();

        _baseHeight = Height;
        _baseOffset = CapsuleCollider.offset.y;
        _baseSprite = _spriteRenderer.sprite;
        _baseSpriteScale = _spriteRenderer.transform.localScale.y;
        _baseSpritePosition = _spriteRenderer.transform.localPosition;
    }

    protected virtual void Update()
    {
        _animator.SetFloat("SpeedX", Mathf.Abs(Velocity.x));
        _animator.SetFloat("SpeedY", Velocity.y);
        _animator.SetBool("IsGrounded", IsGrounded);
        _animator.SetBool("IsCrouched", _isCrouched);

        _spriteRenderer.flipX = (Velocity.x < 0f);

        if (_tryingUnCrouch)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, _baseHeight, GroundMask);

            if(hit.collider == null)
            {
                _tryingUnCrouch = false;
                UnCrouch();
            }
        }

    }

    public void OnCrouch(InputValue value)
    {
        _isCrouched = true;
        _tryingUnCrouch = false;
        CapsuleCollider.size = new Vector2(1, _crouchHeight);
        CapsuleCollider.offset = new Vector2(0, _crouchOffset);

        _spriteRenderer.transform.position = new Vector3(_spriteRenderer.transform.position.x,
            _spriteRenderer.transform.position.y - 0.3f,
            _spriteRenderer.transform.position.z);

        //_spriteRenderer.sprite = _crouchedSprite;
        //Height = _crouchHeight;
        //_spriteRenderer.transform.localScale.Set(1,_spriteCrouchedScale,1);
    }

    public void OnUnCrouch(InputValue value)
    {
        //check if something is blocking player from uncrouching
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up,_baseHeight,GroundMask);

        if(hit.collider != null)
        {
            _tryingUnCrouch = true;
            return;
        }

        UnCrouch();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * _baseHeight));
        
    }

    protected virtual void UnCrouch()
    {
        CapsuleCollider.size = new Vector2(1, _baseHeight);
        CapsuleCollider.offset = new Vector2(0, _baseOffset);
        _spriteRenderer.transform.localPosition = _baseSpritePosition;
        _isCrouched = false;

        //_spriteRenderer.sprite = _baseSprite;
        //Height = _baseHeight;
        //_spriteRenderer.transform.localScale.Set(1,_baseSpriteScale,1);
    }

    public void SetVelocity(Vector2 velocityToSet)
    {
        Velocity = velocityToSet;
    }
}
