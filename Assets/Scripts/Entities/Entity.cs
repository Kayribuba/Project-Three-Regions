using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] internal int _maxHealth = 1;
    [SerializeField] internal int _damage = 1;
    [SerializeField] internal float _speed = 1;

    internal int _health;

    public virtual void Start()
    {
        _health = _maxHealth;
    }

    public virtual void GetHealed(int heal)
    {
        _health = Mathf.Min(_health + heal, _maxHealth);
    }
    public virtual void GetDamaged(int damage)
    {
        _health = Mathf.Max(_health - damage, 0);

        if (_health == 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
