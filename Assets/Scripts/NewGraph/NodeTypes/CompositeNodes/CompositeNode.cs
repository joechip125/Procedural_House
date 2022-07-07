using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BaseNode
{
    public List<BaseNode> children = new ();
}
