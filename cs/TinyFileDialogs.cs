using static TinyFileDialogsSharp.Native;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace TinyFileDialogsSharp;

public static class TinyFileDialogs
{
    public static string Version => tinyfd_getGlobalChar("tinyfd_version") ?? string.Empty;

    /// <summary>
    /// Defines whether tinyfiledialogs should print the command line calls.
    /// </summary>
    public static bool Verbose
    {
        get => tinyfd_getGlobalInt("tinyfd_verbose") == 1;
        set => tinyfd_setGlobalInt("tinyfd_verbose", value ? 1 : 0);
    }

    /// <summary>
    /// Defines whether tinyfiledialogs should hide any warnings or errors.
    /// </summary>
    public static bool Silent
    {
        get => tinyfd_getGlobalInt("tinyfd_silent") == 1;
        set => tinyfd_setGlobalInt("tinyfd_silent", value ? 1 : 0);
    }

    /// <summary>
    /// Asks the OS to make a "beep".
    /// </summary>
    public static void Beep() => tinyfd_beep();

    public static void NotifyPopup(
        string? title = null,
        string? message = null,
        NotifyIconType iconType = NotifyIconType.Info
    )
    {
        string iconTypeStr = iconType switch
        {
            NotifyIconType.Warning => "warning",
            NotifyIconType.Error => "error",
            _ => "info"
        };

        if (OperatingSystem.IsWindows())
            tinyfd_notifyPopupW(title, message, iconTypeStr);
        else
            tinyfd_notifyPopup(title, message, iconTypeStr);
    }

    public static MessageBoxButton MessageBox(
        string? title = null,
        string? message = null,
        DialogType dialogType = DialogType.Ok,
        MessageIconType iconType = MessageIconType.Info,
        MessageBoxButton defaultButton = MessageBoxButton.NoCancel
    )
    {
        string dialogTypeStr = dialogType switch
        {
            DialogType.OkCancel => "okcancel",
            DialogType.YesNo => "yesno",
            DialogType.YesNoCancel => "yesnocancel",
            _ => "ok"
        };

        string iconTypeStr = iconType switch
        {
            MessageIconType.Warning => "warning",
            MessageIconType.Error => "error",
            MessageIconType.Question => "question",
            _ => "info"
        };

        int result;

        if (OperatingSystem.IsWindows())
            result = tinyfd_messageBoxW(
                title,
                message,
                dialogTypeStr,
                iconTypeStr,
                (int)defaultButton
            );
        else
            result = tinyfd_messageBox(
                title,
                message,
                dialogTypeStr,
                iconTypeStr,
                (int)defaultButton
            );

        return (MessageBoxButton)result;
    }

    public static bool InputBox(
        [NotNullWhen(true)] out string? output,
        string? title = null,
        string? message = null,
        string? defaultInput = null,
        bool isPasswordInput = false
    )
    {
        output = null;

        nint resultPtr;

        if (isPasswordInput)
            defaultInput = null;
        else
            defaultInput ??= string.Empty;

        if (OperatingSystem.IsWindows())
        {
            resultPtr = tinyfd_inputBoxW(title, message, defaultInput);

            if (resultPtr != 0)
                output = Marshal.PtrToStringUni(resultPtr);
        }
        else
        {
            resultPtr = tinyfd_inputBox(title, message, defaultInput);

            if (resultPtr != 0)
                output = Marshal.PtrToStringUTF8(resultPtr);
        }

        return output is not null;
    }

    public static bool SaveFileDialog(
        [NotNullWhen(true)] out string? output,
        string? title = null,
        string? defaultPath = null,
        string[]? filterPatterns = null,
        string? filterDescription = null
    )
    {
        output = null;

        nint resultPtr;

        if (OperatingSystem.IsWindows())
        {
            resultPtr = tinyfd_saveFileDialogW(
                title,
                defaultPath,
                filterPatterns?.Length ?? 0,
                filterPatterns,
                filterDescription
            );

            if (resultPtr != 0)
                output = Marshal.PtrToStringUni(resultPtr);
        }
        else
        {
            resultPtr = tinyfd_saveFileDialog(
                title,
                defaultPath,
                filterPatterns?.Length ?? 0,
                filterPatterns,
                filterDescription
            );

            if (resultPtr != 0)
                output = Marshal.PtrToStringUTF8(resultPtr);
        }

        return output is not null;
    }

    public static bool OpenFileDialog(
        [NotNullWhen(true)] out string[]? output,
        string? title = null,
        string? defaultPath = null,
        string[]? filterPatterns = null,
        string? filterDescription = null,
        bool allowMultipleSelects = false
    )
    {
        output = null;

        nint resultPtr;
        string? result = null;

        if (OperatingSystem.IsWindows())
        {
            resultPtr = tinyfd_openFileDialogW(
                title,
                defaultPath,
                filterPatterns?.Length ?? 0,
                filterPatterns,
                filterDescription,
                allowMultipleSelects ? 1 : 0
            );

            if (resultPtr != 0)
                Marshal.PtrToStringUni(resultPtr);
        }
        else
        {
            resultPtr = tinyfd_openFileDialog(
                title,
                defaultPath,
                filterPatterns?.Length ?? 0,
                filterPatterns,
                filterDescription,
                allowMultipleSelects ? 1 : 0
            );

            if (resultPtr != 0)
                Marshal.PtrToStringUTF8(resultPtr);
        }

        if (result is null)
            return false;

        output = result.Split('|');
        return true;
    }

    public static bool SelectFolderDialog(
        [NotNullWhen(true)] out string? output,
        string? title = null,
        string? defaultPath = null
    )
    {
        output = null;

        if (OperatingSystem.IsWindows())
        {
            nint outputPtr = tinyfd_selectFolderDialogW(title, defaultPath);

            if (outputPtr != nint.Zero)
                output = Marshal.PtrToStringUni(outputPtr);
        }
        else
        {
            nint outputPtr = tinyfd_selectFolderDialog(title, defaultPath);

            if (outputPtr != nint.Zero)
                output = Marshal.PtrToStringUTF8(outputPtr);
        }

        return output is not null;
    }

    public static bool ColorChooser(
        out byte[] output,
        string? title = null,
        byte[]? defaultColor = null
    )
    {
        output = new byte[3];

        if (defaultColor is null || defaultColor.Length < 3)
            defaultColor = null;

        nint resultPtr;

        if (OperatingSystem.IsWindows())
            resultPtr = tinyfd_colorChooserW(title, null, defaultColor, output);
        else
            resultPtr = tinyfd_colorChooser(title, null, defaultColor, output);

        return resultPtr != 0;
    }
}
