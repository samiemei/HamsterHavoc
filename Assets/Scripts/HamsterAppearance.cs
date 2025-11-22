using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HamsterAppearance : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public string currentSkinId = "default";
    
    void Reset()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (HamsterSkinManager.Instance != null)
        {
            HamsterSkinManager.Instance.TryEquipSkin(this, currentSkinId, ignoreLock: false);
        }
    }
    
    public void ApplySkin(HamsterSkinManager.SkinDefinition skin)
    {
        if (!spriteRenderer || skin == null) return;
        
        if (skin.idleFront != null)
            spriteRenderer.sprite = skin.idleFront;
    }
    
}
