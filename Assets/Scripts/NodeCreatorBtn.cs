using System;
using UnityEngine;

public class NodeCreatorBtn : MonoBehaviour
{
    [SerializeField] private GameObject node;
    
    public void CreateBtn()
    {
        var new_node = Instantiate(node, GameObject.Find("MainCanvas").transform, false);
        new_node.transform.position = new Vector3(0, 0, -1);
    }
}