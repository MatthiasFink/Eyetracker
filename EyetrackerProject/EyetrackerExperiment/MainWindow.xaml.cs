using Data;
using EyetrackerExperiment.EyeTracking;
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
                "Really delete Test \"{0}\" for {1} with {2} steps completed out of {3}?", 
                test.Test_Definition.Title,
                test.Candidate.personal_code,
                test.LastStep,
                test.NumSteps), "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.Answer.RemoveRange(test.Answer);
                db.Slide_Answer.RemoveRange(test.Slide_Answer);
                db.Tracking.RemoveRange(test.Tracking);
                db.Test.Remove(test);
                db.Tests.Remove(test);
                db.SaveChanges();
            }
        }

        private void DeleteLastStep(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            if (test == null || test.LastStep == 0) return;

            if (test.Test_Definition.EyeTrackerStep == test.LastStep)
            {
                db.Slide_Answer.RemoveRange(test.Slide_Answer);
                test.Slide_Answer.Clear();
                db.Tracking.RemoveRange(test.Tracking);
                test.Tracking.Clear();
            }
            else
            {
                Questionnaire qu = test.Test_Definition.Questionnaire.FirstOrDefault(q => q.Step == test.LastStep);
                if (qu != null)
                {
                    db.Answer.RemoveRange(test.Answer);
                    test.Answer.Clear();
                }
            }
            test.LastStep -= 1;
            if (test.LastStep == 0)
            {
                test.start_time = null;
                test.status_cd = "NEW";
            }
            else if (test.status_cd == "TRM")
            {
                test.status_cd = "PRG";
                test.end_time = null;
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

        private void FilterCheck(object sender, RoutedEventArgs e)
        {
            RibbonSplitMenuItem changed = null;
            if (e.OriginalSource.GetType() == typeof(RibbonSplitMenuItem))
            {
                changed = (RibbonSplitMenuItem)e.OriginalSource;
                if (changed == FilterNEW && changed.IsChecked)
                {
                    FilterPRG.IsChecked = false;
                    FilterTRM.IsChecked = false;
                    FilterNEWPRG.IsChecked = false;
                }
                else if (changed == FilterPRG && changed.IsChecked)
                {
                    FilterNEW.IsChecked = false;
                    FilterTRM.IsChecked = false;
                    FilterNEWPRG.IsChecked = false;
                }
                else if (changed == FilterTRM && changed.IsChecked)
                {
                    FilterNEW.IsChecked = false;
                    FilterPRG.IsChecked = false;
                    FilterNEWPRG.IsChecked = false;
                }
                else if (changed == FilterNEWPRG && changed.IsChecked)
                {
                    FilterNEW.IsChecked = false;
                    FilterPRG.IsChecked = false;
                    FilterTRM.IsChecked = false;
                }
                else if (changed == FilterS1 && changed.IsChecked)
                {
                    FilterNEW.IsChecked = false;
                    FilterS2.IsChecked = false;
                    FilterS3.IsChecked = false;
                }
                else if (changed == FilterS2 && changed.IsChecked)
                {
                    FilterNEW.IsChecked = false;
                    FilterS1.IsChecked = false;
                    FilterS3.IsChecked = false;
                }
                else if (changed == FilterS3 && changed.IsChecked)
                {
                    FilterNEW.IsChecked = false;
                    FilterS1.IsChecked = false;
                    FilterS2.IsChecked = false;
                }
            }
            else
            {
                FilterNEW.IsChecked = false;
                FilterPRG.IsChecked = false;
                FilterTRM.IsChecked = false;
                FilterNEWPRG.IsChecked = false;
                FilterS1.IsChecked = false;
                FilterS2.IsChecked = false;
                FilterS3.IsChecked = false;
            }

            if (FilterNEW.IsChecked || FilterPRG.IsChecked || FilterTRM.IsChecked || FilterNEWPRG.IsChecked ||
                FilterS1.IsChecked || FilterS2.IsChecked || FilterS3.IsChecked)
                rbsFilter.LargeImageSource = new BitmapImage(new Uri("Resources/FilterOff32.png", UriKind.Relative));
            else
                rbsFilter.LargeImageSource = new BitmapImage(new Uri("Resources/Filter32.png", UriKind.Relative));

            gridTests.Items.Filter = null;
            gridTests.Items.Filter = new Predicate<object>(t => TestFilter((Test)t));
        }

        private bool TestFilter(Test t)
        {
            return (
                (!FilterNEW.IsChecked || t.status_cd == "NEW") &&
                (!FilterPRG.IsChecked || t.status_cd == "PRG") &&
                (!FilterTRM.IsChecked || t.status_cd == "TRM") &&
                (!FilterNEWPRG.IsChecked || t.status_cd != "TRM") &&
                (!FilterS1.IsChecked || t.LastStep == 1) &&
                (!FilterS2.IsChecked || t.LastStep == 2) &&
                (!FilterS3.IsChecked || t.LastStep == 3));
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

        private void gridTests_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            gridTests.RowDetailsVisibilityMode = gridTests.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed ?
                DataGridRowDetailsVisibilityMode.VisibleWhenSelected : DataGridRowDetailsVisibilityMode.Collapsed;
        }

        private void bnImportTrackingCurrent_Click(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            if (test == null) return;
            if (test.LastStep < test.Test_Definition.EyeTrackerStep) return;

            bool MergeReplace = false;
            if (test.Tracking.Count > 0)
            {
                if (MessageBox.Show("Test also contains tracking data. Do you want to merge/replace existing data?",
                    "Confirmation",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MergeReplace = true;
                }
                else return;
            }

            Properties.Settings settings = new Properties.Settings();
            String fullPattern = String.Format(EyeTracking.PresentationWindow.TrackingFilePattern,
                settings.TrackingPathRemote,
                test.Candidate.personal_code,
                test.id,
                "*");
            String initialDirectory = System.IO.Path.GetDirectoryName(fullPattern);
            String filter = System.IO.Path.GetFileName(fullPattern);
            filter = "idf tracking for " + test.Candidate.personal_code + "|" + filter + "|" +
                    "txt tracking for " + test.Candidate.personal_code + "|" + filter.Replace(".idf", ".txt");
            String[] fileNames = Configuration.FileDialogs.SelectFiles(initialDirectory, filter);
            if (fileNames != null)
            {
                TrackingReader trackingReader = new TrackingReader(db, test);
                foreach (String fileName in fileNames)
                {
                    trackingReader.Read(fileName);
                }
            }

        }

        private void bnImportTrackingMissing_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
