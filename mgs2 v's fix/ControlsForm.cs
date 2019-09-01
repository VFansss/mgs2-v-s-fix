using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mgs2_v_s_fix
{
    public partial class ControlsForm : Form
    {
        Form1 mainForm;

        public ControlsForm(Form1 parentFormInstance)
        {
            InitializeComponent();

            // Take note of the instance of the main form
            mainForm = parentFormInstance;

        }

        // Main methods

        internal void setFocusOnControlForm(bool focusOnControlForm)
        {
            Visible = focusOnControlForm;
            mainForm.Visible = !focusOnControlForm;

        }

        private void btn_returnToForm1_Click(object sender, EventArgs e)
        {
            setFocusOnControlForm(false);
        }

        #region ex CONTROLLER tab

        // Controller Layout graphics switcher

        private void EnableController_Click(object sender, EventArgs e)
        {
            // Click on one of the EnableController radio buttons

            if (sender.GetType() != typeof(RadioButton))
            {
                // ??
                throw new Exception("Event raised by an unknow element");

            }

            RadioButton pressedRadio = (RadioButton)sender;

            // What controller, and what layout?

            if (pressedRadio.Name.Equals("EnableController_NO"))
            {
                // Hide the gamepad image
                pictureBox1.Image = null;

                pnl_PreferredLayout.Visible = false;

                // Set a default value
                PreferredLayout_V.Checked = true;

            }

            else
            {
                PreferredLayout_UpdateImageAndLayout();
            }

            // Set an help label for the different controllers

            if (pressedRadio.Name.Equals("EnableController_NO"))
            {
                lbl_controllerGuide.Visible = false;
            }

            else
            {

                if (pressedRadio.Name.Equals("EnableController_DS4"))
                {
                    lbl_controllerGuide.Text = "( ONLY if using a DS4 WITHOUT external software. Otherwise, choose 'XBOX' )";
                }
                else if (pressedRadio.Name.Equals("EnableController_STEAM"))
                {
                    lbl_controllerGuide.Text = "( If you are going to play on Steam AND use a controller through its drivers )";

                    // Check if user has selected the SMAA anti-aliasing

                    mainForm.AA_showNeededWarnings();


                }
                else // Xbox
                {
                    lbl_controllerGuide.Text = "( For original Xbox controllers, and those who emulate them )";
                }

                lbl_controllerGuide.Visible = true;

            }

        }

        private void PreferredLayout_Click(object sender, EventArgs e)
        {
            PreferredLayout_UpdateImageAndLayout();
        }

        private void PreferredLayout_UpdateImageAndLayout()
        {
            // What controller, and what layout?

            pnl_PreferredLayout.Visible = true;

            if (EnableController_XBOX.Checked)
            {
                pnl_LayoutChooser.Visible = true;

                if (PreferredLayout_PS2.Checked)
                {
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerXBOX_PS2Layout;
                }

                else
                {
                    // V Layout
                    PreferredLayout_V.Checked = true;
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerXBOX_VLayout;
                }

            }
            else if (EnableController_DS4.Checked)
            {
                pnl_LayoutChooser.Visible = true;

                if (PreferredLayout_PS2.Checked)
                {
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerDS4_PS2Layout;
                }

                else
                {
                    // VLayout
                    PreferredLayout_V.Checked = true;
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerDS4_VLayout;
                }

            }
            else if (EnableController_STEAM.Checked)
            {
                pnl_LayoutChooser.Visible = false;
                pictureBox1.Image = null;
            }
            else
            {
                // ??
                throw new Exception("ERROR: A layout has been selected without knowing your kind of controller!");
            }


            if (Ocelot.NOSYMODE)
            {
                // Decide to not show the image because...reasons

                pictureBox1.Visible = false;
            }

        }

        private void setNewColor(object sender, EventArgs e)
        {
            // !
            //  This method is called by some event trigger.
            //   Check Form1.Designer.cs.

            // Don't put breakpoint in there otherwise button will never change
            //  color while debugging.

            Object obj = sender;
            Type type = sender.GetType();

            if (typeof(Button).IsAssignableFrom(type))
            {
                Button btn = (Button)obj;

                if (btn.ForeColor.Name == System.Drawing.Color.DarkOrange.Name)
                {
                    //ForeColor is orange
                    btn.ForeColor = System.Drawing.Color.Black;
                }
                else
                {   //ForeColor is Black OR SOME OTHER UNDEFINED COLOR
                    btn.ForeColor = System.Drawing.Color.DarkOrange;
                }

            }

            if (typeof(CheckBox).IsAssignableFrom(type))
            {

                CheckBox chb = (CheckBox)obj;

                if (chb.ForeColor.Name == System.Drawing.Color.DarkOrange.Name)
                {
                    //ForeColor is DarkOrange
                    chb.ForeColor = System.Drawing.Color.Black;
                    chb.Text = "OFF";

                    //if (chb.Name == "chb_LaptopMode") { chb.Text = "NO"; }

                }
                else
                {
                    //ForeColor is Black OR SOME OTHER UNDEFINED COLOR
                    chb.ForeColor = System.Drawing.Color.DarkOrange;
                    chb.Text = "ON";

                    //if (chb.Name == "chb_LaptopMode") { chb.Text = "YUP"; }

                }

            }

            if (typeof(RadioButton).IsAssignableFrom(type))
            {
                RadioButton radiob = (RadioButton)sender;

                if (radiob.Checked == true)
                {
                    //FontColor before entering the if was for sure Black
                    radiob.Font = new Font(radiob.Font, FontStyle.Bold);
                    radiob.ForeColor = System.Drawing.Color.DarkOrange;
                }
                else
                {
                    //FontColor before entering the if was for sure DarkOrange
                    radiob.Font = new Font(radiob.Font, FontStyle.Regular);
                    radiob.ForeColor = System.Drawing.Color.Black;
                }

            }

        }


        #endregion

        private void ControlsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
        }
    }
}
