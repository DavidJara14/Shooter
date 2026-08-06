using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth; // para ver la vida del jugador 

    public bool isDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action<GameObject> OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        if (isDead || amount<=0)
        {
            return;
        }

        currentHealth = Math.Max(0f, currentHealth - amount);

        OnHealthChanged?.Invoke(currentHealth, maxHealth); 

        if (currentHealth <= 0f)
        {
            isDead = true;
            OnDeath?.Invoke(source);
        }

    }


}
