using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EyetrackerExperiment
{

    /* General File layout:
     * 
     * File starts with idf version header.
     * 
     * Different sections follow, each prefixed by a one byte letter-code 
     * These sections are either of predefined size or use
     * different means to indicate their length. 
     * 
     * Fixed structures are documented as structures below, variable ones
     * by specific descriptions
     * 
     * Fields where the meaning has not been deducted are prefixed with 'u_' 
     */

    /* Basic Idf header, contains:
     * version, must be 8 (for this reader
     * timestamp in seconds, requires offset of 25597 days (
     * Also contains information on the screen used
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IdfHeader
    {
        public UInt32 IdfVersion;
        public UInt32 Time; // Seconds from 1.1.0050, 
        public UInt32 xRes; // 1920
        public UInt32 yRes; // 1080
        public byte bytesPerPixel; // 3
    }

    /* Calibration marker - 'C' - variable
     * The marker is followed by a sequence of ASCII digits terminated by 0xFF indicating
     * the number of calibration points.
     * The next uint16 indicates the length of the following ascii text describing the calibration
     * points.
     * 
     * Example: 'C' '1' '3' 0xff 0x03A3 
     * Indicates 13 calibration points described as ASCII text of 0x03A3 = 931 bytes.
     * 
     * The text contains a series of point samples describing screen and measured locations
     * as well as an error coefficient. Samples are separated by a [TAB] character. The last entry 
     * is followed by a space an a TAB chracter.
     * 
     * Pattern (variable parts in <...>:
     * "Point<number>: raw(<raw x>, <raw y>) screen(<screen x>,<screen y>) coefficient(<coeff x>,<coeff y>)"
     * 
     * Example: 
     * Point0: raw(-6.64, -27.92) screen(960,540) coefficient(-11.3093,-10.3872)[TAB]Point1: ...
     * 
     * Coefficients values are mostly dececreasing, with the last three samples being 0 / 0. 
     * The number of points is a setting for ISM devices, 13 is recommended for highspeed devices.  
     * The area after appears to be fixed, with little interpretation available regarding structure meaning.
     * Assumption is, that it contains the final geometric transformation achieved by calibration.
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IdfCalibrationSuffix // marker = 'C'
    {
        public double u_d1; // -2,61
        public UInt32 u_l1; // 0
        public UInt32 u_l2; // 0
    };

    /* Unknown section - 'F' - fixed
     * Contains 3 Uint32 values, e.g. 16, 3, 560
    */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IdfFieldSection // marker = 'F'
    {
        public byte u_b1; // 16
        public byte u_b2; // 0
        public byte u_b3; // 0
        public UInt32 camX; // 800 
        public UInt32 camY; // 560
    };

    /* Unknown section - 'J' - fixed
     * Sections contains a single Uint32 value, in our case 0x0001
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IdfSectionJ // marker = 'F'
    {
        public UInt32 u_l1; // 1
    };

    /* Sample table description - 'G' - variable
     * The section starts string of sample field headers,
     * separated by one space (0x20) character. The last entry also includes a 
     * reailing blank.
     * Length of this variable part is determined by scanning vor NUL.
     * 
     * Example:
     * 'TimeStamp SetNum Quality RPupX RPupY RPupDX RPupDY RCr0X RCr0Y RGX RGY ' NUL
     * 
     * This area is followed by another which may as well be variable, e.g. depending on the 
     * number and type of fields.
     * 
     * If we presume that the filelds TimeStamp, Setnum and Quality are always included, we are left with
     * exactly 8 configurable fields. Fields carrying a 0 are usually absolute measurements, 
     * having to do with field descriptions. However, the final double value and previous Uint32 with 
     * value 4 do not fit this explanation. 
     * 
     * We assume that all integer values will always have rather low values, so that the pattern for the first 
     * sample record (marjer 'S', usually followed be 0x0001 can be found easily. For checks, the field Scale 
     * can be assumed positive, between 5 and 100. Misaligned IETF doubles usually show values far outside this
     * range.
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IdfTableSuffix
    {
        public UInt32 u_l1; // 0 - Describing x?
        public UInt32 u_l2; // 0 - Describing y?
        public UInt32 u_l3; // 1 - Describing Dia x
        public UInt32 u_l4; // 1 - Describing Dia y
        public UInt32 u_l5; // 0 - Describing Cr x
        public UInt32 u_l6; // 0 - Dscribing Cr y 
        public UInt32 u_l7; // 1 - Describing POR x
        public UInt32 u_l8; // 4 - Desctibing POR y
        public double Scale; // 12.122 number is similar to relation between nin/max range of measurements and screen resolution.
    }


    /* Sample record - 'S', fixed depending on configured output settings.
    */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IdfData
    {
        public UInt32 trial; // always 1
        public UInt32 time; // measured as microseconds, no absolute reference, probably related to 
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

    public class IdfSection
    {
        public char marker;
        public long fileOffset;
        public object data;
    }

    public class  Calibration
    {
        public Point screen;
        public PointF raw;
        public PointF coeff;
    }



    class IdfFile
    {
        BinaryReader reader;

        public UInt32       IdfVersion = 0;
        public DateTime     fileSaveTime;
        public Point        ScreenSize;
        public double       Scale;
        public String[]     CalibrationPoints;
        public long         SampleStart = -1;

        public Dictionary<char, IdfSection> 
                            Sections = new Dictionary<char, IdfSection>();
        public List<String> Fields = new List<string>();


        public IdfFile(String path) : this(new FileStream(path, FileMode.Open))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            reader = new BinaryReader(stream);
        }

        public IdfFile(FileStream stream)
        {
            reader = new BinaryReader(stream);
        }

        private int ReadStruct<T>(out T structure)
        {
            int structLen = Marshal.SizeOf(typeof(T));

            byte[] buffer = reader.ReadBytes(structLen);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            return structLen;
        }

        public int readHeader()
        {
            reader.BaseStream.Position = 0;
            int bytesRead;
            IdfHeader header;
            bytesRead = ReadStruct<IdfHeader>(out header);
            IdfVersion = header.IdfVersion;
            if (IdfVersion != 8)
                return -1;

            fileSaveTime = (new DateTime(1970, 1, 1)).AddSeconds(header.Time);
            ScreenSize.X = (int)header.xRes;
            ScreenSize.Y = (int)header.yRes;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                char marker = reader.ReadChar();
                IdfSection section = new IdfSection();
                section.fileOffset = reader.BaseStream.Position;
                section.marker = marker;
                switch (marker)
                {
                    case 'C':
                        {
                            String sNumPoints = "";
                            byte b;
                            while ((b = reader.ReadByte()) != 0xff)
                                sNumPoints += (char)b;

                            int numPoints = int.Parse(sNumPoints);
                            UInt16 calibrationLen = reader.ReadUInt16();
                            byte[] calibrationBytes = reader.ReadBytes(calibrationLen);
                            String calibrationText = System.Text.Encoding.ASCII.GetString(calibrationBytes);
                            CalibrationPoints = calibrationText.Split((char)0x09).Where(s => s != "").ToArray();
                            if (CalibrationPoints.Count() != numPoints)
                                return -1;

                            IdfCalibrationSuffix idfCalibrationSuffix;
                            ReadStruct<IdfCalibrationSuffix>(out idfCalibrationSuffix);
                            section.data = idfCalibrationSuffix;
                            break;
                        }
                    case 'F':
                        {
                            IdfFieldSection fieldSection;
                            ReadStruct<IdfFieldSection>(out fieldSection);
                            section.data = fieldSection;
                            break;
                        }
                    case 'J':
                        {
                            section.data = reader.ReadUInt32();
                            break;
                        }
                    case 'G':
                        {
                            String fieldName = "";
                            while (reader.PeekChar() > 30)
                            {
                                char c = reader.ReadChar();
                                if (c == ' ')
                                {
                                    Fields.Add(fieldName);
                                    fieldName = "";
                                }
                                else
                                    fieldName += c;
                            }
                            long previousPos = reader.BaseStream.Position;

                            IdfTableSuffix tableSuffix;
                            ReadStruct<IdfTableSuffix>(out tableSuffix);
                            section.data = tableSuffix;

                            if (reader.PeekChar() != 'S' || tableSuffix.Scale < 1.0 || tableSuffix.Scale > 100.0)
                            {
                                reader.BaseStream.Position = previousPos;
                                while (reader.BaseStream.Position < previousPos + 200)
                                {
                                    double sScale = reader.ReadDouble();
                                    char sMarker = reader.ReadChar();
                                    UInt32 sTrial = reader.ReadUInt32();

                                    if (sMarker == 'S' && sScale > 5.0 && sScale < 100.0 && sTrial == 1)
                                    {
                                        Scale = sScale;
                                        reader.BaseStream.Position -= 5;
                                    }
                                    else reader.BaseStream.Position -= 12;
                                }

                            }
                            else
                            {
                                section.data = tableSuffix;
                                Scale = tableSuffix.Scale;
                            }
                            break;
                        }
                    case 'S':
                        {
                            reader.BaseStream.Position -= 1;
                            SampleStart = reader.BaseStream.Position;
                            return (int)SampleStart;
                        }
                }
                Sections.Add(section.marker, section);
            }
            return (int)reader.BaseStream.Position;
        }

        public int readData(int rec, ref IdfData data)
        {
            char marker;

            marker = reader.ReadChar();
            if (SampleStart <= 0)
                return -1;

            int read = ReadStruct<IdfData>(out data);
            return read;
        }

        public long Count() { return (reader.BaseStream.Length - SampleStart - 3) /(1 + Marshal.SizeOf(typeof(IdfData))); }
    }
}
