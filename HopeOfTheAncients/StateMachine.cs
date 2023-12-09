using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;

public class StateMachine
{
    private readonly Dictionary<INode, Transitions> transitions;
    private readonly HashSet<INode> nodes;
    private INode currentNode;
    public INode CurrentNode 
    {
        get => currentNode;
        private set
        {
            currentNode = value;
            currentNodeTime = currentTime;
        }
    }

    private float currentNodeTime, currentTime;

    public StateMachine(INode startNode)
    {
        transitions = new Dictionary<INode, Transitions>();
        nodes = new HashSet<INode>();
        AddNode(startNode);
        currentNode = null!;
        CurrentNode = startNode;
    }

    public void AddNode(INode node)
    {
        nodes.Add(node);
    }

    public void AddTransition(INode sourceNode, INode targetNode, Func<bool> guard)
    {
        if (!transitions.TryGetValue(sourceNode, out var transitionMap))
        {
            transitionMap = new Transitions();
            transitions.Add(sourceNode, transitionMap);
        }
        transitionMap.AddTransitionTo(targetNode, guard);
    }

    public interface INode
    {
        bool IsCompleted { get; }
        void Update(float elapsedTime, float totalTime);
    }

    public void Update(float elapsedTime)
    {
        currentTime += elapsedTime;

        if (CurrentNode.IsCompleted)
        {
            if (transitions.TryGetValue(CurrentNode, out var transitionMap))
            {
                var transition = transitionMap.GetValidTransition();
                if (transition == null)
                    return;
                CurrentNode = transition.TargetNode;
            }
        }

        CurrentNode.Update(elapsedTime, currentTime - currentNodeTime);
    }

    public class GenericNode : INode, IEquatable<GenericNode?>
    {
        public GenericNode(Func<float, float, bool> updateFunction) => UpdateFunction = updateFunction;

        public bool IsCompleted { get; private set; }
        public Func<float, float, bool> UpdateFunction { get; }


        public override bool Equals(object? obj) => Equals(obj as GenericNode);
        public bool Equals(GenericNode? other) => other != null && EqualityComparer<Func<float, float, bool>>.Default.Equals(UpdateFunction, other.UpdateFunction);
        public override int GetHashCode() => HashCode.Combine(UpdateFunction);

        public void Update(float elapsedTime, float totalTime)
        {
            IsCompleted = UpdateFunction(elapsedTime, totalTime);
        }

        public static bool operator ==(GenericNode? left, GenericNode? right) => EqualityComparer<GenericNode>.Default.Equals(left, right);
        public static bool operator !=(GenericNode? left, GenericNode? right) => !(left == right);
    }
    public class Transitions
    {
        public INode? SourceNode { get; set; }
        private HashSet<Transition> transitions;
        public Transitions()
        {
            transitions = new HashSet<Transition>();
        }

        public void AddTransitionTo(INode targetNode, Func<bool> guard)
        {
            transitions.Add(new Transition(SourceNode, targetNode, guard));
        }

        public Transition? GetValidTransition()
        {
            Transition? transition = null;
            foreach(var t in transitions)
            {
                if (t.Guard())
                {
                    if (transition != null)
                        throw new InvalidOperationException("Two possible state transitions!");
                    transition = t;
                }
            }
            return transition;
        }
    }

    public class Transition : IEquatable<Transition?>
    {
        public Transition(INode? sourceNode, INode targetNode, Func<bool> guard)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            Guard = guard;
        }
        public Func<bool> Guard { get; set; }
        public INode? SourceNode { get; set; }
        public INode TargetNode { get; set; }

        public override bool Equals(object? obj) => Equals(obj as Transition);
        public bool Equals(Transition? other) => other != null && EqualityComparer<Func<bool>>.Default.Equals(Guard, other.Guard) && EqualityComparer<INode>.Default.Equals(SourceNode, other.SourceNode) && EqualityComparer<INode>.Default.Equals(TargetNode, other.TargetNode);
        public override int GetHashCode() => HashCode.Combine(Guard, SourceNode, TargetNode);

        public static bool operator ==(Transition? left, Transition? right) => EqualityComparer<Transition>.Default.Equals(left, right);
        public static bool operator !=(Transition? left, Transition? right) => !(left == right);
    }
}

//Dialing -> Ring moves
//Lock -> Main Chevron locks
//Chevron -> specific chevron lights up
//Wormhole -> activation
//Wormhole -> idle
//Wormhole -> deactivation
//Idle
//Incomming (Condition)


