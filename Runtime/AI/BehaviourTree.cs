using System.Collections.Generic;

namespace OysterUtils
{
  public enum NodeResult
  {
    Running,
    Success,
    Failure,
  }

  public class BehaviourTree
  {
    protected Node rootNode;

    public BehaviourTree(Node _rootNode)
    {
      rootNode = _rootNode;
    }

    public void Tick()
    {
      rootNode.Evaluate();
    }
  }

  public abstract class Node
  {
    public abstract NodeResult Evaluate();
    public abstract List<Node> GetChildren();
  }

  public abstract class DecoratorNode : Node
  {
    protected Node child;

    protected DecoratorNode(Node _child)
    {
      child = _child;
    }

    public override List<Node> GetChildren()
    {
      return new List<Node>() { child };
    }
  }

  public abstract class LeafNode : Node
  {
    public override List<Node> GetChildren()
    {
      return new List<Node>();
    }
  }

  public abstract class CompositeNode : Node
  {
    protected List<Node> children = new List<Node>();

    protected CompositeNode(List<Node> _children)
    {
      children = _children;
    }

    public override List<Node> GetChildren()
    {
      return children;
    }
  }

  // Like a logical AND; iterate through children and return the first failure or running.
  // If all children return success, return success.
  public class SequenceNode : CompositeNode
  {
    public SequenceNode(List<Node> _children) : base(_children) { }

    public override NodeResult Evaluate()
    {
      foreach (Node child in children)
      {
        NodeResult result = child.Evaluate();

        switch (result)
        {
          case NodeResult.Running:
          default:
            return NodeResult.Running;

          case NodeResult.Failure:
            return NodeResult.Failure;

          case NodeResult.Success:
            continue;
        }
      }

      return NodeResult.Success;
    }
  }

  // Like a logical OR; iterate through children and return first success or running.
  // If all children return failure, return failure.
  public class SelectorNode : CompositeNode
  {
    public SelectorNode(List<Node> _children) : base(_children) { }

    public override NodeResult Evaluate()
    {
      foreach (Node child in children)
      {
        NodeResult result = child.Evaluate();

        switch (result)
        {
          case NodeResult.Running:
          default:
            return NodeResult.Running;

          case NodeResult.Success:
            return NodeResult.Success;

          case NodeResult.Failure:
            continue;
        }
      }

      return NodeResult.Failure;
    }
  }
}
