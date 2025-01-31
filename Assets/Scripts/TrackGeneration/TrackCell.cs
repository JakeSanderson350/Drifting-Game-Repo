using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCell : MonoBehaviour
{
    //how much or little the curve is on this cell
    private float curveMagnitude;

    //dimensions of each cell
    private float cellLength;
    private float cellWidth;
    private float cellHeight;
    private float cellRotation;

    //number of objects on and off the track
    private float numOfObjs;

    //create a basic cell (flat plane)
    TrackCell()
    {
        curveMagnitude = 0;
        cellLength = 0;
        cellWidth = 0;
        cellHeight = 0;
        cellRotation = 0;
        numOfObjs = 0;
    }

    TrackCell(float curve, float length, float width)
    {
        curveMagnitude = length;
        cellLength = length;
        cellWidth = width;
    }

    //uses values to generate the desired cell 
    //returns a game object of the created cell
    /*GameObject*/ void GenerateCell()
    {

    }
    

}
