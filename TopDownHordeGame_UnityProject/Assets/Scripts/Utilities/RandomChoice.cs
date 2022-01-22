using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomChoice
{
    /// <summary>
    /// The chance for this item to be selected
    /// </summary>
    public float chance;
    /// <summary>
    /// The object to be chosen
    /// </summary>
    public GameObject obj;

    /// <summary> Chooses a random gameobj from a list (all objects must have the choosable script)</summary>
    public static GameObject ChooseRandom(List<RandomChoice> choices) {
        if (!(choices.Count > 0)) {
            Debug.LogError("ChooseRandom was passed an empty or broken list!");
            Debug.Break();
            return null;
        }

        float sum = 0;
        foreach (RandomChoice choice in choices) {
            sum += choice.chance;
        }

        float rand = UnityEngine.Random.Range(0, sum);
        float psum = 0;
        foreach (RandomChoice choice in choices) {
            if ((choice.chance + psum) >= rand && psum <= rand) {
                return choice.obj;
            }
            psum += choice.chance;
        }
        Debug.LogError("Choose Random Failed!");
        Debug.Break();
        return null;
    }
}
