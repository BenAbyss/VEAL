using System;
using DG.Tweening;
using UnityEngine;

public class InteractiveNodeSpecsPanelManager : NodeSpecsPanelManager
{
    public static event Action HideMenus;
    public static event Action<string> LoadInternals;
    private InteractiveNode Node;
    
    /// <summary>
    /// Method <c>ChangeNode</c> changes the node to focus on.
    /// <param name="new_node">The new node to focus on.</param>
    /// </summary>
    public override void ChangeNode(InteractiveNode new_node)
    {
        Node = new_node;
    }

    /// <summary>
    /// Method <c>EnterInternals</c> enters the node's internals.
    /// </summary>
    public void EnterInternals()
    {
        HideMenus?.Invoke();
        DOTween.Sequence().Append(Node.transform.DOScale(new Vector3(15f, 15f, 0), 1f)
            .SetEase(Ease.Linear)).OnComplete(InitiateEntry);
    }

    /// <summary>
    /// Method <c>InitiateEntry</c> sets up for entering the nodes internals.
    /// </summary>
    private void InitiateEntry()
    {
        LoadInternals?.Invoke(Node.GetInternalsName());
    }
}