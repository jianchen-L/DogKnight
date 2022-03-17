using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

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
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        // TODO: Update UI
        // TODO: 经验update
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
