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
        }

        public void LoadData()
        {
            db.LoadAllCandidatesAndTests();
            DataContext = db;
            gridTests.UpdateLayout();
        }

        private void bnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (DBSettings.ConfigureDB().Value)
            {
                LoadData();
            }
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

        private void RunNextQuestionnaire(object sender, RoutedEventArgs e)
        {
            Test test = (Test)gridTests.SelectedItem;
            Questionnaire questionnaire = null;
            foreach (Questionnaire q in test.Test_Definition.Questionnaire)
            {
                if (test.Answer.Count(a => a.Question.Questionnaire == q) == 0)
                {
                    questionnaire = q;
                    break;
                }
            }
            if (questionnaire != null)
            {
                QuestionnaireWindow qw = new QuestionnaireWindow(db, test, questionnaire);
                qw.Show();
            }
        }
    }
}
