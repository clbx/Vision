using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Vision
{
    public partial class Form1 : Form
    {

        PacketHandler _eventHandler;
        PhotonPacketHandler photonPacketHandler;
     



        public Form1()
        {
            InitializeComponent();
            Settings.loadSettings(this);
    
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateSettings();
            try
            {
                _eventHandler = new PacketHandler();
                photonPacketHandler = new PhotonPacketHandler(_eventHandler);

                Thread t = new Thread(() => createListener());
                t.Start();
            }
            catch (Exception ea)
            {
                Console.WriteLine(ea.ToString());
                //while (true) ;
            }
        }
        public string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        private void createListener()
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            if (allDevices.Count == 0)
            {
                MessageBox.Show("No interfaces found! Make sure WinPcap is installed.");
                return;
            }
            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];

                if (device.Description != null)
                    Console.WriteLine(" (" + device.Description + ")");
                else
                    Console.WriteLine(" (No description available)");
            }

            foreach (PacketDevice selectedDevice in allDevices.ToList())
            {
                // Open the device
                Thread t = new Thread(() =>
                {
                    using (PacketCommunicator communicator =
                           selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                                       // 65536 guarantees that the whole packet will be captured on all the link layers
                                               PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                               1000))                                  // read timeout
                    {
                        // Check the link layer. We support only Ethernet for simplicity.
                        if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                        {
                            Console.WriteLine("This program works only on Ethernet networks.");
                            return;
                        }

                        // Compile the filter
                        using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and udp"))
                        {
                            // Set the filter
                            communicator.SetFilter(filter);
                        }

                        Console.WriteLine("Listening on " + selectedDevice.Description + "...");

                        // start the capture
                        communicator.ReceivePackets(0, photonPacketHandler.PacketHandler);

                    }
                });
                t.Start();
            }

        }

        private void updateSettings()
        {
            
        }
        private void tierCheckChange(object sender, EventArgs e)
        {
            updateSettings();
        }
        private void harvestableCheckChange(object sender, EventArgs e)
        {
            updateSettings();

        }

        private void cbDisplayPeople_CheckedChanged(object sender, EventArgs e)
        {
        }
        private MouseClickMessageFilter Filter;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        //private const UInt32 SWP_NOSIZE = 0x0001;
        //private const UInt32 SWP_NOMOVE = 0x0002;
        //private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

    }
}
