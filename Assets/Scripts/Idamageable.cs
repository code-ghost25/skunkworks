// Any component that can take damage implements this.
// The weapon script only depends on this interface, not on a concrete health system,
// so you can build Health.cs later without touching WeaponController.
public interface IDamageable
{
    void TakeDamage(float amount);
}
