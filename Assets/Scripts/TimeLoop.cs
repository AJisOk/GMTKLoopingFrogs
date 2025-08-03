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
    [SerializeField] private GameObject _smokePoof;

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

    [Header("End Anim")]
    [SerializeField] private Poison _poison;
    [SerializeField] private Teacup _teacup;
    [SerializeField] private Sprite _poisonSprite;
    [SerializeField] private CanvasGroup _curseLiftedCG;
    [SerializeField] private CanvasGroup _witchPoisonedCG;
    [SerializeField] private Animator _animator;
    [SerializeField] private Color _witchPoisonedColour = Color.blue;
    [SerializeField] private Color _curseLiftedColour = Color.yellow;
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private CanvasGroup _gameOverCG;


    public UnityEvent OnLoopDurationExtended;
    public UnityEvent OnLoopReset;

    private float _loopDuration;
    private float _timer = 0f;
    private Color c;
    private TongueMechanic _tongue;
    private bool _isTiming = true;

    private bool _countdown = false;

    private void Awake()
    {
        _loopDuration = _baseLoopDuration;
        _vignette.color = _clearColour;
        _durationIncreasedCG.alpha = 0f;
        _tongue = _characterMovement.GetComponent<TongueMechanic>();

        _gameOverCanvas.gameObject.SetActive(false);
        _gameOverCG.alpha = 0f;

    }

    private void FixedUpdate()
    {
        if(_isTiming) _timer += Time.deltaTime;
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
        
        GameObject.Instantiate(_smokePoof, new Vector3(_characterMovement.transform.position.x, _characterMovement.transform.position.y+1, _characterMovement.transform.position.z) , Quaternion.identity);

        _characterMovement.SetVelocity(new Vector2(0, 0));
        _characterMovement.transform.position = _startLocation.transform.position;

        GameObject.Instantiate(_smokePoof, new Vector3(_characterMovement.transform.position.x, _characterMovement.transform.position.y+1, _characterMovement.transform.position.z) , Quaternion.identity);
        StartCoroutine(GreenPulse());

        OnLoopReset.Invoke();
    }

     public void OnPoisonDelivered()
    {
        StartCoroutine(PoisonDelivered());

    }

    private IEnumerator PoisonDelivered()
    {
        //Time.timeScale = .8f;
        _isTiming = false;

        _characterMovement.transform.position = _startLocation.position;
        GameObject.Instantiate(_smokePoof, new Vector3(_characterMovement.transform.position.x, _characterMovement.transform.position.y+1, _characterMovement.transform.position.z) , Quaternion.identity);
        
        _characterMovement.SetVelocity(new Vector2(-0.1f, 0f));
        _characterMovement.CanMove = false;
        _tongue.CanTongue = false;
        _tongue.ReleaseTongue();

        GameObject.Instantiate(_smokePoof, _poison.transform.position, Quaternion.identity);
        _poison.gameObject.SetActive(false);
        _animator.SetBool("PoisonDelivered", true);
        


        yield return new WaitForSeconds(3.3f);

        //play purple pulse for witch poisoned
        c = _witchPoisonedColour;
        _vignette.color = c;

        _witchPoisonedCG.alpha = 1f;

        float t = 0f;
        while (t < 2f)
        {
            c.a = Mathf.Lerp(_clearColour.a, _witchPoisonedColour.a, _durationIncreaseCurve.Evaluate(t));
            _vignette.color = c;

            _witchPoisonedCG.alpha = _durationIncreaseCurve.Evaluate(t);

            t += Time.deltaTime;
            yield return null;
        }
        _witchPoisonedCG.alpha = 0f;

        //play yellow pulse for curse lifted
        c = _curseLiftedColour;
        _vignette.color = c;

        _curseLiftedCG.alpha = 1f;

        t = 0f;
        while (t < 2f)
        {
            c.a = Mathf.Lerp(_clearColour.a, _curseLiftedColour.a, _durationIncreaseCurve.Evaluate(t));
            _vignette.color = c;

            _curseLiftedCG.alpha = _durationIncreaseCurve.Evaluate(t);

            t += Time.deltaTime;
            yield return null;
        }
        _curseLiftedCG.alpha = 0f;

        _gameOverCanvas.gameObject.SetActive(true);
        t = 0f;
        while (t < 2f)
        {
            _gameOverCG.alpha = Mathf.Lerp(0,1,t);

            t += Time.deltaTime;
            yield return null;
        }
        _gameOverCG.alpha = 1f;


        yield return null;
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
