using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TestShakeGesture.Resources;
using ImageTools.IO.Gif;
using ShakeGestures;
using System.Windows.Threading;
using WakeMeUp.Alarms.AlarmGame.Shake;

namespace TestShakeGesture
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Properties

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            ShakeControl.ShakeFullEvent += ShakeControl_ShakeFullEvent;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShakeControl.StartShakeDetection(GameLevel.Easy);
        }

        private void ShakeControl_ShakeFullEvent(object sender)
        {
            MessageBox.Show("Full Shake");
        }
    }
}