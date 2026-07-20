using UnityEngine;
using UnityEngine.Events;

// Attach to any object that should be able to take damage: a test dummy, an enemy, a player.
// Must be on the SAME GameObject as the Collider that gets hit by the raycast
// (WeaponController also checks parents, so it's fine on a parent of the collider too).
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Events (optional - hook these up in the Inspector)")]
    public UnityEvent<float> OnDamaged; // passes remaining health
    public UnityEvent OnDeath;

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        Debug.Log($"{name} took {amount} damage, {currentHealth}/{maxHealth} left");
        OnDamaged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
}
