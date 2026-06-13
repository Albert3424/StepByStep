using UnityEngine;

public class TargetPlate : MonoBehaviour
{
    private Crate currentCrate = null;

    public void OnCrateEnter(Crate crate)
    {
        currentCrate = crate;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void OnCrateExit(Crate crate)
    {
        if(currentCrate == crate)
        {
            currentCrate = null;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public bool IsOccupied()
    {
        return currentCrate != null;
    }
}
