using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EyetrackerExperiment
{
    /// <summary>
    /// Interaktionslogik für QuestionnaireWindow.xaml
    /// </summary>
    public partial class QuestionnaireWindow : Window
    {
        public EyetrackerEntities Db;
        public QuestionnaireAnswer QuestionnaireAnswer { get; set; }

        public QuestionnaireWindow(EyetrackerEntities db, Test test, Questionnaire questionnaire)
        {
            InitializeComponent();
            Db = db;
            QuestionnaireAnswer = new QuestionnaireAnswer(test, questionnaire);
            if (test.start_time == null)
                test.start_time = DateTime.Now;
            DataContext = QuestionnaireAnswer;
        }
    }
}
