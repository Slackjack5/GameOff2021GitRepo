using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
  [SerializeField] private Encounter[] encounters;
  [SerializeField] private CombatManager combatManager;
  [SerializeField] private Shop shop;
  [SerializeField] private GameObject continueCommand;

  private enum State
  {
    PreEncounter,
    InEncounter
  }

  private State currentState;
  private Encounter currentEncounter;
  private int currentEncounterIndex;
  private int currentGold;

  private static readonly HashSet<Consumable> consumablesOwned = new HashSet<Consumable>();

  public static IEnumerable<Command> ConsumablesOwned => consumablesOwned;

  private void Awake()
  {
    currentState = State.PreEncounter;

    var button = continueCommand.GetComponentInChildren<Button>();
    button.onClick.AddListener(StartEncounter);

    CombatManager.onStateChange.AddListener(OnCombatStateChange);

    shop.onPurchase.AddListener(OnPurchase);
    shop.onClose.AddListener(gold =>
    {
      currentGold = gold;
      EndEncounter(true);
    });
  }

  private void Update()
  {
    if (currentState == State.PreEncounter)
    {
      continueCommand.SetActive(true);
      EventSystem.current.SetSelectedGameObject(continueCommand.GetComponentInChildren<Button>().gameObject);
    }
    else
    {
      continueCommand.SetActive(false);
    }
  }

  private void StartEncounter()
  {
    if (currentState == State.InEncounter)
    {
      Debug.LogError("Failed to start encounter. We are already in an encounter!");
      return;
    }

    if (currentEncounterIndex >= encounters.Length)
    {
      Debug.LogError($"Failed to start encounter. Index {currentEncounterIndex} is out of bounds!");
      return;
    }

    // Reset the current encounter.
    if (currentEncounter != null)
    {
      Destroy(currentEncounter.gameObject);
    }

    currentEncounter = Instantiate(encounters[currentEncounterIndex]);

    if (currentEncounterIndex == 0)
    {
      foreach (Hero hero in CombatManager.Heroes)
      {
        hero.Reset(true);
      }
    }

    if (currentEncounter.IsShop)
    {
      shop.Open(currentGold);
    }
    else
    {
      combatManager.Begin(currentEncounter);
    }

    currentState = State.InEncounter;
  }

  private void EndEncounter(bool isWin)
  {
    if (currentState == State.PreEncounter)
    {
      Debug.LogError("Failed to end encounter. We are not in an encounter!");
      return;
    }

    if (isWin)
    {
      currentGold += encounters[currentEncounterIndex].Gold;
      currentEncounterIndex++;
    }
    else
    {
      currentGold = 0;
      currentEncounterIndex = 0;
    }

    combatManager.Reset();
    Timer.Deactivate();
    currentState = State.PreEncounter;
  }

  private void OnCombatStateChange(CombatManager.State state)
  {
    switch (state)
    {
      case CombatManager.State.EndLose:
        EndEncounter(false);
        break;
      case CombatManager.State.EndWin:
        EndEncounter(true);
        break;
    }
  }

  private void OnPurchase(Consumable consumable)
  {
    if (!consumablesOwned.Contains(consumable))
    {
      consumablesOwned.Add(consumable);
    }

    consumable.IncrementAmountOwned();
  }
}
