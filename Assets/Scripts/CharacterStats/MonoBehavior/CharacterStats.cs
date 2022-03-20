using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    private CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from CharacterData_SO
    public int MaxHealth
    {
        get { return characterData == null ? 0 : characterData.maxHealth; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { return characterData == null ? 0 : characterData.currentHealth; }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get { return characterData == null ? 0 : characterData.baseDefence; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { return characterData == null ? 0 : characterData.currentDefence; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        // Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        // TODO: 经验update
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }
        return (int)coreDamage;
    }

    #endregion
}
