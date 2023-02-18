using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(Character))]
public class InteractionAbility : Ability, IInteract
{
    [SerializeField]
    private InteractionAbilityData _data;
    [SerializeField]
    private Interactable _interactable;
    [SerializeField]
    private bool _isColidedWithInteractable = false;
    private bool _clicked = false;
    protected override void HandleCollisionEnter(Information information)
    {
        if (information.origin.TryGetComponent(out Interactable i))
        {
            _interactable = i;
            _isColidedWithInteractable = true;
        }
    }
    protected override void HandleCollisionExit(Information information)
    {
        if (information.origin.TryGetComponent(out Interactable i))
        {
            _interactable = null;
            _isColidedWithInteractable = false;
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!_player.GetState(BlockState.INTERACT))
        {
            return;
        }
        BoxCollider col = _interactable.GetLookAt().GetComponent<BoxCollider>();
        Vector3 dir = col.ClosestPoint(transform.position);
        dir.y = transform.position.y;
        Vector3 forward = (dir - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(forward);
    }

    IEnumerator OnClick()
    {
        _clicked = true;
        yield return new WaitForSeconds(1f);
        _clicked = false;
    }
    public override void InitAbility(AbilityData data)
    {
        _data = data as InteractionAbilityData;
    }

    private void OnPullInteractableObject()
    {
        _interactable.Interact?.Invoke();
    }

    private void OnCompleteInteract()
    {
        _player.SetIsInteracting(false);
        _interactable = null;
        _isColidedWithInteractable = false;
    }

    protected override void HandleStart()
    {
        _animationController.PullInteractableObject += OnPullInteractableObject;
        _animationController.CompleteInteract += OnCompleteInteract;
    }

    public void TakeInput()
    {
        if (_isColidedWithInteractable == false)
        {
            return;
        }
        if (_clicked)
        {
            return;
        }
        if (_interactable == null)
        {
            return;
        }
        if (!_player.IsEnabled(BlockState.INTERACT))
        {
            return;
        }
        if(_interactable.IsActive())
        {
            return;
        }
        _player.SetIsInteracting(true);
        SetTrigger?.Invoke("Interact");
        HandleInteraction(new Interaction(InteractionType.INTERACT, _player, Utils.MigrateGameobjectToToSafe(_interactable.gameObject), Utils.MigrateGameobjectToToSafe(_interactable.gameObject), Utils.GetTimestamp()));
        _abilitySoundController.PlayAbilitySound(AbilitySoundType.INTERACTION);

        StartCoroutine(OnClick());
    }
}
