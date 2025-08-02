using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeLoop : MonoBehaviour
{
    [SerializeField] private float _baseLoopDuration = 5f;
    [SerializeField] private float _loopIncrement = 2f;
    [SerializeField] private FrogCharacterMovement _characterMovement;
    [SerializeField] private Transform _startLocation;

    [Header("UI")]
    [SerializeField] private Image _screenFill;

    public UnityEvent OnLoopDurationExtended;
    public UnityEvent OnLoopReset;

    private float _loopDuration;
    private float _timer = 0f;

    private void Awake()
    {
        _loopDuration = _baseLoopDuration;
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _screenFill.fillAmount = 1f - Mathf.InverseLerp(0, _loopDuration, _timer);

        if(_timer >= _loopDuration) ResetLoop();

    }

    public void ExtendLoopDuration()
    {
        _loopDuration += _loopIncrement;
    }

    private void ResetLoop()
    {
        _timer = 0f;
        _screenFill.fillAmount = 1f - Mathf.InverseLerp(0, _loopDuration, _timer);
        
        _characterMovement.SetVelocity(new Vector2(0, 0));
        _characterMovement.transform.position = _startLocation.transform.position;

        OnLoopReset.Invoke();
    }
}
