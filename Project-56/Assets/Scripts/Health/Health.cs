using UnityEngine;

public class Health : MonoBehaviour {
    public int health = 100;
    public int maxHealth = 100;

    public delegate void OnDamageTaken(Damage damage);
    public event OnDamageTaken DamageTaken;

    public delegate void OnHealed(Damage damage);
    public event OnHealed HealApplied;

    public delegate void OnDeath(Damage damage);
    public event OnDeath death;

    public void ApplyDamage(Damage damage) {
        health -= damage.damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        DamageTaken?.Invoke(damage);

        if (health <= 0) {
            Die(damage);
        }
    }

    public void ApplyHeal(Damage damage) {
        health += damage.damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        HealApplied?.Invoke(damage);
    }

    void Die(Damage damage) {
        death?.Invoke(damage);
    }
}

public class Damage {
    public int damage;

    public Damage(int damage) {
        this.damage = damage;
    }
}
