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

        /// <summary>
        ///     таймер без тормозов в UI
        /// </summary>
        /// <param name="value"></param>
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

        private void _ShowOnly(int id)
        {
            notifyIcon1.Icon = Icon.FromHandle(((Bitmap)imageList1.Images[id]).GetHicon());
        }

        private void ShowGreenOnly()
        {
            _ShowOnly(1);
        }

        private void ShowYellowOnly()
        {
            _ShowOnly(0);
        }

        private void ShowRedOnly()
        {
            _ShowOnly(2);
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

                // Show green Tray icon
                ShowGreenOnly();

                // Сообщение приветствие
                ShowBalloonMessage("Go!");

                // Начинаем наш цикл ping-ов 
                while (true)
                {
                    // Ping to 1 IP
                    ping(CONST_SERVER_1);
                    // ждём 1.5 sec
                    WaitNotLessThan(1500);

                    // Ping to 2 IP
                    ping(CONST_SERVER_2);
                    // ждём 1.5 sec
                    WaitNotLessThan(1500);

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
        ///     Показываем Balloon сообщение
        /// </summary>
        /// <param name="value"></param>
        private void ShowBalloonMessage(string value)
        {
            notifyIcon1.BalloonTipText = value;
            notifyIcon1.ShowBalloonTip(CONST_BALLON_DISPLAY_MS);
            notifyIcon1.BalloonTipText = "";
        }

        /// <summary>
        ///     Popup menu -- Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // последнее сообщение
            ShowBalloonMessage("Bye");

            // гарантированно прячем, а то иногда может оставаться
            notifyIcon1.Visible = false;

            // Force exit with Exit code 1.
            // https://stackoverflow.com/questions/13046019/winforms-application-exit-vs-environment-exit-vs-form-close
            Environment.Exit(1);
        }
    }
}
