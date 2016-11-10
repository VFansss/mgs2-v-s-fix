using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace mgs2_v_s_fix
{
    public partial class Form1 : Form
    {
        // These are need for MGS2 font importing

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font myFont;

        // You can tell if settings panel is opened. Not used for now.
        public bool settingMode;

        public Form1()
        {
            InitializeComponent();

            // Things for font import

            byte[] fontData = Properties.Resources.MGS2_ttf;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.MGS2_ttf.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.MGS2_ttf.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            myFont = new Font(fonts.Families[0], 27.75F);

            btn_startGame.Font = myFont;
            btn_settings.Font = myFont;
            btn_saveSettings.Font = myFont;
            btn_exit.Font = myFont;

            // Tooltip for WideScreenFIX

            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.lbl_tooltip, "It will avoid stretch ONLY (and I will repeat: ONLY!) on 16:9 or 16:10 resolution!");

        }

        // Main menu buttom

        private void btn_startGame_Click(object sender, EventArgs e)
        {
            Ocelot.startGame();
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {

            // Check if INI contain all field & uncompiled fields

            Ocelot.checkConfFileIntegrity();

            // If there's need to autoconfig (= missing field/s inside .ini file)
            if (Ocelot.needOfAutoConfig == true)
            {

                //  V's will understand best config

                Ocelot.startAutoconfig();


            }

            else
            {
                // There's some valid configuration inside .ini

                // Load already existent settings done in the past

                Ocelot.load_INI_SetTo_InternalConfig();

            }

            // Transfering internal setting to graphic setupper
            load_InternalConfig_SetTo_SetupperConfig();

            // Setting Mode
            settingMode = true;
            btn_startGame.Visible = false;
            btn_settings.Visible = false;
            btn_saveSettings.Visible = true;
            tabControl1.Visible = true;
            BackgroundImage = null;
            otagif.Image = mgs2_v_s_fix.Properties.Resources.otagif;
            otagif.Enabled = true;
            otagif.Visible = true;



            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "mgs2_v_s_fix.MANUAL.Credits.txt";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                tbx_About.Text = reader.ReadToEnd();
            }


        }

        private void btn_saveSettings_Click(object sender, EventArgs e)
        {

            load_SetupperConfig_SetTo_InternalConfig();

            Ocelot.load_InternalConfig_SetTo_INI();

            Ocelot.load_InternalConfig_SetTo_MGS();

            // Setting Mode
            settingMode = false;
            btn_startGame.Visible = true;
            btn_settings.Visible = true;
            btn_saveSettings.Visible = false;
            tabControl1.Visible = false;
            this.BackgroundImage = mgs2_v_s_fix.Properties.Resources.background;
            otagif.Image = null;
            otagif.Enabled = false;
            otagif.Visible = false;

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Internal Logic

        public void load_InternalConfig_SetTo_SetupperConfig()
        {

            // Loading Resolution Settings

            txt_Height.Text = Ocelot.InternalConfiguration.Resolution["Height"];
            txt_Width.Text = Ocelot.InternalConfiguration.Resolution["Width"];

            if (Ocelot.InternalConfiguration.Resolution["WideScreenFIX"] == "true")
            {
                chb_WideScreenFIX.Text = "ON";
                chb_WideScreenFIX.Checked = true;
            }

            else
            {
                chb_WideScreenFIX.Text = "OFF";
                chb_WideScreenFIX.Checked = false;
            }

            if (Ocelot.InternalConfiguration.Resolution["WindowMode"] == "true")
            {
                chb_WindowMode.Text = "ON";
                chb_WindowMode.Checked = true;
            }

            else
            {
                chb_WindowMode.Text = "OFF";
                chb_WindowMode.Checked = false;
            }

            if (Ocelot.InternalConfiguration.Resolution["LaptopMode"] == "true")
            {
                chb_LaptopMode.Text = "ON";
                chb_LaptopMode.Checked = true;
            }

            else
            {
                chb_LaptopMode.Text = "OFF";
                chb_LaptopMode.Checked = false;
            }


            Ocelot.getGraphicsAdapterList();

            lst_vga_list.Items.Clear();
           
            foreach(string s in Ocelot.vgaList){

                lst_vga_list.Items.Add(s);

            }

            lst_vga_list.SelectedIndex = lst_vga_list.FindString(Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"]);

            if (lst_vga_list.SelectedIndex == -1)
            {
                // Strange case:
                //  InternalConfig contain a VGA AdapterName that isn't installed on the machine
                //   Probably someone has fucked with .ini
                //    Standard Behaviour: Select an Intel vga

                lst_vga_list.SelectedIndex = lst_vga_list.FindString("Intel");

                if (lst_vga_list.SelectedIndex == -1)
                {
                    //No integrated graphics. Select the last one V's has found

                    lst_vga_list.SelectedIndex = lst_vga_list.FindString(Ocelot.vgaList.Last.Value);
                }

            }

            // Controls Settings

            if (Ocelot.InternalConfiguration.Controls["360Gamepad"] == "true")
            {
                chb_360Gamepad.Text = "ON";
                chb_360Gamepad.Checked = true;
            }

            else
            {
                chb_360Gamepad.Text = "OFF";
                chb_360Gamepad.Checked = false;
            }

            // Graphics Settings

            foreach(Panel panel in tab_Graphics.Controls.OfType<Panel>()){


                String key = panel.Name.Remove(0, 4);
                // NB: 'key' local variable now contain name of the settings key (ie: "MotionBlur")

                
                String value = Ocelot.InternalConfiguration.Graphics[key];
                // NB: 'value' contain value get from InternalSetting corresponding to the 'key' (ie: "high")
  
                String rad_name = key+"_"+value;
                // rad_name=MotionBlur_high
                //  or rad_name=MotionBlur_medium
                //   or rad_name=MotionBlur_low

                RadioButton rad = (RadioButton)panel.Controls.Find(rad_name,true).GetValue(0);

                rad.Checked = true;

            }

            if (Ocelot.InternalConfiguration.Graphics["BunchOfCoolEffect"] == "true")
            {
                chb_BunchOfCoolEffect.Text = "ON";
                chb_BunchOfCoolEffect.Checked = true;
            }

            else
            {
                chb_BunchOfCoolEffect.Text = "OFF";
                chb_BunchOfCoolEffect.Checked = false;
            }

            if (Ocelot.InternalConfiguration.Graphics["MotionBlur"] == "true")
            {
                chb_MotionBlur.Text = "ON";
                chb_MotionBlur.Checked = true;
            }

            else
            {
                chb_MotionBlur.Text = "OFF";
                chb_MotionBlur.Checked = false;
            }

            // Sound Settings

            foreach (Panel panel in tab_Sound.Controls.OfType<Panel>())
            {

                String key = panel.Name.Remove(0, 4);
                // NB: 'key' local variable now contain name of the settings key (ie: "SoundQuality")


                String value = Ocelot.InternalConfiguration.Sound[key];
                // NB: 'value' contain value get from InternalSetting corresponding to the 'key' (ie: "high")

                String rad_name = key + "_" + value;
                // rad_name=SoundQuality_high
                //  or rad_name=SoundQuality_medium
                //   or rad_name=SoundQuality_low

                RadioButton rad = (RadioButton)panel.Controls.Find(rad_name, true).GetValue(0);
                rad.Checked = true;

            }

            Console.WriteLine();

            lst_sound_list.Items.Clear();

            // GetSoundAdapterList non implemented (for now)
            //  cause is always used "Primary Sound Driver"

            lst_sound_list.Items.Add("Primary Sound Driver");

            lst_sound_list.SelectedIndex = lst_sound_list.FindString("Primary");

            
        }

        public void load_SetupperConfig_SetTo_InternalConfig()
        {

            // Resolution Settings

            Ocelot.InternalConfiguration.Resolution["Height"] = txt_Height.Text.Trim();
            Ocelot.InternalConfiguration.Resolution["Width"] = txt_Width.Text.Trim();

            if (chb_WideScreenFIX.Checked == true)
            {

                Ocelot.InternalConfiguration.Resolution["WideScreenFIX"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Resolution["WideScreenFIX"] = "false";
            }

            if (chb_WindowMode.Checked == true)
            {
                Ocelot.InternalConfiguration.Resolution["WindowMode"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Resolution["WindowMode"] = "false";
            }

            if (chb_LaptopMode.Checked == true)
            {
                Ocelot.InternalConfiguration.Resolution["LaptopMode"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Resolution["LaptopMode"] = "false";
            }

            // NB: Something shoud every be selected inside the Listbox now
            //  either manually or automatically by V's
            //   Exception not controlled. Faith in humanity...

            Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"] = lst_vga_list.SelectedItem.ToString();


            // Controls Settings

            if (chb_360Gamepad.Checked == true)
            {
                
                Ocelot.InternalConfiguration.Controls["360Gamepad"] = "true";
            }
            else
            {
                Ocelot.InternalConfiguration.Controls["360Gamepad"] = "false";
            }

            // Graphics Settings

            foreach (Panel panel in tab_Graphics.Controls.OfType<Panel>())
            {


                String key = panel.Name.Remove(0, 4);
                // NB: 'key' local variable now contain name of the settings key (ie: "MotionBlur")

                var checkedButton = panel.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked);

                string checkedButtonName = checkedButton.Name.ToString();

                string value = checkedButtonName.Substring(checkedButtonName.IndexOf('_') + 1);
                // NB: 'value' contain value get from SetupperConfig corresponding to the 'key' (ie: "high")

                Ocelot.InternalConfiguration.Graphics[key] = value;

            }

            if (chb_BunchOfCoolEffect.Checked == true)
            {

                Ocelot.InternalConfiguration.Graphics["BunchOfCoolEffect"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Graphics["BunchOfCoolEffect"] = "false";
            }

            if (chb_MotionBlur.Checked == true)
            {
                Ocelot.InternalConfiguration.Graphics["MotionBlur"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Graphics["MotionBlur"] = "false";
            }

            // Sound Settings

            foreach (Panel panel in tab_Sound.Controls.OfType<Panel>())
            {


                String key = panel.Name.Remove(0, 4);
                // NB: 'key' local variable now contain name of the settings key (ie: "MotionBlur")

                var checkedButton = panel.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked);

                string checkedButtonName = checkedButton.Name.ToString();

                string value = checkedButtonName.Substring(checkedButtonName.IndexOf('_') + 1);
                // NB: 'value' contain value get from SetupperConfig corresponding to the 'key' (ie: "high")

                Ocelot.InternalConfiguration.Sound[key] = value;

            }

            // NB: Something shoud every be selected inside the Listbox now
            //  either manually or automatically by V's
            //   Exception not controlled. Faith in humanity...

            Ocelot.InternalConfiguration.Sound["SoundAdapterName"] = lst_sound_list.SelectedItem.ToString();

            return;
        }

        // Graphics Adjustment

        private void lst_vga_list_DrawItem(object sender, DrawItemEventArgs e)
        {

            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.DarkOrange);//Choose the color

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(lst_vga_list.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();


        }

        private void lst_sound_list_DrawItem(object sender, DrawItemEventArgs e)
        {

            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.DarkOrange);//Choose the color

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(lst_sound_list.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();


        }

        private void setNewColor(object sender, EventArgs e)
        {
            // !
            //  This method is called by some event trigger.
            //   Check Form1.Designer.cs.

            // Don't put breakpoint in there otherwise button will never change
            //  color while debugging.

            //TODO Change outer 'if' statement with a nice switch

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
                }
                else
                {
                    //ForeColor is Black OR SOME OTHER UNDEFINED COLOR
                    chb.ForeColor = System.Drawing.Color.DarkOrange;
                    chb.Text = "ON";
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

        // Input field check

        private void txt_Width_Click(object sender, EventArgs e)
        {
            txt_Width.SelectAll();
        }

        private void txt_Height_Click(object sender, EventArgs e)
        {
            txt_Height.SelectAll();
        }

        private void txt_Width_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                // Bad input
                e.Handled = true;
            }              

        }

        private void txt_Height_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                // Bad input
                e.Handled = true;
            }
        }

        private void txt_Width_Leave(object sender, EventArgs e)
        {
            if (txt_Width.TextLength == 0)
            {
                txt_Width.Text = Ocelot.InternalConfiguration.Resolution["Width"];
                txt_Height.Text = Ocelot.InternalConfiguration.Resolution["Height"];

            }

        }

        private void txt_Height_Leave(object sender, EventArgs e)
        {
            if (txt_Height.TextLength == 0)
            {
                txt_Width.Text = Ocelot.InternalConfiguration.Resolution["Width"];
                txt_Height.Text = Ocelot.InternalConfiguration.Resolution["Height"];

            }
        }

        // WideScreenFIX auto checker

        private void checkIfWSElegible(object sender, KeyEventArgs e)
        {
            if (txt_Width.Text.Equals("")||(txt_Height.Text.Equals("")))
            {
                // Empty String
            }

            else
            {

                double rapporto = (Double.Parse(txt_Width.Text) / Double.Parse(txt_Height.Text));

                // 1.6 -> 16:10
                // 1.777... -> 16:9

                if ((rapporto == 1.6d) || (rapporto == 1.7777777777777777d))
                {                 
                    chb_WideScreenFIX.Checked = true;
                }

                else
                {
                    chb_WideScreenFIX.Checked = false;
                }

            }
        }

        // WideScreenFIX Tooltip

        private void lbl_tooltip_Click(object sender, EventArgs e)
        {

            MessageBox.Show("It will avoid stretch ONLY (and I will repeat: ONLY!) on 16:9 or 16:10 resolution!", "Little explanation", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void lbl_tooltip_MouseEnter(object sender, EventArgs e)
        {
            lbl_tooltip.Font = new Font(lbl_tooltip.Font, FontStyle.Underline);
        }

        private void lbl_tooltip_MouseLeave(object sender, EventArgs e)
        {
            lbl_tooltip.Font = new Font(lbl_tooltip.Font, FontStyle.Regular);
        }

        private void tbx_About_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

    //END CLASS
    }
}
