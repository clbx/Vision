using Vision.Mobs;
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

        PlayerHandler playerHandler = new PlayerHandler();
        HarvestableHandler harvestableHandler = new HarvestableHandler();
        MobsHandler mobsHandler = new MobsHandler();


        public Form1()
        {
            InitializeComponent();
            Settings.loadSettings(this);
    
        }

        public static Bitmap RotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                //move rotation point to center of image
                g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
                //rotate
                g.RotateTransform(angle);
                //move image back
                g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
                //draw passed in image onto graphics object
                g.DrawImage(b, new Point(0, 0));
            }
            return returnBitmap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateSettings();
            try
            {
                _eventHandler = new PacketHandler(playerHandler, harvestableHandler, mobsHandler);
                photonPacketHandler = new PhotonPacketHandler(_eventHandler);

                Thread t = new Thread(() => createListener());
                t.Start();

                Thread d = new Thread(() => drawerThread());
                d.Start();
            }
            catch (Exception ea)
            {
                Console.WriteLine(ea.ToString());
                //while (true) ;
            }
        }
        public static Double Distance(Single x1, Single x2, Single y1, Single y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        private void drawerThread()
        {
            Color[] mapColors =  {
                    Color.White,
                    Color.Black,
                    Color.Red,
                    Color.Coral,
                    Color.Black,
                    Color.Gray,
                    Color.Blue,
                    Color.Red,
                    Color.Coral,
                    Color.Goldenrod,
                    Color.Silver,
                    Color.Blue,
                    Color.Green,
                    Color.Purple
                };
            Pen linePen = new Pen(Color.Red, 3);
            Brush[] harvestBrushes = {
                Brushes.Black,
                Brushes.Gray,
                Brushes.Gray,
                Brushes.Gray,
                Brushes.Blue,
                Brushes.Red,
                Brushes.Coral,
                Brushes.Goldenrod,
                Brushes.Silver
            };
            Brush[] fontPerColor = {
                Brushes.White,
                Brushes.White,
                Brushes.White,
                Brushes.White,
                Brushes.White,
                Brushes.White,
                Brushes.Black,
                Brushes.Black,
                Brushes.Black

            };
            Pen[] chargePen = {
                new Pen (Color.Black, 1.5f),
                new Pen (Color.Green, 1.5f),
                new Pen(Color.Blue, 1.5f),
                new Pen(Color.Purple, 1.5f),
            };
            Pen playerPen = new Pen(Color.Red, 2f);
            Brush playerBrush = Brushes.Red;
            Brush mobBrush = Brushes.Black;

            int HEIGHT, WIDTH, MULTIPLER = 4;
            Bitmap bitmap = new Bitmap(500, 500);
            bitmap.SetResolution(100, 100);
            HEIGHT = 500;
            WIDTH = 500;
            Single lpX;
            Single lpY;
            Font font = new Font("Arial", 3, FontStyle.Bold);

            float scale = 4.0f;
            //if()
            //mapForm.SetBitmap(bitmap);
            while (true)
            {
                bitmap = new Bitmap(500, 500);
                using (Graphics g = Graphics.FromImage(bitmap))
                {

                    g.Clear(Color.Transparent);
                    lpX = playerHandler.localPlayerPosX();
                    lpY = playerHandler.localPlayerPosY();

                    g.TranslateTransform(WIDTH / 2, HEIGHT / 2);
                    g.FillEllipse(Brushes.Black, -2, -2, 4, 4);
                    g.DrawEllipse(linePen, -80, -80, 160, 160);
                    g.DrawEllipse(linePen, -170, -170, 340, 340);
                    g.DrawEllipse(linePen, -WIDTH / 2 + 6, -HEIGHT / 2 + 6, WIDTH - 6, HEIGHT - 6);

                    g.ScaleTransform(scale, scale);

                    List<Harvestable> hLis = new List<Harvestable>();
                    lock (harvestableHandler)
                    {
                        try
                        {
                            hLis = this.harvestableHandler.HarvestableList.ToList();
                        }
                        catch (Exception e1) { }
                    }
                    foreach (Harvestable h in hLis)
                    {


                        if (h.Size == 0) continue;

                        Single hX = -1 * h.PosX + lpX;
                        Single hY = h.PosY - lpY;

                        g.FillEllipse(harvestBrushes[h.Tier], (float)(hX - 2.5f), (float)(hY - 2.5f), 5f, 5f);
                        g.TranslateTransform(hX, hY);
                        g.RotateTransform(135f);
                        g.DrawString(h.getMapInfo(), font, fontPerColor[h.Tier], -2.5f, -2.5f);
                        g.RotateTransform(-135f);
                        g.TranslateTransform(-hX, -hY);


                        if (h.Charges > 0) g.DrawEllipse(chargePen[h.Charges], hX - 3, hY - 3, 6, 6);
                    }

                    /*
                    if (Settings.DisplayPeople)
                    {
                        List<Player> pLis = new List<Player>();
                        lock (this.playerHandler.PlayersInRange)
                        {
                            try
                            {
                                pLis = this.playerHandler.PlayersInRange.ToList();
                            }
                            catch (Exception e2) { }
                        }

                        foreach (Player p in pLis)
                        {
                            Single hX = -1 * p.PosX + lpX;
                            Single hY = p.PosY - lpY;
                            g.FillEllipse(playerBrush, hX, hY, 2, 2);
                            g.TranslateTransform(hX, hY);
                            g.RotateTransform(135f);
                            Font font2 = new Font("Arial", 2, FontStyle.Regular);
                            g.DrawString(p.Nickname, font2, Brushes.White, 1, -2);
                            g.RotateTransform(-135f);
                            g.TranslateTransform(-hX, -hY);
                        }
                    }*/

                    List<Mob> mList = new List<Mob>();
                    lock (mobsHandler.MobList)
                    {
                        try
                        {
                            mList = this.mobsHandler.MobList.ToList();
                        }
                        catch (Exception e1) { }
                    }

                    foreach (Mob m in mList)
                    {
                        Single hX = -1 * m.PosX + lpX;
                        Single hY = m.PosY - lpY;

                        byte mobTier = 5;
                        MobType mobType = MobType.OTHER;

                        if (m.MobInfo != null)
                        {
                            mobTier = m.MobInfo.Tier;
                            mobType = m.MobInfo.MobType;

                            switch (m.MobInfo.HarvestableMobType)
                            {
                                case HarvestableMobType.ESSENCE:
                                //idk?
                                case HarvestableMobType.SWAMP:
                                    if (!Settings.IsInHarvestable(HarvestableType.FIBER)) continue;
                                    break;
                                case HarvestableMobType.STEPPE:
                                    if (!Settings.IsInHarvestable(HarvestableType.HIDE)) continue;
                                    break;
                                case HarvestableMobType.MOUNTAIN:
                                    if (!Settings.IsInHarvestable(HarvestableType.ORE)) continue;
                                    break;
                                case HarvestableMobType.FOREST:
                                    if (!Settings.IsInHarvestable(HarvestableType.WOOD)) continue;
                                    break;
                                case HarvestableMobType.HIGHLAND:
                                    if (!Settings.IsInHarvestable(HarvestableType.ROCK)) continue;
                                    break;
                            }
                            if (Settings.IsInTiers(mobTier, m.EnchantmentLevel))
                            {
                                g.FillEllipse(harvestBrushes[mobTier], (float)(hX - 2.5f), (float)(hY - 2.5f), 5f, 5f);
                                g.TranslateTransform(hX, hY);
                                g.RotateTransform(135f);
                                g.DrawString(m.getMapStringInfo(), font, fontPerColor[mobTier], -2.5f, -2.5f);
                                g.RotateTransform(-135f);
                                g.TranslateTransform(-hX, -hY);
                            }
                        }
                        else
                        {
                            if (Settings.IsInMobs(MobType.OTHER))
                                g.FillEllipse(Brushes.Black, hX - 1, hY - 1, 2f, 2f);
                            continue;
                        }

                        if (m.EnchantmentLevel > 0)
                        {
                            g.DrawEllipse(chargePen[m.EnchantmentLevel], hX - 3, hY - 3, 6, 6);
                        }
                    }
                }
                //    Thread.Sleep(10);
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
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;


        protected override void OnPaintBackground(PaintEventArgs e)
        // Paint background with underlying graphics from other controls
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }


        private void cbSounds_CheckedChanged(object sender, EventArgs e)
        {
            updateSettings();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void MoveRadarValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(nRadarX.Value + " " + nRadarY.Value);
            updateSettings();
        }

    }
}
