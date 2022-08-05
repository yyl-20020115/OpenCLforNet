using System;
using OpenCLforNet.Function;

namespace OpenCLforNet;

static class Extension
{
    public static void CheckError(this int status)
    {
        var code = (cl_status_code)Enum.ToObject(typeof(cl_status_code), status);
        if (code != cl_status_code.CL_SUCCESS)
        {
            throw new Exception(Enum.GetName(typeof(cl_status_code), code));
        }
    }

}
