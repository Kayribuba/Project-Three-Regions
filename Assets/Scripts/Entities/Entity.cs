using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal float b_maxHealth = 1;
    [SerializeField] internal float b_damage = 1;
    [SerializeField] internal float b_speed = 1;
    [SerializeField] internal float b_knockbackDuration = .5f;
    [SerializeField] internal float tickRate = .1f;

    internal float b_health;
    internal bool b_inputEnabled = true;
    bool UpdateBeh_Enabled = true;

    public virtual void Start()
    {
        b_health = b_maxHealth;

        if (rb == null) rb = GetComponent<Rigidbody2D>();

        SetUpdateCoroutine(true);
    }

    public virtual void AddHealth(float amount) => SetHealth(b_health + amount);
    public virtual void RemoveHealth(float amount) => SetHealth(b_health - amount);
    public void SetHealth(float setTo)
    {
        b_health = Mathf.Clamp(setTo, 0, b_maxHealth);

        if (b_health == 0) Die();
    }
    public void KnockBack(Vector2 direction, float force = 1)
    {
        DisableInputForSeconds(b_knockbackDuration);
        rb.velocity = direction * force;
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
        else
        {
            StopCoroutine(nameof(UpdateEnum));
        }
    }
    internal void DisableInputForSeconds(float seconds)
    {
        if (b_inputEnabled == false) StopCoroutine(nameof(DisableInput));

        StartCoroutine(nameof(DisableInput), seconds);
    }
    internal void SetInputEnabled(bool setTo)
    {
        StopCoroutine(nameof(DisableInput));

        b_inputEnabled = setTo;
    }
    IEnumerator DisableInput(float forSeconds)
    {
        b_inputEnabled = false;
        yield return new WaitForSeconds(forSeconds);
        b_inputEnabled = true;
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