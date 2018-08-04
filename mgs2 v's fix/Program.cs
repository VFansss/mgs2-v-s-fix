using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

namespace mgs2_v_s_fix
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            // ENTRY POINT 

            // Set to unzipper my working path
            Unzip.ApplicationPath = Application.StartupPath;

            // All patch must be applied to start the fix!
            bool forbidStart = false;

            // Check: it's a debug mode?
            // NB: if debug mode is enabled it will write all console log into
            // a .txt file to desktop

            if(System.IO.File.Exists(Application.StartupPath + "\\debug.sss") ||
               (args.Length != 0 && args[0].Contains("-debug")) ){

                // Debug mode enabled!

                // No need to check if exist
                File.Delete(Ocelot.debugMode_filePath);

                Ocelot.debugMode = true;

                Ocelot.PrintToDebugConsole("[!][!][!][!][!][!][!][!][!][!][!][!]");
                Ocelot.PrintToDebugConsole("[!]");
                Ocelot.PrintToDebugConsole("[!] Fix started at "+ DateTime.UtcNow+" (UCT)");
                

            }

            // Check running directory
            // V's fix is designed to work in GAME FOLDER/bin

            if (!(System.IO.File.Exists(Application.StartupPath + "\\mgs2_sse.exe")))
            {

                Ocelot.showMessage("wrong_folder_error");

                    // Can't apply any prior needed patch cause...directory is wrong
                    // Form is never created
                    // Neither fix will try to install/extract anything
                    // V's Fix will now close

                Ocelot.PrintToDebugConsole("[!] Fix is inside wrong folder. Closing.");

            }

            else
            {

                try 
	            {	        
		            // V's Fix is inside bin folder    

                    // Is 2.0 Patch applied?
                    if (!(System.IO.File.Exists(Application.StartupPath + "\\_patch2.0_applied.sss")))
                    {

                        //Nope

                        //Better inform the user

                        Ocelot.showMessage("tip_patcher");

                        Ocelot.PrintToDebugConsole("[ ] Game is not patched. Applying V's 2.0 Homemade Patcher");

                        Unzip.UnZippa("1_homemade_patcher.zip");

                        Process cmd;

                        cmd = Process.Start(new ProcessStartInfo("AreaChecker.exe"));
                        cmd.WaitForExit();
                        Ocelot.PrintToDebugConsole("[ ] || [X] AreaChecker.exe finished");

                        cmd = Process.Start(new ProcessStartInfo("VOXPatch.exe"));

                        cmd.WaitForExit();
                        Ocelot.PrintToDebugConsole("[ ] || [X] VOXPatch.exe finished");

                        File.Delete(Application.StartupPath + "\\AreaChecker.exe");
                        File.Delete(Application.StartupPath + "\\VOXPatch.exe");

                        File.Create(Application.StartupPath + "\\_patch2.0_applied.sss");

                        Ocelot.PrintToDebugConsole("[X] V's 2.0 Homemade Patcher succesfully used!");       

                    }

                    // Is audio fix applied?
                    if (!(System.IO.File.Exists(Application.StartupPath + "\\_audio_fix_applied.sss")))
                    {
                        //Nope
                        Ocelot.PrintToDebugConsole("[ ] Audio not patched. Applying Audio Fix");

                        Unzip.UnZippa("2_audio_fix.zip");

                        File.Create(Application.StartupPath + "\\_audio_fix_applied.sss");

                        Ocelot.PrintToDebugConsole("[X] Audio Fix succesfully applied!");

                    }

                    // Fixed exe has been applied?
                    if (!(System.IO.File.Exists(Application.StartupPath + "\\_fixed_exe_applied.sss")))
                    {
                        //Nope
                        Ocelot.PrintToDebugConsole("[ ] Fixed exe not applied. Applying it");

                        Unzip.UnZippa("3_fixed_exe.zip",true);

                        File.Create(Application.StartupPath + "\\_fixed_exe_applied.sss");

                        Ocelot.PrintToDebugConsole("[X] Fixed Exe succesfully applied!");

                    }

                
                    // Boycott useless original setupper
                    // MGS2SConfig.exe

                    if (System.IO.File.Exists(Application.StartupPath + "\\MGS2SConfig.exe"))
                    {

                        if (System.IO.File.Exists(Application.StartupPath + "\\_MGS2SConfig.oldandcrappy"))
                        {
                            File.Delete(Application.StartupPath + "\\_MGS2SConfig.oldandcrappy");
                            Ocelot.PrintToDebugConsole("[X] _MGS2SConfig.oldandcrappy deleted");
                        }

                        System.IO.File.Move("MGS2SConfig.exe", "_MGS2SConfig.oldandcrappy");
                        Ocelot.PrintToDebugConsole("[X] MSG2Config.exe nuked");
                    }

                    // Boycott useless original config .ini file
                    // MGS2SSET.ini

                    if (System.IO.File.Exists(Application.StartupPath + "\\MGS2SSET.ini"))
                    {

                        if (System.IO.File.Exists(Application.StartupPath + "\\_MGS2SSET.oldandcrappy"))
                        {
                            File.Delete(Application.StartupPath + "\\_MGS2SSET.oldandcrappy");
                            Ocelot.PrintToDebugConsole("[X] _MGS2SSET.oldandcrappy deleted");
                        }

                        System.IO.File.Move("MGS2SSET.ini", "_MGS2SSET.oldandcrappy");
                        Ocelot.PrintToDebugConsole("[X] MGS2SSET.ini nuked");
                    }

                    // Boycott useless non-sse executable
                    // mgs2.exe

                    if (System.IO.File.Exists(Application.StartupPath + "\\mgs2.exe"))
                    {

                        if (System.IO.File.Exists(Application.StartupPath + "\\_mgs2.exe.oldandcrappy"))
                        {
                            File.Delete(Application.StartupPath + "\\_mgs2.exe.oldandcrappy");
                            Ocelot.PrintToDebugConsole("[X] _mgs2.exe.oldandcrappy deleted");
                        }

                        System.IO.File.Move("mgs2.exe", "_mgs2.exe.oldandcrappy");
                        Ocelot.PrintToDebugConsole("[X] mgs2.exe nuked");
                    }

                    // V's Setupper has been used in the past?

                    if (!(System.IO.File.Exists(Application.StartupPath + "\\Configuration_file.ini")))
                    {
                        //Nope
                        // Deploying a basic .ini
                        Ocelot.PrintToDebugConsole("[ ] No Configuration_file.ini . Deploying it");

                        Unzip.UnZippa("Configuration_file.zip");

                        Ocelot.PrintToDebugConsole("[X] Configuration_file.ini now it's there!");

                    }

                    // Someone has messed with Configuration_file.ini?
                    // Let's check it out

                    Ocelot.checkConfFileIntegrity();
	            }
	            catch (Exception e)
	            {
                    Ocelot.PrintToDebugConsole(e.ToString());
                    forbidStart = true;
	            }



                if (forbidStart == false)
                {
                    // All file cleaned.
                    // All patch applied.
                    // Now I'm sure there aren't old files inside the directory

                    Ocelot.PrintToDebugConsole("[!] forbidStart is false. Starting winform");

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }

                else
                {

                    if (Ocelot.debugMode) Ocelot.showMessage("debug_mode");
                    
                    Ocelot.showMessage("initiating_error");
                    Ocelot.PrintToDebugConsole("[!] forbidStart is true. Closing.");

                }

            }

        }

    }
}
