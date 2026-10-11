
using System.Collections.Generic;

public class SequenceNode : BTNode
{
    private List<BTNode> children;

    public SequenceNode(List<BTNode> children)
    {
        this.children = children;
    }

    public override NodeState Tick()
    {
        foreach (BTNode child in children)
        {
            NodeState state = child.Tick();

            if (state == NodeState.Failure)
                return NodeState.Failure;

            if (state == NodeState.Running)
                return NodeState.Running;
        }

        return NodeState.Success;
    }
}
