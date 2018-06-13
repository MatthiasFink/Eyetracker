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
    /// Interaktionslogik für NewTest.xaml
    /// </summary>
    public partial class NewTest : Window
    {
        public Test Test { get; set; }
        public EyetrackerEntities Db { get; }

        public NewTest(EyetrackerEntities db)
        {
            InitializeComponent();
            Db = db;
            Test = new Test { Candidate = new Candidate() };
            DataContext = Test;
            cbTestDefinition.ItemsSource = db.TestDefinitions;
            if (db.TestDefinitions.Count > 0)
                cbTestDefinition.SelectedIndex = 0;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            CalcPersonalCode();
        }

        private void CalcPersonalCode()
        {
            int age = -1;
            if (birthDay.DisplayDate != null) {
                age = DateTime.Now.Year - birthDay.DisplayDate.Year;
                if (DateTime.Now.AddYears(-age) < birthDay.DisplayDate)
                    age -= 1;
                this.age.Text = age.ToString();
            }
            String code =
                (birthDay.DisplayDate.Day % 10).ToString() +
                (firstName.Text.Length > 0 ? firstName.Text.ToUpper().First() : '?') +
                (age % 10).ToString()[0] +
                (lastName.Text.Length > 0 ? lastName.Text.ToUpper().Last() : '?') +
                (birthDay.DisplayDate.Year % 10).ToString() +
                (firstName.Text.Length > 0 ? firstName.Text.ToUpper().Last() : '?') +
                (birthDay.DisplayDate.Month / 10).ToString() +
                (lastName.Text.Length > 0 ? lastName.Text.ToUpper().First() : '?');

            int checksum = 0;
            foreach (char c in code)
            {
                if (Char.IsDigit(c))
                    checksum += c - '0';
                sumDigits.Text = checksum.ToString("00");
            }
            personalCode.Text = code + checksum.ToString();
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalcPersonalCode();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String code = personalCode.Text;
            if (code == null || code.Length < 9 || code.Contains('?'))
                return;

            if (Test.Candidate.gender == null)
                return;

            if (Test.Test_Definition == null)
                return;
            else
                Test.test_definition_id = Test.Test_Definition.Id;

            foreach (AgeRange a in EyetrackerEntities.ageRanges)
            {
                if (Test.Candidate.age_range_low == a.rangeLow)
                    Test.Candidate.age_range_high = a.rangeHigh;
            }

            Candidate existing = Db.Candidates.FirstOrDefault(c => c.personal_code == code);
            if (existing != null)
            {
                Test.Candidate = existing;
            }
            else
            {
                Test.Candidate.personal_code = code;
                // Db.Candidate.Add(Test.Candidate);
                // Db.Candidates.Add(Test.Candidate);
            }

            Test.status_cd = "NEW";
            Db.Test.Add(Test);
            Db.Tests.Add(Test);

            Db.SaveChanges();

            DialogResult = true;
            Close();
        }
    }
}
