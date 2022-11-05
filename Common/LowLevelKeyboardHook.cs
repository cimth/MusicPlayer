using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Common;

/// <summary>
///
///
/// See for more details:
/// https://docs.microsoft.com/en-us/windows/win32/winmsg/hooks
/// </summary>
public class LowLevelKeyboardHook
{
    // ==============
    // CONSTANTS
    // ==============
    
    /// <summary>
    /// The hook procedure for low level keyboard events.
    /// </summary>
    private const int WH_KEYBOARD_LL = 13;
    
    /// <summary>
    /// The code for the low level 'key down' event.
    /// </summary>
    private const int WM_KEYDOWN = 0x0100;

    // ==============
    // FIELDS
    // ==============

    /// <summary>
    /// The keys for which the <see cref="OnKeyPressed"/> event should be invoked.
    /// </summary>
    private readonly Key[]? _registeredKeys;
    
    /// <summary>
    /// The function called every time a new low level key input event happens.
    /// </summary>
    private readonly LowLevelKeyboardProc _proc;

    /// <summary>
    /// The ID of the registered hook.
    /// </summary>
    private IntPtr _hookId = IntPtr.Zero;
    
    // ==============
    // DELEGATE FOR THE HOOK PROCEDURE
    // ==============
    
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    // ==============
    // EVENT FOR THE ACTUAL APPLICATION CODE
    // ==============
    
    /// <summary>
    /// Gets fired when one of the registered keys is pressed.
    /// This is the event that should be handled by the actual application code.
    /// </summary>
    public event EventHandler<Key>? OnKeyPressed;
    
    // ==============
    // ACCESS TO EXTERNAL DLLs
    // ==============

    /// <summary>
    /// Installs the given hook procedure (LowLevelKeyboardProc) into a hook chain.
    /// This allows the application to monitor the system for certain types of events, here for low level keyboard
    /// events.
    /// <para/>
    /// See: "https://docs.microsoft.com/de-de/windows/win32/api/winuser/nf-winuser-setwindowshookexa"
    /// </summary>
    /// <param name="idHook">
    /// The id of the hook procedure to be installed.
    /// Will be set to <c>WH_KEYBOARD_LL</c> in this class to hook low level keyboard input events.
    /// </param>
    /// <param name="lpfn">
    /// A pointer to the hook procedure that is called when an low level keyboard input event happens.
    /// </param>
    /// <param name="hMod">
    /// A handle to the DLL containing the hook procedure pointed to by the <see cref="lpfn"/> parameter.
    /// </param>
    /// <param name="dwThreadId">
    /// The identifier of the thread with which the hook procedure is to be associated.
    /// Will be set to <c>0</c> in this class so that the hook procedure is associated with all existing threads
    /// running in this app.
    /// </param>
    /// <returns>The handle to the hook procedure</returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>
    /// Removes the given hook from the application.
    /// Has to be called when the application closes.
    /// <para/>
    /// See: "https://docs.microsoft.com/de-de/windows/win32/api/winuser/nf-winuser-unhookwindowshookex"
    /// </summary>
    /// <param name="hhk">A handle to the hook to be removed.</param>
    /// <returns>If the function succeeds, the return value is true.</returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    /// <summary>
    /// Passes the hook information to the next hook procedure in the current hook chain.
    /// With this method, the low level keyboard events will be forwarded to the next hooked application to process
    /// the input if desired.
    /// <para/>
    /// See: https://docs.microsoft.com/de-de/windows/win32/api/winuser/nf-winuser-callnexthookex"
    /// </summary>
    /// <param name="hhk">This parameter is ignored.</param>
    /// <param name="nCode">A code to determine how to process the message.</param>
    /// <param name="wParam">
    /// The identifier of the keyboard message. One of <c>WM_KEYDOWN</c>, <c>WM_KEYUP</c>,
    /// <c>WM_SYSKEYDOWN</c>, or <c>WM_SYSKEYUP</c>.
    /// </param>
    /// <param name="lParam">
    /// A pointer to the struct which contains information about the low-level keyboard
    /// input event, e.g. to the VKey code.
    /// </param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // ==============
    // INITIALIZATION
    // ==============

    public LowLevelKeyboardHook(Key[]? registeredKeys = null)
    {
        this._registeredKeys = registeredKeys;

        // Initialize the callback method that should be invoked after a low level key event.
        // Is stored as private field because if it was not stored, the Garbage Collection would collect it.
        _proc = HookCallback;
    }
    
    // ==============
    // HOOK AND UNHOOK
    // ==============

    public void HookKeyboard()
    {
        _hookId = SetHook(_proc);
    }

    public void UnHookKeyboard()
    {
        UnhookWindowsHookEx(_hookId);
    }

    /// <summary>
    /// Hooks on the keyboard events.
    /// </summary>
    /// <param name="proc">The given hook procedure that is called when an key input event happens</param>
    /// <returns></returns>
    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        // Get the current process' main module
        using Process process = Process.GetCurrentProcess();
        using ProcessModule? module = process.MainModule;
        
        // Install the given hook procedure into the hook chain for low level keyboard events
        if (module != null && module.ModuleName != null)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }

        // Throw exception if the hooking failed
        throw new Exception("The hook could not be set up correctly.");
    }

    /// <summary>
    /// The callback method when a key input event happens.
    /// Checks if there is one of the registered key pressed and invokes the <see cref="OnKeyPressed"/> event
    /// which forwards the key input event to the actual application code.
    /// <para/>
    /// For signature details, check out
    /// https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)
    /// </summary>
    /// <param name="nCode">A code to determine how to process the message.</param>
    /// <param name="wParam">
    /// The identifier of the keyboard message. One of <c>WM_KEYDOWN</c>, <c>WM_KEYUP</c>,
    /// <c>WM_SYSKEYDOWN</c>, or <c>WM_SYSKEYUP</c>.
    /// </param>
    /// <param name="lParam">
    /// A pointer to the struct which contains information about the low-level keyboard
    /// input event, e.g. to the VKey code.
    /// </param>
    /// <returns></returns>
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // Forward the key event to the application if it is valid.
        if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
        {
            // Get the pressed key and convert it to a System.Windows.Input.Key object for easier usage.
            int vkCode = Marshal.ReadInt32(lParam);
            Key key = KeyInterop.KeyFromVirtualKey(vkCode);
            
            // Only forward the desired key events to the actual application code.
            if (this._registeredKeys != null && this._registeredKeys.Contains(key)) {
                OnKeyPressed?.Invoke(this, key);
            }
        }

        // Call the next hook for the keyboard event so that other applications can process the key events.
        return CallNextHookEx(_hookId, nCode, wParam, lParam);            
    }
}