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
using static mgs2_v_s_fix.Flags;
using Newtonsoft.Json.Linq;

namespace mgs2_v_s_fix
{
    class Ocelot
    {

        // Current version of the V's Fix - Format is YYMMDD
        public const string VERSION = "180527";

        // UPDATE

        public const string GITHUB_API = "https://api.github.com/repos/VFansss/mgs2-v-s-fix/releases/latest";
        public const string GITHUB_RELEASE = "https://github.com/VFansss/mgs2-v-s-fix/releases";
        public const string GITHUB_WIKI = "https://github.com/VFansss/mgs2-v-s-fix/wiki";

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

            if (needOfAutoConfig) { Ocelot.console("[+] Configuration_file.ini seem missing some key. Need to autoconfig!"); }

            return;


        }

        // I don't think these 2 need to be edited frequently
        public static void load_INI_SetTo_InternalConfig()
        {

            // Time to pair Key-Value from Configuration_file.ini and
            //  set it to Ocelot.dataFromConfFile

            Ocelot.console("[ ] .ini contain a valid configuration. Loading from it...");

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

            Ocelot.console("[ ] Information from .ini succesfully loaded into setupper!");
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

            Ocelot.console("[ ] InternalConfig succesfully exported into Configuration_file.ini");

            return;
        }

        // !!!! Big function that apply V's Fix settings
        internal static void load_InternalConfig_SetTo_MGS()
        {
            
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
                    File.Delete(Application.StartupPath + "\\enbconvertor.ini");

                    Unzip.UnZippa("DXWrapper.zip",true);

                    #endregion

                    ////// 
                    //--------- Resolution
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

                    // 0: delete all (if present) existing WideScreenFIX files/directory

                    if (Directory.Exists(Application.StartupPath + "\\scripts"))
                    {
                        Directory.Delete(Application.StartupPath + "\\scripts", true);
                        File.Delete(Application.StartupPath + "\\msacm32.dll");
                        File.Delete(Application.StartupPath + "\\dsound_x64.dll");
                        // This is an old library used in version <= 1.02
                        // better delete it if exist
                        File.Delete(Application.StartupPath + "\\winmmbase.dll");
                    }

                    if (Ocelot.InternalConfiguration.Resolution["WideScreenFIX"].Equals("true"))
                    {

                        // 1: WSF.zip must be extracted
                        // 2: Resolution must be set inside scripts/mgs2w.ini


                        // 1
                        Unzip.UnZippa("WSF.zip");

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

                    // FIX FOR ATI/NVIDIA

                    #region TANTAROBA
                    using (var stream = new FileStream(Application.StartupPath + "\\mgs2_sse.exe", FileMode.Open, FileAccess.ReadWrite))
                    {

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

                        }

                        if (Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"].Contains("Radeon"))
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

                        }

                        if (Ocelot.InternalConfiguration.Resolution["GraphicAdapterName"].Contains("Intel"))
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

                        }

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

                    }

                    #endregion

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


                        if (Ocelot.InternalConfiguration.Controls["EnableController"].Equals("DS4"))
                        {
                            // What layout?

                            if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                            {
                                Unzip.UnZippa("ControllerDS4_PS2Layout.zip", true);
                            }

                            else // V Layout
                            {
                                Unzip.UnZippa("ControllerDS4_VLayout.zip", true);
                            }

                        }

                        else // XBOX
                        {
                            // What layout?

                            if (Ocelot.InternalConfiguration.Controls["PreferredLayout"].Equals("PS2"))
                            {
                                Unzip.UnZippa("ControllerXBOX_PS2Layout.zip", true);
                            }

                            else // V Layout
                            {
                                Unzip.UnZippa("ControllerXBOX_VLayout.zip", true);
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
                        ini.WriteLine("0041" + "\t" + "0001");
                        ini.WriteLine("0042" + "\t" + "0001");
                        ini.WriteLine("0043" + "\t" + "0001");
                        ini.WriteLine("004A" + "\t" + "0001");
                    }

                    // MotionBlur

                    if (Ocelot.InternalConfiguration.Graphics["MotionBlur"].Equals("true"))
                    {
                        ini.WriteLine("0044" + "\t" + "0001");
                        ini.WriteLine("0045" + "\t" + "0001");
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

                    if (Ocelot.InternalConfiguration.Graphics["AA"].Equals("true"))
                    {

                        // 1: V_s_sweetFX.zip must be extracted

                        Unzip.UnZippa("V_s_sweetFX.zip");

                        try
                        {

                            // 2: Trying to automatically apply the Windows XP SP3 Compatibility Mode

                            string registry_path = "Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\";
                            string game_exe_path = Application.StartupPath + "\\mgs2_sse.exe";
                            Microsoft.Win32.RegistryKey key;

                            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registry_path);

                            key.SetValue(game_exe_path, "~ RUNASADMIN WINXPSP3", Microsoft.Win32.RegistryValueKind.String);
                            key.Close();

                        }

                        catch
                        {
                            // BIG NOPE
                        }

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

                    // FixAfterPlaying

                    // If is set to FALSE it will sabotage automatical V's Fix opening after game quit

                    using (var stream = new FileStream(Application.StartupPath + "\\mgs2_sse.exe", FileMode.Open, FileAccess.ReadWrite))
                    {

                        if (Ocelot.InternalConfiguration.Sound["FixAfterPlaying"].Equals("true"))
                        {

                            // Fix must be opened. Restoring original .exe condition

                            // 2
                            stream.Position = 0x60213E;
                            stream.WriteByte(0x32);

                        }

                        else
                        {
                            // Broking game .exe

                            // X
                            stream.Position = 0x60213E;
                            stream.WriteByte(0x58);
                        }

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

                // Avoid strange Windows UAC issues setting full
                // access on MGS2 files from every windows user

                string homeDirectory = Directory.GetParent(Application.StartupPath).ToString();

                // Create the savedata folder,if not already exist

                Directory.CreateDirectory(homeDirectory+"\\savedata");

                // Grant access to everyone, if possible

                bool success = grantAccessToEveryUser(homeDirectory);

                Ocelot.console("[ ] grantAccess has returned "+success);

            }

            catch
            {
                Ocelot.showMessage("UAC_error");

            }

            Ocelot.console("[ ] InternalConfig succesfully exported into mgs2.ini");

            if (Ocelot.debugMode)
            {
                Ocelot.debug_printInternalConfig();
            }

            return;
        }

        /// Autoconfig

        public static void startAutoconfig()
        {

            Ocelot.console("[ ] Autoconfig started. I'm looking for a nice config...");
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

            Ocelot.console("[!] -VVV- No VGA Selected.");

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

                    Ocelot.console("[!] -AUTOCONFIG- Found a manual inserted one: " + explicitedVGAName);
                }

            }

            else if (vgaList.Count > 1)
            {

                Ocelot.showMessage("tip_vga");
                
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

            // Graphics

            defaultConfig.Graphics["RenderingSize"]="high";
            defaultConfig.Graphics["ShadowDetail"] = "high";
            defaultConfig.Graphics["ModelQuality"] = "medium";
            defaultConfig.Graphics["RenderingClearness"] = "high";
            defaultConfig.Graphics["EffectQuantity"] = "high";
            defaultConfig.Graphics["BunchOfCoolEffect"] = "true";
            defaultConfig.Graphics["MotionBlur"] = "true";
            defaultConfig.Graphics["AA"] = "false";

            // Sound

            defaultConfig.Sound["SoundAdapterName"]="Primary Sound Driver";
            defaultConfig.Sound["Quality"]="high";
            defaultConfig.Sound["SE"]="high";
            defaultConfig.Sound["SoundQuality"]="high";
            defaultConfig.Sound["FixAfterPlaying"] = "true";

            InternalConfiguration = defaultConfig;

            Ocelot.console("[+] Autoconfig finished. InternalConfiguration Filled");

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

        // Make MGS2 files readable by every user on the system

        private static bool grantAccessToEveryUser(string fullPath)
        {

            bool success = false;

            try{

                DirectoryInfo dInfo = new DirectoryInfo(fullPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                dSecurity.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl,
                                                                 InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                                                 PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);

                success = true;

            }

            catch
            {
                success = false;
            }

            return success;

        }

        // show a MessageBox with custom message based on a string code

        public static void showMessage(string code)
        {

            switch (code)
            {

                // INFORMATION TIP    

                case "update_crashedinfire":

                    MessageBox.Show(
                    "Can't reach GitHub for updates (Are you offline?)",
                    "Ehi!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "update_noupdates":

                    MessageBox.Show(
                    "Seems that there isn't any updates for the V's Fix.\n\nHappy playing :)",
                    "Ehi!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "update_available":

                    MessageBox.Show(
                    "It seems that someone has actually worked!\n\nPress 'OK' to open the V's Fix GitHub Release page to download the latest release!",
                    "UPDATE AVAILABLE!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "debug_mode":

                    MessageBox.Show(
                    "Debug mode enabled!",
                    "Entering the matrix", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_patcher":

                    MessageBox.Show(
                    "V's Fix will now run some extra applications for patching the game into 2.0 Version.\n\nOn some system it can prompt an UAC warning.\nIsn't doing anything harmful; let it do its job!",
                    "Just an info...", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_AA":

                    MessageBox.Show("Activating anti-aliasing requires that game (mgs2_sse.exe) must run in WINDOWS XP SP3 compatibility mode.\n\nV's Fix will try to set it automatically but (like all things in life) may fail so check it out MANUALLY.\n\nRunning the game without XP compatibility will result in a BLACK SCREEN ON GAME STARTUP!\n\nAlso, it isn't compatible with 'High' model quality preset.",
                    "Back to 2001!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case "tip_vga":

                    MessageBox.Show("Please be aware that since more VGAs are installed on your system you have to be sure that the executable of the game (mgs2_sse.exe) is bounded correctly to the right graphics adapter!\n\nThis MUST be done MANUALLY from your graphics adapter's control panel!",
                    "More graphics adapter detected!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                    // ERROR MESSAGE

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

                        MessageBox.Show(
                    "V's Fix isn't in the correct place.\nPut it into GAME DIRECTORY\\bin folder\n\n"+extra_hint,
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                    break;

                case "unzipping_error":

                    // TODO change this
                    MessageBox.Show(
                    "V's Fix isn't able to create some files into game's directory.\nFix require full permission to create,delete and extract file into that directory. Try to run the fix using 'Admin rights'!",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "no_vga":

                    MessageBox.Show(
                    "V's hasn't found any VGA installed in your system.\n\nIF you are able to read this, is probably wrong.\n\nUnfortunatelly you must insert your VGA name manually.\n\nPlease read the V's Fix manual\n\nChapter: Settings Menu - Resolution tab\n\nParagraph: 6 - Graphical Adapter \n\n for an easy workaround.\n\nClosing this message will open your browser pointing to V's Guide.\n\nWould you kindly press the 'OK' button?",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "no_donate":

                    MessageBox.Show(
                    "V's Fix isn't able to open PayPal website! Please don't give up :(",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "UAC_error":

                    MessageBox.Show("Some operation has been blocked by operating system, and is a bad thing: current action has been aborted.\n\nCheck out UAC or your Admin rights and retry again!",
                    "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                case "initiating_error":

                    MessageBox.Show("Some operation has been blocked by operating system, and is a VERY bad thing: V's Fix cannot start without applying all preliminary actions, and thus it will close :( \n\n"+
                        "You have few option:\n"+
                        "1) Check out UAC or your Admin rights and retry again!\n\n"+
                        "2) Try to start the V's Fix in 'debug mode'\n\n"+
                        "   a) creating a \"debug.sss\" file (without quote) inside you game folder\n" +
                        "OR\n"+
                        "   b) starting MGS2SSetup.exe with -debug argument\n\n"+
                        "and see the log saved in your desktop\n\n" +
                        "If you cannot succeed to start it in any way read the V's Fix manual for some extra help.",
                    "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;

                default:

                    // TODO change this
                    MessageBox.Show("You shouldn't be able to read this message: /",
                    "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            Ocelot.console("[!] Showed a messagebox with code: "+code);
        }

        // write message into console (and into a file, if debug mode is enabled)

        public static void console(string output)
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

            Ocelot.console("[D] ----------");
            Ocelot.console("[D] Printing InternalConfig:");

            Ocelot.console("[D] - VGA List:");

            foreach (string single_vga in Ocelot.vgaList)
            {
                Ocelot.console("[D] "+single_vga);
            }


            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Resolution)
            {
                Ocelot.console("[D] Key: "+entry.Key+" -> Value: "+entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Controls)
            {
                Ocelot.console("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Graphics)
            {
                Ocelot.console("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            foreach (KeyValuePair<string, string> entry in InternalConfiguration.Sound)
            {
                Ocelot.console("[D] Key: " + entry.Key + " -> Value: " + entry.Value);
            }

            Ocelot.console("[D] END");

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
                        PublishedAt = remoteResponse.published_at,
                        //Changelog = remoteResponse.body
                    };

                    // Get VERSION

                    DateTime myDate = DateTime.Parse(response.PublishedAt);

                    response.VERSION = myDate.ToString("yyMMdd");

                    JObject ohi = (JObject)remoteResponse;
                    int assetsOnGitHub = ohi["assets"].Count();

                    if (assetsOnGitHub > 1)
                    {
                        // ???
                        return UPDATE_AVAILABILITY.ResponseMismatch;
                    }

                    else
                    {
                        response.LatestZipName = remoteResponse.assets[0].name;
                        response.LatestZipURL = remoteResponse.assets[0].browser_download_url;
                        response.SizeInByte = remoteResponse.assets[0].size;

                        // MY RESPONSE IS THERE!

                        // Calculating if an update is available

                        int currentVersion = getIntFromThisString(VERSION);
                        int latestVersion = getIntFromThisString(response.VERSION);

                        console("CurrentVersion: "+ currentVersion+"   LatestVersion: "+latestVersion);

                        if (latestVersion>currentVersion)
                        {
                            // Yeah!
                            return UPDATE_AVAILABILITY.UpdateAvailable;
                        }

                        else
                        {
                            return UPDATE_AVAILABILITY.NoUpdates;
                        }
                        
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

                Ocelot.console(e.Message);

            }

            return returnValue;
        }

        // END CLASS

    }

}
