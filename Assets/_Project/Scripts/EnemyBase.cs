using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable, IDamageSource
{
    [field: SerializeField] private int _maxHealth;
    [field: SerializeField] public int _contactDamage;
    private int _currentHealth;
    public int CurrentHealth => _currentHealth;
    public int DamageAmount => _contactDamage;

    [Header("Visuals")]
    [SerializeField] private float _flashDuration = 0.5f;
    [SerializeField] private float _flashAmount = 0.5f;
    private SpriteRenderer _spriteRenderer;
    private Material _flashMaterial;
    private Coroutine _flashCoroutine;

    private static readonly int FlashAmountProperty = Shader.PropertyToID("_FlashAmount");

    void Awake()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _flashMaterial = _spriteRenderer.material;
        }
    }

    protected virtual void OnEnable()
    {
        ResetFlashEffect();
    }

    public void TakeDamage(int amount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth = Math.Max(0, _currentHealth - Math.Max(0, amount));

        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }
        _flashCoroutine = StartCoroutine(HitFlashRoutine());

        if (_currentHealth <= 0)
        {
            OnDie();
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        if (_flashMaterial == null) yield break;

        float correntFlashAmount = _flashAmount;
        _flashMaterial.SetFloat(FlashAmountProperty, correntFlashAmount);

        while (correntFlashAmount > 0f)
        {
            correntFlashAmount -= Time.deltaTime / _flashDuration;
            _flashMaterial.SetFloat(FlashAmountProperty, Mathf.Max(0f, correntFlashAmount));
            yield return null;
        }
    }

    protected virtual void OnDie()
    {
        _currentHealth = _maxHealth;
        if (_flashMaterial != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }

        this.gameObject.SetActive(false);
    }

    private void ResetFlashEffect()
    {
        if (_flashMaterial != null)
            _flashMaterial.SetFloat(FlashAmountProperty, 0f);
    }
}