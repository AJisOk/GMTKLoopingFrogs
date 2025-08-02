using CharacterMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrogCharacterMovement : CharacterMovement2D
{
    [SerializeField] protected Animator _animator;

    [Header("Crouching")]
    [SerializeField] protected float _crouchHeight;
    [SerializeField] protected Sprite _crouchedSprite;
    [SerializeField] protected float _spriteCrouchedScale = 1.5f;
    

    protected float _baseHeight;
    protected float _baseSpriteScale;
    protected Sprite _baseSprite;
    protected SpriteRenderer _spriteRenderer;

    protected bool _isCrouched = false;

    protected override void Awake()
    {
        base.Awake();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_animator == null) _animator = GetComponent<Animator>();

        _baseHeight = Height;
        _baseSprite = _spriteRenderer.sprite;
        _baseSpriteScale = _spriteRenderer.transform.localScale.y;
    }

    protected virtual void Update()
    {
        _animator.SetFloat("SpeedX", Mathf.Abs(Velocity.x));
        _animator.SetFloat("SpeedY", Velocity.y);
        _animator.SetBool("IsGrounded", IsGrounded);
        _animator.SetBool("IsCrouched", _isCrouched);

        _spriteRenderer.flipX = (Velocity.x < 0f);

    }

    public void OnCrouch(InputValue value)
    {
        _isCrouched = true;
        CapsuleCollider.size = new Vector2(1, _crouchHeight);

        //_spriteRenderer.sprite = _crouchedSprite;
        //Height = _crouchHeight;
        //_spriteRenderer.transform.localScale.Set(1,_spriteCrouchedScale,1);
    }

    public void OnUnCrouch(InputValue value)
    {
        _isCrouched = false;
        CapsuleCollider.size = new Vector2(1, _baseHeight);
        
        
        //_spriteRenderer.sprite = _baseSprite;
        //Height = _baseHeight;
        //_spriteRenderer.transform.localScale.Set(1,_baseSpriteScale,1);
    }

    public void SetVelocity(Vector2 velocityToSet)
    {
        Velocity = velocityToSet;
    }
}
