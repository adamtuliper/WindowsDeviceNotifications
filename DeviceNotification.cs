using System;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential)]
internal class DEV_BROADCAST_HDR
{
    public int dbch_size;
    public int dbch_devicetype;
    public int dbch_reserved;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct DEV_BROADCAST_DEVICEINTERFACE
{
    public int dbcc_size;
    public int dbcc_devicetype;
    public int dbcc_reserved;
    public Guid dbcc_classguid;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public char[] dbcc_name;
}

static class DeviceNotification
{
    //https://msdn.microsoft.com/en-us/library/aa363480(v=vs.85).aspx
    public const int DbtDeviceArrival = 0x8000; // system detected a new device        
    public const int DbtDeviceRemoveComplete = 0x8004; // device is gone     
    public const int DbtDevNodesChanged = 0x0007; //A device has been added to or removed from the system.

    public const int WmDevicechange = 0x0219; // device change event      
    public const int DbtDevtypDeviceinterface = 5;
    //https://msdn.microsoft.com/en-us/library/aa363431(v=vs.85).aspx
    private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
    private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
    private static IntPtr notificationHandle;

    /// <summary>
    /// Registers a window to receive notifications when devices are plugged or unplugged.
    /// </summary>
    /// <param name="windowHandle">Handle to the window receiving notifications.</param>
    /// <param name="usbOnly">true to filter to USB devices only, false to be notified for all devices.</param>
    public static void RegisterDeviceNotification(IntPtr windowHandle, bool usbOnly = false)
    {
        var dbi = new DevBroadcastDeviceinterface
        {
            DeviceType = DbtDevtypDeviceinterface,
            Reserved = 0,
            ClassGuid = GuidDevinterfaceUSBDevice,
            Name = 0
        };

        dbi.Size = Marshal.SizeOf(dbi);
        IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
        Marshal.StructureToPtr(dbi, buffer, true);

        notificationHandle = RegisterDeviceNotification(windowHandle, buffer, usbOnly ? 0 : DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
    }

    /// <summary>
    /// Unregisters the window for device notifications
    /// </summary>
    public static void UnregisterDeviceNotification()
    {
        UnregisterDeviceNotification(notificationHandle);
    }

    public static string GetDeviceName(IntPtr lParam)
    {
        string deviceName = "";
        string deviceNameFiltered;
        DEV_BROADCAST_HDR hdr;
        hdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));
        if (hdr.dbch_devicetype == DeviceNotification.DbtDevtypDeviceinterface) //If it is a USB Mass Storage or Hard Drive
        {
            //Save Device name
            DEV_BROADCAST_DEVICEINTERFACE deviceInterface = (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
            deviceName = new string(deviceInterface.dbcc_name);
            deviceNameFiltered = deviceName.Substring(0, deviceName.IndexOf('{'));
            return deviceNameFiltered;
            // vid = GetVid(deviceName);
            // pid = GetPid(deviceName);
        }
        else
        {
            return "unknown";
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

    [DllImport("user32.dll")]
    private static extern bool UnregisterDeviceNotification(IntPtr handle);

    [StructLayout(LayoutKind.Sequential)]
    private struct DevBroadcastDeviceinterface
    {
        internal int Size;
        internal int DeviceType;
        internal int Reserved;
        internal Guid ClassGuid;
        internal short Name;
    }
}