using UnityEngine;

public class PlayerAttackVfxEventRelay : MonoBehaviour
{
    private Player _player;

    void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public void RelayExecuteAttack()
    {
        _player.ExecuteAttack();
    }
}
