using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Klassen zur Bereitstellung von Fragen eines Questionnaires mit initial leeren Antworten.
 * Unterscheidliche Fragetypen werden in eigene Suklassen aufgelöst, um das Templating in
 * WPF zu erleichtern.
 * Gleichzeitig werden so einfacher ereichbare Properties für die Beantwortung geschaffen.
 */
namespace Data
{
    public class QuestionnaireAnswer : ObservableCollection<BaseQuestionAnswer>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;
        public Test Test { get; }
        public bool IsReadOnly { get; set; }
        private Questionnaire Questionnaire { get; }

        // Argumentloser Konstruktor nur zur Deklaration als DataContext
        public QuestionnaireAnswer()
        { }

        public QuestionnaireAnswer(Test test, Questionnaire questionnaire)
        {
            Test = test;
            Questionnaire = questionnaire;
            foreach (Question q in questionnaire.Question.OrderBy(q => q.num))
                Add(BaseQuestionAnswer.NewQuestionAnswer(q, this));
            IsReadOnly = (NumQuestionsAnswered > 0);
        }

        public void OnAnswerChanged()
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs("NumQuestionsAnswered"));
        }

        public String Title { get { return Questionnaire == null ? "Questionnaire" : Questionnaire.Title + " - " + Test.Candidate.personal_code; } }

        public int NumQuestions { get { return this.Count(q => q.QuestionEntity.type_cd != "O"); } }
        public int NumQuestionsAnswered { get { return this.Count(q => q.AnswerEntity != null); } }

        public bool HasChanges(EyetrackerEntities db)
        {
            foreach (BaseQuestionAnswer qa in this.Where(qa => qa.QuestionEntity.type_cd != "O"))
            {
                if (qa.AnswerEntity != null && (db.Entry(qa.AnswerEntity) == null || db.Entry(qa.AnswerEntity).State != System.Data.Entity.EntityState.Unchanged))
                    return true;
            }
            return false;
        }

        public bool IsComplete()
        {
            foreach (BaseQuestionAnswer qa in this.Where(qa => qa.QuestionEntity.type_cd != "O"))
            {
                if (qa.AnswerEntity == null) 
                    return false;
            }
            return true;
        }

        public void SaveChanges(EyetrackerEntities db)
        {
            int countNew = 0;
            int countChanged = 0;
            int countMissing = 0;
            foreach (BaseQuestionAnswer qa in this.Where(qa => qa.QuestionEntity.type_cd != "O"))
            {
                if (qa.AnswerEntity != null)
                {
                    DbEntityEntry entry = null;

                    if (qa.AnswerEntity.id == 0)
                        entry = db.Entry(db.Answer.Add(qa.AnswerEntity));
                    else
                    {
                        entry = db.Entry(qa.AnswerEntity);
                        if (entry == null)
                            entry = db.Entry(db.Answer.Attach(qa.AnswerEntity));
                    }
                    switch (entry.State)
                    {
                        case EntityState.Modified: countChanged++; break;
                        case EntityState.Added: countNew++; break;
                    }
                }
                else countMissing++;
            }
            if (countNew + countChanged > 0)
            {
                if (Test.LastStep < Questionnaire.Step)
                {
                    Test.LastStep = Questionnaire.Step;
                    if (Test.NumSteps == Test.LastStep)
                    {
                        Test.end_time = DateTime.Now;
                        Test.status_cd = "TRM";
                    }
                    else if (Test.status_cd == "NEW")
                        Test.status_cd = "PRG";
                    db.SaveChanges();
                }
            }
        }
    }

    public class BaseQuestionAnswer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected QuestionnaireAnswer questionAnswerSet;
        protected Question q;
        protected Answer a;


        public Question QuestionEntity { get { return q; } }
        public Answer AnswerEntity { get { return a; } }
        public Questionnaire questionnaire { get { return q.Questionnaire; } set { q.Questionnaire = value; } }
        public decimal Num { get { return q.num; } set { q.num = value; } }
        public Test Test { get { return questionAnswerSet.Test; } }
        public String Question { get { return q.question1; } }

        public BaseQuestionAnswer() { }
        public BaseQuestionAnswer(Question question, QuestionnaireAnswer questA)
        {
            q = question;
            questionAnswerSet = questA;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
            questionAnswerSet.OnAnswerChanged();
        }

        public static BaseQuestionAnswer NewQuestionAnswer(Question question, QuestionnaireAnswer questionAnswerSet)
        {
            switch (question.type_cd)
            {
                case "O": return new OnlyCommentQuestionAnswer(question, questionAnswerSet);
                case "T": return new StringQuestionAnswer(question, questionAnswerSet);
                case "Y": return new YesNoQuestionAnswer(question, questionAnswerSet);
                case "C": return new ChoiceQuestionAnswer(question, questionAnswerSet);
                default: return null;
            }
        }
    }

    public class OnlyCommentQuestionAnswer : BaseQuestionAnswer
    {
        public OnlyCommentQuestionAnswer() { }
        public OnlyCommentQuestionAnswer(Question question, QuestionnaireAnswer questionAnswerSet)
            : base(question, questionAnswerSet)
        { }
    }


    public class StringQuestionAnswer : BaseQuestionAnswer
    {
        public StringQuestionAnswer() { }
        public StringQuestionAnswer(Question question, QuestionnaireAnswer questionAnswerSet)
            : base(question, questionAnswerSet)
        { }

        public String Answer
        {
            get
            {
                return (a == null) ? "" : a.answer1;
            }

            set
            {
                if (value == null)
                {
                    if (a != null)
                    {
                        a = null;
                        OnPropertyChanged("Answer");
                    }
                    return;
                }
                if (a == null)
                {
                    a = new Answer();
                    a.Test = questionAnswerSet.Test;
                    a.Question = q;
                }
                if (!value.Equals(a.answer1))
                {
                    a.answered = DateTime.Now;
                    if (q.correct_answer != null)
                        a.is_correct = q.correct_answer.Equals(value);
                    a.answer1 = value;
                    OnPropertyChanged("Answer");
                }
            }
        }
    }

    public class QuestionAnswerChoice
    {
        private ChoiceQuestionAnswer cqa;
        private Choice c;

        // Verwendet für Gruppierung von Radio Buttons 
        public String ChoiceGroupName { get { return "group" + cqa.Num; } }

        public String Shortcut { get { return c.shortcut; } }
        public String Label { get { return c.choice1; } }

        public QuestionAnswerChoice(ChoiceQuestionAnswer choiceQuestionAnswer, Choice choice)
        {
            cqa = choiceQuestionAnswer;
            c = choice;
        }

        // Helper for binding Radiobutton.IsChecked or IsSelected property of list items
        public bool IsSelected
        {
            get { return (cqa.AnswerChoice == c); }
            set { if (value) cqa.AnswerChoice = c; }
        }

    }

    public class YesNoQuestionAnswer : BaseQuestionAnswer
    {
        public YesNoQuestionAnswer() : base() { }

        public YesNoQuestionAnswer(Question question, QuestionnaireAnswer questionAnswerSet)
            : base(question, questionAnswerSet)
        { }

        public String YesNoGroupName { get { return "group" + q.num; } }

        public bool? AnswerYesNo {
            get
            {
                return (a == null) ? (bool?)null : a.answer1 == "Y";
            }
            set
            {
                if (!value.HasValue) 
                {
                    if (a != null)
                    {
                        a = null;
                        OnPropertyChanged("AnswerChoice");
                    }
                    return;
                }
                if (a == null)
                {
                    a = new Answer();
                    a.Question = q;
                    a.answered = DateTime.Now;
                    a.Test = questionAnswerSet.Test;
                }
                if ((value.Value ? "Y" : "N") != a.answer1)
                {
                    a.answer1 = value.Value ? "Y" : "N";
                    a.answered = DateTime.Now;
                    OnPropertyChanged("AnswerChoice");
                }
            }
        }
        public bool IsYes { get { return AnswerYesNo.HasValue && AnswerYesNo.Value; } set { if (value) AnswerYesNo = true; } }
        public bool IsNo { get { return AnswerYesNo.HasValue && !AnswerYesNo.Value; } set { if (value) AnswerYesNo = false; } }
    }

    public class ChoiceQuestionAnswer : BaseQuestionAnswer
    {
        private List<QuestionAnswerChoice> choices;

        public ChoiceQuestionAnswer() : base()
        {
            choices = new List<QuestionAnswerChoice>();
        }

        public ChoiceQuestionAnswer(Question question, QuestionnaireAnswer questionAnswerSet)
            : base(question, questionAnswerSet)
        {
            choices = new List<QuestionAnswerChoice>();
            foreach (Choice c in question.Choice.OrderBy(c => c.num))
                choices.Add(new QuestionAnswerChoice(this, c));
        }

        public List<QuestionAnswerChoice> Choices { get { return choices; } }

        public Choice AnswerChoice
        {
            get
            {
                return a == null ? null : a.Choice;
            }
            set
            {
                if (value == null)
                {
                    if (a != null)
                    {
                        a = null;
                        OnPropertyChanged("AnswerChoice");
                    }
                    return;
                }

                if (value.Question != q)
                    return;
                if (a == null)
                {
                    a = new Answer();
                    a.Test = questionAnswerSet.Test;
                    a.answered = DateTime.Now;
                    a.Question = q;
                    OnPropertyChanged("AnswerChoice");
                }
                if (a.Choice != value)
                {
                    a.answered = DateTime.Now;
                    a.is_correct = value.is_correct;
                    a.answer1 = value.shortcut;
                    a.Choice = value;
                    OnPropertyChanged("AnswerChoice");
                }
            }
        }
    }
}
