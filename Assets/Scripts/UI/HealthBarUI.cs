using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public bool alwaysVisible;
    public Transform barPoint;
    float visibleTime = 3;
    float timeLife;
    Image healthSlider;
    Transform UIbar;
    Transform cam;
    CharacterStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        cam = Camera.main.transform;
        foreach (var canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);
        UIbar.gameObject.SetActive(true);
        timeLife = visibleTime;
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIbar)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = cam.forward;
            if (timeLife <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLife -= Time.deltaTime;
        }
    }
}
