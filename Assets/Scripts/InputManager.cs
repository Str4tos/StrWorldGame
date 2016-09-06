using UnityEngine;
using System.Collections;

public class InputManager : ScriptableObject
{
    // Mouse0 - LeftClick
    // Mouse1 - RightClick


    public static KeyCode MoveCode = KeyCode.Mouse0;
    public static KeyCode MainSkillCode = KeyCode.Mouse1;
    public static KeyCode SplitItem = KeyCode.L;
    public static KeyCode InventoryCode = KeyCode.V;
    public static KeyCode StorageCode = KeyCode.E;
    public static KeyCode CharacterSystemCode = KeyCode.C;
    public static KeyCode CraftSystemCode = KeyCode.K;
    public static KeyCode[] HotBarItemsCode = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
    public static KeyCode[] HotBarSkillsCode = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    
}
