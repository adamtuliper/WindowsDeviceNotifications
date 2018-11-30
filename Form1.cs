using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeviceConnectionLog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DeviceNotification.RegisterDeviceNotification(this.Handle);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == DeviceNotification.WmDevicechange)
            {
                switch ((int)m.WParam)
                {
                    case DeviceNotification.DbtDeviceRemoveComplete:

                        Console.WriteLine(DeviceNotification.GetDeviceName(m.LParam));
                        // if (hdr.dbch_devicetype == DBT_DEVTYP_VOLUME)
                        // {
                        //     DEV_BROADCAST_VOLUME volume;
                        //     volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                        //     //Translate mask to device letter
                        //     driveLetter = DriveMaskToLetter(volume.dbcv_unitmask);
                        // }

                        // DEV_BROADCAST_HDR DevHdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));
                        // if (DevHdr.dbch_DeviceType == DBT_DEVTYP_DEVICEINTERFACE)
                        // {
                        //     DEV_BROADCAST_DEVICEINTERFACE DisplayDevIF = (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
                        //     String devName = new String(DisplayDevIF.dbcc_name);

                        // }
                        //m.LParam
                        //pointer to DEV_BROADCAST_HDR 

                        break;
                    case DeviceNotification.DbtDeviceArrival:
                        ///Usb_DeviceAdded(); // this is where you do your magic
                        Console.WriteLine(DeviceNotification.GetDeviceName(m.LParam));
                        break;
                }
            }
        }
    }

}

