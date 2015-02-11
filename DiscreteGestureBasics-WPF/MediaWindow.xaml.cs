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

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    /// <summary>
    /// Interaction logic for MediaWindow.xaml
    /// </summary>
    public partial class MediaWindow : Window
    {
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

            Console.WriteLine(Media.IsVisible+ " " +Media.IsLoaded);
            
        }

        internal void StopMedia()
        {
            Media.Stop();
        }
    }
}
