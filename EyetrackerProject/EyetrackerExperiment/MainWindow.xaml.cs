using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyetrackerExperiment
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public EyetrackerEntities db;
 
        public MainWindow()
        {
            InitializeComponent();

            db = EyetrackerEntities.EyeTrackerDB;

            LoadData();
            DataContext = db;
        }

        public void LoadData()
        {
            db.LoadAllCandidatesAndTests();
            DataContext = db;
        }

        private void bnDbSettings_Click(object sender, RoutedEventArgs e)
        {
            if (DBSettings.ConfigureDB().Value)
            {
                LoadData();
            }
        }

        private void bnPathSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Configuration.PathConfig()).ShowDialog();
        }

        private void bnEyetrackerSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Configuration.EyeTrackerConfig()).ShowDialog();
        }

        private void bnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void bnSave_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
        }



        private void AddNewTest(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(db);
            if (newTest.ShowDialog().GetValueOrDefault(false))
            {
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DeleteTest(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            if (test == null) return;

            if (MessageBox.Show(String.Format(
                "Really delete Test {0} for {1} with {2] steps completed out of {3}?", 
                test.Test_Definition.Title,
                test.Candidate.personal_code,
                test.LastStep,
                test.NumSteps), "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            { }
        }

        private void DeleteLastStep(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            if (test == null || test.LastStep == 0) return;

            if (test.Test_Definition.EyeTrackerStep == test.LastStep)
            {
                List<Slide_Answer> toDelete = test.Slide_Answer.ToList();
                foreach (Slide_Answer sa in toDelete)
                    db.Slide_Answer.Remove(sa);
            }
            else
            {
                Questionnaire qu = test.Test_Definition.Questionnaire.FirstOrDefault(q => q.Step == test.LastStep);
                if (qu != null)
                {
                    List<Answer> toDelete = test.Answer.Where(a => a.Question.Questionnaire == qu).ToList();
                    foreach (Answer a in toDelete)
                        db.Answer.Remove(a);
                }
            }
            test.LastStep -= 1;
            if (test.LastStep == 0)
            {
                test.start_time = null;
                test.status_cd = "NEW";
            }
            db.SaveChanges();
        }

        private void RunNextStep(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            if (test == null) return;
            if (test.LastStep + 1 == test.Test_Definition.EyeTrackerStep)
            {
                EyeTracking.PresentationWindow trackingWindow = new EyeTracking.PresentationWindow(db, test);
            }
            else
            {
                Questionnaire questionnaire = test.Test_Definition.Questionnaire.FirstOrDefault(q => q.Step == test.LastStep + 1);
                if (questionnaire != null)
                {
                    QuestionnaireWindow qw = new QuestionnaireWindow(db, test, questionnaire);
                    qw.Show();
                }
            }
        }


        private void bnImportImages_Click(object sender, RoutedEventArgs e)
        {
            foreach (Test_Definition td in db.TestDefinitions)
            {
                foreach (Slide s in td.Slide)
                {
                    if (s.image == null && s.filepath != null)
                    {
                        System.IO.FileStream bmStream = new System.IO.FileStream(s.filepath, System.IO.FileMode.Open);
                        byte[] bmData = new byte[bmStream.Length];
                        bmStream.Read(bmData, 0, (int)bmStream.Length);
                        s.image = bmData;
                        //s.filepath = null;
                        db.SaveChanges();
                    }
                }
            }

        }
    }
}
