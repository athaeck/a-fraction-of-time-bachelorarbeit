using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReconstructionInteractionAbility : ReconstructAbilitty
{
    [SerializeField]
    private InteractionAbilityData _data;
    [SerializeField]
    private Interactable _interactable;


    public override void InitAbility(AbilityData data)
    {
        _data = data as InteractionAbilityData;
    }

    protected override void HandleCollisionEnter(Information information)
    {
        if (information.origin.TryGetComponent(out Interactable i))
        {
            _interactable = i;
        }
    }

    protected override void HandleCollisionExit(Information information)
    {
        if (information.origin.TryGetComponent(out Interactable i))
        {
            _interactable = null;
        }
    }
    protected override void OnStart()
    {
        animator.CompleteInteract += OnCompleteInteract;
    }
    private void OnCompleteInteract()
    {
        abilitySoundController.PlayAbilitySound(AbilitySoundType.INTERACTION, false);
    }
    protected override void OnReconstructInteraction(Interaction interaction)
    {
        if (interaction.type != InteractionType.INTERACT)
        {
            return;
        }
        if (_interactable == null)
        {
            throw new Exception("Das Interaktionsobjekt existiert nicht oder nicht mehr!");
        }
        if (_interactable.gameObject.GetInstanceID() != interaction.interactedObject.Name)
        {
            throw new Exception("Das Intraktionsobjekt in der Nähe stimmt nicht mit dem interagiertem Objekt überein");
        }
        if (_interactable.IsActive())
        {
            throw new Exception("Das Intraktionsobjekt wurde bereits aktiviert.");
        }
            
        abilitySoundController.PlayAbilitySound(AbilitySoundType.INTERACTION);


        animator.GetAnimator().SetTrigger("Interact");

        _interactable.Interact?.Invoke();


    }

}
