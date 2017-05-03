using System;
using System.Runtime.InteropServices;

namespace SerialLibrary

{
    public static class DataTools
    { 
        /// <summary>
        /// converts object to byte[]
        /// </summary>
        /// <param name="anything">source object</param>
        /// <returns>byteArray</returns>
        public static byte[] RawSerialize(object anything)
        {
        int rawSize = Marshal.SizeOf(anything);
        IntPtr buffer = Marshal.AllocHGlobal(rawSize);
        Marshal.StructureToPtr(anything, buffer, false);
        byte[] rawDatas = new byte[rawSize];
        Marshal.Copy(buffer, rawDatas, 0, rawSize);
        Marshal.FreeHGlobal(buffer);
        return rawDatas;
    }

    /// <summary>
    /// convert byte[] to object
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="rawData">source byte[]</param>
    /// <param name="position">byte[X]</param>
    /// <returns>object<TT></TT></returns>
    public static T RawDeserialize<T>(byte[] rawData, int position)
    {
        int rawsize = Marshal.SizeOf(typeof(T));
        if (rawsize > rawData.Length - position)
            throw new ArgumentException("Not enough data to fill struct. Array length from position: " + (rawData.Length - position) + ", Struct length: " + rawsize);
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.Copy(rawData, position, buffer, rawsize);
        T retobj = (T)Marshal.PtrToStructure(buffer, typeof(T));
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }
}
}
