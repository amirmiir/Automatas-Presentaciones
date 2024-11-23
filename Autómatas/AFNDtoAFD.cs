using System.Collections.Generic;
using System.Linq;
public class State
{
    public string Name { get; set; }
    public bool IsFinal { get; set; }

    public State(string name, bool isFinal = false)
    {
        Name = name;
        IsFinal = isFinal;
    }

    public override string ToString()
    {
        return Name;
    }
}

public class Transition
{
    public State FromState { get; set; }
    public char? Symbol { get; set; } 
    public List<State> ToStates { get; set; } 

    public Transition(State fromState, char? symbol, List<State> toStates)
    {
        FromState = fromState;
        Symbol = symbol;
        ToStates = toStates;
    }
}

public class Automaton
{
    public List<State> States { get; set; }
    public List<char> Alphabet { get; set; } 
    public List<Transition> Transitions { get; set; }
    public State InitialState { get; set; }

    public Automaton()
    {
        States = new List<State>();
        Alphabet = new List<char>();
        Transitions = new List<Transition>();
    }
    
    public void AddState(State state)
    {
        States.Add(state);
    }
    
    public void AddTransition(Transition transition)
    {
        Transitions.Add(transition);
    }

    public List<Transition> GetTransitionsFromState(State state, char? symbol = null)
    {
        return Transitions.Where(t => t.FromState == state && (symbol == null || t.Symbol == symbol)).ToList();
    }
}

public class NFAtoDFAConverter
{
    public Automaton ConvertToDFA(Automaton nfa)
    {
        Automaton dfa = new Automaton();
        dfa.Alphabet = nfa.Alphabet;

        //Crea el estado inicial del AFD y le asigna el estado inicial del AFND
        var initialDFAState = new HashSet<State> { nfa.InitialState };
        Queue<HashSet<State>> stateQueue = new Queue<HashSet<State>>();
        Dictionary<HashSet<State>, State> dfaStateMapping = new Dictionary<HashSet<State>, State>(new SetComparer<State>());

        State dfaInitialState = new State(string.Join(",", initialDFAState.Select(s => s.Name)), initialDFAState.Any(s => s.IsFinal));
        dfa.AddState(dfaInitialState);
        dfa.InitialState = dfaInitialState;
        dfaStateMapping[initialDFAState] = dfaInitialState;
        stateQueue.Enqueue(initialDFAState);

        // Halla los nuevos estados y las transiciones
        while (stateQueue.Count > 0)
        {
            var currentNFAStates = stateQueue.Dequeue();

            foreach (char symbol in nfa.Alphabet)
            {
                var reachableStates = new HashSet<State>();

                foreach (var state in currentNFAStates)
                {
                    var transitions = nfa.GetTransitionsFromState(state, symbol);
                    foreach (var transition in transitions)
                    {
                        reachableStates.UnionWith(transition.ToStates);
                    }
                }

                if (reachableStates.Count > 0)
                {
                    if (!dfaStateMapping.ContainsKey(reachableStates))
                    {
                        var newDFAState = new State(string.Join(",", reachableStates.Select(s => s.Name)), reachableStates.Any(s => s.IsFinal));
                        dfa.AddState(newDFAState);
                        dfaStateMapping[reachableStates] = newDFAState;
                        stateQueue.Enqueue(reachableStates);
                    }

                    dfa.AddTransition(new Transition(dfaStateMapping[currentNFAStates], symbol, new List<State> { dfaStateMapping[reachableStates] }));
                }
            }
        }

        return dfa;
    }
}

//Clase de ayuda para hacer comparación de Sets
public class SetComparer<T> : IEqualityComparer<HashSet<T>>
{
    public bool Equals(HashSet<T> x, HashSet<T> y)
    {
        return x.SetEquals(y);
    }

    public int GetHashCode(HashSet<T> obj)
    {
        int hash = 0;
        foreach (var element in obj)
        {
            hash ^= element.GetHashCode();
        }
        return hash;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Crear AFND
        var nfa = new Automaton();

        var s0 = new State("s0");
        var s1 = new State("s1");
        var s2 = new State("s2", isFinal: true);

        nfa.AddState(s0);
        nfa.AddState(s1);
        nfa.AddState(s2);
        
        nfa.InitialState = s0;
        nfa.Alphabet.Add('0');
        nfa.Alphabet.Add('1');

        nfa.AddTransition(new Transition(s0, '0', new List<State> { s0, s1 }));
        nfa.AddTransition(new Transition(s1, '1', new List<State> { s2 }));
        nfa.AddTransition(new Transition(s2, '0', new List<State> { s0 }));

        // Convertir AFND a AFD
        var converter = new NFAtoDFAConverter();
        var dfa = converter.ConvertToDFA(nfa);

        // Imprimir el AFD resultante
        Console.WriteLine("Estados del AFD:");
        foreach (var state in dfa.States)
        {
            Console.WriteLine($"Estado: {state.Name}, Final: {state.IsFinal}");
        }

        Console.WriteLine("Transiciones del AFD:");
        foreach (var transition in dfa.Transitions)
        {
            Console.WriteLine($"De {transition.FromState} con '{transition.Symbol}' a {string.Join(", ", transition.ToStates)}");
        }
        Console.WriteLine("Initial State: " + dfa.InitialState);
    }
}
