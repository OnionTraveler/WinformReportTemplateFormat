using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportTemplateFormat.ChildForms
{
    public partial class FormReport1 : Form
    {
        public FormReport1()
        {
            InitializeComponent();
            CollapseMenu(this.icnbtnMenu.Size.Width);
        }

        private void icnbtnMenuConditions_Click(object sender, EventArgs e)
        {
            CollapseMenu(icnbtnMenu.Size.Width);
        }

        private void CollapseMenu(int btnMenuWidth)
        {
            if (this.pnlMenu.Width > btnMenuWidth + 50)  // Menu縮小
            {
                //icnbtnMenu.IconChar = FontAwesome.Sharp.IconChar.ChevronCircleRight;
                icnbtnMenu.IconChar = FontAwesome.Sharp.IconChar.StepForward;
                this.pnlMenu.Width = btnMenuWidth + 14;
                lblStatements.Visible = false;
                pnlHorizontalLine.Visible = false;
                pnlMenu.BackColor = Color.FromArgb(234, 255, 255);
                icnbtnMenu.Dock = DockStyle.Left;
                //foreach (Button btnButtonsInMenu in pnlMenu.Controls.OfType<Button>())
                //{
                //    btnButtonsInMenu.Text = "";
                //    btnButtonsInMenu.ImageAlign = ContentAlignment.MiddleCenter;
                //    btnButtonsInMenu.Padding = new Padding(0);
                //}
            }
            else  // Menu展開
            {
                //icnbtnMenu.IconChar = FontAwesome.Sharp.IconChar.ChevronCircleLeft;
                icnbtnMenu.IconChar = FontAwesome.Sharp.IconChar.StepBackward;
                this.pnlMenu.Width = 250;
                lblStatements.Visible = true;
                pnlHorizontalLine.Visible = true;
                pnlMenu.BackColor = Color.FromArgb(255, 255, 255);
                icnbtnMenu.Dock = DockStyle.Right;
                //foreach (Button btnButtonsInMenu in pnlMenu.Controls.OfType<Button>())
                //{
                //    btnButtonsInMenu.Size = new Size(btnMenuWidth, btnButtonsInMenu.Size.Height);
                //    btnButtonsInMenu.Text = $"  {btnButtonsInMenu.Tag.ToString()}";
                //    btnButtonsInMenu.ImageAlign = ContentAlignment.MiddleLeft;
                //    btnButtonsInMenu.Padding = new Padding(10, 0, 0, 0);
                //}
            }
        }
    }
}
