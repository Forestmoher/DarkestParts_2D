using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject _actionsContainer;
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _fleeButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TextMeshProUGUI _currentActorText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _strengthText;
    [SerializeField] private TextMeshProUGUI _speedText;

    //managers
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private CombatManager _combatManager;

    public UnityEvent EnterMovePhase;
    public UnityEvent EnterActionPhase;
    public UnityEvent CancelCurrentPhase;
    public UnityEvent TryFleeCombat;


    private void OnEnable()
    {
        _combatManager.TurnStarted += UpdateActorUI;
        ApplyButtonHandlers();
    }

    private void OnDisable()
    {
        RemoveButtonHandlers();
        _combatManager.TurnStarted -= UpdateActorUI;
    }

    private void ApplyButtonHandlers()
    {
        _moveButton.onClick.AddListener(OnClickMove);
        _attackButton.onClick.AddListener(OnClickAttack);
        _cancelButton.onClick.AddListener(OnClickCancel);
        _fleeButton.onClick.AddListener(OnClickFlee);
    }

    private void RemoveButtonHandlers()
    {
        _moveButton.onClick.RemoveAllListeners();
        _attackButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
        _fleeButton.onClick.RemoveAllListeners();
    }

    private void OnClickMove()
    {
        if(!_combatManager.hasMoved)
        {
            ShowActionsContainer(false);
            EnterMovePhase.Invoke();
        }
    }

    private void OnClickAttack()
    {
        if (!_combatManager.hasTakenAction)
        {
            ShowActionsContainer(false);
            EnterActionPhase.Invoke();
        }
    }

    private void OnClickFlee()
    {
        ShowActionsContainer(false);
        TryFleeCombat.Invoke();
    }

    private void OnClickCancel()
    {
        ShowActionsContainer(true);
        CancelCurrentPhase.Invoke();
    }

    public void ShowActionsContainer(bool show)
    {
        _moveButton.gameObject.SetActive(!_combatManager.hasMoved); 
        _actionsContainer.SetActive(show);
        _cancelButton.gameObject.SetActive(!show);
    }

    //TODO: remove all ui interaction on opponent turn 
    private void UpdateActorUI(Actor actor)
    {
        if (actor.isPlayer)
        {
            ShowActionsContainer(true);
        }
        else
        {
            ShowActionsContainer(false);
            _cancelButton.gameObject.SetActive(false);
        }

        print(actor.name);  
        _currentActorText.text = $" {actor.name}'s Turn";
        _nameText.text = actor.stats.actorName;
        _healthText.text = $"{actor.currentHealth}/{actor.stats.maxHealth}";
        _strengthText.text = actor.stats.strength.ToString();
        _speedText.text = actor.stats.speed.ToString();

    }
}
