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
}
