/*using UnityEngine;
using UnityEngine.Splines;

public class OgTrackManager : MonoBehaviour
{
    public static OgTrackManager instance;
    
    public void Awake()
    {
        instance = this;
    }

    //carves track out of a given cell
    //generates spline and then modifies terrain accordingly
    //sets cell.TrackStartPos and cell.TrackEndPos for future use
    //sets cell.EndingTrackAngle for future use
    public void CarveTrack(CellGenerator cell)
    {
        Spline cellSpline = SplineGenerator(cell);
        TerrainModifier(cell, cellSpline);
    }

    //generates a random spline (track) for a given cell
    //stays within the cell.Bounds
    public Spline SplineGenerator(CellGenerator cell)
    {
        return new Spline();
    }

    //uses the previously created spline
    //changes cell heightmap, and edits the terrain around the track
    //does the actual modifying of the basic cell, using the spline as reference
    public void TerrainModifier(CellGenerator cell, Spline generatedSpline)
    {

    }
}
*/