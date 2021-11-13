using TMPro;
using UnityEngine;

public class StatusCard : MonoBehaviour
{
  [SerializeField] private Hero hero;

  private TextMeshProUGUI heroNameComponent;
  private StatusBar healthBar;
  private StatusBar staminaBar;

  private void Start()
  {
    // Name should be the first TextMeshProUGUI component encountered.
    heroNameComponent = GetComponentInChildren<TextMeshProUGUI>();
    heroNameComponent.text = hero.Name;

    // Health bar should be the first bar.
    healthBar = GetComponentsInChildren<StatusBar>()[0];
    healthBar.SetMaxValue(hero.MaxHealth);
    healthBar.SetValue(hero.CurrentHealth);

    // Stamina bar should be the second bar.
    staminaBar = GetComponentsInChildren<StatusBar>()[1];
    staminaBar.SetMaxValue(hero.MaxStamina);
    staminaBar.SetValue(hero.CurrentStamina);
  }

  private void Update()
  {
    healthBar.SetValue(hero.CurrentHealth);
    staminaBar.SetValue(hero.CurrentStamina);
  }
}
