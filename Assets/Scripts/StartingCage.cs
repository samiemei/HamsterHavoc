using UnityEngine;

public class StartingCage : MonoBehaviour
{
    void Start()
    {
        var cage = GetComponent<CageArea>();
        if (cage && CameraCageController.Instance)
        {
            CameraCageController.Instance.RegisterCage(cage);
        }
    }
}
