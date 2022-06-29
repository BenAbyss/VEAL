using System;
using DG.Tweening;
using UnityEngine;

public class InteractiveNodeSpecsPanelManager : NodeSpecsPanelManager
{
    public static event Action HideMenus;
    public static event Action<string> LoadInternals;
    
    public override void ChangeNode(InteractiveNode new_node)
    {
        Node = new_node;
    }

    public void EnterInternals()
    {
        HideMenus?.Invoke();
        DOTween.Sequence().Append(Node.transform.DOScale(new Vector3(15f, 15f, 0), 1f)
            .SetEase(Ease.Linear)).OnComplete(InitiateEntry);
    }

    private void InitiateEntry()
    {
        LoadInternals?.Invoke(Node.GetInternalsName());
    }
}