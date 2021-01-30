using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.Drawing.Text;
using System.IO;
using System.Reflection;
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

        private string checkForUpdateDefaultString = "Click on the GitHub logo aside to check for V's Fix updates";

        // Don't show these warnings (until the next fix reboot, at least)
        
        private bool tip_antialiasingANDmodelquality_showed = false;

        // Take in memory the instance of the second utility form called 'ControlsForm'

        private ControlsForm secondFormInstance;

        // DEBUG MODE

        private Boolean setupButtonPressed = false;

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
                lbl_debugMode.Text = @"Debug Mode - Press 'SETTINGS' button";
            }

            // Background things

            setNewBackground();

            // Bind lbl_checkForUpdate.Text to default text

            lbl_checkForUpdate.Text = checkForUpdateDefaultString;

            // Check for fatal errors

            Ocelot.PrintToDebugConsole("[FATALERROR CHECKING] Checking for fatal errors...");

            FATALERRORSFOUND errorsFound = Ocelot.CheckForFatalErrors();

            Ocelot.PrintToDebugConsole("[FATALERROR CHECKING] Check has returned this code: "+errorsFound.ToString());

            if (errorsFound != FATALERRORSFOUND.NoneDetected)
            {
                // Trouble incoming

                if (errorsFound.HasFlag(FATALERRORSFOUND.ErrorWhileReadingFile))
                {
                    Ocelot.showMessage("UAC_error");
                }

                if (errorsFound.HasFlag(FATALERRORSFOUND.WrongVideoAdapter))
                {

                    // Open the guide to the right chapter?

                    DialogResult answer = Ocelot.showMessage("fatalError_WrongVideoAdapter");

                    if(answer == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki/Settings-Menu#resolution-tab");
                        }

                        catch
                        {
                            Ocelot.showMessage("UAC_error");
                        }
                    }
                  
                }

            }

            // Show the form

            Ocelot.PrintToDebugConsole("[+] Form1 constructor has done. Waiting user input.");

        }

        #region MAIN MENU

        private void btn_startGame_Click(object sender, EventArgs e)
        {

            // CHECK: the game has been configured at least once?
            // I honestly hope that no one try to start the game without doing it, but...

            if (File.Exists("mgs2.ini"))
            {
                Ocelot.startGame();

                Ocelot.PrintToDebugConsole("[+] Start game button pressed");
            }

            else
            {
                Ocelot.showMessage("gameNeverConfigured");

            }

            

        }

        private void btn_settings_Click(object sender, EventArgs e)
        {

            Ocelot.PrintToDebugConsole("[ ] Settings button pressed");

            // Check if INI contain all field and/or uncompiled fields

            Ocelot.checkConfFileIntegrity();

            // If there's need to autoconfig (= missing field/s inside .ini file)
            if (Ocelot.needAutoconfig)
            {

                //  V's will try to understand the best config

                Ocelot.startAutoconfig();

            }

            else
            {
                // There's already a valid configuration done in the past

                // Load already existent settings done in the past

                Ocelot.load_INI_SetTo_InternalConfig();

            }

            // Instantiate a new controls form
            secondFormInstance = new ControlsForm(this);

            // Transfering internal setting to graphical setupper
            if (load_InternalConfig_SetTo_SetupperConfig() == false)
            {
                // Something gone wrong. A message has already show to the user;
                return;
            }

            Ocelot.PrintToDebugConsole("[ ] Settings - settings from internal config correctly attached to setupper");

            if (Ocelot.debugMode && !setupButtonPressed)
            {
                // User hasn't pressed at least once the SETTINGS button, and I don't have a good debug file
                lbl_debugMode.Text = "Debug Mode - Now press 'SAVE' button";

            }
            

            // Chang some controls on Settings tabs
            btn_startGame.Visible = false;
            btn_settings.Visible = false;
            btn_saveSettings.Visible = true;
            tabControl1.Visible = true;
            pic_background.BackgroundImage = null;

            if (Ocelot.NOSYMODE)
            {
                // Don't show the gif.
                otagif.Enabled = false;
                otagif.Visible = false;
            }
            else
            {
                otagif.Image = mgs2_v_s_fix.Properties.Resources.otagif;

                otagif.Enabled = true;
                otagif.Visible = true;

                setNewIcon();

            }
  
            lbl_ManualLink.Visible = true;
            pictureBox2.Visible = true;

            // About tab

            abt_Regards.Checked = true;

            // Must display the warning about winXP compatiblity?
            // NOT NEEDED ANYMORE FROM 1.7 FIX VERSION

            /*if (!Ocelot.InternalConfiguration.Others["CompatibilityWarningDisplayed"].Equals("true"))
            {
                // Display it!

                Ocelot.showMessage("compatibilityWarning");

                // Not display anymore

                Ocelot.InternalConfiguration.Others["CompatibilityWarningDisplayed"] = "true";

            }*/

            // Check if any compatibility flags are set on the mgs2_sse.exe, and warn the user
            // that they aren't needed anymore

            if (Ocelot.CompatibilityFlagsExists())
            {
                // Warn the user
                Ocelot.showMessage("compatibilityFlagsNotNeeded");

            }

            // Check if savegame will be moved to the new location in "My Games", and warn the user if there are issues

            SAVEGAMEMOVING evaluationResult = Ocelot.SavegameMustBeMoved();

            Ocelot.PrintToDebugConsole("[!] SavegameMustBeMoved evaluation result is "+ evaluationResult);

            switch (evaluationResult)
            {
                case SAVEGAMEMOVING.IsAGOGInstallation:

                    // Don't show anything

                    break;


                case SAVEGAMEMOVING.NoSuccesfulEvaluationPerformed:

                    Ocelot.showMessage("UAC_error");

                    // Abort everything until user manually solve the situation

                    Program.ForceClosing();

                    break;

                case SAVEGAMEMOVING.MovingPossible:

                    Ocelot.showMessage("savegameWillBeMoved");

                    break;

                case SAVEGAMEMOVING.BothFolderExist:

                    Ocelot.showMessage("savegameCantBeMoved");

                    // Abort everything until user manually solve the situation

                    Program.ForceClosing();

                    break;

                default:

                    // The last remaining evaluation is SAVEGAMEMOVING.NoSavegame2Move, and doesn't require any warning

                    break;
            }

            Ocelot.PrintToDebugConsole("[+] Settings has been displayed.");

        }

        private void btn_saveSettings_Click(object sender, EventArgs e)
        {

            Ocelot.PrintToDebugConsole("[ ] Save button pressed");

            if (Ocelot.debugMode && !setupButtonPressed)
            {
                // User hasn't pressed at least once the SETTINGS button, and I don't have a good debug file
                lbl_debugMode.Text = "Debug Mode - Now the log is OK :)";
                

            }

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

            Ocelot.PrintToDebugConsole("[+] 'SAVE' has finished saving (!)");

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

            // Keyboard part
            if (Ocelot.InternalConfiguration.Controls["UseDefaultKeyboardLayout"] == "true")
            {
                secondFormInstance.chb_UseDefaultKeyboardLayout.Text = "ON";
                secondFormInstance.chb_UseDefaultKeyboardLayout.Checked = true;
            }
            else
            {
                secondFormInstance.chb_UseDefaultKeyboardLayout.Text = "OFF";
                secondFormInstance.chb_UseDefaultKeyboardLayout.Checked = false;
            }

            // Controller part

            // What controller, and what layout?

            bool controllerSelected = false;
            bool steamControllerSelected = false;

            // Set a default value
            secondFormInstance.chb_InvertTriggersWithDorsals.Text = "OFF";
            secondFormInstance.chb_InvertTriggersWithDorsals.Checked = false;

            if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("XBOX"))
            {
                secondFormInstance.EnableController_XBOX.Checked = true;
                secondFormInstance.lbl_InvertTriggersWithDorsals.Text = "Invert RT/LT with RB/LB:";

                controllerSelected = true;

                if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                {
                    secondFormInstance.PreferredLayout_PS2.Checked = true;
                    secondFormInstance.pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerXBOX_PS2Layout;
                }

                else
                {
                    // V Layout
                    secondFormInstance.PreferredLayout_V.Checked = true;
                    secondFormInstance.pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerXBOX_VLayout;
                }

            }

            else if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("DS4"))
            {
                secondFormInstance.EnableController_DS4.Checked = true;
                secondFormInstance.lbl_InvertTriggersWithDorsals.Text = "Invert R2/L2 with R1/L1:";

                controllerSelected = true;

                if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                {
                    secondFormInstance.PreferredLayout_PS2.Checked = true;
                    secondFormInstance.pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerDS4_PS2Layout;
                }

                else
                {
                    // V Layout
                    secondFormInstance.PreferredLayout_V.Checked = true;
                    secondFormInstance.pictureBox1.Image = mgs2_v_s_fix.Properties.Resources.ControllerDS4_VLayout;
                }

            }

            else if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("STEAM"))
            {
                secondFormInstance.EnableController_STEAM.Checked = true;
                secondFormInstance.PreferredLayout_V.Checked = true;

                steamControllerSelected = true; // Hide the panel with extra options'

            }

            if (controllerSelected)
            {
                // Things visible for every controller
                secondFormInstance.pnl_Gamepad_PreferredLayout.Visible = true;

                if (Ocelot.InternalConfiguration.Controls["InvertTriggersWithDorsals"].Equals("true"))
                {
                    secondFormInstance.chb_InvertTriggersWithDorsals.Text = "ON";
                    secondFormInstance.chb_InvertTriggersWithDorsals.Checked = true;
                }

            }
            else
            {
                if (!steamControllerSelected) secondFormInstance.EnableController_NO.Checked = true;

                secondFormInstance.pnl_Gamepad_PreferredLayout.Visible = false;

                // Set a default layout value for saving it a first time

                secondFormInstance.PreferredLayout_V.Checked = true;

            }

            if (Ocelot.NOSYMODE)
            {
                // Don't show the gamepad image because...reasons
                secondFormInstance.pictureBox1.Visible = false;
            }

            #endregion

            // Graphics Settings

            #region lot_of_things
            foreach (Panel panel in tab_Graphics.Controls.OfType<Panel>()){
                
                if (panel.Name.Equals("pnl_AA"))
                {
                    // Note to myself after 2 years of developing
                    // ok, in retrospective this wasn't a perfect idea

                    // AA will be decided below
                    continue;
                }

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

            if (Ocelot.InternalConfiguration.Graphics["DepthOfField"] == "true")
            {
                chb_DepthOfField.Text = "ON";
                chb_DepthOfField.Checked = true;
            }

            else
            {
                chb_DepthOfField.Text = "OFF";
                chb_DepthOfField.Checked = false;
            }

            // Anti-Aliasing
            // I have to contemplate when AA was true or false in the old V's Fix version

            if (Ocelot.InternalConfiguration.Graphics["AA"].Equals("smaa") || Ocelot.InternalConfiguration.Graphics["AA"].Equals("true"))
            {
                AA_smaa.Checked = true;
            }
            else if (Ocelot.InternalConfiguration.Graphics["AA"].Equals("fxaa"))
            {
                AA_fxaa.Checked = true;
            }
            else{
                AA_no.Checked = true;
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

            // All went well. I can show the settings menu

            return true;
        }

        /* this will retrieve settings from setupper and will storage it inside Ocelot.InternalConfiguration */

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

            // What keyboard layout?

            if (secondFormInstance.chb_UseDefaultKeyboardLayout.Checked)
            {
                Ocelot.InternalConfiguration.Controls["UseDefaultKeyboardLayout"] = "true";
            }

            else
            {
                Ocelot.InternalConfiguration.Controls["UseDefaultKeyboardLayout"] = "false";
            }

            // What controller?

            if (secondFormInstance.EnableController_XBOX.Checked) Ocelot.InternalConfiguration.Controls["EnableController"] = "XBOX";

            else if (secondFormInstance.EnableController_DS4.Checked) Ocelot.InternalConfiguration.Controls["EnableController"] = "DS4";

            else if (secondFormInstance.EnableController_STEAM.Checked) Ocelot.InternalConfiguration.Controls["EnableController"] = "STEAM";

            else Ocelot.InternalConfiguration.Controls["EnableController"] = "NO";

            // What layout?

            if (secondFormInstance.PreferredLayout_PS2.Checked)
            {
                Ocelot.InternalConfiguration.Controls["PreferredLayout"] = "PS2";
            }
            else
            {
                // V's Layout
                Ocelot.InternalConfiguration.Controls["PreferredLayout"] = "V";
            }

            // Invert triggers with dorsals?

            if (secondFormInstance.chb_InvertTriggersWithDorsals.Checked)
            {
                Ocelot.InternalConfiguration.Controls["InvertTriggersWithDorsals"] = "true";
            }
            else
            {
                Ocelot.InternalConfiguration.Controls["InvertTriggersWithDorsals"] = "false";
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

            if (chb_DepthOfField.Checked == true)
            {
                Ocelot.InternalConfiguration.Graphics["DepthOfField"] = "true";

            }

            else
            {
                Ocelot.InternalConfiguration.Graphics["DepthOfField"] = "false";
            }

            // ANTI-ALIASING

            if (AA_smaa.Checked)
            {
                Ocelot.InternalConfiguration.Graphics["AA"] = "smaa";

            }
            else if (AA_fxaa.Checked)
            {
                Ocelot.InternalConfiguration.Graphics["AA"] = "fxaa";
            }
            else
            {
                Ocelot.InternalConfiguration.Graphics["AA"] = "no";
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

            if (Ocelot.NOSYMODE)
            {
                // Don't show anything because people are spying below your shoulder
                pic_background.BackgroundImage = null;
                return;
            }

            Random rnd = new Random();
            int ran_number = 0;

            // CHANGE THIS IF MORE SCREENSHOT ARE AVAILABLE
            int NUMBER_OF_SCREENSHOTS = 9;

            do { ran_number = rnd.Next(1, NUMBER_OF_SCREENSHOTS+1); }
            while (!(ran_number != bg_number));

            string resourceName = "bg" + ran_number;

            bg_number = ran_number;
            pic_background.BackgroundImage = (System.Drawing.Image)mgs2_v_s_fix.Properties.Resources.ResourceManager.GetObject(resourceName);

            return;
        }

        private void setNewIcon()
        {

            if (Ocelot.NOSYMODE)
            {
                // Don't show anything because people are spying below your shoulder
                pictureBox2.Image = null;
                return;
            }

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

            pictureBox2.Image = ohi.ToBitmap();

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

        private void checkIfElegibleForEnhancements(object sender, KeyEventArgs e)
        {
            if (txt_Width.Text.Equals("")||(txt_Height.Text.Equals("")))
            {
                // Wait until I have both Width and Height
            }

            else
            {

                // Calculate display ratio of my screen

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

                if (rapporto == 1.7777777777777777d) chb_OptimizedFOV.Checked = true;
                else chb_OptimizedFOV.Checked = false;

                FullscreenCutscene_setVisibility(this,new MouseEventArgs(System.Windows.Forms.MouseButtons.None,1,1,1,1));

                // Calculate if res is higher than 2K (= height >= 1440) and set "8K" in "Internal resolution" config

                if (Double.Parse(txt_Height.Text) >= 1440d && !InternalResolution_8K.Checked)
                {
                    if (Ocelot.showMessage("tip_prompt8KInternalRes") == DialogResult.Yes) InternalResolution_8K.Checked = true;
                }

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

        // Build 190901: lot of methods moved inside ControlsForm.cs assembly

        private void pnl_ControlsSubPanel_Click(object sender, EventArgs e)
        {
            GoToSecondForm();
        }

        private void lbl_goToControlForm_Click(object sender, EventArgs e)
        {
            GoToSecondForm();
        }

        private void GoToSecondForm()
        {
            secondFormInstance.AddOwnedForm(this);
            secondFormInstance.setFocusOnControlForm(true);
        }

        private void help_control_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Ocelot.GITHUB_WIKI_CONTROLS);
            }

            catch
            {
                Ocelot.showMessage("UAC_error");
            }
        }

        #endregion

        #region GRAPHICS tab

        // Show an helper

        private void InternalResolution_Click(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(RadioButton))
            {
                // ??
                throw new Exception("Event raised by an unknow element");

            }

            InternalResolution_showTheRightHelper(((RadioButton)sender).Name);
        }

        private void InternalResolution_showTheRightHelper(string pressedButtonName)
        {
            switch (pressedButtonName)
            {
                case "InternalResolution_512":

                    lbl_InternalResolutionGuide.Text = "( Slightly better than the PS2 version. Just for the truly nostalgic )";

                    break;

                case "InternalResolution_720":

                    lbl_InternalResolutionGuide.Text = "( Framebuffer: 2048x1024. Good for non-gaming laptop and handheld )";

                    break;

                case "InternalResolution_2K":

                    lbl_InternalResolutionGuide.Text = "( Framebuffer: 4096x2048. Good for not stressing your gaming PC )";

                    break;

                case "InternalResolution_8K":

                    lbl_InternalResolutionGuide.Text = "( Framebuffer: 8192x8192. Fantastic downsample for high-performance PCs )";

                    break;

            }

            lbl_InternalResolutionGuide.Visible = true;

        }

        private void AA_Click(object sender, EventArgs e)
        {

            if (sender.GetType() != typeof(RadioButton))
            {
                // ??
                throw new Exception("Event raised by an unknow element");

            }

            RadioButton pressedRadio = (RadioButton)sender;

            AA_showTheRightHelper();

        }

        private void AA_showTheRightHelper()
        {
            // See what AA option is selected

            string selectedAAoption = "";

            foreach (RadioButton singleRadioButton in pnl_AA.Controls.OfType<RadioButton>())
            {

                if (singleRadioButton.Checked)
                {
                    selectedAAoption = singleRadioButton.Name;
                    break;
                }

            }

            // Set an help label for the different controllers

            if (selectedAAoption.Equals("AA_no"))
            {
                lbl_AAGuide.Visible = false;
            }

            else
            {

                if (selectedAAoption.Equals("AA_fxaa"))
                {
                    lbl_AAGuide.Text = "( Lighter and faster, but less effective than SMAA. Recommended for laptops )";
                }
                else if(selectedAAoption.Equals("AA_smaa")) 
                {
                    // SMAA
                    lbl_AAGuide.Text = "( Better quality than FXAA, but heavier. NOT compatible with Steam overlay )";
                }
                else
                {
                    // ??
                    throw new Exception("Seems that no AA radio button has been pressed");
                }

                lbl_AAGuide.Visible = true;

                // Show warnings, if needed

                AA_showNeededWarnings();

            }
        }

        // Esclusive button logic

        private void ModelQuality_high_Click(object sender, EventArgs e)
        {
            AA_showNeededWarnings();
        }

        internal void AA_showNeededWarnings(bool imGoingToUseAddGame2Steam = false)
        {
            // CHECK: the user want to use Steam AND use SMAA?
            // Action: must choose FXAA and warn the user
            
            if( AA_smaa.Checked && ( imGoingToUseAddGame2Steam || secondFormInstance.EnableController_STEAM.Checked ))
            {

                Ocelot.showMessage("tip_smaaANDsteam");

                AA_fxaa.Checked = true;

                AA_showTheRightHelper();

            }


            // CHECK: the user use any anti-aliasing AND Model quality to High?
            // Action: warn the user

            if( tip_antialiasingANDmodelquality_showed == false && ( AA_fxaa.Checked || AA_smaa.Checked) && ModelQuality_high.Checked)
            {
                Ocelot.showMessage("tip_antialiasingANDmodelquality");

                // Don't show until next reboot
                tip_antialiasingANDmodelquality_showed = true;

            }


        }

        #endregion

        #region SOUND tab

        private void help_sound_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki/Troubleshooting-&-Debug-mode#common-problems--common-solutions");
            }

            catch
            {
                Ocelot.showMessage("UAC_error");
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
                System.Diagnostics.Process.Start(Ocelot.GITHUB_WIKI_INDEX);
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

        private void help_resolutionMenuWarning_Click(object sender, EventArgs e)
        {
            DialogResult openTheWiki = Ocelot.showMessage("tip_explainSelectionForVGAs");

            if(openTheWiki == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start("https://github.com/VFansss/mgs2-v-s-fix/wiki/Settings-Menu#resolution-tab");
                }

                catch
                {
                    Ocelot.showMessage("UAC_error");
                }
            }

        }

        #endregion

        #region EXTRA

        // 'Fix after playing' toggle

        private void chb_FixAfterPlaying_Click(object sender, EventArgs e)
        {
            //CHECK: If Steam controller is enabled? If yes, force user to keep it to "OFF"

            if (secondFormInstance.EnableController_STEAM.Checked)
            {
                Ocelot.showMessage("tip_openVsAfterPlayingANDsteam");

                chb_FixAfterPlaying.Checked = false;
            }
        }

        // 'Check for update' button

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

        // 'Add-To-Steam' button

        private void ptb_Steam_Click(object sender, EventArgs e)
        {
            if (Ocelot.IsThisProcessRunning("Steam"))
            {
                Ocelot.showMessage("steamIsRunning");
                return;
            }

            DialogResult doYouWantToProceed = MessageBox.Show(
                "V's Fix will now try to add on your Steam the game :"+
                "\n\n" +
                "METAL GEAR SOLID 2: SUBSTANCE"+
                "\n\n"+
                "Also, it will automatically set 'Open V's Fix after playing the game' to false, so you can easily interact with the game directly from Steam"+
                "\n\n"+
                "Are you sure you want to continue?","Add the game on Steam", MessageBoxButtons.YesNo);

            if(doYouWantToProceed != DialogResult.Yes)
            {
                return;
            }

            // Let's go

            ADD2STEAMSTATUS workResult = Ocelot.AddMGS2ToSteam();

            Ocelot.PrintToDebugConsole("[ STEAM ] Add2Steam has returned "+workResult.ToString());

            switch (workResult)
            {
                case ADD2STEAMSTATUS.AddedForOneUser:

                    Ocelot.showMessage("AddedForOneUser");

                    break;

                case ADD2STEAMSTATUS.AddedForMoreUsers:

                    Ocelot.showMessage("AddedForMoreUsers");

                    break;

                case ADD2STEAMSTATUS.NothingDone:

                    Ocelot.showMessage("NothingDone");

                    break;

                case ADD2STEAMSTATUS.CantFindNecessaryPaths:

                    Ocelot.showMessage("SteamNotFound");

                    break;

                default:

                    Ocelot.showMessage("Add2SteamError");

                    break;
            }

            // Don't open the fix after playing

            chb_FixAfterPlaying.Checked = false;

            AA_showNeededWarnings(true);

            // Save this inside the configuration .INI, even if the user didn't pressed the SAVE button

            load_SetupperConfig_SetTo_InternalConfig();

            Ocelot.load_InternalConfig_SetTo_INI();


            DialogResult startSteamAnswer = MessageBox.Show(
                "Do you want to launch Steam?", "Answer wisely", MessageBoxButtons.YesNo);

            if (startSteamAnswer == DialogResult.Yes) Ocelot.StartSteam();

            askToShowSteamArtworksPage();

        }
        
        // 'Steam artworks' button

        private void ptb_SteamArtworks_Click(object sender, EventArgs e)
        {
            askToShowSteamArtworksPage();
        }

        private void askToShowSteamArtworksPage()
        {
            DialogResult openPage = Ocelot.showMessage("ExplainSteamArtworksPage");

            if (openPage == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(Ocelot.GITHUB_WIKI_STEAMLIBRARYARTWORKS);
                }

                catch
                {
                    Ocelot.showMessage("UAC_error");
                }
            }
        }

        private void lbl_findSavegames_Click(object sender, EventArgs e)
        {
            Ocelot.PrintToDebugConsole("[!] lbl_findSavegames_Click");

            SAVEGAMEMOVING evaluationResult = Ocelot.SavegameMustBeMoved();

            Ocelot.PrintToDebugConsole("[!] SavegameMustBeMoved evaluation result is " + evaluationResult);

            string windowTitle = "SAVEDATA LOCATION";

            string rootGamePath = Application.StartupPath + "\\..";
            string originalSavegameFolder = Path.GetFullPath(rootGamePath + "\\savedata");

            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string retailNewSavegameFolderPath = Path.Combine(myDocumentsPath + "\\My Games\\METAL GEAR SOLID 2 SUBSTANCE");

            string evaluatedSavedataPath;

            if (evaluationResult == SAVEGAMEMOVING.IsAGOGInstallation)
            {
                evaluatedSavedataPath = originalSavegameFolder;

                MessageBox.Show(
                    "Game is using the original savedata location, as GOG would like" + "\n\n" +
                    "Your save data will be stored inside this folder:" + "\n\n" +
                    originalSavegameFolder,
                    windowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if(evaluationResult == SAVEGAMEMOVING.MovingPossible)
            {
                evaluatedSavedataPath = retailNewSavegameFolderPath;
                Ocelot.showMessage("savegameWillBeMoved");
            }
            else{

                evaluatedSavedataPath = retailNewSavegameFolderPath;

                MessageBox.Show(
                    "After the V's fix patching, savedata are stored inside 'My Games'" + "\n\n" +
                    "Your save data will be stored inside this folder:" + "\n\n" +
                    retailNewSavegameFolderPath,
                    windowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            // Try to open the folder

            try
            {

                if (Directory.Exists(evaluatedSavedataPath))
                {
                    System.Diagnostics.Process.Start(evaluatedSavedataPath);
                }

            }

            catch
            {
                // I can't open the folder? Who cares. (Yeah, I know what you're thinking right now...)
            }

        }

        #endregion

        //END CLASS
    }
}
