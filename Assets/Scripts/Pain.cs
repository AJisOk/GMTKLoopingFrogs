using UnityEngine;

public class Pain : MonoBehaviour
{
    [SerializeField] private Collider2D _trigger;
    [SerializeField] private TimeLoop _timeLoop;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<FrogCharacterMovement>(out FrogCharacterMovement movement))
        {
            _timeLoop.ResetLoop();
        }
    }

}
