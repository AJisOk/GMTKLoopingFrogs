using UnityEngine;
using UnityEngine.Events;

public class Consumable : MonoBehaviour
{
    [SerializeField] protected int _playerLayer = 7;

    public UnityEvent OnConsume;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != _playerLayer) return;

        Consume();
    }

    protected virtual void Consume()
    {
        OnConsume.Invoke();

        Destroy(this.gameObject);
    }
}
