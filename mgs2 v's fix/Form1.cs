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

        // Remember current background image. Aoid repetition.
        private int bg_number;

        // Remember current icon image. Avoid repetition.
        private int ico_number;

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
            toolTip1.SetToolTip(this.lbl_tooltip, "It will avoid stretch ONLY on 16:9, 16:10 or 21:9 resolution!");

            // SET DEBUG label on main menu 

            if (Ocelot.debugMode)
            {
                this.lbl_debugMode.Visible = true;
            }

            // Background things

            setNewBackground();

            Ocelot.console("[+] Form1 constructor has done. Waiting user input.");

        }

        // Main menu buttom

        private void btn_startGame_Click(object sender, EventArgs e)
        {
            Ocelot.startGame();

            Ocelot.console("[+] Start game button pressed");

        }

        private void btn_settings_Click(object sender, EventArgs e)
        {

            Ocelot.console("[ ] Settings button pressed");

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

            Ocelot.console("[ ] Settings - settings fron internal config attached to settings");

            // Settings Mode
            btn_startGame.Visible = false;
            btn_settings.Visible = false;
            btn_saveSettings.Visible = true;
            tabControl1.Visible = true;
            BackgroundImage = null;
            otagif.Image = mgs2_v_s_fix.Properties.Resources.otagif;
            otagif.Enabled = true;
            otagif.Visible = true;
            

            setNewIcon();

            lbl_ManualLink.Visible = true;
            pictureBox2.Visible = true;

            // Filling 'About'

            abt_Contacts.Checked = true;

            Ocelot.console("[+] Settings has been displayed.");

        }

        private void btn_saveSettings_Click(object sender, EventArgs e)
        {

            Ocelot.console("[ ] Save button pressed");

            load_SetupperConfig_SetTo_InternalConfig();

            Ocelot.load_InternalConfig_SetTo_INI();

            Ocelot.load_InternalConfig_SetTo_MGS();

            // Settings Mode
            btn_startGame.Visible = true;
            btn_settings.Visible = true;
            btn_saveSettings.Visible = false;
            tabControl1.Visible = false;

            setNewBackground();

            otagif.Image = null;
            otagif.Enabled = false;
            otagif.Visible = false;

            pictureBox2.Visible = false;
            lbl_ManualLink.Visible = false;

            Ocelot.console("[+] Save has been saved (!)");

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();

            Ocelot.console("[+] Exit button pressed");

        }

        // Internal Logic

            /* this will edit setupper graphics and control based from properties inside InternalConfig */

        public void load_InternalConfig_SetTo_SetupperConfig()
        {

            // Loading Resolution Settings

            #region lot_of_things
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

            // Just "FullscreenCutscene" things...
            if (Ocelot.InternalConfiguration.Resolution["WideScreenFIX"] == "true")
            {
                lbl_FullscreenCutscene.Visible = true;
                chb_FullscreenCutscene.Visible = true;

                if (Ocelot.InternalConfiguration.Resolution["FullscreenCutscene"] == "true")
                { 
                chb_FullscreenCutscene.Text = "ON";
                chb_FullscreenCutscene.Checked = true;
                }
                else
                {
                chb_FullscreenCutscene.Text = "OFF";
                chb_FullscreenCutscene.Checked = false;
                }
            }

            else
            {
                lbl_FullscreenCutscene.Visible = false;
                chb_FullscreenCutscene.Visible = false;
                chb_FullscreenCutscene.Text = "OFF";
                chb_FullscreenCutscene.Checked = false;

            }

            #endregion

            // Controls Settings

            #region lot_of_things

            foreach (Panel panel in tab_Controls.Controls.OfType<Panel>())
            {

                String key = panel.Name.Remove(0, 4);
                // NB: 'key' local variable now contain name of the settings key (ie: "SoundQuality")


                String value = Ocelot.InternalConfiguration.Controls[key];
                // NB: 'value' contain value get from InternalSetting corresponding to the 'key' (ie: "high")

                String rad_name = key + "_" + value;
                // rad_name=SoundQuality_high
                //  or rad_name=SoundQuality_medium
                //   or rad_name=SoundQuality_low

                RadioButton rad = (RadioButton)panel.Controls.Find(rad_name, true).GetValue(0);
                rad.Checked = true;

            }

            switch (Ocelot.InternalConfiguration.Controls["XboxGamepad"])
            {
                case "V":

                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.XboxController_Layout_V;

                    break;

                case "PS2":

                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.XboxController_Layout_PS2;

                    break;

                default:

                    pictureBox1.Image = null;

                    break;
            }

            #endregion

            // Graphics Settings

            #region lot_of_things
            foreach (Panel panel in tab_Graphics.Controls.OfType<Panel>()){


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

            if (Ocelot.InternalConfiguration.Graphics["AA"] == "true")
            {
                chb_AA.Text = "ON";
                chb_AA.Checked = true;
            }

            else
            {
                chb_AA.Text = "OFF";
                chb_AA.Checked = false;
            }

            #endregion

            // Sound Settings

            #region lot_of_things

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

            lst_sound_list.Items.Clear();

            // GetSoundAdapterList non implemented (for now)
            //  cause is always used "Primary Sound Driver"

            lst_sound_list.Items.Add("Primary Sound Driver");

            lst_sound_list.SelectedIndex = lst_sound_list.FindString("Primary");

            if (Ocelot.InternalConfiguration.Sound["FixAfterPlaying"] == "true")
            {
                chb_FixAfterPlaying.Text = "ON";
                chb_FixAfterPlaying.Checked = true;
            }

            else
            {
                chb_FixAfterPlaying.Text = "OFF";
                chb_FixAfterPlaying.Checked = false;
            }

            #endregion


            // Suggestion time!

            if (Double.Parse(txt_Width.Text) == 1366 && Double.Parse(txt_Height.Text) == 768)
            {
                // A suggestion.

                Ocelot.showMessage("laptop_res_suggestion");

            }

        }

            /* this will retrieve settings from setupper and storage in inside Ocelot.InternalConfiguration */

        public void load_SetupperConfig_SetTo_InternalConfig()
        {

            // Resolution Settings

            #region lot_of_things

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

            if ((chb_WideScreenFIX.Checked == true) && (chb_FullscreenCutscene.Checked == true))
            {

                Ocelot.InternalConfiguration.Resolution["FullscreenCutscene"] = "true";

            }
            else
            {
                Ocelot.InternalConfiguration.Resolution["FullscreenCutscene"] = "false";
            }

            if (chb_FullscreenCutscene.Checked == true)
            {
                Ocelot.InternalConfiguration.Resolution["FullscreenCutscene"] = "true";
            }
            else
            {
                Ocelot.InternalConfiguration.Resolution["FullscreenCutscene"] = "false";
            }

            #endregion

            // Controls Settings

            #region lot_of_things

            foreach (Panel panel in tab_Controls.Controls.OfType<Panel>())
            {


                String key = panel.Name.Remove(0, 4);
                // NB: 'key' local variable now contain name of the settings key (ie: "MotionBlur")

                var checkedButton = panel.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked);

                string checkedButtonName = checkedButton.Name.ToString();

                string value = checkedButtonName.Substring(checkedButtonName.IndexOf('_') + 1);
                // NB: 'value' contain value get from SetupperConfig corresponding to the 'key' (ie: "high")

                Ocelot.InternalConfiguration.Controls[key] = value;

            }
            #endregion

            // Graphics Settings

            #region lot_of_things

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

            if (chb_AA.Checked == true)
            {
                Ocelot.InternalConfiguration.Graphics["AA"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Graphics["AA"] = "false";
            }

            #endregion

            // Sound Settings

            #region lot_of_things

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

            if (chb_FixAfterPlaying.Checked == true)
            {

                Ocelot.InternalConfiguration.Sound["FixAfterPlaying"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Sound["FixAfterPlaying"] = "false";
            }


            #endregion

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

                    if (chb.Name == "chb_LaptopMode") { chb.Text = "NO"; }

                }
                else
                {
                    //ForeColor is Black OR SOME OTHER UNDEFINED COLOR
                    chb.ForeColor = System.Drawing.Color.DarkOrange;
                    chb.Text = "ON";

                    if (chb.Name == "chb_LaptopMode") { chb.Text = "YUP"; }

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

                if (Double.Parse(txt_Width.Text) == 1366 && Double.Parse(txt_Height.Text) == 768)
                {
                    // A suggestion.

                    Ocelot.showMessage("laptop_res_suggestion");

                }

                // 1.6 -> 16:10
                // 1.777... -> 16:9
                // 2.333... -> 21:9
                // 2.370period -> 21:9

                if ((rapporto == 1.6d) || (rapporto == 1.7777777777777777d) || (rapporto == 2.3333333333333333d) || (rapporto == 2.3703703703703703d))
                {                 
                    chb_WideScreenFIX.Checked = true;
                }

                else
                {
                    chb_WideScreenFIX.Checked = false;
                }

                FullscreenCutscene_setVisibility(this,new MouseEventArgs(System.Windows.Forms.MouseButtons.None,1,1,1,1));

            }
        }

        // WideScreenFIX Tooltip

        private void lbl_tooltip_Click(object sender, EventArgs e)
        {

            Ocelot.showMessage("tip_aspect_ratio");

        }

        private void lbl_tooltip_MouseEnter(object sender, EventArgs e)
        {
            lbl_tooltip.Font = new Font(lbl_tooltip.Font, FontStyle.Underline);
        }

        private void lbl_tooltip_MouseLeave(object sender, EventArgs e)
        {
            lbl_tooltip.Font = new Font(lbl_tooltip.Font, FontStyle.Regular);
        }

        // About tab

        private void tbx_About_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void abt_CheckedChanged(object sender, EventArgs e)
        {
            setNewColor(sender, e);

            RadioButton rdi = (RadioButton)sender;

            loadTXT(rdi.Name.Substring(4));

            return;

        }

        private void loadTXT(string NAMEFILE)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "mgs2_v_s_fix.PAPERS."+NAMEFILE+".txt";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                tbx_About.Text = reader.ReadToEnd();
            }

            tbx_About.SelectionStart = 0;
            tbx_About.SelectionLength = 1;
            tbx_About.ScrollToCaret();

        }

        // Exclusive toggle logic

        private void FullscreenCutscene_setVisibility(object sender, MouseEventArgs e)
        {

            if (chb_WideScreenFIX.Checked == true)
            {
                lbl_FullscreenCutscene.Visible = true;
                chb_FullscreenCutscene.Visible = true;
            }

            else
            {
                lbl_FullscreenCutscene.Visible = false;
                chb_FullscreenCutscene.Visible = false;
            }
        }

        private void chb_AA_MouseClick(object sender, MouseEventArgs e)
        {

            if (chb_AA.Checked == true)
            {

                Ocelot.showMessage("tip_AA");
                

                if (ModelQuality_high.Checked == true)
                {
                    ModelQuality_medium.Checked = true;
                }

            }

        }

        private void ModelQuality_high_MouseClick(object sender, MouseEventArgs e)
        {

            if (ModelQuality_high.Checked == true)
            {
                chb_AA.Checked = false;
            }

        }

        // Background chooser

        private void setNewBackground()
        {

            Random rnd = new Random();
            int ran_number = 0;

            // From 1 to 5! 
            
            do {ran_number = rnd.Next(1, 6);}
            while(!(ran_number!=bg_number));
            
            string resourceName = "bg"+ran_number;

            bg_number = ran_number;
            this.BackgroundImage = (System.Drawing.Image)mgs2_v_s_fix.Properties.Resources.ResourceManager.GetObject(resourceName);

            return;
        }

        private void setNewIcon()
        {

            Random rnd = new Random();
            int ran_number = 0;

            // From 1 to 12! 

            do { ran_number = rnd.Next(1, 13); }
            while (!(ran_number != ico_number));

            string resourceName = "ico" + ran_number;

            ico_number = ran_number;

            Icon ohi = (Icon)mgs2_v_s_fix.Properties.Resources.ResourceManager.GetObject(resourceName);

            #region Changing label next to icon

            string name = "";

            switch (ico_number)
            {
                case 1:

                    name = "Colonel";

                    break;

                case 2:

                    name = "Emma";

                    break;

                case 3:

                    name = "Fatman";

                    break;

                case 4:

                    name = "Fortune";

                    break;

                case 5:

                    name = "Ocelot";

                    break;

                case 6:

                    name = "Olga";

                    break;

                case 7:

                    name = "Otacon";

                    break;

                case 8:

                    name = "Raiden";

                    break;

                case 9:

                    name = "Rose";

                    break;

                case 10:

                    name = "Snake";

                    break;

                case 11:

                    name = "Solidus";

                    break;

                case 12:

                    name = "Vamp";

                    break;

            }

            lbl_ManualLink.Text = name + " says: read the manual!";

            #endregion

            this.pictureBox2.Image = ohi.ToBitmap();

            return;
            

        }

        // Controller Layout graphics switcher

        private void XboxGamepad_V_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.XboxController_Layout_V;
        }

        private void XboxGamepad_PS2_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.XboxController_Layout_PS2;
        }

        private void XboxGamepad_V_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.XboxController_Layout_V;
        }

        private void XboxGamepad_PS2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.XboxController_Layout_PS2;
        }

        private void XboxGamepad_NO_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        // Go to manual link

        private void lbl_ManualLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki");
        }

    //END CLASS
    }
}
