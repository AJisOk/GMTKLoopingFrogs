using UnityEngine;

public class Teacup : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _trigger;


    private bool _teaPoisoned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Poison>(out Poison poison)) return;

        _teaPoisoned = true;

        Debug.Log("Tea Poisoned");

        //play animation of bottle pooring into teacup\

        //release frog tongue
    }
}
