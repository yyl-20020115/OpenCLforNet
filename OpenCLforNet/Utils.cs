using System;

namespace OpenCLforNet;
public static class Utils
{
    public static bool Is32Bit => IntPtr.Size == sizeof(int);
    public static bool Is64Bit => IntPtr.Size == sizeof(long);
}
