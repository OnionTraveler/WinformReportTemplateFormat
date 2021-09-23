using CustomControls.RJControls;
using ReportTemplateFormat.ChildForms;
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

namespace ReportTemplateFormat
{
    public partial class MainForm : Form
    {
        //Fields
        private int iBorderSize = 2;
        private Size szFormSize; //Keep form size when it is minimized and restored.Since the form is resized because it takes into account the size of the title bar and borders.
        private Form fCurrentChildForm;
        public MainForm()
        {

            InitializeComponent();
            CollapseMenu(this.icnbtnMenu.Size.Width);
            this.Padding = new Padding(iBorderSize);  // Border Size
            //this.BackColor = Color.FromArgb(98, 102, 244);  // Border Color
            this.BackColor = Color.FromArgb(21, 21, 34);  // Border Color

            vInitialDropDownMenu();
        }

        // Drag Form
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);


        private void pnlTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // Override Methods

        /// <summary>
        /// 把Form的預設邊框(FormBorderStyle)移除，同時當窗體最大化時，不會遮蔽下方的工具列
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //// 使得窗體放到最大的時候，不會遮蔽下方的工具列
            //const int WM_NCCALCSIZE = 0x0083;
            //if (m.Msg == WM_NCCALCSIZE  // 把Form的預設邊框(FormBorderStyle)移除
            //    && m.WParam.ToInt32() == 1  // 當窗體最大化時，不會遮蔽下方的工具列
            //    ) return;

            //base.WndProc(ref m);


            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;
            #region Form Resize
            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right
            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>
            if (m.Msg == WM_NCHITTEST)
            { //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                {
                    if ((int)m.Result == HTCLIENT)//If the result of the m (mouse pointer) is in the client area of the window
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          
                        if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                m.Result = (IntPtr)HTBOTTOM;
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            //Remove border and keep snap window
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }


            ////Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            //if (m.Msg == WM_SYSCOMMAND)
            //{
            //    /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
            //    /// Quote:
            //    /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
            //    /// are used internally by the system.To obtain the correct result when testing 
            //    /// the value of wParam, an application must combine the value 0xFFF0 with the 
            //    /// wParam value by using the bitwise AND operator.
            //    int wParam = (m.WParam.ToInt32() & 0xFFF0);
            //    if (wParam == SC_MINIMIZE)  //Before
            //        formSize = this.ClientSize;
            //    if (wParam == SC_RESTORE)// Restored form(Before)
            //        this.Size = formSize;
            //}


            base.WndProc(ref m);
        }



        private void MainForm_Resize(object sender, EventArgs e)
        {
            AdjustForm();
        }

        private void AdjustForm()
        {
            switch (this.WindowState)
            {
                case FormWindowState.Normal:
                    if (this.Padding.Top != iBorderSize)
                        this.Padding = new Padding(iBorderSize);
                    break;
                //case FormWindowState.Minimized:
                //    break;
                case FormWindowState.Maximized:
                    this.Padding = new Padding(8, 8, 8, 8);
                    break;
                default:
                    break;
            }
        }

        private void vFormMaximize()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                szFormSize = this.ClientSize;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = szFormSize;
            }

        }

        private void icnbtnMaximize_Click(object sender, EventArgs e)
        {
            vFormMaximize();
        }

        private void icnbtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void icnbtnClosedApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void icnbtnMenu_Click(object sender, EventArgs e)
        {
            CollapseMenu(icnbtnMenu.Size.Width);
        }

        private void CollapseMenu(int btnMenuWidth)
        {
            if (this.pnlMenu.Width > btnMenuWidth + 50)  // Menu縮小
            {
                icnbtnMenu.IconChar = FontAwesome.Sharp.IconChar.ArrowAltCircleRight;
                this.pnlMenu.Width = btnMenuWidth;
                picbxMenuTitle.Visible = false;
                icnbtnMenu.Dock = DockStyle.Left;
                foreach (Button btnButtonsInMenu in pnlMenu.Controls.OfType<Button>())
                {
                    btnButtonsInMenu.Text = "";
                    btnButtonsInMenu.ImageAlign = ContentAlignment.MiddleCenter;
                    btnButtonsInMenu.Padding = new Padding(0);
                }
            }
            else  // Menu展開
            {
                icnbtnMenu.IconChar = FontAwesome.Sharp.IconChar.ArrowAltCircleLeft;
                this.pnlMenu.Width = 185;
                picbxMenuTitle.Visible = true;
                icnbtnMenu.Dock = DockStyle.Right;
                foreach (Button btnButtonsInMenu in pnlMenu.Controls.OfType<Button>())
                {
                    btnButtonsInMenu.Size = new Size(btnMenuWidth, btnButtonsInMenu.Size.Height);
                    btnButtonsInMenu.Text = $"  {btnButtonsInMenu.Tag.ToString()}";
                    btnButtonsInMenu.ImageAlign = ContentAlignment.MiddleLeft;
                    btnButtonsInMenu.Padding = new Padding(10, 0, 0, 0);
                }
            }
        }



        private void Open_DropdownMenu(RJDropdownMenu DropdownMenu, object sender)
        {
            Control control = (Control)sender;
            DropdownMenu.VisibleChanged += new EventHandler((sender2, ev)
                => DropdownMenu_VisibleChanged(sender2, ev, control)
                );
            DropdownMenu.Show(control, control.Width, 0);
        }

        private void DropdownMenu_VisibleChanged(object sender, EventArgs ev, Control ctrl)
        {
            RJDropdownMenu DropdownMenu = (RJDropdownMenu)sender;
            if (!DesignMode)
            {
                if (DropdownMenu.Visible)
                    //ctrl.BackColor = Color.FromArgb(159, 161, 224);
                    ctrl.BackColor = Color.FromArgb(191, 192, 234);
                else
                    //ctrl.BackColor = Color.FromArgb(98, 102, 244);
                    ctrl.BackColor = Color.FromArgb(21, 21, 34);
            }
        }

        private void vInitialDropDownMenu()
        {
            this.ddmReport.IsMainMenu = true;
            this.ddmReport.PrimaryColor = Color.FromArgb(191, 192, 234);

            this.ddmSetup.IsMainMenu = true;
            this.ddmSetup.PrimaryColor = Color.FromArgb(191, 192, 234);
        }

        private void icnbtnReport_Click(object sender, EventArgs e)
        {
            Open_DropdownMenu(this.ddmReport, sender);
        }

        private void icnbtnSetup_Click(object sender, EventArgs e)
        {
            Open_DropdownMenu(this.ddmSetup, sender);
        }

        private void icnbtnHome_Click(object sender, EventArgs e)
        {
            vReset();
        }

        private void vReset()
        {
            if (fCurrentChildForm != null)
                // Only Open Form
                fCurrentChildForm.Close();

            string sIcnbtnHomeTag = icnbtnHome.Tag.ToString();
            lblTitleChildForm.Text = sIcnbtnHomeTag.Split(' ').Last<string>();
        }

        private void OpenChildForm(Form fChildForm)
        {
            if (fCurrentChildForm != null)
            {
                // Only Open Form
                fCurrentChildForm.Close();
            }

            fCurrentChildForm = fChildForm;
            fChildForm.TopLevel = false;
            fChildForm.FormBorderStyle = FormBorderStyle.None;
            fChildForm.Dock = DockStyle.Fill;
            pnlMainZone.Controls.Add(fChildForm);
            pnlMainZone.Tag = fChildForm;
            fChildForm.BringToFront();
            fChildForm.Show();

            lblTitleChildForm.Text = fChildForm.Text;
        }

        private void ddmItemReport_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormReport1());
        }


    }
}
