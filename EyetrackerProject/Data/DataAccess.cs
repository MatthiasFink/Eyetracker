using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Tests : ObservableCollection<Test> { }
    public class Candidates : ObservableCollection<Candidate> { }
    public class TestDefinitions : ObservableCollection<Test_Definition> { }

    public partial class EyetrackerEntities : DbContext
    {
        public EyetrackerEntities(String connString = null)
            : base((connString == null ? EyetrackerEntities.buildConnString() : connString))
        {
            candidates = new Candidates();
            testDefinitions = new TestDefinitions();
            tests = new Tests();
        }

        public static String buildConnString()
        {
            Properties.Settings s = new Properties.Settings();
            String connString = @"data source=" + s.DBServer + ";initial catalog=" + s.DBCatalog +
                (s.DBWindowsAuthentication ? ";integrated security=True" : ";user=" + s.DBUser + ";password=" + s.DBPassword) +
                ";MultipleActiveResultSets=True;App=EntityFramework;";
            EntityConnectionStringBuilder esb = new EntityConnectionStringBuilder();
            esb.Metadata = "res://Data/Model.csdl|res://Data/Model.ssdl|res://Data/Model.msl";
            esb.Provider = "System.Data.SqlClient";
            esb.ProviderConnectionString = connString;
            return esb.ConnectionString;
        }

        private static EyetrackerEntities db;

        public static EyetrackerEntities EyeTrackerDB
        {
            get
            {
                while (db == null)
                {
                    db = new EyetrackerEntities(buildConnString());
                    try
                    {
                        db.Database.Connection.Open();
                    }
                    catch (Exception e)
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

        public Candidates candidates { get; }
        public TestDefinitions testDefinitions { get; }
        public Tests tests { get; }

        public void LoadAllCandidatesAndTests()
        {
            tests.Clear();
            candidates.Clear();
            testDefinitions.Clear();
            foreach (Test_Definition td in Test_Definition)
                testDefinitions.Add(td);

            foreach (Candidate c in Candidate)
            {
                candidates.Add(c);
                foreach (Test t in c.Test)
                    tests.Add(t);
            }
        }
    }
}
