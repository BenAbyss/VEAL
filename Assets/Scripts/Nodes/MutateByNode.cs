using System;
using UnityEngine.EventSystems;

public class MutateByNode : BasicNode, IEndDragHandler, IPointerDownHandler
{
    protected override string NodeType => "MutateBy";
    
    /// <summary>
    /// Method <c>OnPointerUp</c> marks the node as selected and reveals the side menu as the node is placed.
    /// </summary>
    public new void OnEndDrag(PointerEventData event_data)
    {
        try
        {
            NodeConnectors.BeginConnecting(nodeId);
            base.OnEndDrag(event_data);
        } catch(NullReferenceException) {}
    }

    /// <summary>
    /// Method <c>OnPointerDown</c> marks the node as selected and reveals the side menu.
    /// </summary>
    public new void OnPointerDown(PointerEventData event_data)
    {
        try
        {
            NodeConnectors.BeginConnecting(nodeId);
            base.OnPointerDown(event_data);
        } catch(NullReferenceException) {}
    }
}
