
using System;

public class ConditionNode : BTNode
{
    private Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Tick()
    {
        return condition() ? NodeState.Success : NodeState.Failure;
    }
}
