/*RandomChoice.cs
 * Author: Remington Ward
 * Date: 1/24/2022
 * This script is written for the Unity Game Engine. It provides a simple way for a List of gameobjects to be 
 * chosen based on some weight value. Just create a list<RandomChoice> that is serialized by unity and then add 
 * your chances and objects in the inspector! Then in your code call RandomChoice.ChooseRandom(List<RandomChoice> list)
 * and it will return one of the objects with apropriate randomness.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomChoice
{
    /// <summary>
    /// The chance for this item to be selected
    /// </summary>
    public float weight;
    /// <summary>
    /// The object to be chosen
    /// </summary>
    public GameObject obj;

    /// <summary> Chooses a random gameobj from a list (all objects must have the choosable script)</summary>
    public static GameObject ChooseRandom(List<RandomChoice> choices) {
        //An Empty list should not be passed to this funcit
        if (!(choices.Count > 0)) {
            Debug.LogError("ChooseRandom was passed an empty or broken list!");
            Debug.Break();
            return null;
        }

        //We need the sum of weights in order to choose correctly
        float sum = 0;
        foreach (RandomChoice choice in choices) {
            sum += choice.weight;
        }

        //We loop through assigning a portion of the total weights to each object and checking if the
        //random number is within the portion if it is we retun that Choice's object
        float rand = UnityEngine.Random.Range(0, sum);
        float psum = 0;
        foreach (RandomChoice choice in choices) {
            if ((choice.weight + psum) >= rand && psum <= rand) {
                return choice.obj;
            }
            psum += choice.weight;
        }
        //Error if no choices were picked
        Debug.LogError("Choose Random Failed!");
        Debug.Break();
        return null;
    }
}
