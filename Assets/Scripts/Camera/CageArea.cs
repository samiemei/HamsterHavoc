using UnityEngine;

public class CageArea : MonoBehaviour
{
    [Tooltip("Where the camera should focus for this cage.")]
    public Transform cameraFocus;
    
    public int cageId = 0;
    
    [Tooltip("The primary hamster in this cage (for HUD).")]
    public HamsterNeeds primaryHamster;

    void Reset()
    {
        if (!cameraFocus)
        {
            var t = transform.Find("CameraFocus");
            if (t) cameraFocus = t;
        }

        if (!primaryHamster)
            primaryHamster = GetComponentInChildren<HamsterNeeds>();
    }

    void Awake()
    {
        if (!primaryHamster)
            primaryHamster = GetComponentInChildren<HamsterNeeds>();
    }
    
    public static void CageAreaAssignIds(CageArea area)
    {
        if (!area) return;

        int id = area.cageId;
        
        foreach (var interactable in area.GetComponentsInChildren<Interactable>(true))
            interactable.cageId = id;
        
        foreach (var brain in area.GetComponentsInChildren<HamsterBrain>(true))
            brain.cageId = id;
    }
}