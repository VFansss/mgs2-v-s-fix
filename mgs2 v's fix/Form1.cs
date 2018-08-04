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
using static mgs2_v_s_fix.Flags;
using System.Threading.Tasks;

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

        // Remember current background image to avoid repetition.
        private int bg_number;

        // Remember current icon image to avoid repetition.
        private int ico_number;

        // UPDATE Variable

        private bool UPDATE_checkInProgress = false;

        private string checkForUpdateDefaultString = "(Click on the GitHub logo above to check for updates)";

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

            // SET DEBUG label on main menu 

            if (Ocelot.debugMode)
            {
                this.lbl_debugMode.Visible = true;
            }

            // Background things

            setNewBackground();

            Ocelot.PrintToDebugConsole("[+] Form1 constructor has done. Waiting user input.");

            // Bind lbl_checkForUpdate.Text to default text

            lbl_checkForUpdate.Text = checkForUpdateDefaultString;

        }

        #region MAIN MENU

        private void btn_startGame_Click(object sender, EventArgs e)
        {
            Ocelot.startGame();

            Ocelot.PrintToDebugConsole("[+] Start game button pressed");

        }

        private void btn_settings_Click(object sender, EventArgs e)
        {

            Ocelot.PrintToDebugConsole("[ ] Settings button pressed");

            // Check if INI contain all field and/or uncompiled fields

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
            if (load_InternalConfig_SetTo_SetupperConfig() == false)
            {
                // Something gone wrong. A message has already show to the user;
                return;
            }

            Ocelot.PrintToDebugConsole("[ ] Settings - settings from internal config correctly attached to setupper");

            // Settings Mode
            btn_startGame.Visible = false;
            btn_settings.Visible = false;
            btn_saveSettings.Visible = true;
            tabControl1.Visible = true;
            pic_background.BackgroundImage = null;
            otagif.Image = mgs2_v_s_fix.Properties.Resources.otagif;
            otagif.Enabled = true;
            otagif.Visible = true;


            setNewIcon();

            lbl_ManualLink.Visible = true;
            pictureBox2.Visible = true;

            // Filling 'About'

            abt_Contacts.Checked = true;

            // Must display the warning about winXP compatiblity?

            if (!Ocelot.InternalConfiguration.Others["CompatibilityWarningDisplayed"].Equals("true"))
            {
                // Display it!

                Ocelot.showMessage("compatibilityWarning");

                // Not display anymore

                Ocelot.InternalConfiguration.Others["CompatibilityWarningDisplayed"] = "true";

            }

            Ocelot.PrintToDebugConsole("[+] Settings has been displayed.");

        }

        private void btn_saveSettings_Click(object sender, EventArgs e)
        {

            Ocelot.PrintToDebugConsole("[ ] Save button pressed");

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

            Ocelot.PrintToDebugConsole("[+] Save has been saved (!)");

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();

            Ocelot.PrintToDebugConsole("[+] Exit button pressed");

        }

        #endregion

        #region Internal Logic

        /* this will edit setupper graphics and control based from properties inside InternalConfig */

        public bool load_InternalConfig_SetTo_SetupperConfig()
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

            /*if (Ocelot.InternalConfiguration.Resolution["LaptopMode"] == "true")
            {
                chb_LaptopMode.Text = "ON";
                chb_LaptopMode.Checked = true;
            }

            else
            {
                chb_LaptopMode.Text = "OFF";
                chb_LaptopMode.Checked = false;
            }*/


            Ocelot.getGraphicsAdapterList();

            lst_vga_list.Items.Clear();

            foreach(string s in Ocelot.vgaList){

                lst_vga_list.Items.Add(s);

            }

            lst_vga_list.SelectedIndex = lst_vga_list.FindString(Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"]);

            if (lst_vga_list.SelectedIndex == -1)
            {

                Ocelot.PrintToDebugConsole("[!] -VVV- No VGA Selected.");

                if (Ocelot.vgaList.Count == 0)
                {

                    Ocelot.PrintToDebugConsole("[!] -WHY?- No VGA found on system. Awaiting manual input.");

                    // Strange case:
                    //  V's wasn't able to understand which video cards are available
                    //   User need to do a manual insertion, if it hasn't already done it

                    Ocelot.InternalConfiguration.Resolution.TryGetValue("GraphicAdapterName", out string explicitedVGAName);

                    if (explicitedVGAName!= null && !explicitedVGAName.Equals("") && explicitedVGAName.Length > 0)
                    {
                        Ocelot.PrintToDebugConsole("[!] -RESULT?- Found a manual inserted one: "+ explicitedVGAName);

                        // Manually inserted
                        lst_vga_list.Items.Add(explicitedVGAName);
                        lst_vga_list.SelectedIndex = lst_vga_list.FindString(explicitedVGAName);
                    }

                    else
                    {
                        // Prompt a message to V's Wiki

                        Ocelot.PrintToDebugConsole("[!] -RESULT?- NO MANUAL INPUT. SHOWING MANUAL and ABORTING.");

                        Ocelot.showMessage("no_vga");

                        try
                        {
                            System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki/Settings-Menu#resolution-tab");
                        }

                        catch
                        {
                            Ocelot.showMessage("UAC_error");
                        }

                        return false;

                    }

                }

                else
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

            }

            // IF WSF is enabled
            if (Ocelot.InternalConfiguration.Resolution["WideScreenFIX"] == "true")
            {
                lbl_FullscreenCutscene.Visible = true;
                chb_FullscreenCutscene.Visible = true;

                // Just "FullscreenCutscene" things...
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

                // OptimizedFOV
                if (Ocelot.InternalConfiguration.Resolution["OptimizedFOV"] == "16:9")
                {
                    chb_OptimizedFOV.Text = "ON";
                    chb_OptimizedFOV.Checked = true;
                }
                else
                {
                    chb_OptimizedFOV.Text = "OFF";
                    chb_OptimizedFOV.Checked = false;
                }

            }

            else
            {
                lbl_FullscreenCutscene.Visible = false;
                chb_FullscreenCutscene.Visible = false;
                chb_FullscreenCutscene.Text = "OFF";
                chb_FullscreenCutscene.Checked = false;

                lbl_OptimizedFOV.Visible = false;
                chb_OptimizedFOV.Visible = false;
                chb_OptimizedFOV.Text = "OFF";
                chb_OptimizedFOV.Checked = false;

            }

            #endregion

            // Controls Settings

            #region lot_of_things

            // What controller, and what layout?

            if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("XBOX"))
            {
                pnl_PreferredLayout.Visible = true;
                EnableController_XBOX.Checked = true;

                if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                {
                    PreferredLayout_PS2.Checked = true;
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerXBOX_PS2Layout;
                }

                else
                {
                    // V Layout
                    PreferredLayout_V.Checked = true;
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerXBOX_VLayout;
                }

            }

            else if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("DS4"))
            {
                pnl_PreferredLayout.Visible = true;
                EnableController_DS4.Checked = true;

                if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                {
                    PreferredLayout_PS2.Checked = true;
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerDS4_PS2Layout;
                }

                else
                {
                    // VLayout
                    PreferredLayout_V.Checked = true;
                    pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerDS4_VLayout;
                }

            }

            else
            {
                // No recognized controller is set. Hide everything
                EnableController_NO.Checked = true;
                pnl_PreferredLayout.Visible = false;

                // Set a default layout value for saving it a first time

                PreferredLayout_V.Checked = true;

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

            // All gone well. Can show the settings menu

            return true;
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

            /*if (chb_LaptopMode.Checked == true)
            {
                Ocelot.InternalConfiguration.Resolution["LaptopMode"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Resolution["LaptopMode"] = "false";
            }*/

            // NB: Something shoud every be selected inside the Listbox now
            //  either manually or automatically by V's
            //   Exception not controlled. Faith in V's...

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

            if (chb_OptimizedFOV.Checked == true)
            {
                Ocelot.InternalConfiguration.Resolution["OptimizedFOV"] = "16:9";
            }
            else
            {
                Ocelot.InternalConfiguration.Resolution["OptimizedFOV"] = "NO";
            }

            #endregion

            // Controls Settings

            #region lot_of_things

            // What controller?

            if (EnableController_XBOX.Checked)
            {
                Ocelot.InternalConfiguration.Controls["EnableController"] = "XBOX";
            }

            else if (EnableController_DS4.Checked)
            {
                Ocelot.InternalConfiguration.Controls["EnableController"] = "DS4";
            }

            else
            {
                // No controller
                Ocelot.InternalConfiguration.Controls["EnableController"] = "NO";
            }

            // What layout?

            if (PreferredLayout_PS2.Checked)
            {
                Ocelot.InternalConfiguration.Controls["PreferredLayout"] = "PS2";
            }
            else
            {
                // V's Layout
                Ocelot.InternalConfiguration.Controls["PreferredLayout"] = "V";
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

        #endregion

        #region Graphical Adjustment

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

        // Background chooser

        private void setNewBackground()
        {

            Random rnd = new Random();
            int ran_number = 0;

            // CHANGE THIS IF MORE SCREENSHOT ARE AVAILABLE
            int NUMBER_OF_SCREENSHOTS = 7;

            do { ran_number = rnd.Next(1, NUMBER_OF_SCREENSHOTS+1); }
            while (!(ran_number != bg_number));

            string resourceName = "bg" + ran_number;

            bg_number = ran_number;
            pic_background.BackgroundImage = (System.Drawing.Image)mgs2_v_s_fix.Properties.Resources.ResourceManager.GetObject(resourceName);

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

        #endregion

        #region RESOLUTION tab

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

                // NB: This is replied in Ocelot.startAutoconfig

                // 1.6 -> 16:10
                // 1.777... -> 16:9       
                // 1.778645883... -> 1366x768

                // 2.3703703703703703d -> 2560x1080
                // 2.3888888888888888d -> 3440x1440

                if (
                    (rapporto == 1.6d) ||
                    (rapporto == 1.7777777777777777d) ||
                    (rapporto == 1.7786458333333333d) ||
                    (rapporto == 2.3703703703703703d) ||
                    (rapporto == 2.3888888888888888d)
                    )
                {                 
                    chb_WideScreenFIX.Checked = true;
                }

                else
                {
                    chb_WideScreenFIX.Checked = false;
                }

                if (rapporto == 1.7777777777777777d)
                {
                    chb_OptimizedFOV.Checked = true;
                }

                else
                {
                    chb_OptimizedFOV.Checked = false;
                }

                FullscreenCutscene_setVisibility(this,new MouseEventArgs(System.Windows.Forms.MouseButtons.None,1,1,1,1));

            }
        }

        // Exclusive toggle logic

        private void FullscreenCutscene_setVisibility(object sender, MouseEventArgs e)
        {

            if (chb_WideScreenFIX.Checked == true)
            {
                lbl_FullscreenCutscene.Visible = true;
                chb_FullscreenCutscene.Visible = true;

                lbl_OptimizedFOV.Visible = true;
                chb_OptimizedFOV.Visible = true;

            }

            else
            {
                lbl_FullscreenCutscene.Visible = false;
                chb_FullscreenCutscene.Visible = false;

                lbl_OptimizedFOV.Visible = false;
                chb_OptimizedFOV.Visible = false;
            }
        }

        #endregion

        #region CONTROLLER tab

        // Controller Layout graphics switcher

        private void EnableController_Click(object sender, EventArgs e)
        {
            // Click on one of the EnableController radio buttons

            if(sender.GetType() != typeof(RadioButton))
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
                PreferredLayout_UpdateImage();
            }


        }

        private void PreferredLayout_Click(object sender, EventArgs e)
        {
            PreferredLayout_UpdateImage();
        }

        private void PreferredLayout_UpdateImage()
        {
            // What controller, and what layout?

            pnl_PreferredLayout.Visible = true;

            if (EnableController_XBOX.Checked)
            {
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

            else
            {
                // ??
                throw new Exception("ERROR: A layout has been selected without knowing your kind of controller!");
            }
        }

        #endregion

        #region GRAPHICS tab

        // Exclusive toggle logic

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

        #endregion

        #region ABOUT tab

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
            var resourceName = "mgs2_v_s_fix.PAPERS." + NAMEFILE + ".txt";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                tbx_About.Text = reader.ReadToEnd();
            }

            tbx_About.SelectionStart = 0;
            tbx_About.SelectionLength = 1;
            tbx_About.ScrollToCaret();

        }

        private void lbl_donate_Click(object sender, EventArgs e)
        {
            donate();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            donate();
        }

        private void donate()
        {
            try
            {
                System.Diagnostics.Process.Start("https://paypal.me/VFansss");
            }

            catch
            {
                Ocelot.showMessage("no_donate");
            }
        }

        #endregion

        #region HELP links

        private void lbl_ManualLink_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Ocelot.GITHUB_WIKI);
            }

            catch
            {
                Ocelot.showMessage("UAC_error");
            }

        }

        private void help_resolution_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki/Troubleshooting-&-Debug-mode");
            }

            catch
            {
                Ocelot.showMessage("UAC_error");
            }
        }

        private void help_controls_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki/Controllers-&-Actions");
            }

            catch
            {
                Ocelot.showMessage("UAC_error");
            }
        }

        private void ptb_comPLAY_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.complay.info/");
            }

            catch
            {
                Ocelot.showMessage("UAC_error");
            }
        }

        private void help_compatibilityWarning_Click(object sender, EventArgs e)
        {
            Ocelot.showMessage("compatibilityWarning");
        }

        #endregion

        #region UPDATE

        // UPDATE

        private async void ptb_GitHubLogo_Click(object sender, EventArgs e)
        {
            if (UPDATE_checkInProgress)
            {
                // Do nothing
                return;
            }

            // Lock

            UPDATE_checkInProgress = true;

            lbl_checkForUpdate.Text = "...Checking...";

            // Set the async
            UPDATE_AVAILABILITY remoteStatus = await Task.Run(() => Ocelot.CheckForUpdatesAsync());

            UPDATE_checkFinished(remoteStatus);

        }

        private void UPDATE_checkFinished(UPDATE_AVAILABILITY result)
        {

            switch (result)
            {
                case UPDATE_AVAILABILITY.NoUpdates:

                    Ocelot.showMessage("update_noupdates");

                    break;

                case UPDATE_AVAILABILITY.UpdateAvailable:

                    Ocelot.showMessage("update_available");

                    try
                    {
                        System.Diagnostics.Process.Start(Ocelot.GITHUB_RELEASE);
                    }

                    catch
                    {
                        Ocelot.showMessage("UAC_error");
                    }

                    break;

                default:

                    // Response mismatch & network errors

                    Ocelot.showMessage("update_crashedinfire");

                    break;
            }

            // Re-allow the update checks

            lbl_checkForUpdate.Text = checkForUpdateDefaultString;
            UPDATE_checkInProgress = false;
        }

        #endregion

        //END CLASS
    }
}
