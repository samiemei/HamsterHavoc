using System.Collections.Generic;
using UnityEngine;

public class CameraCageController : MonoBehaviour
{
    public static CameraCageController Instance { get; private set; }

    [Header("Follow Settings")]
    public float moveSpeed = 4f; 
    public float snapDistance = 0.02f;

    private readonly List<CageArea> cages = new();
    private int currentIndex = -1;
    private float defaultZ;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        defaultZ = transform.position.z;
    }

    void Update()
    {
        if (currentIndex < 0 || currentIndex >= cages.Count)
            return;

        var target = cages[currentIndex].cameraFocus;
        if (!target) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, defaultZ);

        // smoothing
        Vector3 nextPos = Vector3.Lerp(currentPos, targetPos, moveSpeed * Time.deltaTime);
        
        if ((targetPos - nextPos).sqrMagnitude < snapDistance * snapDistance)
            nextPos = targetPos;

        transform.position = nextPos;
    }
    
    public void RegisterCage(CageArea cage)
    {
        if (!cage) return;
        cages.Add(cage);
        
        if (currentIndex == -1)
        {
            currentIndex = 0;
            SnapToCurrent();
            SetHUDTargetForCurrent();
        }
    }


    public void GoToCage(int index)
    {
        if (index < 0 || index >= cages.Count) return;
        currentIndex = index;
    }

    public void NextCage()
    {
        if (cages.Count == 0) return;
        currentIndex = (currentIndex + 1) % cages.Count;
        SetHUDTargetForCurrent();
    }

    public void PreviousCage()
    {
        if (cages.Count == 0) return;
        currentIndex--;
        if (currentIndex < 0) currentIndex = cages.Count - 1;
        SetHUDTargetForCurrent();
    }


    void SnapToCurrent()
    {
        if (currentIndex < 0 || currentIndex >= cages.Count) return;
        var target = cages[currentIndex].cameraFocus;
        if (!target) return;

        transform.position = new Vector3(target.position.x, target.position.y, defaultZ);
    }
    
    void SetHUDTargetForCurrent()
    {
        if (NeedsHUDController.Instance == null) return;
        if (currentIndex < 0 || currentIndex >= cages.Count) return;

        var cage = cages[currentIndex];
        if (cage && cage.primaryHamster)
            NeedsHUDController.Instance.SetTarget(cage.primaryHamster);
    }

}
