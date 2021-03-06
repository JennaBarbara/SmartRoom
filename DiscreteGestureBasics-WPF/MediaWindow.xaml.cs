﻿using System;
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
using System.Windows.Navigation;


namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    /// <summary>
    /// Interaction logic for MediaWindow.xaml
    /// </summary>
    public partial class MediaWindow : Window
    {
        double timeElapsed;
        public MediaWindow()
        {
            InitializeComponent();
        }
        public void SetMedia(string location)
        {
            Uri locationUri = new Uri(location, UriKind.Absolute);
            Media.Source = locationUri;
        }
        public void PlayMedia()
        {
            Media.IsMuted = false;
            Media.Visibility = Visibility.Visible;
            Media.Play();
        }

        internal void StopMedia()
        {
            Media.Pause();
        }

        internal void SetVolume(double amt)
        {
            double volume;
            volume = Media.Volume + amt;
            if (volume>= 100)
            {
                volume = 99;
            }
            else if (volume < 0)
            {
                volume = 0;
            }
            Media.Volume = volume;
        }
    }
}
