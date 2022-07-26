using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AutoClicker.Classes;
public class Hotkeys : IDisposable
{
    public Action Play { get; set; }
    public Action Pause { get; set; }
    private List<int> PressedKeys;
    private bool disposedValue;
    public Hotkeys()
    {
        KeyboardFns.CreateHook();
        PressedKeys = new();
        KeyboardFns.KeyPressed += KeyboardHook_KeyPressed;
        KeyboardFns.KeyReleased += KeyboardFns_KeyReleased;
    }
    private void KeyboardHook_KeyPressed(object sender, KeyPressedEventArgs e)
    {
        if (!PressedKeys.Contains(e.KeyCode))
        {
            PressedKeys.Add(e.KeyCode);
            Trigger();
        }
    }
    private void KeyboardFns_KeyReleased(object sender, KeyPressedEventArgs e)
    {
        if (PressedKeys.Contains(e.KeyCode))
            PressedKeys.Remove(e.KeyCode);
    }
    private void Trigger()
    {
        if (Settings.Main.PauseHotkeys.Length > 0 && !Settings.Main.PauseHotkeys.ToList().Except(PressedKeys).Any())
        {
            Pause?.Invoke();
        }
        if (Settings.Main.PlayHotkeys.Length > 0 && !Settings.Main.PlayHotkeys.ToList().Except(PressedKeys).Any())
        {
            Play?.Invoke();
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                KeyboardFns.DisposeHook();
            }
            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
public delegate IntPtr KeyboardProcess(int nCode, IntPtr wParam, IntPtr lParam);
public sealed class KeyboardFns
{
    public static event EventHandler<KeyPressedEventArgs> KeyPressed;
    public static event EventHandler<KeyPressedEventArgs> KeyReleased;
    private const int WH_KEYBOARD = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private static KeyboardProcess keyboardProc = HookCallback;
    private static IntPtr hookID = IntPtr.Zero;
    public static void CreateHook()
    {
        hookID = SetHook(keyboardProc);
    }
    public static void DisposeHook()
    {
        UnhookWindowsHookEx(hookID);
    }
    private static IntPtr SetHook(KeyboardProcess keyboardProc)
    {
        using Process currentProcess = Process.GetCurrentProcess();
        using ProcessModule currentProcessModule = currentProcess.MainModule;
        return SetWindowsHookEx(WH_KEYBOARD, keyboardProc, GetModuleHandle(currentProcessModule.ModuleName), 0);
    }
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            KeyPressed?.Invoke(null, new KeyPressedEventArgs(vkCode));
        }
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            KeyReleased?.Invoke(null, new KeyPressedEventArgs(vkCode));
        }
        return CallNextHookEx(hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProcess lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);
}
public class KeyPressedEventArgs : EventArgs
{
    public int KeyCode { get; set; }
    public KeyPressedEventArgs(int Key)
    {
        KeyCode = Key;
    }
}
