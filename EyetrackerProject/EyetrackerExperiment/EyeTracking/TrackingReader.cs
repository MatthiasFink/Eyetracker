using Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EyetrackerExperiment.EyeTracking
{

    class TrackingReader
    {
        private Test test;
        private EyetrackerEntities db;

        public TrackingReader(EyetrackerEntities Db, Test Test)
        {
            db = Db;
            test = Test;
        }

        public enum ReturnCodes
        {
            FileNotFound = -1,
            WrongFileExtension = -2,
            MalformedFileName = -3,
            WrongCandidate = -4,
            WrongTest = -5,
            UnsupportedFileFormat = -6,
            DataPresent = -7
        }

        public int Read(String filePath, bool mergeReplace)
        {
            if (!File.Exists(filePath))
                return (int)ReturnCodes.FileNotFound;

            String fileExtension = Path.GetExtension(filePath);

            if (!fileExtension.ToUpper().Equals(".TXT") && !fileExtension.ToUpper().Equals(".IDF"))
                return (int)ReturnCodes.WrongFileExtension;

            String fileName = Path.GetFileNameWithoutExtension(filePath);
            String[] nameParts = fileName.Split('-');

            if (nameParts.Count() != 3)
                return (int)ReturnCodes.MalformedFileName;

            if (!nameParts[0].Equals(test.Candidate.personal_code))
                return (int)ReturnCodes.WrongCandidate;

            int testId = int.Parse(nameParts[1]);

            if (test.id != testId)
                return (int)ReturnCodes.WrongTest;

            int slideNum = int.Parse(nameParts[2]);
            Slide_Answer slideAnswer = test.Slide_Answer.FirstOrDefault(sa => sa.Slide.num == slideNum);

            if (slideAnswer == null)
                return (int)ReturnCodes.MalformedFileName;

            if (test.Tracking.Count(t => t.Slide == slideAnswer.Slide) > 0)
            {
                if (mergeReplace)
                {
                    db.Tracking.RemoveRange(test.Tracking.Where(t => t.Slide == slideAnswer.Slide));
                }
                else return (int)ReturnCodes.DataPresent;
            }

            FileStream stream = new FileStream(filePath, FileMode.Open);

            if (filePath.ToUpper().EndsWith(".IDF"))
                return ReadIdf(stream, slideAnswer);
            else if (filePath.ToUpper().EndsWith(".TXT"))
                return ReadTxt(stream, slideAnswer);
            else
                return (int)ReturnCodes.WrongFileExtension;
        }


        public int ReadIdf(FileStream stream, Slide_Answer slideAnswer)
        {
            int numWritten = 0;
            IdfFile idfFile = new IdfFile(stream);
            IdfHeader header = idfFile.readHeader();
            IdfData data = new IdfData();

            long time;
            long timeOffset = -1;

            for (int rec = 0; rec < idfFile.Count(); rec++)
            {
                int result = idfFile.readData(rec, ref data);

                if (data.marker != 0x53)
                    Console.WriteLine("Record {0}; Non SMP marker {1} encountered ", rec, data.marker);
                if (data.rPupX < 0.0 || data.rPupX > 300.0)
                    Console.WriteLine("Record {0}: rPupX value {1} outside expected range", rec, data.rPupX);
                if (data.rPupY < 0.0 || data.rPupX > 300.0)
                    Console.WriteLine("Record {0}: rPupX value {1} outside expected range", rec, data.rPupX);
                if (data.rPupDX < 0.0 || data.rPupDX > 8.0)
                    Console.WriteLine("Record {0}: rPupDX value {1} outside expected range", rec, data.rPupX);
                if (data.rPupDX < 0.0 || data.rPupDX > 8.0)
                    Console.WriteLine("Record {0}: rPupDX value {1} outside expected range", rec, data.rPupX);


                if (timeOffset < 0)
                    timeOffset = data.time;

                time = data.time - timeOffset;

                Tracking tracking = new Tracking();
                tracking.occurred = slideAnswer.slide_start_time.Value.AddSeconds(time / 1000000.0);
                tracking.x = (decimal)data.rPupX;
                tracking.y = (decimal)data.rPupY;
                tracking.dia_x = (decimal)data.rPupDX;
                tracking.dia_y = (decimal)data.rPupDY;
                tracking.cr_x = (decimal)data.rCr0X;
                tracking.cr_y = (decimal)data.rCr0Y;
                tracking.por_x = (decimal)data.rGX;
                tracking.por_y = (decimal)data.rGY;
                tracking.timing = 0;
                tracking.trigger = 0;
                tracking.Slide = slideAnswer.Slide;
                tracking.Test = test;
                test.Tracking.Add(tracking);

                numWritten++;
            }
            db.Tracking.AddRange(test.Tracking);
            db.SaveChanges();
            test.Tracking.Clear();
            return numWritten;
        }

        public int ReadTxt(FileStream stream, Slide_Answer slideAnswer)
        {
            int numWritten = 0;

            StreamReader sr = new StreamReader(stream);
            String line;
            long timeOffset = -1;
            while (!sr.EndOfStream && (line = sr.ReadLine()).StartsWith("##")) ;

            NumberFormatInfo formatter = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            formatter.NumberDecimalSeparator = ".";
            formatter.NumberGroupSeparator = ",";

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();

                String[] lineParts = line.Split((char)9);

                if (lineParts.Count() != 13) continue;

                long time = long.Parse(lineParts[0]);

                if (timeOffset < 0)
                {
                    timeOffset = time;
                    time = 0;
                }
                else
                    time -= timeOffset;

                Tracking tracking = new Tracking();
                tracking.occurred = slideAnswer.slide_start_time.Value.AddSeconds(time / 1000000.0);
                tracking.x = Decimal.Parse(lineParts[3], formatter);
                tracking.y = Decimal.Parse(lineParts[4], formatter);
                tracking.dia_x = Decimal.Parse(lineParts[5], formatter);
                tracking.dia_y = Decimal.Parse(lineParts[6], formatter);
                tracking.cr_x = Decimal.Parse(lineParts[7], formatter);
                tracking.cr_y = Decimal.Parse(lineParts[8], formatter);
                tracking.por_x = Decimal.Parse(lineParts[9], formatter);
                tracking.por_y = Decimal.Parse(lineParts[10], formatter);
                tracking.timing = int.Parse(lineParts[11]);
                tracking.trigger = int.Parse(lineParts[12]);
                tracking.Slide = slideAnswer.Slide;
                tracking.Test = test;
                test.Tracking.Add(tracking);
                numWritten++;
            }
            db.Tracking.AddRange(test.Tracking);
            db.SaveChanges();
            test.Tracking.Clear();
            return numWritten;
        }
    }
}
