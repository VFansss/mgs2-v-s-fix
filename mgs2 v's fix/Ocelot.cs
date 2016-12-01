using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace mgs2_v_s_fix
{
    class Ocelot
    {

        // Contain Key-Value from Configuration_file.ini when X is called
        public static ConfSheet InternalConfiguration = new ConfSheet();

        // List of Graphics Adapter present in the machine
        public static LinkedList<string> vgaList= new LinkedList<string>();

        public static bool needOfAutoConfig = false;

        public static IniFile ConfFile = new IniFile("Configuration_file.ini");

        public Ocelot()
        {


        }

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
                Ocelot.showUacWarning();
            }
            
        }

        /// Settings Menu Function

        public static void checkConfFileIntegrity()
        {

            // If field key don't exist inside Configuration_file.ini
            // V's fix will create it
            // THEN flag Autoconfigurator to do something later
            
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

            if (needOfAutoConfig) { Console.WriteLine("[+] Configuration_file.ini seem missing some key. Need to autoconfig!"); }

            return;


        }

                // I don't think these 2 need to be edited frequently
        public static void load_INI_SetTo_InternalConfig()
        {

            // Time to pair Key-Value from Configuration_file.ini and
            //  set it to Ocelot.dataFromConfFile

            Console.WriteLine("[+] .ini contain a valid configuration. Loading from it...");

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

            Console.WriteLine("[+] Information from .ini succesfully loaded into setupper!");
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

            Console.WriteLine("[+] InternalConfig succesfully exported into Configuration_file.ini");

            return;
        }

                // This...maybe
        internal static void load_InternalConfig_SetTo_MGS()
        {
            
            // Delete already existing mgs.ini

            File.Delete(Application.StartupPath + "\\mgs2.ini");

            File.Create(Application.StartupPath + "\\mgs2.ini").Close();

            // NB: Operation are done following the usual pattern
            //  Resolution -> Controls -> Graphics -> Sound

            using (StreamWriter ini = new StreamWriter(Application.StartupPath + "\\mgs2.ini", true))
            {

                byte[] ba;
                StringBuilder hexString = new StringBuilder();
                int opcode;

                ////// 
                //--------- Resolution
                ////// 

                #region lot_of_things
                // Width

                hexString.Clear();
                hexString.Append(string.Format("{0:x}", Int32.Parse(Ocelot.InternalConfiguration.Resolution["Width"])).ToUpper());

                while(hexString.Length < 4)
                {
                    // Need 0 padding to left
                    hexString.Insert(0,0);
                }

                ini.WriteLine("0003"+"\t"+hexString.ToString());

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
                    Directory.Delete(Application.StartupPath + "\\scripts",true);
                    File.Delete(Application.StartupPath + "\\winmmbase.dll");
                    File.Delete(Application.StartupPath + "\\dsound_x64.dll");  
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
                    ws_ini.Write("cutscenes_top_black_border","0", "MISC");
                    ws_ini.Write("cutscenes_bottom_black_border","0", "MISC");
                    }
                    else
                    {
                    ws_ini.Write("cutscenes_top_black_border", "480", "MISC");
                    ws_ini.Write("cutscenes_bottom_black_border", "480", "MISC");
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
                    ini.WriteLine("00"+opcode+"\t" + hexString.ToString().Substring(0,8));

                    opcode++;
                    hexString = hexString.Remove(0,8);
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

                    foreach (string vganame in vgaList)
                    {
                        if ((Ocelot.InternalConfiguration.Resolution["LaptopMode"] == "true") &&
                            (vganame.Contains("Intel"))&&
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

                    }

                }

                #endregion 

                // WindowMode

                if(Ocelot.InternalConfiguration.Resolution["WindowMode"].Equals("true")){
                    ini.WriteLine("0001"+"\t"+"0001");
                }

                ini.WriteLine("0005" + "\t" + "0001");

                #endregion

                ////// 
                //--------- Controls
                ////// 

                #region lot_of_things
                // 360Gamepad

                // 0: delete all (if present) existing 360Gamepad files

                File.Delete(Application.StartupPath + "\\Dinput.dll");
                File.Delete(Application.StartupPath + "\\Dinput8.dll");
                File.Delete(Application.StartupPath + "\\XInput1_3.dll");
                File.Delete(Application.StartupPath + "\\XInputPlus.ini");

                if (Ocelot.InternalConfiguration.Controls["360Gamepad"].Equals("true"))
                {
                    // 1: 360Gamepad.zip must be extracted

                    Unzip.UnZippa("360Gamepad.zip",true);

                }

                #endregion

                ////// 
                //--------- Graphics
                //////

                #region lot_of_things

                // ! Important thing !
                //  These switch can be more optimized with clever use of 'break'
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
                        //NB: These 2 are dangerous on some config. Watch out.
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
                File.Delete(Application.StartupPath + "\\enbconvertor.ini");
                File.Delete(Application.StartupPath + "\\dxgi.dll");
                File.Delete(Application.StartupPath + "\\d3d9.dll");
                File.Delete(Application.StartupPath + "\\d3d8.dll");

                if (Ocelot.InternalConfiguration.Graphics["AA"].Equals("true"))
                {

                    // 1: V_s_sweetFX.zip must be extracted

                    Unzip.UnZippa("V_s_sweetFX.zip");

                    try {

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

            Console.WriteLine("[+] InternalConfig succesfully exported into mgs2.ini");

            return;
        }

        /// Autoconfig

        public static void startAutoconfig()
        {

            Console.WriteLine("[+] Autoconfig started. I'm looking for a nice config...");
            ConfSheet defaultConfig = new ConfSheet();

            // Resolution
            defaultConfig.Resolution["Height"] = Screen.PrimaryScreen.Bounds.Size.Height.ToString();
            defaultConfig.Resolution["Width"] = Screen.PrimaryScreen.Bounds.Size.Width.ToString(); ;

            System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;


            double rapporto = (Double.Parse(defaultConfig.Resolution["Width"]) / Double.Parse(defaultConfig.Resolution["Height"]));

            if ((rapporto == 1.6d) || (rapporto == 1.7777777777777777d))
            {
                defaultConfig.Resolution["WideScreenFIX"] = "true";
            }
            else
            {
                defaultConfig.Resolution["WideScreenFIX"] = "false";
            }         

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

            if (vgaList.Count > 1)
            {
                MessageBox.Show("Please be aware that since more VGA are installed on your system you have to be sure that executable of the game (mgs2_sse.exe) is bounded correctly to the right graphics adapter!\n\nThis HAS to be done MANUALLY from your driver control panel!", "More graphics adapter detected!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            defaultConfig.Controls["360Gamepad"] = "false";

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

            Console.WriteLine("[+] Autoconfig finished. InternalConfiguration Filled");

            Ocelot.needOfAutoConfig = false;

            return;
        }

        public static void getGraphicsAdapterList()
        {
            vgaList.Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject mo in searcher.Get())
            {
                PropertyData currentBitsPerPixel = mo.Properties["CurrentBitsPerPixel"];
                PropertyData description = mo.Properties["Description"];

                vgaList.AddLast(description.Value.ToString());

            }
        }

        // UAC Warning

        public static void showUacWarning()
        {
            MessageBox.Show("Some disk operation has failed. Check out UAC or your Admin rights to write/read file in game directory!", "Can't do a sh*t!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.WriteLine("[!!!] UAC/Admin Rights has blocked something. Stacca stacca! [!!!]");
        }

    // END CLASS

    }

}
