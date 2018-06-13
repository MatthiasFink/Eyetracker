using Data;
using EyetrackerExperiment.EyeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

            LoadTrackingWorker.DoWork += LoadTracking;
            LoadTrackingWorker.RunWorkerCompleted += LoadTrackingCompleted;

            db = EyetrackerEntities.EyeTrackerDB;

            LoadData();
            DataContext = db;
            LoggingStatus.PostMessage(Severity.Info, String.Format("Total of {0} tests loaded from database.", db.Tests.Count));
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
            Candidate candidate = test.Candidate;
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
                db.Database.ExecuteSqlCommand("delete from tracking where test_id = {0}", test.id);
                db.Test.Remove(test);
                db.Tests.Remove(test);
                db.SaveChanges();
            }
            if (candidate.Test.FirstOrDefault() == null)
            {
                if (MessageBox.Show(String.Format(
                    "Candidate {0} has no other tests planned or performed. Delete candidate?",
                    candidate.personal_code), "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Candidate.Remove(candidate);
                    db.SaveChanges();
                }

            }

        }

        private void DeleteLastStep(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            if (test == null || test.LastStep == 0) return;

            if (test.Test_Definition.EyeTrackerStep == test.LastStep)
            {
                db.Slide_Answer.RemoveRange(db.Slide_Answer.Where(sa => sa.test_id == test.id));
                db.Database.ExecuteSqlCommand("delete from tracking where test_id={0}", test.id);
                db.SaveChanges();
            }
            else
            {
                Questionnaire qu = test.Test_Definition.Questionnaire.FirstOrDefault(q => q.Step == test.LastStep);
                if (qu != null)
                {
                    db.Answer.RemoveRange(test.Answer);
                    //test.Answer.Clear();
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

                    // Fachlich auszuschließen 
                    FilterTY.IsChecked = false;
                    FilterS1.IsChecked = false;
                    FilterS2.IsChecked = false;
                    FilterS3.IsChecked = false;
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
                else if (changed == FilterTY && changed.IsChecked)
                {
                    FilterTN.IsChecked = false;

                    // Fachlich auszuschließen 
                    FilterNEW.IsChecked = false;
                }
                else if (changed == FilterTN && changed.IsChecked)
                {
                    FilterTY.IsChecked = false;
                }
            }
            else
                foreach (Object m in rbsFilter.Items)
                    if (m.GetType().Equals(typeof(RibbonSplitMenuItem)))
                        ((RibbonMenuItem)m).IsChecked = false;

            if (FilterNEW.IsChecked || FilterPRG.IsChecked || FilterTRM.IsChecked || FilterNEWPRG.IsChecked ||
                FilterS1.IsChecked || FilterS2.IsChecked || FilterS3.IsChecked ||
                FilterTY.IsChecked || FilterTN.IsChecked)
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
                (!FilterS3.IsChecked || t.LastStep == 3) &&
                (!FilterTY.IsChecked || t.HasTracking) &&
                (!FilterTN.IsChecked || !t.HasTracking));
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

        private void viewStepDetail_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Step s = (Step)cb.DataContext;
            if (s.IsCompleted)
            {
                Test t = s.Test;
                if (s.Num == t.Test_Definition.EyeTrackerStep)
                {
                    new TrackingViewer(t);
                }
                else
                {

                }
            }
            cb.IsChecked = s.IsCompleted;
        }

        private void bnImportTrackingCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (LoadTrackingWorker.IsBusy) return;
            Test test = (Test)gridTests.SelectedItem;
            if (test == null) return;
            if (test.LastStep < test.Test_Definition.EyeTrackerStep)
            {
                LoggingStatus.PostMessage(Severity.Warning, "Cannot load tracking data, eyetracker experiment not executed yet");
                return;
            }

            bool mergeReplace = false;
            if (test.HasTracking)
            {
                if (MessageBox.Show("Test already contains tracking data. Do you want to merge/replace existing data?",
                    "Confirmation",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    mergeReplace = true;
                }
                else return;
            }

            Properties.Settings settings = new Properties.Settings();
            String fullPattern = String.Format(EyeTracking.PresentationWindow.TrackingFilePattern,
                settings.TrackingPathRemote,
                test.Candidate.personal_code,
                test.id,
                "*");
            String initialDirectory = Path.GetDirectoryName(fullPattern);
            String filter = Path.GetFileName(fullPattern);
            filter = "idf tracking for " + test.Candidate.personal_code + "|" + filter + "|" +
                    "txt tracking for " + test.Candidate.personal_code + "|" + filter.Replace(".idf", ".txt");
            String[] fileNames = Configuration.FileDialogs.SelectFiles(initialDirectory, filter);
            if (fileNames != null)
            {
                LoggingStatus.PostMessage(Severity.Info, String.Format("Examining {0} tracking files...", fileNames.Count()));
                LoadFilesArgument arg = new LoadFilesArgument() { test = test, mergeReplace = mergeReplace };
                arg.fileNames.AddRange(fileNames);
                List<LoadFilesArgument> args = new List<LoadFilesArgument>();
                args.Add(arg);
                LoadTrackingWorker.RunWorkerAsync(args);
            }
        }


        private void bnImportTrackingMissing_Click(object sender, RoutedEventArgs e)
        {
            if (LoadTrackingWorker.IsBusy) return;
            Properties.Settings settings = new Properties.Settings();
            String path = settings.TrackingPathRemote;
            path = Configuration.FileDialogs.SelectFolder(path);


            if (path == null)
                return;

            LoggingStatus.PostMessage(Severity.Info, String.Format("Scanning directory {0} and all subdirectories for missing tracking data...", path));
            List<LoadFilesArgument> args = new List<LoadFilesArgument>();
            ScanForMissingTracking(path, args);
            int numTests = args.Count;
            int numFiles = args.Sum(a => a.fileNames.Count);
            if (numFiles > 0)
            {
                LoggingStatus.PostMessage(Severity.Info, String.Format("Found total of {0} files related to {1} different tests.", numFiles, numTests));
                LoadTrackingWorker.RunWorkerAsync(args);
            }
            else
                LoggingStatus.PostMessage(Severity.Info, "No suitable tracking files identified.");
        }

        private class LoadFilesArgument
        {
            public Test test;
            public List<String> fileNames = new List<string>();
            public bool mergeReplace;
        }

        private readonly BackgroundWorker LoadTrackingWorker = new BackgroundWorker();

        private void LoadTrackingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EyetrackerExperiment.App app = (EyetrackerExperiment.App)App.Current;
            app.Log(Severity.Info, String.Format("Total of {0} files loaded successfully...", (int)e.Result));
        }

        private void LoadTracking(object sender, DoWorkEventArgs e)
        {
            EyetrackerExperiment.App app = (EyetrackerExperiment.App)App.Current;
            List<LoadFilesArgument> args = (List<LoadFilesArgument>)e.Argument;
            int numFilesLoaded = 0;
            foreach (LoadFilesArgument arg in args)
            {
                TrackingReader trackingReader = new TrackingReader(arg.test);
                foreach (String fileName in arg.fileNames)
                {
                    app.Log(Severity.Info, String.Format("Trying to load {0}...", fileName));
                    int numSamples = trackingReader.Read(fileName, arg.mergeReplace);
                    if (numSamples > 0)
                    {
                        app.Log(Severity.Info, String.Format("Successfully loaded {0} samples from {1} into database.", numSamples, fileName));
                        numFilesLoaded++;
                    }
                    else if (numSamples == 0)
                        app.Log(Severity.Warning, String.Format("No samples found in {0}.", fileName));
                    else
                        app.Log(Severity.Error, String.Format("Loading of file {0} failed: {1}.", fileName, TrackingReader.getReturnCodeMsg(numSamples)));
                }
            }
            e.Result = numFilesLoaded;
        }

        private void ScanForMissingTracking(String path, List<LoadFilesArgument> args)
        {
            LoggingStatus.PostMessage(Severity.Info, String.Format("Scanning files in {0}...", path));
            try
            {
                foreach (String filePath in Directory.GetFiles(path))
                {
                    String fileName = Path.GetFileNameWithoutExtension(filePath);
                    String extension = Path.GetExtension(filePath);

                    if (!extension.ToUpper().Equals(".TXT") && !extension.ToUpper().Equals(".IDF"))
                        continue;

                    String[] nameParts = fileName.Split('-');

                    if (nameParts.Count() != 3)
                        continue;

                    if (nameParts[0].Length != 10)
                        continue;

                    int slideNum = -1;
                    int testId = -1;

                    if (!int.TryParse(nameParts[1], out testId) || !int.TryParse(nameParts[2], out slideNum))
                        continue;

                    Test test = db.Tests.FirstOrDefault(t => t.id == testId);
                    if (test == null)
                    {
                        LoggingStatus.PostMessage(Severity.Warning, String.Format("Encountered file {0} but could not find corresponding test.", fileName + extension));
                        continue;
                    }

                    Slide_Answer slideAnswer = test.Slide_Answer.FirstOrDefault(sa => sa.Slide.num == slideNum);
                    if (slideAnswer == null)
                    {
                        LoggingStatus.PostMessage(Severity.Warning, String.Format("Encountered file {0} but could not find corresponding answer for slide {1} in test.", fileName + extension, slideNum));
                        continue;
                    }
                    LoadFilesArgument arg = args.FirstOrDefault(a => a.test.id == test.id);
                    if (arg == null)
                    {
                        arg = new LoadFilesArgument() { test = test, mergeReplace=false };
                        args.Add(arg);
                    }
                    arg.fileNames.Add(filePath);
                }
            }
            catch (System.UnauthorizedAccessException)
            { }

            try
            {
                foreach (String subDir in Directory.GetDirectories(path))
                    ScanForMissingTracking(subDir, args);
            }
            catch (System.UnauthorizedAccessException)
            { }
        }
    }
}
