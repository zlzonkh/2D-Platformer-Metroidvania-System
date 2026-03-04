using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable, IDamageSource
{
    [field: SerializeField] private int _maxHealth;
    [field: SerializeField] public int _contactDamage;
    private int _currentHealth;
    public int CurrentHealth => _currentHealth;
    public int DamageAmount => _contactDamage;

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