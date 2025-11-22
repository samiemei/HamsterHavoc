using UnityEngine;

public class HamsterSkinManager : MonoBehaviour
{
    public static HamsterSkinManager Instance { get; private set; }

    [System.Serializable]
    public class SkinDefinition
    {
        public string id;
        public string displayName;

        [Header("Sprites")]
        public Sprite idleFront;
        public Sprite idleSide;
        public Sprite idleBack;
        public Sprite walkFront;
        public Sprite walkSide;
        public Sprite walkBack;
    }

    [Header("Available Skins")]
    public SkinDefinition[] skins;

    [Header("Defaults")]
    public string defaultSkinId = "default";

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    

    public SkinDefinition GetSkin(string id)
    {
        if (string.IsNullOrEmpty(id))
            id = defaultSkinId;

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].id == id)
                return skins[i];
        }
        
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].id == defaultSkinId)
                return skins[i];
        }

        return skins.Length > 0 ? skins[0] : null;
    }
    
    public bool IsSkinUnlocked(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        
        if (id == defaultSkinId)
            return true;

        string key = $"skin_{id}_unlocked";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    public bool TryEquipSkin(HamsterAppearance hamster, string skinId, bool ignoreLock = false)
    {
        if (!hamster)
        {
            Debug.LogWarning("[HamsterSkinManager] Tried to equip skin on null hamster.");
            return false;
        }

        var skin = GetSkin(skinId);
        if (skin == null)
        {
            Debug.LogWarning($"[HamsterSkinManager] No skin with id '{skinId}' found.");
            return false;
        }
        
        if (!ignoreLock && !IsSkinUnlocked(skinId))
        {
            Debug.Log($"[HamsterSkinManager] Skin '{skinId}' is not unlocked.");
            return false;
        }

        hamster.ApplySkin(skin);
        hamster.currentSkinId = skinId;
        
        return true;
    }
}
