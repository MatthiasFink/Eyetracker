using Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            UnsupportedFileFormat
        }

        public int Read(String filePath)
        {
            if (filePath.ToUpper().EndsWith(".IDF"))
                return (int)ReturnCodes.UnsupportedFileFormat;
            else if (filePath.ToUpper().EndsWith(".TXT"))
                return ReadText(filePath);
            else
                return (int)ReturnCodes.WrongFileExtension;
        }

        public int ReadText(String filePath, bool forceExtension = false)
        {
            int numWritten = 0;

            if (!File.Exists(filePath))
                return (int)ReturnCodes.FileNotFound;

            String fileExtension = Path.GetExtension(filePath);

            if (!fileExtension.ToUpper().Equals(".TXT") && !forceExtension)
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

            db.Tracking.RemoveRange(test.Tracking.Where(t => t.Slide == slideAnswer.Slide));


            StreamReader sr = new StreamReader(filePath, true);
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
