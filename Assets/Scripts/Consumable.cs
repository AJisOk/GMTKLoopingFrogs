using UnityEngine;
using UnityEngine.Events;

public class Consumable : MonoBehaviour
{
    [SerializeField] protected LayerMask _playerLayer;

    public UnityEvent OnConsume;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == _playerLayer.value) return;

        Consume();
    }

    protected virtual void Consume()
    {
        OnConsume.Invoke();

        Destroy(this.gameObject);
    }
}
