using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [field: SerializeField] private int _maxHealth;
    private int _currentHealth;
    public int CurrentHealth => _currentHealth;

    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Math.Max(0, _currentHealth - Math.Max(0, amount));
        if (_currentHealth <= 0)
        {
            OnDie();
        }
    }

    protected virtual void OnDie()
    {
        _currentHealth = _maxHealth;
        this.gameObject.SetActive(false);
    }
}