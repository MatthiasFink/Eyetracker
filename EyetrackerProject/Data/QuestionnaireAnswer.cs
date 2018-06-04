using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class QuestionnaireAnswer : ObservableCollection<BaseQuestionAnswer>
    {
        public Test Test { get; }
        private Questionnaire Questionnaire { get; }

        // Argumentloser Konstruktor nur zur Deklaration als DataContext
        public QuestionnaireAnswer()
        { }

        public QuestionnaireAnswer(Test test, Questionnaire questionnaire)
        {
            Test = test;
            Questionnaire = questionnaire;
            foreach (Question q in questionnaire.Question.OrderBy(q => q.num))
                Add(BaseQuestionAnswer.NewQuestionAnswer(q, test));
        }

        public String Title { get { return Questionnaire == null ? "Questionnaire" : Questionnaire.Title + " - " + Test.Candidate.personal_code; } }

        public int NumQuestions { get { return Count; } }
        public int NumQuestionsAnswered { get { return this.Count(q => q.AnswerEntity != null); } }
    }

    public class BaseQuestionAnswer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected Question q;
        protected Test t;
        protected Answer a;

        public Question QuestionEntity { get { return q; } }
        public Answer AnswerEntity { get { return a; } }
        public Questionnaire questionnaire { get { return q.Questionnaire; } set { q.Questionnaire = value; } }
        public decimal Num { get { return q.num; } set { q.num = value; } }
        public Test Test { get { return t; } set { t = value; } }
        public String Question { get { return q.question1; } }

        public BaseQuestionAnswer() { }
        public BaseQuestionAnswer(Question question, Test test)
        {
            q = question;
            t = test;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public static BaseQuestionAnswer NewQuestionAnswer(Question question, Test test)
        {
            switch (question.type_cd)
            {
                case "O": return new OnlyCommentQuestionAnswer(question, test);
                case "T": return new StringQuestionAnswer(question, test);
                case "C": return new ChoiceQuestionAnswer(question, test);
                default: return null;
            }
        }
    }

    public class OnlyCommentQuestionAnswer : BaseQuestionAnswer
    {
        public OnlyCommentQuestionAnswer() { }
        public OnlyCommentQuestionAnswer(Question question, Test test)
            : base(question, test)
        { }
    }


    public class StringQuestionAnswer : BaseQuestionAnswer
    {
        public StringQuestionAnswer() { }
        public StringQuestionAnswer(Question question, Test test)
            : base(question, test)
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
                    a.Test = t;
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

        public QuestionAnswerChoice(ChoiceQuestionAnswer choiceQuestionAnswer, Choice choice)
        {
            cqa = choiceQuestionAnswer;
            c = choice;
        }

        public bool IsSelected
        {
            get { return (cqa.AnswerChoice == c); }
            set { if (value) cqa.AnswerChoice = c; }
        }

        // Verwendet für Gruppierung von Radio Buttons 
        public String ChoiceGroupName { get { return "group" + cqa.Num; } }

        public String Shortcut { get { return c.shortcut; } }
        public String Label { get { return c.choice1; } }
    }

    public class ChoiceQuestionAnswer : BaseQuestionAnswer
    {
        private List<QuestionAnswerChoice> choices;

        public ChoiceQuestionAnswer() : base()
        {
            choices = new List<QuestionAnswerChoice>();
        }

        public ChoiceQuestionAnswer(Question question, Test test)
            : base(question, test)
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
                    a.Test = t;
                    a.Question = q;
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
