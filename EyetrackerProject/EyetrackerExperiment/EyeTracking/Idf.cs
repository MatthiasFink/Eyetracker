using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EyetrackerExperiment
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct IdfHeader
    {
        public UInt32      IdfVersion;
        public byte marker_B;
        public byte frameRate;
        public UInt32 xRes;
        public UInt32 yRes;
        public byte bytesPerPixel;
        public byte        markerC;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IdfData
    {
        public byte marker;
        public UInt32 trial;
        public UInt32 time;
        public UInt64 setNum; // always 1
        public UInt32 quality; // // around 630 +/- 10
        public double rPupX; // around 100
        public double rPupY;
        public double rPupDX;
        public double rPupDY;
        public double rCr0X;
        public double rCr0Y;
        public double rGX;
        public double rGY;
        public UInt32 next;
    }

    class IdfFile
    {
        FileStream stream;

        public IdfFile(String path) : this(new FileStream(path, FileMode.Open))
        {
            stream = new FileStream(path, FileMode.Open);
        }

        public IdfFile(FileStream stream)
        {
            this.stream = stream;
        }

        public IdfHeader readHeader()
        {
            stream.Position = 0;
            IdfHeader header = new IdfHeader();
            byte[] buffer = new byte[Marshal.SizeOf(header)];
            stream.Read(buffer, 0, Marshal.SizeOf(header));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            header = (IdfHeader) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(IdfHeader));
            return header;
        }

        public int readData(int rec, ref IdfData data)
        {
            long pos = 1099 + rec * Marshal.SizeOf(typeof(IdfData));
            if (pos + Marshal.SizeOf(typeof(IdfData)) > stream.Length)
                return -1;

            stream.Position = pos;

            byte[] buffer = new byte[Marshal.SizeOf(typeof(IdfData))];
            int read = stream.Read(buffer, 0, Marshal.SizeOf(typeof(IdfData)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            data = (IdfData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(IdfData));
            return read;
        }

        public long Count() { return (stream.Length - 1099) / Marshal.SizeOf(typeof(IdfData)); }
    }
}
