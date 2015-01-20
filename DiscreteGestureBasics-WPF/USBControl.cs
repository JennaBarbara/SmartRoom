using PowerUSB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//username:pwrusb
//password:ADS5WZ04ZM8Q
namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    public class USBControl
    {
        public static void init()
        {
            int p1, p2, p3;
            bool pause;
            int model, pwrUsbConnected=0;
            StringBuilder firmware = new StringBuilder(128);
            p1=p2=p3=0;
            
            Console.WriteLine("Inializing PowerUSB");
        if (PwrUSBWrapper.InitPowerUSB(out model, firmware) > 0)
        {
            Console.Write("PowerUSB Connected. Model:{0:D}  Firmware:", model);
            Console.WriteLine(firmware);
            pwrUsbConnected = PwrUSBWrapper.CheckStatusPowerUSB();
            PwrUSBWrapper.SetPortPowerUSB(p1, p2, p3);
        }
        else
            Console.WriteLine("PowerUSB could not be initialized");
        }
        
    }
}
