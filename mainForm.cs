using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MinecraftGameModel;

namespace MinecraftExternalCmd
{
    public partial class mainForm : Form
    {
        #region Graphics
        #region WinAPi Imports
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys _key);

        #endregion

        public mainForm()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private bool mouseDown;
        private Point lastLocation;

        private void PnlTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void PnlTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void PnlTitle_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void TimerOpacity_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1.0f)
            {
                this.Opacity += 0.1f;
                if (this.Opacity >= 1.0f)
                {
                    TimerOpacity.Enabled = false;
                }
            }
        }

        private void TimerOpacity2_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0.0f)
            {
                this.Opacity -= 0.1f;
                if (this.Opacity <= 0.0f)
                {
                    this.Opacity = 0.0f;
                    TimerOpacity2.Enabled = false;
                    this.Visible = false;
                }
            }
        }

        private void btnSend_MouseEnter(object sender, EventArgs e)
        {
            btnSend.Image = Properties.Resources.SendBtnMEnter;
        }

        private void btnSend_MouseLeave(object sender, EventArgs e)
        {
            btnSend.Image = Properties.Resources.SendBtn;
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rtb_TextChanged(object sender, EventArgs e)
        {
            rtb.SelectionStart = rtb.Text.Length;
            rtb.ScrollToCaret();
        }

        #endregion

        public void AppendText(string text, Color color, bool isNewLine = false)
        {
            rtb.SuspendLayout();
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;

            rtb.SelectionColor = color;
            rtb.AppendText(isNewLine ? $"{text}{ Environment.NewLine}" : text);
            rtb.SelectionColor = rtb.ForeColor;
            rtb.ScrollToCaret();
            rtb.ResumeLayout();
        }

        void printmsg(string txt, string type) 
        {
            switch (type) 
            {
                case "savecoords":
                    if (txt.Contains("%"))
                    {
                        AppendText(txt.Split('%')[0], Color.White);
                        AppendText(txt.Split('%')[1], Color.LightGreen);
                        AppendText(txt.Split('%')[2], Color.White);
                        AppendText(txt.Split('%')[3], Color.LightGreen);
                        AppendText(txt.Split('%')[4], Color.White);
                        AppendText(txt.Split('%')[5], Color.LightGreen);
                        AppendText(txt.Split('%')[6], Color.White,true);
                    }
                    else 
                    {
                        AppendText(txt, Color.Red, true);
                    }
                    break;
                case "loadcoords":
                    if (txt.Contains("%"))
                    {
                        AppendText(txt.Split('%')[0], Color.White);
                        AppendText(txt.Split('%')[1], Color.LightGreen);
                        AppendText(txt.Split('%')[2], Color.White);
                        AppendText(txt.Split('%')[3], Color.LightGreen);
                        AppendText(txt.Split('%')[4], Color.White);
                        AppendText(txt.Split('%')[5], Color.LightGreen);
                        AppendText(txt.Split('%')[6], Color.White, true);
                    }
                    else
                    {
                        AppendText(txt, Color.Red, true);
                    }
                    break;
                case "tp":
                    if (txt.Contains("%"))
                    {
                        AppendText(txt.Split('%')[0], Color.White);
                        AppendText(txt.Split('%')[1], Color.LightGreen);
                        AppendText(txt.Split('%')[2], Color.White);
                        AppendText(txt.Split('%')[3], Color.LightGreen);
                        AppendText(txt.Split('%')[4], Color.White);
                        AppendText(txt.Split('%')[5], Color.LightGreen);
                        AppendText(txt.Split('%')[6], Color.White, true);
                    }
                    else
                    {
                        AppendText(txt, Color.Red, true);
                    }
                    break;
                case "cmd":
                    AppendText("Commands:", Color.Gold,true);
                    AppendText("tp <x,y,z>", Color.White, true);
                    AppendText("savecoords", Color.White, true);
                    AppendText("loadcoords", Color.White, true);
                    AppendText("--------", Color.Gold, true);
                    break;
                default:
                    AppendText(txt, Color.Red, true);
                    break;
            }
        }

        void HandleEvents() 
        {
            string msg = string.Empty;
            string tmp = string.Empty;
            float[] coords = new float[3];
            if (!string.IsNullOrEmpty(msgText.Text))
            {
                switch (msgText.Text.Trim().ToLower().Split(' ')[0])
                {
                    case "savecoords":
                        msg = GameModel.SaveCoords();
                        printmsg(msg, "savecoords");
                        break;
                    case "loadcoords":
                        msg = GameModel.LoadCoords();
                        printmsg(msg, "loadcoords");
                        break;
                    case "tp":
                        tmp = msgText.Text.Trim().ToLower();
                        if (!MinecraftMemory.mem.AttachProcess("Minecraft.Windows"))
                        {
                            msg = "Game Not Found!";
                        }
                        else if (MinecraftMemory.mem.AttachProcess("Minecraft.Windows") && tmp.Split(' ').Length != 4)
                        {
                            msg = "Wrong Coordinates!";
                        }
                        else if ((float.TryParse(tmp.Split(' ')[1].Insert(tmp.Split(' ')[1].Length, ",0"), out coords[0])) &&
                            (float.TryParse(tmp.Split(' ')[2].Insert(tmp.Split(' ')[2].Length, ",0"), out coords[1])) &&
                            (float.TryParse(tmp.Split(' ')[3].Insert(tmp.Split(' ')[3].Length, ",0"), out coords[2])))
                        {
                            msg = GameModel.Teleport(coords);
                        }
                        printmsg(msg, "tp");
                        break;
                    case "commands":
                        printmsg(null, "cmd");
                        break;
                    default:
                        msg = "Unknown Command!";
                        printmsg(msg, null);
                        break;
                }
                msgText.Text = string.Empty;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            HandleEvents();
        }

        private void msgText_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode== Keys.Enter) 
            {
                e.SuppressKeyPress = true;
                HandleEvents();
            }
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            bgw.RunWorkerAsync();
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true) 
            {
                if (GetAsyncKeyState(Keys.F2) != 0) 
                {
                    while (GetAsyncKeyState(Keys.F2) != 0) ;
                    if (this.Visible == true)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            TimerOpacity2.Enabled = true;
                        }));
                    }
                    else 
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.Opacity = 0.0f;
                            this.Visible = true;
                            TimerOpacity.Enabled = true;
                        }));
                    }
                   
                }
                //Unreachable code
                System.Threading.Thread.Sleep(50);
            }
        }

    }
}
