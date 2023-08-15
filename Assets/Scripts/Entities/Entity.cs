using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Transform _barrel;
    [SerializeField] internal GameObject _projectile;
    [SerializeField] internal float _maxHealth = 1;
    [SerializeField] internal float _damage = 1;
    [SerializeField] internal float _speed = 1;
    [SerializeField] internal float tickRate = .1f;

    internal float _health;
    bool UpdateBeh_Enabled = true;

    public virtual void Start()
    {
        _health = _maxHealth;

        if (rb == null) rb = GetComponent<Rigidbody2D>();

        SetUpdateCoroutine(true);
    }

    public void AddHealth(float amount) => SetHealth(_health + amount);
    public void RemoveHealth(float amount) => SetHealth(_health - amount);
    public void SetHealth(float SetTo)
    {
        _health = Mathf.Clamp(SetTo, 0,_maxHealth);

        if (_health == 0) Die();
    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }
    public void ToggleUpdateCoroutine() => SetUpdateCoroutine(!UpdateBeh_Enabled);
    public void SetUpdateCoroutine(bool enable)
    {
        UpdateBeh_Enabled = enable;

        if (enable)
        {
            StartCoroutine(nameof(UpdateEnum));
        }
    }
    internal abstract void UpdateBehaviour();
    IEnumerator UpdateEnum()
    {
        while (UpdateBeh_Enabled)
        {
            UpdateBehaviour();

            yield return new WaitForSeconds(tickRate);
        }
    }
}