using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoopNodeSpecsPanelManager : NodeSpecsPanelManager
{
    [SerializeField] private TextMeshProUGUI loopsToMake;
    private LoopNode _node;
    
    /// <summary>
    /// Method <c>SetLoopsToMake</c> sets the selected nodes' number of loops made.
    /// </summary>
    public void SetLoopsToMake()
    {
        StartCoroutine(TimedSetLoopsToMake());
    }

    /// <summary>
    /// Method <c>TimedSetLoopsToMake</c> sets the selected nodes' number of loops made after a brief pause.
    /// </summary>
    private IEnumerator TimedSetLoopsToMake()
    {
        _node.LoopCount = Convert.ToInt32(loopsToMake.text);
        yield return new WaitForSecondsRealtime(0.1f);
    }

    /// <summary>
    /// Method <c>ChangeNode</c> changes the node to focus on.
    /// <param name="new_node">The new node to focus on.</param>
    /// </summary>
    public override void ChangeNode(InteractiveNode new_node)
    {
        _node = (LoopNode) new_node;
        loopsToMake.text = _node.LoopCount.ToString();
    }
}
