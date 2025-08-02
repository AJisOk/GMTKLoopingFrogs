using CharacterMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrogCharacterMovement : CharacterMovement2D
{
    [Header("Crouching")]
    [SerializeField] protected float _crouchHeight;
    [SerializeField] protected Sprite _crouchedSprite;
    [SerializeField] protected float _spriteCrouchedScale = 1.5f;

    protected float _baseHeight;
    protected float _baseSpriteScale;
    protected Sprite _baseSprite;
    protected SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        _baseHeight = Height;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _baseSprite = _spriteRenderer.sprite;
        _baseSpriteScale = _spriteRenderer.transform.localScale.y;
    }

    public void OnCrouch(InputValue value)
    {
        _spriteRenderer.sprite = _crouchedSprite;


        //Height = _crouchHeight;
        CapsuleCollider.size = new Vector2(1, _crouchHeight);
        //_spriteRenderer.transform.localScale.Set(1,_spriteCrouchedScale,1);
    }

    public void OnUnCrouch(InputValue value)
    {
        _spriteRenderer.sprite = _baseSprite;
        
        
        //Height = _baseHeight;
        CapsuleCollider.size = new Vector2(1, _baseHeight);
        //_spriteRenderer.transform.localScale.Set(1,_baseSpriteScale,1);
    }

    public void SetVelocity(Vector2 velocityToSet)
    {
        Velocity = velocityToSet;
    }
}
