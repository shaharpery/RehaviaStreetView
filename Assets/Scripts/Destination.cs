using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination {
    public enum DestType {
        Place,
        Human
    }


    DestType type;
    int[] cylinderIndexes;
    bool visited;
    string destName; // For example: in case of Place: "Bank", in case of Human: "Gavriel"


    public Destination(DestType _type, int[] indexes, string _destName) {
        type = _type;
        cylinderIndexes = indexes;
        visited = false;
        destName = _destName;
    }

    public Destination() {
        type = DestType.Place;
        cylinderIndexes = new int[0];
        visited = false;
    }

    /// <summary>
    /// Checking if we are at one of the destination indexes. If so, set visited bool to true;
    /// </summary>
    /// <param name="currentCylinderIndex"> the index of the cylinder the player at</param>
    public void CheckIfVisitingThisDest(int currentCylinderIndex) {
        for (int i = 0; i < cylinderIndexes.Length; i++) {
            if (cylinderIndexes[i] == currentCylinderIndex) {
                visited = true;
                Debug.Log("Reached Destination!!!");
                return;
            }
        }
    }

    public DestType GetDestType() {
        return type;
    }

    public int[] GetDestCylinderIndexes() {
        return cylinderIndexes;
    }

    public bool DidVisitDest() {
        return visited;
    }

    public string GetDestinationName() {
        return destName;
    }
}
