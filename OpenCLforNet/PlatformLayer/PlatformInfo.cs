using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenCLforNet.Function;

namespace OpenCLforNet.PlatformLayer;

public unsafe class PlatformInfo
{
    public int Index { get; }
    public readonly List<DeviceInfo> DeviceInfos  = new ();

    private readonly Dictionary<string, byte[]> infos = new ();

    protected internal PlatformInfo(int index)
    {
        Index = index;

        // get a platform
        uint count = 0;
        var s0 = OpenCL.clGetPlatformIDs(0, null, &count);
        if (s0 == 0)
        {
            var platforms = (void**)Marshal.AllocCoTaskMem((int)(count * IntPtr.Size));
            void* platform = null;
            try
            {

                var s1 = OpenCL.clGetPlatformIDs(count, platforms, &count);
                if (s1 != 0)
                {
                    platform = platforms[index];
                    // get platform infos
                    foreach (long info in Enum.GetValues(typeof(cl_platform_info)))
                    {
                        var size = new IntPtr();
                        var s2 = OpenCL.clGetPlatformInfo(platform, info, IntPtr.Zero, null, &size);
                        if (s2 == 0)
                        {
                            var value = new byte[(int)size];
                            fixed (byte* valuePointer = value)
                            {
                                var s3 = OpenCL.clGetPlatformInfo(platform, info, size, valuePointer, null);
                                if (s3 == 0)
                                {
                                    infos.Add(Enum.GetName(typeof(cl_platform_info), info), value);
                                }
                            }
                        }
                    }

                    // get devices
                    var s4 = OpenCL.clGetDeviceIDs(platform, (long)cl_device_type.CL_DEVICE_TYPE_ALL, 0, null, &count);

                    if (s4 == 0)
                    {
                        // create device infos
                        for (int i = 0; i < count; i++)
                            DeviceInfos.Add(new DeviceInfo(platform, i));
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(new IntPtr(platforms));
            }
        }
    }

    public List<string> Keys => infos.Keys.ToList();

    public string this[string key] => Encoding.UTF8.GetString(infos[key], 0, infos[key].Length).Trim();

    public string GetValueAsString(string key) => Encoding.UTF8.GetString(infos[key], 0, infos[key].Length).Trim();

}
