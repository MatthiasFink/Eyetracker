using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Data
{
    public class Tests : ObservableCollection<Test> { }
    public class Candidates : ObservableCollection<Candidate> { }
    public class TestDefinitions : ObservableCollection<Test_Definition> { }

    public class AgeRange
    {
        public String name;
        public int rangeLow;
        public int rangeHigh;
        public override String ToString()
        {
            if (rangeLow == 0)
                return "under " + rangeHigh;
            else if (rangeHigh >= 99)
                return "above " + rangeLow;
            else
                return "between " + rangeLow + " and " + rangeHigh;
        }
    }

    public partial class EyetrackerEntities : DbContext
    {
        public EyetrackerEntities(String connString = null)
            : base((connString ?? EyetrackerEntities.BuildConnString()))
        {
            Candidates = new Candidates();
            TestDefinitions = new TestDefinitions();
            Tests = new Tests();
        }

        public static String BuildConnString()
        {
            Properties.Settings s = new Properties.Settings();
            String connString = @"data source=" + s.DBServer + ";initial catalog=" + s.DBCatalog +
                (s.DBWindowsAuthentication ? ";integrated security=True" : ";user=" + s.DBUser + ";password=" + s.DBPassword) +
                ";MultipleActiveResultSets=True;App=EntityFramework;";
            EntityConnectionStringBuilder esb = new EntityConnectionStringBuilder
            {
                Metadata = "res://Data/Model.csdl|res://Data/Model.ssdl|res://Data/Model.msl",
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = connString
            };
            return esb.ConnectionString;
        }

        private static EyetrackerEntities db;

        public static EyetrackerEntities EyeTrackerDB
        {
            get
            {
                while (db == null)
                {
                    db = new EyetrackerEntities(BuildConnString());
                    try
                    {
                        db.Database.Connection.Open();
                    }
                    catch (Exception)
                    {
                        db = null;
                        Boolean? dlgResult = DBSettings.ConfigureDB();
                        if (!dlgResult.HasValue || !dlgResult.Value)
                            break;
                    }
                }
                return db;
            }
        }

        public Candidates Candidates { get; set; }
        public TestDefinitions TestDefinitions { get; set; }
        public Tests Tests { get; set; }

        public void LoadAllCandidatesAndTests()
        {
            Tests.Clear();
            Candidates.Clear();
            TestDefinitions.Clear();

            Test_Definition.Load();
            Candidate.Load();

            foreach (Test_Definition td in Test_Definition)
                TestDefinitions.Add(td);

            foreach (Candidate c in Candidate)
            {
                Candidates.Add(c);
                foreach (Test t in c.Test)
                    Tests.Add(t);
            }
        }

        public static List<AgeRange> ageRanges = new List<AgeRange>()
        {
            new AgeRange() {name="0", rangeLow=0, rangeHigh=24 },
            new AgeRange() {name="25", rangeLow=25, rangeHigh=35 },
            new AgeRange() {name="36", rangeLow=36, rangeHigh=45 },
            new AgeRange() {name="46", rangeLow=46, rangeHigh=55 },
            new AgeRange() {name="56", rangeLow=56, rangeHigh=99 }
        };
    }

    public class Step
    {
        public int Num { get; }
        public enum StepType { Questionnaire, EyeTracker}
        public StepType Type;
        public String Title { get; }
        public bool IsCompleted { get; }
        public Test Test;
        public Step(int num, StepType stepType, String title, bool isCompleted, Test test)
        {
            Num = num; Type = stepType; Title = title; IsCompleted = isCompleted; Test = test;
        }
        public String Description
        {
            get
            {
                return String.Format("{0}. {1} - {2}",
                    Num, Title, IsCompleted ? "Done" : "Pending");
            }
        }
    }

    public partial class Test
    {
        public int NumSteps
        {
            get
            {
                return Test_Definition == null ? 0 : 
                    (Test_Definition.Questionnaire == null ? 0 : Test_Definition.Questionnaire.Count()) + 
                    (Test_Definition.Slide == null ? 0 : (Test_Definition.Slide.Count > 0 ? 1 : 0));
            }
        }
        
        public List<Step> Steps
        {
            get
            {
                List<Step> steps = new List<Step>();
                foreach (Questionnaire q in Test_Definition.Questionnaire)
                {
                    steps.Add(new Step(q.Step, Step.StepType.Questionnaire, q.Title, q.Step <= this.LastStep, this));
                }
                if (Test_Definition.Slide.Count > 0)
                    steps.Add(new Step(Test_Definition.EyeTrackerStep, Step.StepType.EyeTracker, "Eyetracker Experiment", Slide_Answer.Count > 0, this));
                return steps.OrderBy(s => s.Num).ToList();

            }
        }

        public bool HasTracking { get { return EyetrackerEntities.EyeTrackerDB.Tracking.FirstOrDefault(t => t.test_id == this.id) != null; } }

        public bool ShowDetails { get; set; }
    }

    public partial class Candidate
    {
        public String Description
        {
            get
            {
                return String.Format("Age: {0}, Gender: {1}",
                       EyetrackerEntities.ageRanges.FirstOrDefault(a => a.rangeLow == age_range_low).ToString(),
                       gender == "M" ? "male" : (gender == "F" ? "female" : "unknown"));
            }
        }
    }

    public partial class Slide
    {
        public Image SlideImage
        {
            get
            {
                if (image != null && image.Length > 0)
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Write(image, 0, image.Length);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
                else if (filepath != null && File.Exists(filepath))
                    return Image.FromFile(filepath);
                else
                    return null;
            }
        }

        public BitmapImage SlideBitmapImage
        {
            get
            {
                if (image != null && image.Length > 0)
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Write(image, 0, image.Length);
                    ms.Position = 0;
                    BitmapImage bm = new BitmapImage();
                    bm.BeginInit();
                    bm.StreamSource = ms;
                    bm.EndInit();
                    return bm;
                }
                else if (filepath != null && File.Exists(filepath))
                    return new BitmapImage(new Uri("file://" + filepath));
                else
                    return null;
            }
        }
    }
}
