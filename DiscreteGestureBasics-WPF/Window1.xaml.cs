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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
        }

        internal void SetState(int state)
        {
            if(state ==0)
            {
                StateName.Content = "Base State";
            }
            else if(state ==1)
            {
                StateName.Content = "Temperature Control";
            }
            else if (state == 2)
            {
                StateName.Content = "Music Control";
            }
            else if (state == 3)
            {
                StateName.Content = "Video Control";
            }
            else if (state == 4)
            {
                StateName.Content = "Control Inactive";
            }
        }

        internal void SetTemp(int temp)
        {
            TempValue.Content = temp;
        }


    }
}
