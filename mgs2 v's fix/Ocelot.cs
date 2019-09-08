using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Windows.Forms;
using System.Management;
using System.IO;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace mgs2_v_s_fix
{
    class Ocelot
    {

        // Internal version of the V's Fix - Format is YYMMDD
        public const string VERSION = "190331";

        // Hide background images and more "appariscent" graphical things
        public static bool NOSYMODE = false;

        // UPDATE

        public const string GITHUB_API = "https://api.github.com/repos/VFansss/mgs2-v-s-fix/releases/latest";
        public const string GITHUB_RELEASE = "https://github.com/VFansss/mgs2-v-s-fix/releases";
        public const string GITHUB_WIKI = "https://github.com/VFansss/mgs2-v-s-fix/wiki";
        public const string GITHUB_WIKI_INDEX = GITHUB_WIKI+"#chapters-of-the-guide";
        public const string GITHUB_WIKI_CONTROLS = GITHUB_WIKI + "/Controllers-&-Actions";

        // Contain Key-Value from Configuration_file.ini when 'Ocelot.load_INI_SetTo_InternalConfig' is called
        public static ConfSheet InternalConfiguration = new ConfSheet();

        // List of Graphics Adapter present in the machine
        public static LinkedList<string> vgaList= new LinkedList<string>();

        public static bool needOfAutoConfig = false;

        public static IniFile ConfFile = new IniFile("Configuration_file.ini");

        // debug mode flag

        public static bool debugMode = false;
        public static string debugMode_filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\MGS2_VFix_debug.txt";

        /// Main Menu Function

        public static void startGame()
        {
            Process cmd;

            try
            {
                cmd = Process.Start(new ProcessStartInfo("mgs2_sse.exe"));
                Application.Exit();
            }
            catch
            {
                // UAC Blocked?
                Ocelot.showMessage("UAC_error");
            }
            
        }

        public static void StartSteam()
        {
            Process cmd;

            try
            {
                string steamFolder = RetrieveSteamInstallationFolder();

                string steamExePath = steamFolder + "\\steam.exe";

                if (File.Exists(steamExePath))
                {
                    cmd = Process.Start(new ProcessStartInfo(steamExePath));
                }

                
            }
            catch
            {
                // UAC Blocked?
                Ocelot.showMessage("UAC_error");
            }
        }

        /// Settings Menu Function

        public static void checkConfFileIntegrity()
        {

            // If key field doesn't exist inside Configuration_file.ini
            // V's fix will create it
            // THEN flag Autoconfigurator to do something later

            // NB: Doesn't actually save any readed value

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Resolution)
            {
                if (!ConfFile.KeyExists(entry.Key, "Resolution"))
                {
                 ConfFile.Write(entry.Key, "", "Resolution");
                 needOfAutoConfig = true;
                }
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Controls)
            {
                if (!ConfFile.KeyExists(entry.Key, "Controls"))
                {
                    ConfFile.Write(entry.Key, "", "Controls");
                    needOfAutoConfig = true;
                }
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Graphics)
            {
                if (!ConfFile.KeyExists(entry.Key, "Graphics"))
                {
                    ConfFile.Write(entry.Key, "", "Graphics");
                    needOfAutoConfig = true;
                }
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Sound)
            {
                if (!ConfFile.KeyExists(entry.Key, "Sound"))
                {
                    ConfFile.Write(entry.Key, "", "Sound");
                    needOfAutoConfig = true;
                }
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Others)
            {
                if (!ConfFile.KeyExists(entry.Key, "Others"))
                {
                    ConfFile.Write(entry.Key, "", "Others");
                    needOfAutoConfig = true;
                }
            }

            // NB: Cheats category doesn't need to be checked

            if (needOfAutoConfig) { Ocelot.PrintToDebugConsole("[+] Configuration_file.ini seem missing some key. Need to autoconfig!"); }

            return;


        }

        // I don't think these 2 need to be edited frequently
        public static void load_INI_SetTo_InternalConfig()
        {

            // Time to pair Key-Value from Configuration_file.ini and
            //  set it to Ocelot.dataFromConfFile

            Ocelot.PrintToDebugConsole("[ ] .ini contain a valid configuration. Loading from it...");

            foreach(var entry in InternalConfiguration.Resolution.ToList())
            {
                
                var tempstring = ConfFile.Read(entry.Key.ToString(),"Resolution");
                InternalConfiguration.Resolution[entry.Key] = tempstring;

            }

            foreach (var entry in InternalConfiguration.Controls.ToList())
            {

                var tempstring = ConfFile.Read(entry.Key.ToString(), "Controls");
                InternalConfiguration.Controls[entry.Key] = tempstring;

            }

            foreach (var entry in InternalConfiguration.Graphics.ToList())
            {

                var tempstring = ConfFile.Read(entry.Key.ToString(), "Graphics");
                InternalConfiguration.Graphics[entry.Key] = tempstring;

            }

            foreach (var entry in InternalConfiguration.Sound.ToList())
            {

                var tempstring = ConfFile.Read(entry.Key.ToString(), "Sound");
                InternalConfiguration.Sound[entry.Key] = tempstring;

            }

            foreach (var entry in InternalConfiguration.Others.ToList())
            {

                var tempstring = ConfFile.Read(entry.Key.ToString(), "Others");
                InternalConfiguration.Others[entry.Key] = tempstring;

            }

            foreach (var entry in InternalConfiguration.Cheats.ToList())
            {

                var tempstring = ConfFile.Read(entry.Key.ToString(), "Cheats");
                InternalConfiguration.Cheats[entry.Key] = tempstring;

            }

            Ocelot.PrintToDebugConsole("[ ] Information from .ini succesfully loaded into setupper!");
            return;
        }

        internal static void load_InternalConfig_SetTo_INI()
        {

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Resolution)
            {

                ConfFile.Write(entry.Key, entry.Value, "Resolution");

            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Controls)
            {

                ConfFile.Write(entry.Key, entry.Value, "Controls");

            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Graphics)
            {

                ConfFile.Write(entry.Key, entry.Value, "Graphics");

            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Sound)
            {

                ConfFile.Write(entry.Key, entry.Value, "Sound");

            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Others)
            {

                ConfFile.Write(entry.Key, entry.Value, "Others");

            }

            // NB: Cheats category doesn't need to be written

            Ocelot.PrintToDebugConsole("[ ] InternalConfig succesfully exported into Configuration_file.ini");

            return;
        }

        // !!!! Big function that apply V's Fix settings !!!!!!!!!!!!!!!!!!!!!!!!!!!
        internal static void load_InternalConfig_SetTo_MGS()
        {
            PrintToDebugConsole("[ SET TO MGS METHOD ] Method starting...");

            // Delete already existing mgs.ini

            try
            {

                File.Delete(Application.StartupPath + "\\mgs2.ini");

                File.Create(Application.StartupPath + "\\mgs2.ini").Close();

                // NB: Operation are done following the usual pattern
                //  Resolution -> Controls -> Graphics -> Sound

                using (StreamWriter ini = new StreamWriter(Application.StartupPath + "\\mgs2.ini", true))
                {

                    byte[] ba;
                    StringBuilder hexString = new StringBuilder();
                    int opcode;

                    // PRELIMINARY ACTIONS

                    #region lot_of_things

                    // Extract DX Wrapper

                    File.Delete(Application.StartupPath + "\\d3d8.dll");

                    // 'enbconvertor.ini' is not used anymore, but I've retained its deletion
                    // to ensure backward compatibility
                    File.Delete(Application.StartupPath + "\\enbconvertor.ini");

                    Unzip.UnZippa("DXWrapper.zip",true);

                    // Extract fixed files for the "Green screen" bug (Issue #26), if not already there

                    if (!File.Exists("quartz.dll") || !File.Exists("winmm.dll"))
                    {
                        Unzip.UnZippa("GreenScreenFix.zip", true);
                    }

                    // Extract Ultimate ASI Loader, and folder for scripts
                    // ( Used for Widescreen Fix and Savegame Location Changer )

                    if (!File.Exists("msacm32.dll"))
                    {

                        Unzip.UnZippa("UltimateASILoader.zip", true);

                        Directory.CreateDirectory(Application.StartupPath + "\\scripts");  

                    }

                    // Move savegames from 'savedata' folder inside 'My Games', if needed

                    SAVEGAMEMOVING evaluationResult = Ocelot.SavegameMustBeMoved();

                    Ocelot.PrintToDebugConsole("[!] SavegameMustBeMoved evaluation result is " + evaluationResult);

                    if (evaluationResult == SAVEGAMEMOVING.NoSuccesfulEvaluationPerformed)
                    {
                        throw new Exception();

                    }

                    else if (evaluationResult == SAVEGAMEMOVING.MovingPossible)
                    {
                        MoveSavegamesToNewLocation();
                    }

                    else if (evaluationResult == SAVEGAMEMOVING.BothFolderExist)
                    {
                        throw new Exception();
                    }

                    else
                    {
                        // The last remaining evaluation is SAVEGAMEMOVING.NoSavegame2Move, and doesn't require any warning
                    }

                    // Extract SavegameLocationChanger.asi

                    if (File.Exists(Application.StartupPath + "\\scripts\\SavegameLocationChanger.asi"))
                    {

                        File.Delete(Application.StartupPath + "\\scripts\\SavegameLocationChanger.asi");

                    }

                    Unzip.UnZippa("SavegameLocationChanger.zip", true);

                    #endregion


                    ////// 
                    //--------- Resolution + .exe writing fixes
                    ////// 

                    #region lot_of_things
                    // Width

                    hexString.Clear();
                    hexString.Append(string.Format("{0:x}", Int32.Parse(Ocelot.InternalConfiguration.Resolution["Width"])).ToUpper());

                    while (hexString.Length < 4)
                    {
                        // Need 0 padding to left
                        hexString.Insert(0, 0);
                    }

                    ini.WriteLine("0003" + "\t" + hexString.ToString());

                    // Heigth

                    hexString.Clear();
                    hexString.Append(string.Format("{0:x}", Int32.Parse(Ocelot.InternalConfiguration.Resolution["Height"])).ToUpper());

                    while (hexString.Length < 4)
                    {
                        // Need 0 padding to left
                        hexString.Insert(0, 0);
                    }

                    ini.WriteLine("0004" + "\t" + hexString.ToString());

                    hexString.Clear();

                    // WideScreenFIX

                    // 0: delete all (if present) WideScreenFIX files

                    if (Directory.Exists(Application.StartupPath + "\\scripts"))
                    {
                        File.Delete(Application.StartupPath + "\\scripts\\fov.data");
                        File.Delete(Application.StartupPath + "\\scripts\\mgs2w.asi");
                        File.Delete(Application.StartupPath + "\\scripts\\mgs2w.ini");
                        File.Delete(Application.StartupPath + "\\dsound_x64.dll");
                        // This is an old library used in version <= 1.02
                        // better delete it, if present
                        File.Delete(Application.StartupPath + "\\winmmbase.dll");
                    }

                    if (Ocelot.InternalConfiguration.Resolution["WideScreenFIX"].Equals("true"))
                    {

                        // 1: WidescreenFix.zip must be extracted
                        // 2: Resolution must be set inside scripts/mgs2w.ini


                        // 1
                        Unzip.UnZippa("WidescreenFix.zip");

                        // 2

                        IniFile ws_ini = new IniFile(Application.StartupPath + "\\scripts\\mgs2w.ini");

                        ws_ini.Write("Width", Ocelot.InternalConfiguration.Resolution["Width"], "MAIN");
                        ws_ini.Write("Height", Ocelot.InternalConfiguration.Resolution["Height"], "MAIN");

                        if (Ocelot.InternalConfiguration.Resolution["FullscreenCutscene"].Equals("true"))
                        {
                            ws_ini.Write("cutscenes_top_black_border", "0", "MISC");
                            ws_ini.Write("cutscenes_bottom_black_border", "0", "MISC");
                        }
                        else
                        {
                            ws_ini.Write("cutscenes_top_black_border", "480", "MISC");
                            ws_ini.Write("cutscenes_bottom_black_border", "480", "MISC");
                        }

                        if (Ocelot.InternalConfiguration.Resolution["OptimizedFOV"].Equals("16:9"))
                        {
                            ws_ini.Write("custom_fov", "1", "MISC");
                        }
                        else
                        {
                            ws_ini.Write("custom_fov", "0", "MISC");
                        }

                    }

                    // GraphicAdapterName

                    hexString.Clear();
                    ba = Encoding.Default.GetBytes(Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"]);
                    hexString = new StringBuilder(BitConverter.ToString(ba));
                    hexString = hexString.Replace("-", "");

                    opcode = 70;

                    while (hexString.Length > 8)
                    {
                        ini.WriteLine("00" + opcode + "\t" + hexString.ToString().Substring(0, 8));

                        opcode++;
                        hexString = hexString.Remove(0, 8);
                    }

                    if (hexString.Length <= 8)
                    {

                        while (hexString.Length < 8)
                        {
                            // Need 0 padding to right
                            hexString = hexString.Insert(hexString.Length, 0);
                        }

                        ini.WriteLine("00" + opcode + "\t" + hexString.ToString());

                    }

                 
                    PrintToDebugConsole("[ EXE OPENING ] Open the game EXE for writing operations...");

                    
                    using (var stream = new FileStream(Application.StartupPath + "\\mgs2_sse.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        #region FIX FOR ATI/NVIDIA

                        // First things to do: sabotage certain API call

                        // P
                        stream.Position = 0x5FD83C;
                        stream.WriteByte(0x50);

                        // A
                        stream.Position = 0x5FD83D;
                        stream.WriteByte(0x41);

                        // C
                        stream.Position = 0x5FD83E;
                        stream.WriteByte(0x43);

                        // H
                        stream.Position = 0x5FD83F;
                        stream.WriteByte(0x48);

                        // I
                        stream.Position = 0x5FD840;
                        stream.WriteByte(0x49);

                        // N
                        stream.Position = 0x5FD841;
                        stream.WriteByte(0x4E);

                        // K
                        stream.Position = 0x5FD842;
                        stream.WriteByte(0x4B);

                        if (Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"].Contains("GeForc"))
                        {
                            // NVidia Card. Apply NVidia FIX

                            // G
                            stream.Position = 0x5FD834;
                            stream.WriteByte(0x47);

                            // e
                            stream.Position = 0x5FD835;
                            stream.WriteByte(0x65);

                            // F
                            stream.Position = 0x5FD836;
                            stream.WriteByte(0x46);

                            // o
                            stream.Position = 0x5FD837;
                            stream.WriteByte(0x6F);

                            // r
                            stream.Position = 0x5FD838;
                            stream.WriteByte(0x72);

                            // c
                            stream.Position = 0x5FD839;
                            stream.WriteByte(0x63);

                            PrintToDebugConsole("[ SET TO MGS METHOD ] NVIDIA card fix chosen");

                        }

                        else if (Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"].Contains("Radeon"))
                        {
                            // RADEON Card. Apply RADEON FIX

                            // R
                            stream.Position = 0x5FD834;
                            stream.WriteByte(0x52);

                            // a
                            stream.Position = 0x5FD835;
                            stream.WriteByte(0x61);

                            // d
                            stream.Position = 0x5FD836;
                            stream.WriteByte(0x64);

                            // e
                            stream.Position = 0x5FD837;
                            stream.WriteByte(0x65);

                            // o
                            stream.Position = 0x5FD838;
                            stream.WriteByte(0x6F);

                            // n
                            stream.Position = 0x5FD839;
                            stream.WriteByte(0x6E);

                            PrintToDebugConsole("[ SET TO MGS METHOD ] RADEON card fix chosen");

                        }

                        else if (Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"].Contains("Intel"))
                        {
                            // Intel Graphics. Apply Intel Fix

                            // I
                            stream.Position = 0x5FD834;
                            stream.WriteByte(0x49);

                            // N
                            stream.Position = 0x5FD835;
                            stream.WriteByte(0x6E);

                            // T
                            stream.Position = 0x5FD836;
                            stream.WriteByte(0x74);

                            // E
                            stream.Position = 0x5FD837;
                            stream.WriteByte(0x65);

                            // L
                            stream.Position = 0x5FD838;
                            stream.WriteByte(0x6C);

                            // (
                            stream.Position = 0x5FD839;
                            stream.WriteByte(0x28);

                            PrintToDebugConsole("[ SET TO MGS METHOD ] INTEL card fix chosen");

                        }

                        // FixAfterPlaying

                        PrintToDebugConsole("[ FixAfterPlaying ] Starting...");

                        // If is set to FALSE it will sabotage automatical V's Fix opening after game quit

                        if (Ocelot.InternalConfiguration.Sound["FixAfterPlaying"].Equals("true"))
                        {

                            // I want to open the fix after playing. Restoring original .exe condition

                            // 2
                            stream.Position = 0x60213E;
                            stream.WriteByte(0x32);

                        }

                        else
                        {
                            // Broking game .exe calling when game exit

                            // X
                            stream.Position = 0x60213E;
                            stream.WriteByte(0x58);
                        }

                        PrintToDebugConsole("[ FixAfterPlaying ] Done");

                        // Laptop FIX

                        // Patching mgs2_sse.exe for dual graphics machine

                        // This will fix shadow/water bug for those has an Intel integrated gpu AND are on a laptop
                        // THAT BUG OCCUR EVEN IF GAME WILL RUN ON A DEDICATED GRAPHICS

                        // NB: Incredibly, this patch doesn't need to be applied if SweetFX is used. Don't know why.

                        // March '18 Note - I tell you why: because if SweetFX was enabled the game use Dx8 to Dx9 wrapper.
                        // Now the wrapper is enabled by default, thus is not needed anymore to use the laptop fix button.

                        /*foreach (string vganame in vgaList)
                        {
                            if ((Ocelot.InternalConfiguration.Resolution["LaptopMode"] == "true") &&
                                (vganame.Contains("Intel")) &&
                                (Ocelot.InternalConfiguration.Graphics["AA"] == "false")
                                )
                            {

                                //Time to apply the fix

                                // I
                                stream.Position = 0x5FD834;
                                stream.WriteByte(0x49);

                                // N
                                stream.Position = 0x5FD835;
                                stream.WriteByte(0x6E);

                                // T
                                stream.Position = 0x5FD836;
                                stream.WriteByte(0x74);

                                // E
                                stream.Position = 0x5FD837;
                                stream.WriteByte(0x65);

                                // L
                                stream.Position = 0x5FD838;
                                stream.WriteByte(0x6C);

                                // (
                                stream.Position = 0x5FD839;
                                stream.WriteByte(0x28);

                            }

                        }*/

                        #endregion

                        #region CHEATS

                        PrintToDebugConsole("[ Cheats ] Appliying...");

                        // DREBIN MODE

                        Byte[] drebinMode_firstBatch;

                        if (Ocelot.InternalConfiguration.Cheats["DrebinMode"].Equals("true"))
                        {

                            drebinMode_firstBatch = new byte[] { 0x66, 0xB8, 0x0F, 0x27 };

                        }
                        else
                        {
                            // Restore the default values...

                            drebinMode_firstBatch = new byte[] { 0x0F, 0xBF, 0x04, 0x41 };

                        }

                        // DREBIN MODE - WRITING...

                        stream.Position = 0x0047E9CA;
                        stream.Write(drebinMode_firstBatch, 0, drebinMode_firstBatch.Length);

                        stream.Position = 0x0047E9DA;
                        stream.Write(drebinMode_firstBatch, 0, drebinMode_firstBatch.Length);

                        stream.Position = 0x0047E9EA;
                        stream.Write(drebinMode_firstBatch, 0, drebinMode_firstBatch.Length);

                        stream.Position = 0x0047E9FA;
                        stream.Write(drebinMode_firstBatch, 0, drebinMode_firstBatch.Length);

                        // UNLOCKED RADAR

                        Byte[] unlockRadar_firstBatch;
                        Byte[] unlockRadar_secondBatch;

                        if (Ocelot.InternalConfiguration.Cheats["UnlockRadar"].Equals("true"))
                        {
                            unlockRadar_firstBatch = new byte[] { 0x66, 0xB8, 0x00, 0x00, 0x90, 0x90, 0x90 };

                            unlockRadar_secondBatch = new byte[] { 0x90, 0x90};

                        }
                        else
                        {
                            unlockRadar_firstBatch = new byte[] { 0x66, 0x8B, 0x82, 0x1A, 0x01, 0x00, 0x00 };

                            unlockRadar_secondBatch = new byte[] { 0x75, 0x0D };
                        }

                        // RADAR DURING ALERT - WRITING...

                        stream.Position = 0x00441E4B;
                        stream.Write(unlockRadar_firstBatch, 0, unlockRadar_firstBatch.Length);

                        stream.Position = 0x00478BEE;
                        stream.Write(unlockRadar_secondBatch, 0, unlockRadar_secondBatch.Length);

                        #endregion

                    }



                    // WindowMode

                    if (Ocelot.InternalConfiguration.Resolution["WindowMode"].Equals("true"))
                    {
                        ini.WriteLine("0001" + "\t" + "0001");
                    }

                    ini.WriteLine("0005" + "\t" + "0001");

                    #endregion

                    ////// 
                    //--------- Controls
                    ////// 

                    #region lot_of_things

                    // 0: delete all (if present) existing XInputPlus files

                    File.Delete(Application.StartupPath + "\\Dinput.dll");
                    File.Delete(Application.StartupPath + "\\Dinput8.dll");
                    File.Delete(Application.StartupPath + "\\XInput1_3.dll");
                    File.Delete(Application.StartupPath + "\\XInputPlus.ini");

                    // What controller?

                    if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("NO"))
                    {
                        Unzip.UnZippa("NoController.zip", true);
                    }

                    else
                    {
                        // Extract XInput Plus
                        Unzip.UnZippa("XInputPlus.zip", true);

                        AvailableGamepads choosenGamepad;
                        AvailableLayouts choosenLayout;
                        bool InvertTriggersWithDorsals;

                        // What gamepad?
                        if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("DS4"))
                        {
                            choosenGamepad = AvailableGamepads.DUALSHOCK4;
                        }
                        else
                        {
                            choosenGamepad = AvailableGamepads.XBOX;

                        }

                        // What layout?
                        if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                        {
                            choosenLayout = AvailableLayouts.PS2;
                        }                
                        else
                        {
                            choosenLayout = AvailableLayouts.V;
                        }

                        // Invert triggers with dorsals?
                        if (Ocelot.InternalConfiguration.Controls["InvertTriggersWithDorsals"].Equals("true"))
                        {
                            InvertTriggersWithDorsals = true;
                        }
                        else
                        {
                            InvertTriggersWithDorsals = false;
                        }

                        // Write the button/analog bindings to files

                        ButtonLayouts myLayout = new ButtonLayouts(choosenGamepad, choosenLayout, InvertTriggersWithDorsals);

                        // Declare a UTF-8 Encoding WITHOUT BOM (VERY IMPORTANT!)

                        Encoding utf8WithoutBom = new UTF8Encoding(false);

                        // Write padbtn.ini

                        using (StreamWriter writetext = new StreamWriter(Application.StartupPath + "\\padbtn.ini",false, utf8WithoutBom))
                        {
                            foreach ((ButtonActions, string) singleBinding in myLayout.ButtonBindings)
                            {
                                // Example row: 00  A
                                writetext.WriteLine(singleBinding.Item2+ "  "+singleBinding.Item1);
                            }
                            
                        }

                        // Write padbtns.ini

                        using (StreamWriter writetext = new StreamWriter(Application.StartupPath + "\\padbtns.ini", false, utf8WithoutBom))
                        {
                            foreach ((ButtonActions, string) singleBinding in myLayout.ButtonBindings)
                            {
                                // Example row:00  A
                                writetext.WriteLine(singleBinding.Item2 + "  " + singleBinding.Item1);
                            }

                        }

                        // Write padana.ini

                        using (StreamWriter writetext = new StreamWriter(Application.StartupPath + "\\padana.ini", false, utf8WithoutBom))
                        {
                            foreach ((string, AnalogActions, string) singleBinding in myLayout.AnalogBindings)
                            {
                                // Example row:00  Rx  N
                                writetext.WriteLine(singleBinding.Item1 + "  " + singleBinding.Item2+ "  "+singleBinding.Item3);
                            }

                        }

                        // Write padanas.ini

                        using (StreamWriter writetext = new StreamWriter(Application.StartupPath + "\\padanas.ini", false, utf8WithoutBom))
                        {
                            foreach ((string, AnalogActions, string) singleBinding in myLayout.AnalogBindings)
                            {
                                // Example row:00  Rx  N
                                writetext.WriteLine(singleBinding.Item1 + "  " + singleBinding.Item2 + "  " + singleBinding.Item3);
                            }

                        }


                    }

                    #endregion

                    ////// 
                    //--------- Graphics
                    //////

                    #region lot_of_things

                    // ! Important thing !
                    //  These switch can be optimized with clever use of 'break'
                    //   and removing some redundant 'case'
                    //    I choosed to not do it cause I think it's much more simple to
                    //     overhaul and mantain it in second time.

                    // RenderingSize

                    switch (Ocelot.InternalConfiguration.Graphics["RenderingSize"])
                    {
                        case "low":
                            ini.WriteLine("0006" + "\t" + "0200");
                            ini.WriteLine("0007" + "\t" + "0100");
                            break;

                        case "medium":
                            ini.WriteLine("0006" + "\t" + "0400");
                            ini.WriteLine("0007" + "\t" + "0200");
                            break;

                        case "high":
                            ini.WriteLine("0006" + "\t" + "0800");
                            ini.WriteLine("0007" + "\t" + "0400");
                            break;

                    }

                    // ShadowDetail

                    switch (Ocelot.InternalConfiguration.Graphics["ShadowDetail"])
                    {
                        case "low":
                            ini.WriteLine("0031" + "\t" + "0020");
                            ini.WriteLine("0032" + "\t" + "0020");
                            break;

                        case "medium":
                            ini.WriteLine("0031" + "\t" + "0100");
                            ini.WriteLine("0032" + "\t" + "0100");
                            ini.WriteLine("0040" + "\t" + "0001");
                            break;

                        case "high":
                            ini.WriteLine("0031" + "\t" + "0200");
                            ini.WriteLine("0032" + "\t" + "0200");
                            ini.WriteLine("0040" + "\t" + "0001");
                            break;

                    }

                    // ModelQuality

                    switch (Ocelot.InternalConfiguration.Graphics["ModelQuality"])
                    {
                        case "low":
                            // Do nothing
                            break;

                        case "medium":
                            ini.WriteLine("0010" + "\t" + "0001");
                            break;

                        case "high":
                            ini.WriteLine("0010" + "\t" + "0001");
                            //NB: These 2 can be dangerous on some config. Watch out.
                            ini.WriteLine("0020" + "\t" + "0001");
                            ini.WriteLine("0021" + "\t" + "0001");
                            break;

                    }

                    // RenderingClearness

                    switch (Ocelot.InternalConfiguration.Graphics["RenderingClearness"])
                    {
                        case "low":
                            ini.WriteLine("0034" + "\t" + "F000");
                            ini.WriteLine("0035" + "\t" + "F000");
                            break;

                        case "medium":
                            ini.WriteLine("0034" + "\t" + "7000");
                            ini.WriteLine("0035" + "\t" + "7000");
                            break;

                        case "high":
                            // Do nothing
                            break;

                    }

                    // EffectQuantity

                    switch (Ocelot.InternalConfiguration.Graphics["EffectQuantity"])
                    {
                        case "low":
                            // below is CrossFade opcode
                            ini.WriteLine("0033" + "\t" + "0007");
                            break;

                        case "medium":
                            ini.WriteLine("0048" + "\t" + "0090");
                            ini.WriteLine("0049" + "\t" + "0090");
                            // below are CrossFade opcode
                            ini.WriteLine("0033" + "\t" + "0007");
                            ini.WriteLine("0047" + "\t" + "0001");
                            break;

                        case "high":
                            ini.WriteLine("0048" + "\t" + "0100");
                            ini.WriteLine("0049" + "\t" + "0100");
                            // below are CrossFade opcode
                            ini.WriteLine("0047" + "\t" + "0001");
                            break;

                    }

                    // BunchOfCoolEffect

                    if (Ocelot.InternalConfiguration.Graphics["BunchOfCoolEffect"].Equals("true"))
                    {
                        ini.WriteLine("0041" + "\t" + "0001"); // Stealth Effect
                        ini.WriteLine("0043" + "\t" + "0001"); // Codec Focus
                        ini.WriteLine("004A" + "\t" + "0001"); // VR MODE
                    }

                    // MotionBlur

                    if (Ocelot.InternalConfiguration.Graphics["MotionBlur"].Equals("true"))
                    {
                        ini.WriteLine("0044" + "\t" + "0001");
                        ini.WriteLine("0045" + "\t" + "0001");
                    }

                    // DepthOfField

                    if (Ocelot.InternalConfiguration.Graphics["DepthOfField"].Equals("true"))
                    {
                        ini.WriteLine("0042" + "\t" + "0001"); // Focus - Depth of Field
                    }

                    // AA

                    // 0: delete all (if present) existing SweetFX files/directory

                    if (Directory.Exists(Application.StartupPath + "\\SweetFX"))
                    {
                        Directory.Delete(Application.StartupPath + "\\SweetFX", true);
                    }

                    File.Delete(Application.StartupPath + "\\SweetFX_settings.txt");
                    File.Delete(Application.StartupPath + "\\SweetFX_preset.txt");
                    //File.Delete(Application.StartupPath + "\\enbconvertor.ini");
                    File.Delete(Application.StartupPath + "\\dxgi.dll");
                    File.Delete(Application.StartupPath + "\\d3d9.dll");
                    //File.Delete(Application.StartupPath + "\\d3d8.dll");

                    if (Ocelot.InternalConfiguration.Graphics["AA"].Equals("smaa"))
                    {

                        Unzip.UnZippa("V_s_sweetFX.zip");
                        Unzip.UnZippa("sweetFX_SMAA.zip");

                    }
                    else if (Ocelot.InternalConfiguration.Graphics["AA"].Equals("fxaa"))
                    {
                        Unzip.UnZippa("V_s_sweetFX.zip");
                        Unzip.UnZippa("sweetFX_FXAA.zip");
                    }
                    {
                        // No anti-aliasing today, baby
                    }

                    #endregion

                    ////// 
                    //--------- Sound
                    ////// 

                    #region lot_of_things

                    // SoundAdapterName

                    hexString.Clear();
                    ba = Encoding.Default.GetBytes(Ocelot.InternalConfiguration.Sound["SoundAdapterName"]);
                    hexString = new StringBuilder(BitConverter.ToString(ba));
                    hexString = hexString.Replace("-", "");

                    opcode = 90;

                    while (hexString.Length > 8)
                    {
                        ini.WriteLine("00" + opcode + "\t" + hexString.ToString().Substring(0, 8));

                        opcode++;
                        hexString = hexString.Remove(0, 8);
                    }

                    if (hexString.Length <= 8)
                    {

                        while (hexString.Length < 8)
                        {
                            // Need 0 padding to right
                            hexString = hexString.Insert(hexString.Length, 0);
                        }

                        ini.WriteLine("00" + opcode + "\t" + hexString.ToString());

                    }

                    // Quality

                    switch (Ocelot.InternalConfiguration.Sound["Quality"])
                    {
                        case "low":
                            // Nothing
                            break;

                        case "medium":
                            ini.WriteLine("009D" + "\t" + "0001");
                            break;

                        case "high":
                            ini.WriteLine("009D" + "\t" + "0002");
                            break;

                    }

                    // SE

                    switch (Ocelot.InternalConfiguration.Sound["SE"])
                    {
                        case "low":
                            // Nothing
                            break;

                        case "medium":
                            ini.WriteLine("009E" + "\t" + "0005");
                            break;

                        case "high":
                            ini.WriteLine("009E" + "\t" + "0009");
                            break;

                    }

                    // Sound Quality

                    switch (Ocelot.InternalConfiguration.Sound["SE"])
                    {
                        case "low":
                            // Nothing
                            break;

                        case "medium":
                            ini.WriteLine("009F" + "\t" + "0001");
                            break;

                        case "high":
                            ini.WriteLine("009F" + "\t" + "0002");
                            break;

                    }

                    #endregion

                    ////// 
                    //--------- Extra Opcode
                    ////// 

                    // No idea what they do.
                    //  Pachinko Opcode?

                    ini.WriteLine("0002" + "\t" + "0001");
                    ini.WriteLine("0030" + "\t" + "0004");
                    ini.WriteLine("0046" + "\t" + "0001");

                    //End Using
                }

                /*

                // Avoid strange Windows UAC issues setting full
                // access on MGS2 files from every windows user

                string homeDirectory = Directory.GetParent(Application.StartupPath).ToString();

                // Create the savedata folder,if not already exist

                Directory.CreateDirectory(homeDirectory+"\\savedata");

                // Grant access to everyone, if possible

                bool success = grantAccessToEveryUser(homeDirectory);

                Ocelot.console("[ ] grantAccess has returned "+success);

                */

                // DEPRECATED FROM VERSION 1.7 - Set the compatibility flags

                //SetCompatibilityFlags();

                if (Ocelot.CompatibilityFlagsExists())
                {
                    Ocelot.RemoveCompatibilityFlags();
                }

                // Create a file to remember the user to check to new location

                string savedataReminderFilePath = Directory.GetParent(Application.StartupPath).FullName + "\\SAVEDATA ARE INSIDE 'MY GAMES' FOLDER";

                if (File.Exists(savedataReminderFilePath))
                {

                    PrintToDebugConsole("[ SavedataReminder ] File already exist");

                }
                else
                {

                    PrintToDebugConsole("[ SavedataReminder ] File created!");

                    File.Create(savedataReminderFilePath);

                }
         
            }

            catch(Exception ex)
            {
                PrintToDebugConsole("[ EXCEPTION ] Message:"+ex.Message+"\n\nStacktrace: "+ex.StackTrace);
                Ocelot.showMessage("UAC_error");

            }

            Ocelot.PrintToDebugConsole("[ ] InternalConfig succesfully exported into mgs2.ini");

            if (Ocelot.debugMode)
            {
                Ocelot.debug_printInternalConfig();
            }

            return;
        }

        /// Autoconfig

        public static void startAutoconfig()
        {

            Ocelot.PrintToDebugConsole("[ ] Autoconfig started. I'm looking for a nice config...");
            ConfSheet defaultConfig = new ConfSheet();

            // Resolution
            defaultConfig.Resolution["Height"] = Screen.PrimaryScreen.Bounds.Size.Height.ToString();
            defaultConfig.Resolution["Width"] = Screen.PrimaryScreen.Bounds.Size.Width.ToString(); ;

            System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;


            double rapporto = (Double.Parse(defaultConfig.Resolution["Width"]) / Double.Parse(defaultConfig.Resolution["Height"]));

            // NB: This is replied in Form1.checkIfWSElegible

            if (
                (rapporto == 1.6d) ||
                (rapporto == 1.7777777777777777d) ||
                (rapporto == 1.7786458333333333d) ||
                (rapporto == 2.3703703703703703d) ||
                (rapporto == 2.3888888888888888d)
                )
            {   
                defaultConfig.Resolution["WideScreenFIX"] = "true";
            }
            else
            {
                defaultConfig.Resolution["WideScreenFIX"] = "false";
            };

            // Set 16:9 optimized FOV multiplier

            if (rapporto == 1.7777777777777777d) // If 16:9...
            {
                defaultConfig.Resolution["OptimizedFOV"] = "16:9";
            }
            else
            {
                defaultConfig.Resolution["OptimizedFOV"] = "16:9";
            };

            Ocelot.getGraphicsAdapterList();

            // If there's more than one vga... 
            //  V's Policy: by default set game to Intel Integrated (if present)
            //   Otherwise....to the last retrivied adapter

            foreach (string vganame in vgaList)
            {
                if(vganame.Contains("Intel")){
                    defaultConfig.Resolution["GraphicAdapterName"]= vganame;
                    break;
                }

                defaultConfig.Resolution["GraphicAdapterName"] = vganame;

            }

            Ocelot.PrintToDebugConsole("[!] -VVV- No VGA Selected.");

            if(vgaList.Count == 0)
            {
                // No found VGA on the system. User has inserted one manually into configuration ini file?

                string explicitedVGAName = "";

                if (ConfFile.KeyExists("GraphicAdapterName", "Resolution"))
                {
                    explicitedVGAName = ConfFile.Read("GraphicAdapterName", "Resolution").ToString();
                }

                if (explicitedVGAName != null && !explicitedVGAName.Equals("") && explicitedVGAName.Length > 0)
                {
                    // Seems that user has done its job!
                    vgaList.AddLast(explicitedVGAName);

                    defaultConfig.Resolution["GraphicAdapterName"] = explicitedVGAName;

                    Ocelot.PrintToDebugConsole("[!] -AUTOCONFIG- Found a manual inserted one: " + explicitedVGAName);
                }

            }

            else if (vgaList.Count > 1)
            {

                Ocelot.showMessage("tip_moreVGAs");
                
            }


            defaultConfig.Resolution["WindowMode"] = "false";

            // Detect if PC is laptop or a desktop
            // NOT 100% true,however
            if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
            {
                defaultConfig.Resolution["LaptopMode"] = "false";
            }
            else
            {
                defaultConfig.Resolution["LaptopMode"] = "true";
            }

            defaultConfig.Resolution["FullscreenCutscene"] = "false";

            // Controls

            //defaultConfig.Controls["XboxGamepad"] = "NO";
            defaultConfig.Controls["EnableController"] = "NO";
            defaultConfig.Controls["PreferredLayout"] = "V";
            defaultConfig.Controls["InvertTriggersWithDorsals"] = "false";

            // Graphics

            defaultConfig.Graphics["RenderingSize"]="high";
            defaultConfig.Graphics["ShadowDetail"] = "high";
            defaultConfig.Graphics["ModelQuality"] = "medium";
            defaultConfig.Graphics["RenderingClearness"] = "high";
            defaultConfig.Graphics["EffectQuantity"] = "high";
            defaultConfig.Graphics["BunchOfCoolEffect"] = "true";
            defaultConfig.Graphics["MotionBlur"] = "true";
            defaultConfig.Graphics["AA"] = "false";
            defaultConfig.Graphics["DepthOfField"] = "true";

            // Sound

            defaultConfig.Sound["SoundAdapterName"]="Primary Sound Driver";
            defaultConfig.Sound["Quality"]="high";
            defaultConfig.Sound["SE"]="high";
            defaultConfig.Sound["SoundQuality"]="high";
            defaultConfig.Sound["FixAfterPlaying"] = "true";

            // Others

            defaultConfig.Others["CompatibilityWarningDisplayed"] = "false";
                
            // Finished!

            InternalConfiguration = defaultConfig;

            Ocelot.PrintToDebugConsole("[+] Autoconfig finished. InternalConfiguration Filled");

            Ocelot.needOfAutoConfig = false;

            return;
        }

        public static void getGraphicsAdapterList()
        {
            // Reset the VGA list
            vgaList.Clear();

            // Issues during retrieving?
            bool notYetSnake = false;

            try // Method 1
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    PropertyData currentBitsPerPixel = mo.Properties["CurrentBitsPerPixel"];
                    PropertyData description = mo.Properties["Description"];

                    vgaList.AddLast(description.Value.ToString());

                }

            }

            catch
            {
                // Try another method
                notYetSnake = true;
            }

            if (notYetSnake)
            {
                notYetSnake = false;
                vgaList.Clear();

                try // Method 2
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayControllerConfiguration");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        PropertyData currentBitsPerPixel = mo.Properties["CurrentBitsPerPixel"];
                        PropertyData description = mo.Properties["Description"];

                        vgaList.AddLast(description.Value.ToString());
                    }

                }

                catch
                {
                    // Try another method
                    notYetSnake = true;
                }

            }

            if (notYetSnake)
            {
                vgaList.Clear();

                // I give up. A message will be prompt to the user

            }


        }

        // Legacy method: Too risky - Make MGS2 files readable by every user on the system
        private static bool grantAccessToEveryUser(string fullPath)
        {

            bool success = false;

            try{

                DirectorySecurity sec = Directory.GetAccessControl(fullPath);
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                Directory.SetAccessControl(fullPath, sec);

                success = true;

            }

            catch
            {
                success = false;
            }

            return success;

        }

        // show a MessageBox with custom message based on a string code

        public static DialogResult showMessage(string code)
        {
            DialogResult answer = DialogResult.OK;

            switch (code)
            {

                // COMPATIBILITY FLAGS INFO    

                case "compatibilityFlagsNotNeeded":

                    answer = MessageBox.Show(
                    "From this version of the Fix, the game doesn't need any compatibility flags anymore"+"\n\n"+
                    "This will greatly enhance the game compatibility, expecially with Win10 and Steam!"+"\n\n"+
                    "Next time you press 'SAVE', these compatibility flags will be automatically removed."+"\n\n"+
                    "Happy playing, and have fun :)",
                    "Improvement incoming...", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "compatibilityWarning": // NB: Not used anymore

                    answer = MessageBox.Show(
                    "When applying your settings, V's Fix will automatically try to set these compatibility flags :" +
                    "\n\n" +
                    "- Run the game in WindowsXP SP3 Compatibility Mode" + "\n" +
                    "- Execute the game with Admin rights" +
                    "\n\n" +
                    "to the main game executable (mgs2_sse.exe) but it may fail or be blocked by various actors, so please be sure that they have been succesfully activated!" +
                    "\n\n" +
                    "Running the game without these flags WILL results in a BLACK SCREEN ON GAME STARTUP, or others gamebreaking issues." +
                    "\n\n" +
                    "You can read again this message once again from the 'Resolution tab'." + "\n",
                    "*Suddenly green hills appear in the background*", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                // GENERAL INFOS

                case "savegameWillBeMoved":

                    answer = MessageBox.Show(
                    "From the next time you press 'SAVE', the V's Fix will patch the game to search savedata inside the 'My Games' folder!" + "\n\n" +
                    "This will also enhance game compatibility with modern systems!" + "\n\n"+
                    "Your save data will be stored inside this folder:" + "\n\n"+
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\METAL GEAR SOLID 2 SUBSTANCE",
                    "Improvement incoming...", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "savegameCantBeMoved":

                    string oldFolderPath = Directory.GetParent(Application.StartupPath).FullName+"\\savedata";
                    string newFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\METAL GEAR SOLID 2 SUBSTANCE";

                    answer = MessageBox.Show(
                    "This version of the V's Fix will patch the game to search savedata inside 'My Documents\\My Games'" + "\n\n" +
                    "The fix has also detected that you could move savedata from old directory to new, but a folder already exists in the new location."+"\n\n"+
                    "I don't know what are your most recent savedata so please delete one of the following folder:"+ "\n\n" +
                    oldFolderPath + "\n\n"+
                    "( or )" + "\n\n" +
                    newFolderPath + "" + "\n\n"+
                    "Please manually fix this situation, and reopen the fix."+"\n\n"+
                    "Now the fix is going to close. Sorry dude :(",
                    "Trouble incoming...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "gameNeverConfigured":

                    answer = MessageBox.Show(
                    "To start the game, you have to configure it at least once!"+"\n\n"+
                    "Please press 'SETTINGS' and configure the game, then retry :)",
                    "And that's why it doesn't work...", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                // UPDATE messages

                case "update_crashedinfire":

                    answer = MessageBox.Show(
                    "Can't reach GitHub for updates (Are you offline?)",
                    "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "update_noupdates":

                    answer = MessageBox.Show(
                    "Seems that there aren't any updates for the V's Fix.\n\nHappy playing :)",
                    "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "update_available":

                    answer = MessageBox.Show(
                    "It seems that someone has actually worked!\n\nPress 'OK' to open the V's Fix GitHub Release page to download the latest release!",
                    "UPDATE AVAILABLE!", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    break;

                // DEBUG Mode messages

                case "debugModeEnabled":

                    answer = MessageBox.Show(
                    "Debug mode is ENABLED!"+"\n\n"+
                    "You can find the debug log on your Desktop"+ "\n\n" +
                    "You can analize it by yourself and/or report it to me on GitHub!",
                    "A dud!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "debugModeDisabled":

                    answer = MessageBox.Show(
                    "Debug mode is DISABLED!" + "\n\n" +
                    "Please go to V's Fix Wiki - Chapter 'Troubleshooting & Debug mode'" + "\n\n" +
                    "to learn how to enable debug mode and let us understand better what is going on!",
                    "A dud!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                // FATAL ERROR(s) WHILE STARTING THE GAME

                case "fatalError_WrongVideoAdapter":

                    answer = MessageBox.Show(
                    "V's has detected that your game has started with a different VGA from the one selected from the V's Fix."+"\n\n"+
                    "This can cause glitches and bugs."+"\n\n"+
                    "V's Fix can't solve this for you, so to quickly solve the issue (in less than 30 seconds) please read the V's Fix manual"+"\n\n"+
                    "Chapter: Settings Menu - Resolution tab\n\nParagraph: 6 - Graphical Adapter"+"\n\n"+
                    "Do you want to open the V's Fix Wiki on that page?",
                    "Helper in action...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    break;

                // ERROR MESSAGE WHILE USING FIX

                case "wrong_folder_error":

                    // What a dumb user. Fix started from the wrong folder.

                    #region Searching in registry the game path

                    bool directory_found = false;
                    string possible_path = "";
                    string extra_hint = "";

                    try{

                        // I have only 2 idea 

                        possible_path = "HKEY_CURRENT_USER\\SOFTWARE\\KONAMI\\MGS2S";
                        possible_path = (string)Registry.GetValue(possible_path, "InstallDir", new Object().ToString());

                        if (Directory.Exists(possible_path + "\\bin"))
                        {
                            directory_found = true;
                        }

                        else
                        {
                            possible_path = "HKEY_LOCAL_MACHINE\\SOFTWARE\\KONAMI\\MGS2S";
                            possible_path = (string)Registry.GetValue(possible_path, "InstallDir", new Object().ToString());

                            if (Directory.Exists(possible_path + "\\bin"))
                            {
                                directory_found = true;
                            }

                        }

                    }
                    catch{

                        // Ok, at least I've tried. No need to manage anything

                    }

                    #endregion

                    finally{

                        if (directory_found)
                        {
                            extra_hint = "Doing some magic I've discovered that you have to put MGS2SSetup.exe inside:\n\n" + possible_path+"\\bin"+"\n\nDon't waste time and do it now!";
                        }

                        else
                        {
                            extra_hint = "I've tried (I swear) but I didn't found myself the install directory of the game. Check the manual on GitHub for extra help.";
                        }

                    answer = MessageBox.Show(
                    "V's Fix isn't in the correct place.\nPut it into GAME DIRECTORY\\bin folder\n\n"+extra_hint,
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                    break;

                case "unzipping_error":

                    answer = MessageBox.Show(
                    "V's Fix isn't able to create some files into game's directory.\nFix require full permission to create,delete and extract file into that directory. Try to run the fix using 'Admin rights'!",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "no_vga":

                    answer = MessageBox.Show(
                    "V's hasn't found any VGA installed in your system.\n\nIf you are able to read this, is probably wrong.\n\nUnfortunatelly you must insert your VGA name manually.\n\nPlease read the V's Fix manual\n\nChapter: Settings Menu - Resolution tab\n\nParagraph: 6 - Graphical Adapter \n\n for an easy workaround.\n\nClosing this message will open your browser pointing to V's Guide.\n\nWould you kindly press the 'OK' button?",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "no_donate":

                    answer = MessageBox.Show(
                    "V's Fix isn't able to open PayPal website! Please don't give up :(",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "UAC_error":

                    answer = MessageBox.Show("Some operation has been blocked by operating system :( \n\n" +
                        "You have few things you can do:" + "\n\n" +
                        "I) Start the fix using 'Admin rights'" + "\n\n"+
                        "II) Install game in another directory that isn't 'Program Files'"+ "\n\n" +
                        "III) Ensure that you have enough read/write permissions on the game folder" + "\n\n" +
                        "If you can't solve in any way, you have to choose the last option:"+"\n\n"+
                        "IV) Start the V's Fix in 'debug mode'\n\n" +
                        "   a) creating a \"debug.sss\" file (without quote) inside your game folder\n" +
                        "OR\n" +
                        "   b) starting MGS2SSetup.exe with -debug argument\n\n" +
                        "and see the log saved in your desktop (that you could also send to me for troubleshooting)" + "\n\n" +
                        "Please read the V's Fix Wiki - Chapter 'Troubleshooting & Debug mode' for extra info",
                    "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "forbidStartIsTrue":

                    answer = MessageBox.Show("Not all preliminary actions has been completed by V's Fix, and is a VERY bad thing: V's Fix cannot start without, and thus it will close after these messages :(",
                    "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                // STEAM

                case "steamIsRunning":

                    answer = MessageBox.Show(
                        "Steam is actually running!"+
                        "\n\n"+
                        "To let V's Fix work safely, is better to close it before proceeding."
                        +"\n\n"+
                        "Please close it manually and try again :)");

                    break;

                case "AddedForOneUser":

                    answer = MessageBox.Show(
                        "MGS2 has been added for one Steam user!"+
                        "\n\n"+
                        "Start Steam, and have fun :D","Yeah"                 
                        );

                    break;

                case "AddedForMoreUsers":

                    answer = MessageBox.Show(
                        "MGS2 has been added for more Steam users!" +
                        "\n\n" +
                        "Start Steam, and have fun :D", "Yeah"
                        );

                    break;

                case "NothingDone":

                    answer = MessageBox.Show(
                        "Seems that MGS2 has already been added in the past."+
                        "\n\n"+
                        "( ????? )"+
                        "\n\n" +
                        "If you want to make V's Fix re-add the game, please delete it manually and launch this another time!"
                        );

                    break;

                case "SteamNotFound":

                    answer = MessageBox.Show(
                        "V's Fix has found that Steam is not installed on your system." +
                        "\n\n" +
                        "If you don't think the same, please activate the DEBUG MODE and report this to me!"
                        );

                    break;

                // TIPS

                case "tip_antialiasingANDmodelquality":

                    answer = MessageBox.Show(
                        "Model quality is set to 'High' and Anti-Aliasing is activated."+
                        "\n\n"+
                        "Having both activated at the same time can, on some configuration, cause graphical glitches or freezes."+
                        "\n\n"+
                        "If you have these problems during the game, please deactivate one of the two things mentioned above."
                        + "\n\n"+
                        "This message will not show up until a next fix reboot, so consider yourself warned :D",
                    "Please read carefully", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_smaaANDsteam":

                    answer = MessageBox.Show(
                    "SMAA Anti-Aliasing is activated, and this can cause glitches with the Steam overlay, and consequently with the Steam controller(s)" +
                    "\n\n" +
                    "For this reason, FXAA Anti-Aliasing has been selected instead."+
                    "\n\n" +
                    "( HINT: If you want SMAA at all costs, unselect 'Steam' from the 'Controls' tab of the fix, or enable it manually from 'SweetFX_settings.txt' after you press 'SAVE' )",
                    "Please read carefully", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_patcher":

                    answer = MessageBox.Show(
                    "V's Fix will now run some extra applications for patching the game into 2.0 Version.\n\nOn some system it can prompt an UAC warning.\nIsn't doing anything harmful; let it do its job!",
                    "Just an info...", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_moreVGAs":

                    answer = MessageBox.Show(
                    "Please be aware that since more VGAs are installed on your system you have to be sure that the executable of the game (mgs2_sse.exe) is bounded correctly to the right graphics adapter!" +
                    "\n\n" +
                    "This MUST be done MANUALLY from your graphics adapter's control panel!"+
                    "\n\n" +
                    "To better understand why, please press the link on the right of the red ! sign on 'Resolution tab' ",
                    "More graphics adapter detected!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_explainSelectionForVGAs":

                    answer = MessageBox.Show(
                    "To make the game work flawlessly, V's Fix must know your video graphic adapters (VGA) model/brand to apply certain fixes to the game."+
                    "\n\n"+
                    "Below, you can find the list of detected VGAs on your system."+
                    "\n\n"+
                    "If you have a SINGLE VGA ON YOUR PC, you can stop reading here :D"+
                    "\n\n"+
                    "If you have MORE THAN ONE VGA ON YOUR PC, below you can select the one that will run the game." +
                    "\n\n"+
                    "In that case, simply selecting a VGA from the fix MAY not be enough, though."+
                    "\n\n"+
                    "Some 'power saving' settings from your VGAs driver could interfere with that decision, based on how the driver has decided to run the game (i.e. on your laptop integrated GPU to save power)"+
                    "\n\n"+
                    "You have to be sure that your VGA driver is reflecting the decision you made below, and this MUST be done MANUALLY from your ATI/Intel/NVidia VGA control panel!"+
                    "V's Fix can't do it for you, unfortunatelly!"+
                    "\n\n"+
                    "If you never done it before, on the V's Fix manual I wrote some examples (with images) for various VGA brands!" +
                    "\n\n" +
                    "Chapter: Settings Menu - Resolution tab\n\nParagraph: 6 - Graphical Adapter" + "\n\n" +
                    "Do you want to open the V's Fix Wiki on that page?",
                    "Estimated time for reading: 1 minute", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    break;

                // DEFAULT MESSAGE

                default:

                    answer = MessageBox.Show("You shouldn't be able to read this message: /",
                    "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            Ocelot.PrintToDebugConsole("[!] Showed a messagebox with code: "+code);

            return answer;

        }

        // NOT USED ANYMORE - Apply the 'Windows XP SP3 Compatibility Mode' and 'Run as Admin' flags

        private static void SetCompatibilityFlags()
        {

            try
            {
                string registry_path = "Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\";
                string game_exe_path = Application.StartupPath + "\\mgs2_sse.exe";
                Microsoft.Win32.RegistryKey key;

                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registry_path);

                key.SetValue(game_exe_path, "~ RUNASADMIN WINXPSP3", Microsoft.Win32.RegistryValueKind.String);
                key.Close();

                PrintToDebugConsole("[ :) ] Compatibility flags set!");

            }

            catch(Exception ex)
            {
                // Signal to debugger

                Ocelot.PrintToDebugConsole("[ EXCEPTION ] " + ex.Message);

                PrintToDebugConsole("[ :( ] Exception while setting compatibility flags!");

            }

        }

        // Check if any compatibility flag is set
        public static bool CompatibilityFlagsExists()
        {
            // Set a default value
            bool returnValue = false;

            PrintToDebugConsole("[C.FLAGS CHECK] Checking for compatibility flags...");

            try
            {
                string registry_path = "Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\";
                string game_exe_path = Application.StartupPath + "\\mgs2_sse.exe";
                Microsoft.Win32.RegistryKey key;

                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registry_path);

                object retrievedValue = key.GetValue(game_exe_path, null);

                if (retrievedValue != null)
                {
                    PrintToDebugConsole("[C.FLAGS CHECK] Compatibility flags found: " + retrievedValue.ToString());

                    if (!retrievedValue.ToString().Equals("HIGHDPIAWARE") && !retrievedValue.ToString().Equals("$ ElevateCreateProcess HIGHDPIAWARE"))
                    {

                        // Somethings there. Cast value to string
                        string actualValue = retrievedValue.ToString();

                        key.Close();

                        // Set the return value for method caller
                        returnValue = true;

                    }

                }
                else
                {
                    PrintToDebugConsole("[C.FLAGS CHECK] Compatibility flags NOT found :)");
                }

            }

            catch(Exception ex)
            {
                // Signal to debugger

                PrintToDebugConsole("[ EXCEPTION ] " + ex.Message);

                PrintToDebugConsole("[C.FLAGS CHECK] Exception while checking compatibility flags!");

            }

            return returnValue;

        }

        // Remove compatibility flags, if any
        private static void RemoveCompatibilityFlags()
        {
            PrintToDebugConsole("[C.FLAGS REMOVAL] Starting...");

            try
            {
                string registry_path = "Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\";
                string game_exe_path = Application.StartupPath + "\\mgs2_sse.exe";
                Microsoft.Win32.RegistryKey key;

                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registry_path);

                key.DeleteValue(game_exe_path, false);

                PrintToDebugConsole("[C.FLAGS REMOVAL] Compatibility flags removed!");

            }

            catch(Exception ex)
            {
                // Signal to debugger

                Ocelot.PrintToDebugConsole("[ EXCEPTION ] " + ex.Message);

                PrintToDebugConsole("[C.FLAGS REMOVAL] Exception while removing compatibility flags!");

            }

        }

        // Move savegames to new location in "My Games"

        public static void MoveSavegamesToNewLocation()
        {

            Ocelot.PrintToDebugConsole("[MOVE SAVEGAME TO NEW LOCATION] Method starting...");

            try
            {
                string oldSavedataPath = Directory.GetParent(Application.StartupPath).FullName + "\\savedata";
                string newSavedataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\METAL GEAR SOLID 2 SUBSTANCE";

                Ocelot.PrintToDebugConsole("[MOVE SAVEGAME TO NEW LOCATION] old: " + oldSavedataPath+" | new: "+ newSavedataPath);

                Ocelot.MoveFolder(oldSavedataPath, newSavedataPath);

                Ocelot.PrintToDebugConsole("[MOVE SAVEGAME TO NEW LOCATION] Folder moved :)");

                // Set savedata folder permission to 'inerithed'

                var fs = File.GetAccessControl(newSavedataPath);
                fs.SetAccessRuleProtection(false, false);
                File.SetAccessControl(newSavedataPath, fs);

                Ocelot.PrintToDebugConsole("[MOVE SAVEGAME TO NEW LOCATION] Permission inerithing just set!");

            }

            catch(Exception ex)
            {
                Ocelot.PrintToDebugConsole("[ EXCEPTION ] " + ex.Message);

                showMessage("UAC_error");

                Application.Exit();

            }

        }

        // Check for fatal errors, and prompt an aid to the user

        public static FATALERRORSFOUND CheckForFatalErrors()
        {

            // Set a default value
            FATALERRORSFOUND returnValue = default(FATALERRORSFOUND);

            try
            {

                // Read the last.log file in both possible location

                string lastLogPath = RecoverLastLogPath();

                if (!File.Exists(lastLogPath))
                {
                    // last.log has never been created
                    return FATALERRORSFOUND.NoneDetected;
                }

                // last.log actually exist. Read it and memorize locally

                string contents = File.ReadAllText(lastLogPath);

                // Check for a wrong bound VGA

                if (contents.Contains("Can't Find Device:"))
                {
                    returnValue = returnValue.Add(FATALERRORSFOUND.WrongVideoAdapter);
                }

                // TODO check for others kind of issues

            }
            catch
            {
                showMessage("UAC_error");

                return FATALERRORSFOUND.ErrorWhileReadingFile;

            }

            return returnValue;

        }

        // Recover the last.log in the right path

        public static string RecoverLastLogPath()
        {
            {
                string PathOfLastLogInVirtualStore = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VirtualStore\\" + Application.StartupPath.Substring(3) + "\\last.log";
                bool TheseIsVirtualStoreLog = File.Exists(PathOfLastLogInVirtualStore);

                string PathOfLastLogInApplicationFolder = Application.StartupPath + "\\last.log";
                bool TheseIsApplicationFolderLog = File.Exists(PathOfLastLogInApplicationFolder);

                if (TheseIsVirtualStoreLog && TheseIsApplicationFolderLog)
                {
                    Ocelot.PrintToDebugConsole("[RETRIEVE LAST.LOG] More Last.log detected");

                    // Decide what Log is the most recent

                    if (File.GetLastWriteTimeUtc(PathOfLastLogInApplicationFolder) > File.GetLastWriteTimeUtc(PathOfLastLogInVirtualStore))
                    {
                        // Application Folder log is more recent. Chose it.
                        Ocelot.PrintToDebugConsole("[RETRIEVE LAST.LOG] Choosed last.log in Application Folder");
                        return PathOfLastLogInApplicationFolder;
                    }
                    else
                    {
                        // Chose VirtualStore log
                        Ocelot.PrintToDebugConsole("[RETRIEVE LAST.LOG] Choosed last.log in VirtualStore");
                        return PathOfLastLogInVirtualStore;
                    }

                }
                else
                {
                    // There is only one log, or none.

                    if (TheseIsVirtualStoreLog)
                    {
                        Ocelot.PrintToDebugConsole("[RETRIEVE LAST.LOG] last.log found in VirtualStore");
                        return PathOfLastLogInVirtualStore;
                    }
                    else if (TheseIsApplicationFolderLog)
                    {
                        Ocelot.PrintToDebugConsole("[RETRIEVE LAST.LOG] last.log found in ApplicationFolder");
                        return PathOfLastLogInApplicationFolder;
                    }
                    else
                    {
                        Ocelot.PrintToDebugConsole("[RETRIEVE LAST.LOG] last.log not found");
                        // last.log has never been created. Return an empty path
                        return "";
                    }

                }

            }

        }

        // write message into console (and into a file, if debug mode is enabled)

        public static void PrintToDebugConsole(string output)
        {

            output = DateTime.Now.ToString("hh.mm.ss.fff") + " -> " + output;

            Console.WriteLine(output);

            if (debugMode)
            {
                File.AppendAllText(debugMode_filePath, output+Environment.NewLine);
            }
        }

        public static void debug_printInternalConfig()
        {

            Ocelot.PrintToDebugConsole("[D] ----------");
            Ocelot.PrintToDebugConsole("[D] Printing InternalConfig:");

            Ocelot.PrintToDebugConsole("[D] - VGA List:");

            foreach (string single_vga in Ocelot.vgaList)
            {
                Ocelot.PrintToDebugConsole("[D] "+single_vga);
            }


            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Resolution)
            {
                Ocelot.PrintToDebugConsole("[D] Key: "+entry.Key+" -> Value: "+entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Controls)
            {
                Ocelot.PrintToDebugConsole("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Graphics)
            {
                Ocelot.PrintToDebugConsole("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Sound)
            {
                Ocelot.PrintToDebugConsole("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Others)
            {
                Ocelot.PrintToDebugConsole("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            Ocelot.PrintToDebugConsole("[D] END");

        }

        // UPDATE action

        public static async Task<UPDATE_AVAILABILITY> CheckForUpdatesAsync()
        {

            // NB: It will become a JSON
            dynamic remoteResponse = null;
            bool validResponse = false;

            string URL = GITHUB_API;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Anything");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add TLS support
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                try
                {
                    // GO! - Send request in GET
                    var response = client.GetAsync(URL).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // HTML Request returned 200

                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Response is a JSON

                        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                        {
                            DateParseHandling = DateParseHandling.None
                        };

                        remoteResponse = JsonConvert.DeserializeObject(responseBody);

                        if (remoteResponse != null)
                        {
                            validResponse = true;
                        }

                    }

                    else
                    {
                        // Server has responsed, but with an HTML code different from 200
                        return UPDATE_AVAILABILITY.NetworkError;
                    }

                }
                catch
                {
                    // Error during request

                    /* Note to future myself:
                     * Try to catch for (HttpRequestException e)
                     * You can read whats happened reading e.InnerException.Message
                     * 
                     * */

                    return UPDATE_AVAILABILITY.NetworkError;
                }

            }

            if (validResponse)
            {

                // Copy remote version data into a local copy

                try
                {
                    // Bind from JSON to local object

                    UpdateSoftwareSheet response = new UpdateSoftwareSheet()
                    {
                        ReleaseName = remoteResponse.name,
                        ReleaseURL = remoteResponse.html_url,
                        CreatedAt = remoteResponse.created_at,
                        //Changelog = remoteResponse.body
                    };

                    // Get VERSION

                    DateTime myDate = DateTime.Parse(response.CreatedAt);

                    response.VERSION = myDate.ToString("yyMMdd");

                    JObject ohi = (JObject)remoteResponse;
                    int assetsOnGitHub = ohi["assets"].Count();

                    if (assetsOnGitHub > 1)
                    {
                        // EDIT: Allow more assets

                        //return UPDATE_AVAILABILITY.ResponseMismatch;
                    }

                    response.LatestZipName = remoteResponse.assets[0].name;
                    response.LatestZipURL = remoteResponse.assets[0].browser_download_url;
                    response.SizeInByte = remoteResponse.assets[0].size;

                    // MY RESPONSE IS THERE!

                    // Calculating if an update is available

                    int localVersion = getIntFromThisString(VERSION);
                    int remoteVersion = getIntFromThisString(response.VERSION);

                    PrintToDebugConsole("CurrentVersion: "+ localVersion+"   LatestVersion: "+remoteVersion);

                    if (remoteVersion>localVersion)
                    {
                        // Yeah!
                        return UPDATE_AVAILABILITY.UpdateAvailable;
                    }

                    else
                    {
                        return UPDATE_AVAILABILITY.NoUpdates;
                    }

                }

                catch
                {
                    // Response mismatch! Server has send somethings unexpected

                    return UPDATE_AVAILABILITY.ResponseMismatch;

                }

            }

            else
            {
                // Server has responsed something, but isn't well formatted
                return UPDATE_AVAILABILITY.ResponseMismatch;
            }

        }

        public static int getIntFromThisString(string inputString)
        {
            int returnValue = 0;

            try
            {
                string numericPhone = new String(inputString.Where(Char.IsDigit).ToArray());

                returnValue = Convert.ToInt32(numericPhone);
            }

            catch(Exception e)
            {
                // Do nothing

                Ocelot.PrintToDebugConsole(e.Message);

            }

            return returnValue;
        }

        // Add-To-Steam Functions

        public static ADD2STEAMSTATUS AddMGS2ToSteam()
        {
            
            try
            {
                // Retrieve Steam path

                string SteamPath = RetrieveSteamInstallationFolder();

                // Retrieve the current MGS2 location

                string MGS2Path = AppDomain.CurrentDomain.BaseDirectory;

                if(!Directory.Exists(SteamPath) || !Directory.Exists(MGS2Path))
                {
                    return ADD2STEAMSTATUS.CantFindNecessaryPaths;
                }

                // Ok, we can work

                // Retrieve all Steam profiles

                string SteamProfilesDir = Path.Combine(SteamPath, "userdata");

                string[] steamProfiles = Directory.GetDirectories(SteamProfilesDir);

                int numberOfMGS2Added = 0;

                foreach (string singleProfileFolder in steamProfiles)
                {
                    // CHECK: is the useless profile 0?

                    if (Path.GetFileName(singleProfileFolder).Equals("0"))
                    {
                        // No need to add anything. Skip...
                        continue;
                    }

                    Ocelot.PrintToDebugConsole("[ ADD2STEAM ] Working in this location: "+ singleProfileFolder);

                    // Find the shortcut.vdf file

                    string vdfLocation = Path.Combine(singleProfileFolder+ "\\config\\shortcuts.vdf");

                    // Useful Byte constant
                    byte NUL = 0x00;
                    byte SOH = 0x01;
                    byte STX = 0x02;
                    byte BS = 0x08;       

                    if (!File.Exists(vdfLocation))
                    {

                        Ocelot.PrintToDebugConsole("[ ADD2STEAM ] Creating a base .vdf");

                        // Create a base shortcuts.vdf

                        Directory.CreateDirectory(Directory.GetParent(vdfLocation).ToString());

                        using (var fs = new FileStream(vdfLocation, FileMode.Create, FileAccess.ReadWrite))
                        {
                            
                            
                            List<byte> shortcuts = Encoding.Default.GetBytes("shortcuts").ToList();

                            List<byte> writeThis = new List<byte>();

                            writeThis.Add(NUL);
                            writeThis.AddRange(shortcuts);
                            writeThis.Add(NUL);
                            writeThis.Add(BS);
                            writeThis.Add(BS);

                            fs.Write(writeThis.ToArray(), 0, writeThis.Count);

                        }

                        Ocelot.PrintToDebugConsole("[ ADD2STEAM ] Base vdf created");

                    }

                    // Count existing games

                    int nonSteamGames;

                    string entireFile = File.ReadAllText(vdfLocation, Encoding.ASCII);

                    string[] split = entireFile.Split(new string[] { "tags\0" }, StringSplitOptions.None);

                    nonSteamGames = split.Count() - 1;

                    Ocelot.PrintToDebugConsole("[ ADD2STEAM ] This profile has "+nonSteamGames+" non-steam games");

                    // CHECK: Is METAL GEAR SOLID 2: SUBSTANCE already inserted in the past?

                    if (entireFile.Contains("METAL GEAR SOLID 2: SUBSTANCE") && entireFile.Contains("mgs2_sse.exe"))
                    {
                        // Yup :D
                        continue;
                    }

                    // Delete the last 2 BS at the end of the file

                    entireFile = entireFile.Substring(0,entireFile.Length - 2);

                    File.WriteAllText(vdfLocation, entireFile,Encoding.ASCII);

                    // Append to the end of the file the hyper string for the new game

                    using (var fs = new FileStream(vdfLocation, FileMode.Append, FileAccess.Write))
                    {

                        List<byte> writeThis = new List<byte>();

                        writeThis.Add(NUL);
                        writeThis.AddRange(Encoding.Default.GetBytes(nonSteamGames.ToString()).ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.AddRange(Encoding.Default.GetBytes("appname").ToList());
                        writeThis.Add(NUL);
                        writeThis.AddRange(Encoding.Default.GetBytes("METAL GEAR SOLID 2: SUBSTANCE").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.AddRange(Encoding.Default.GetBytes("exe").ToList());
                        writeThis.Add(NUL);
                        writeThis.AddRange(Encoding.Default.GetBytes(Sanitize_pathForCMD(MGS2Path+"mgs2_sse.exe")).ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.AddRange(Encoding.Default.GetBytes("StartDir").ToList());
                        writeThis.Add(NUL);
                        writeThis.AddRange(Encoding.Default.GetBytes(Sanitize_pathForCMD(MGS2Path)).ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.AddRange(Encoding.Default.GetBytes("icon").ToList());
                        writeThis.Add(NUL);
                        writeThis.AddRange(Encoding.Default.GetBytes(Sanitize_pathForCMD(MGS2Path + "MGS2SSetup.exe")).ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.AddRange(Encoding.Default.GetBytes("ShortcutPath").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.AddRange(Encoding.Default.GetBytes("LaunchOption").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(STX);
                        writeThis.AddRange(Encoding.Default.GetBytes("IsHidden").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(STX);
                        writeThis.AddRange(Encoding.Default.GetBytes("AllowDesktopConfig").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(STX);
                        writeThis.AddRange(Encoding.Default.GetBytes("AllowOverlay").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(SOH);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(STX);
                        writeThis.AddRange(Encoding.Default.GetBytes("OpenVR").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(STX);
                        writeThis.AddRange(Encoding.Default.GetBytes("LastPlayTime").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.Add(NUL);
                        writeThis.AddRange(Encoding.Default.GetBytes("tags").ToList());
                        writeThis.Add(NUL);
                        writeThis.Add(BS);
                        writeThis.Add(BS);

                        // Append the 2 BS at the end to terminate the file
                        writeThis.Add(BS);
                        writeThis.Add(BS);

                        fs.Write(writeThis.ToArray(), 0 , writeThis.Count);

                    }

                    // THE END!
                    // for this profile, at least...

                    numberOfMGS2Added = numberOfMGS2Added + 1;

                } // End foreach

                // Signal something to UI

                ADD2STEAMSTATUS returnValue = default(ADD2STEAMSTATUS);

                if(numberOfMGS2Added > 1)
                {
                    returnValue = ADD2STEAMSTATUS.AddedForMoreUsers;
                }
                else if(numberOfMGS2Added == 1)
                {
                    returnValue = ADD2STEAMSTATUS.AddedForOneUser;
                }
                else
                {
                    returnValue = ADD2STEAMSTATUS.NothingDone;
                }

                Ocelot.PrintToDebugConsole("[ ADD2STEAM ] Final result: "+returnValue);

                return returnValue;

            }

            catch(Exception ex)
            {
                Ocelot.PrintToDebugConsole("[ EXCEPTION ] " + ex.Message);

                return ADD2STEAMSTATUS.AccessError;
            }
          

        }

        // Find Steam Location

        public static string RetrieveSteamInstallationFolder()
        {
            string SteamPath = "";

            try
            {
                string possible_path = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", new Object().ToString());

                if (Directory.Exists(possible_path) && File.Exists(possible_path+"\\Steam.exe"))
                {
                    Ocelot.PrintToDebugConsole("[ STEAM ] Steam is installed in this location: " + SteamPath);
                    SteamPath = possible_path;
                }

                else
                {
                    Ocelot.PrintToDebugConsole("[ STEAM ] Steam not found ");
                }

            }

            catch
            {
                Ocelot.PrintToDebugConsole("[ STEAM ] Exception while searching for Steam location");

                // Do nothing
            }


            return SteamPath;

        }

        // Check if a process is running

        public static bool IsThisProcessRunning(string processName)
        {

            bool processFound = false;

            try
            {
                Process[] pname = Process.GetProcessesByName(processName);

                if (pname.Length != 0)
                {
                    processFound = true;
                }

            }

            catch
            {
                Ocelot.PrintToDebugConsole("[ ERROR-ERROR-ERROR ] Error while detecting if "+processName+" is running");

            }

            Ocelot.PrintToDebugConsole("[ IsThisProcessRunning ] "+processName+" isRunning value is "+processFound);

            return processFound;

        }

        // Append " at a dir path
        public static string Sanitize_pathForCMD(string originalPath)
        {
            if (originalPath.Contains(' '))
            {
                return "\"" + originalPath + "\"";
            }

            else
            {
                return originalPath;
            }
        }

        // Check if a directory is empty
        public static bool IsThisDirectoryEmpty(string path)
        {
            // I prefere to return true in case of sudden exceptions...
            bool isEmpty = true;

            try
            {
                string[] filesAndDir = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories).ToArray<string>();
                isEmpty = !filesAndDir.Any();
            }
            catch
            {
                // Who cares...
            }

            return isEmpty;

        }

        // Check if savegames must be moved to 'My Games' directory
        public static SAVEGAMEMOVING SavegameMustBeMoved()
        {

            try
            {
                string originalSavegameFolder = Application.StartupPath + "\\..\\savedata";

                string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string newSavegameFolder = Path.Combine(myDocumentsPath + "\\My Games\\METAL GEAR SOLID 2 SUBSTANCE");

                // CHECK: One/both folder exist but are empty?

                if (Directory.Exists(originalSavegameFolder) && IsThisDirectoryEmpty(originalSavegameFolder))
                {
                    // Directory esist, but there aren't files inside the old savegame folder
                    Directory.Delete(originalSavegameFolder, true);
                    PrintToDebugConsole("[ SAVEGAMEMUSTBEMOVED ] " + originalSavegameFolder + " folder is empty, and has been deleted");
                }

                if (Directory.Exists(newSavegameFolder) && IsThisDirectoryEmpty(newSavegameFolder))
                {
                    // Directory esist, but there aren't files inside the new savegame folder
                    Directory.Delete(newSavegameFolder, true);
                    PrintToDebugConsole("[ SAVEGAMEMUSTBEMOVED ] " + newSavegameFolder + " folder is empty, and has been deleted");
                }

                // Now both folder has been purged, and if they exist mean that they contain files that I can't arbitrary delete

                // Check if savegame can be moved to the new location in "My Games"
                if (Directory.Exists(originalSavegameFolder))
                {

                    // There are files inside the old savegame directory. Must move things!

                    // Check if the new location already have savegames...

                    if (Directory.Exists(newSavegameFolder))
                    {
                        return SAVEGAMEMOVING.BothFolderExist;

                    }

                    return SAVEGAMEMOVING.MovingPossible;

                }

                else
                {
                    return SAVEGAMEMOVING.NoSavegame2Move;
                }
                    

            }
            catch (Exception ex)
            {
                Ocelot.PrintToDebugConsole("[ EXCEPTION ] " + ex.Message);
            }

            // It shoudn't even come so far.
            return SAVEGAMEMOVING.NoSuccesfulEvaluationPerformed;

        }

        // Move folder and files having directory paths...

        public static void MoveFolder(string sourcePath, string targetPath)
        {
            try
            {
                DirectoryInfo diSource = new DirectoryInfo(sourcePath);
                DirectoryInfo diTarget = new DirectoryInfo(targetPath);

                CopyAll(diSource, diTarget);

                Directory.Delete(sourcePath, true);

            }
            catch (Exception ex)
            {

                PrintToDebugConsole("[ EXCEPTION ] Message:" + ex.Message + "\n\nStacktrace: " + ex.StackTrace);
                Ocelot.showMessage("UAC_error");

            }

        }

        // Aiding method - Recursive method to move files and directory

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }// END CLASS

}
