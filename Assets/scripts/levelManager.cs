using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CompoundState
{
    public string compoundName; // e.g., "H2O"
    public string state;        // e.g., "liquid"
}

public class levelManager : MonoBehaviour
{
    [SerializeField]
    private List<CompoundState> compoundStates = new List<CompoundState>();

    // Optional: Method to convert the list into a dictionary at runtime
    public Dictionary<string, string> GetCompoundStateDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (var compoundState in compoundStates)
        {
            dictionary[compoundState.compoundName] = compoundState.state;
        }
        return dictionary;
    }
}
