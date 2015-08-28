using System;
using System.Windows;
using System.Windows.Controls;
using ShakeGestures;
using System.Windows.Media;
using ImageTools.IO.Gif;
using System.Windows.Threading;

namespace WakeMeUp.Alarms.AlarmGame.Shake
{
    public enum GameLevel
    {
        Easy, Medium, Hard
    }

    public partial class ShakeGameControl : UserControl
    {
        #region Property
        public Uri ImageSource { get; set; }

        //Min number of shake needed to raise a shake event
        private int NumofMinShake;
        //Amount of height increase when a shake event is raised
        private int NumofFillperShake;

        //the higher level, the harder and stronger shake needed
        private GameLevel _level;
        public GameLevel Level
        {
            private set
            {
                _level = value;

                NumofMinShake = (int)_level + 1;
                NumofFillperShake = 45 - (int)_level * 15;
                ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = NumofMinShake;
            }
            get
            {
                return _level;
            }
        }
        #endregion

        public ShakeGameControl()
        {
            InitializeComponent();

            ImageSource = new Uri("/Assets/wave.gif", UriKind.Relative);
            ImageTools.IO.Decoders.AddDecoder<GifDecoder>();

            this.Loaded += ShakeGameControl_Loaded;
            this.Unloaded += ShakeGameControl_Unloaded;

            LayoutRoot.DataContext = this;
            //NotificationTxt.Foreground = new SolidColorBrush(Global.settingsManager.GetColor());
        }

        public void StartShakeDetection(GameLevel level)
        {
            Level = level;

            ProgressTxt.Text = "0 %";
            WaveImg.Height = 20;
            // start shake detection
            ShakeGesturesHelper.Instance.ShakeGesture += Instance_ShakeGesture;
            ShakeGesturesHelper.Instance.Active = true;
        }

        #region Frame Event
        private void ShakeGameControl_Loaded(object sender, RoutedEventArgs e)
        {
            //ShakeAnimation.RepeatBehavior = new System.Windows.Media.Animation.RepeatBehavior(3);
            ShakeStoryboard.Begin();
        }

        private void ShakeGameControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ShakeGesturesHelper.Instance.ShakeGesture -= Instance_ShakeGesture;
        }
        #endregion

        #region Shake Event
        private void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                double h = WaveImg.Height;

                //Calculate progress in percent
                int progress = (int)(((h - 20) / BarRect.Height) * 100);
                ProgressTxt.Text = progress.ToString() + " %";

                //Stop the animation
                FillStoryboard.Stop();
                //Full Shake 
                if (h >= BarRect.Height)
                {
                    WaveImg.Height = BarRect.Height;
                    ProgressTxt.Text = "100 %";
                    ShakeGesturesHelper.Instance.Active = false;

                    //Wait about 400ms more and raise Full Shake event
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(400);
                    timer.Tick += timer_Tick;
                    timer.Start();

                    return;
                }
                //Setup and start animation
                FillAnimation.From = h;
                FillAnimation.By = NumofFillperShake;
                FillStoryboard.Begin();
            });
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            if (ShakeFullEvent != null)
            {
                ShakeFullEvent(this);
            }
        }
        #endregion

        #region Event
        public event ShakeFullHandler ShakeFullEvent;
        public delegate void ShakeFullHandler(object sender);
        #endregion
    }
}
