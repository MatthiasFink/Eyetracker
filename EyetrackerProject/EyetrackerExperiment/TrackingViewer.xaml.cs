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
    /// Interaktionslogik für TrackingViewer.xaml
    /// </summary>
    public partial class TrackingViewer : Window
    {
        private Test test;
        public Test Test { get { return test; } set { test = value; SlideNum = Test.Test_Definition.Slide.Min(s => s.num); } }
        private int slideNum;
        public int SlideNum { get { return slideNum; }
            set
            {
                Slide slide = test.Test_Definition.Slide.FirstOrDefault(s => s.num == value);
                if (slide == null) return;
                slideNum = value;
                CurrentSlide.ImageSource = slide.SlideBitmapImage;
                tbSlideNum.Text = String.Format("Slide {0} of {1}", slideNum, Test.Test_Definition.Slide.Count);

                Track.Data = null;
                if (Test.Tracking.Count(t => t.Slide == slide) > 0)
                {
                    PathFigure figure = new PathFigure();
                    Point max = new Point() { X=0, Y=0};
                    Point min = new Point() { X = 1E10, Y = 1E10};

                    foreach (Tracking tracking in Test.Tracking.Where(t => t.Slide == slide).OrderBy(t => t.occurred))
                    {
                        if (tracking.x == 0 && tracking.y == 0)
                            continue;

                        Point p = new Point() { X = (double)tracking.x, Y = (double)tracking.y };

                        if (p.X < min.X) min.X = p.X;
                        if (p.Y < min.Y) min.Y = p.Y;
                        if (p.X > max.X) max.X = p.X;
                        if (p.Y > max.Y) max.Y = p.Y;

                        if (figure.StartPoint.X == 0 && figure.StartPoint.Y == 0)
                            figure.StartPoint = p;
                        else
                            figure.Segments.Add(new LineSegment(p, true));
                    }
                    Point scale = new Point(
                        SlideView.ActualWidth * 0.9 / (max.X - min.X),
                        SlideView.ActualHeight * 0.7 / (max.Y - min.Y));

                    MatrixTransform m = new MatrixTransform(
                        scale.X, 0,
                        0, scale.Y,
                        -min.X * scale.X * 0.95, -min.Y * scale.Y * 0.9);

                    figure.StartPoint = m.Transform(figure.StartPoint);
                    foreach (PathSegment ps in figure.Segments)
                        ((LineSegment)ps).Point = m.Transform(((LineSegment)ps).Point);

                    PathGeometry geo = new PathGeometry();
                    geo.Figures.Add(figure);
                    Track.Data = geo;
                }
            }
        }
        public TrackingViewer()
        {
            InitializeComponent();
            Show();
        }

        public TrackingViewer(Test test)
        {
            InitializeComponent();
            Test = test;
            SlideNum = Test.Test_Definition.Slide.Min(s => s.num);
            Show();
        }

        private void goNextSlide(object sender, RoutedEventArgs e) { SlideNum++; }
        private void goPrevSlide(object sender, RoutedEventArgs e) { SlideNum--; }

    }
}
