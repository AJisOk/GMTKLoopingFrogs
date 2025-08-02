using System.Collections;
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
    [SerializeField] private Image _vignette;
    [SerializeField] private CanvasGroup _durationIncreasedCG;
    [SerializeField] private Color _startColour = Color.green;
    [SerializeField] private Color _pulse1Colour = Color.red;
    [SerializeField] private Color _pulse2Colour = Color.red;
    [SerializeField] private Color _durationIncreaseColour = Color.blue;
    [SerializeField] private Color _clearColour = Color.red;
    [SerializeField] private AnimationCurve _pulseCurve;
    [SerializeField] private AnimationCurve _durationIncreaseCurve;

    public UnityEvent OnLoopDurationExtended;
    public UnityEvent OnLoopReset;

    private float _loopDuration;
    private float _timer = 0f;
    private Color c;

    private bool _countdown = false;

    private void Awake()
    {
        _loopDuration = _baseLoopDuration;
        _vignette.color = _clearColour;
        _durationIncreasedCG.alpha = 0f;

    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _screenFill.fillAmount = 1f - Mathf.InverseLerp(0, _loopDuration, _timer);


        if (_timer >= _loopDuration)
        {
            ResetLoop();
            return;
        }

        if (!_countdown && _loopDuration - _timer <= 1f)
        {
            StartCoroutine(YellowPulse());
            return;
        }

        if (!_countdown && _loopDuration - _timer <= 2f)
        {
            StartCoroutine(RedPulse());
            return;
        }
    }

    public void ExtendLoopDuration()
    {
        _loopDuration += _loopIncrement;
        StopAllCoroutines();
        StartCoroutine(BluePulse());
    }

    public void ResetLoop()
    {
        _timer = 0f;
        _screenFill.fillAmount = 1f - Mathf.InverseLerp(0, _loopDuration, _timer);
        
        _characterMovement.SetVelocity(new Vector2(0, 0));
        _characterMovement.transform.position = _startLocation.transform.position;

        StartCoroutine(GreenPulse());

        OnLoopReset.Invoke();
    }

    private IEnumerator BluePulse()
    {
        _countdown = true;
        c = _durationIncreaseColour;
        _vignette.color = c;

        _durationIncreasedCG.alpha = 1f;

        float t = 0f;
        while (t < 2f)
        {
            c.a = Mathf.Lerp(_clearColour.a, _durationIncreaseColour.a, _durationIncreaseCurve.Evaluate(t));
            _vignette.color = c;

            _durationIncreasedCG.alpha = _durationIncreaseCurve.Evaluate(t);

            t += Time.deltaTime;
            yield return null;
        }
        _durationIncreasedCG.alpha = 0f;
        _countdown = false;

        yield return null;
    }

    private IEnumerator RedPulse()
    {
        _countdown = true;
        c = _pulse1Colour;
        _vignette.color = c;

        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(_clearColour.a, _pulse1Colour.a, _pulseCurve.Evaluate(t));
            _vignette.color = c;

            t += Time.deltaTime;
            yield return null;
        }
        _countdown = false;

        yield return null;
    }

    private IEnumerator YellowPulse()
    {

        _countdown = true;
        c = _pulse2Colour;
        _vignette.color = c;
        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(_clearColour.a, _pulse2Colour.a, _pulseCurve.Evaluate(t));
            _vignette.color = c;

            t += Time.deltaTime;
            yield return null;
        }
        _countdown = false;

        yield return null;
    }

    private IEnumerator GreenPulse()
    {

        _countdown = true;
        c = _startColour;
        _vignette.color = c;
        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(_clearColour.a, _startColour.a, _pulseCurve.Evaluate(t));
            _vignette.color = c;

            t += Time.deltaTime;
            yield return null;
        }
        _countdown = false;
        yield return null;
    }

}
