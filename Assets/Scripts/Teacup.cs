using UnityEngine;
using UnityEngine.Events;

public class Teacup : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _trigger;

    private UnityEvent OnTeaPoisoned;

    private bool _teaPoisoned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Poison>(out Poison poison) || _teaPoisoned) return;

        _teaPoisoned = true;

        OnTeaPoisoned.Invoke();

        //play animation of bottle pooring into teacup\

        //release frog tongue
    }
}
