using UnityEngine;
using UnityEngine.InputSystem;

//所有輸入都從這裡讀（Input System），其他腳本不直接碰輸入API，之後要改按鍵或支援其他裝置只要改這裡
public static class GameInput
{
    //指標在螢幕上的位置（像素），滑鼠、觸控、觸控筆都算指標
    public static Vector2 PointerPosition => Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;

    public static bool PointerDown => Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
    public static bool PointerHeld => Pointer.current != null && Pointer.current.press.isPressed;

    //這一幀指標移動了幾個像素
    public static Vector2 PointerDelta => Pointer.current != null ? Pointer.current.delta.ReadValue() : Vector2.zero;

    //旋轉鏡頭：A是1，D是-1
    public static float CameraTurn
    {
        get
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return 0f;

            return (keyboard.aKey.isPressed ? 1f : 0f) - (keyboard.dKey.isPressed ? 1f : 0f);
        }
    }

    //測試用快捷鍵
    public static bool DebugRespawnPressed => Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
    public static bool DebugGameOverPressed => Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame;
    public static bool DebugGameStartPressed => Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame;
}
