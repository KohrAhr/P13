using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Call2020
{
    public partial class Form1 : Form
    {
        private const string CONST_SERVER_1 = "sdk.1click.lv";
        private const string CONST_SERVER_2 = "1click.lv";
        private const string CONST_SERVER_3 = "x2020.1click.lv";

        public bool estproblema = false;

        // 1 sec.
        private const int CONST_BALLON_DISPLAY_MS = 1000;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     https://stackoverflow.com/questions/11800958/using-ping-in-c-sharp
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void ping(string ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply replyPing;
                replyPing = ping.Send(ip);

                if (replyPing.Status == IPStatus.Success)
                {
                    //lblResult.Text = "Ping to " + s.ToString() + "[" + r.Address.ToString() + "]" + " Successful"
                    //   + " Response delay = " + r.RoundtripTime.ToString() + " ms" + "\n";

                    if (estproblema)
                    {
                        ShowGreenOnly();
                    }
                    estproblema = false;
                }
                else
                {
                    estproblema = true;
                    ShowRedOnly();
                }
            }
            catch
            {
                ShowRedOnly();
                estproblema = true;
            }
        }

        private void WaitNotLessThan(int value)
        {
            const int CONST_TICK = 50;
            int c = value;

            while (c > 0)
            {
                System.Threading.Thread.Sleep(CONST_TICK);
                c -= CONST_TICK;

                // https://stackoverflow.com/questions/34613362/what-does-application-processmessage-do
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     проверка что пароль правильный
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsPasswordValid(string value)
        {
            return value == "2020" || value == "100500" || value == "666";
        }

        private void ShowGreenOnly()
        {
            notifyIcon3.Visible = false;
            notifyIcon2.Visible = false;
            notifyIcon1.Visible = true;
        }

        private void ShowYellowOnly()
        {
            notifyIcon3.Visible = false;
            notifyIcon2.Visible = true;
            notifyIcon1.Visible = false;
        }

        private void ShowRedOnly()
        {
            notifyIcon3.Visible = true;
            notifyIcon2.Visible = false;
            notifyIcon1.Visible = false;
        }

        /// <summary>
        ///     Основная программа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Verify PIN
            if (IsPasswordValid(textBox1.Text))
            {
                // https://www.c-sharpcorner.com/UploadFile/f9f215/how-to-minimize-your-application-to-system-tray-in-C-Sharp/
                // Hide main form
                Hide();

                // Show green Tray icon and hide Yellow
                ShowGreenOnly();

                // Run ping Cycle 
                while (true)
                {
                    // Ping to 1 IP
                    ping(CONST_SERVER_1);
                    // ждём 1 sec
                    WaitNotLessThan(1000);

                    // Ping to 2 IP
                    ping(CONST_SERVER_2);
                    // ждём 1 sec
                    WaitNotLessThan(1000);

                    // Ping to 3 IP
                    ping(CONST_SERVER_3);
                    // ждём 1.5 sec
                    WaitNotLessThan(1500);

                    // https://stackoverflow.com/questions/34613362/what-does-application-processmessage-do
                    Application.DoEvents();
                }
            }

            // Exit if wrong password. Exit code is 0.
            Application.Exit();
        }

        /// <summary>
        ///     Popup menu -- Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // https://stackoverflow.com/questions/13046019/winforms-application-exit-vs-environment-exit-vs-form-close
            // Force exit with Exit code 1.
            Environment.Exit(1);
        }
    }
}
