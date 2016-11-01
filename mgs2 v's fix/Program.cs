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
        static void Main()
        {

            // ENTRY POINT 

            // Set to unzipper my working path
            Unzip.ApplicationPath = Application.StartupPath;

            // All patch must be applied to start the fix!
            bool forbidStart = false;

            // Check running directory
            // V's fix is designed to work in GAME FOLDER/bin

            if (!(System.IO.File.Exists(Application.StartupPath + "\\mgs2_sse.exe")))
            {

                MessageBox.Show(
                    "V's Fix isn't in the correct place.\nPut it into GAME DIRECTORY/bin folder",
                    "Guru meditation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //Form is never created
                    //Neither fix will try to install/extract anything
                    //V's Fix will now close
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
                        Console.WriteLine("-- Game is not patched. Applying V's 2.0 Homemade Patcher");

                        Unzip.UnZippa("1_homemade_patcher.zip");

                        Process cmd;

                        cmd = Process.Start(new ProcessStartInfo("AreaChecker.exe"));
                        cmd.WaitForExit();
                        Console.WriteLine("-- || -- AreaChecker.exe finished");

                        try
                        {
                            cmd = Process.Start(new ProcessStartInfo("VOXPatch.exe"));

                            cmd.WaitForExit();
                            Console.WriteLine("-- || -- VOXPatch.exe finished");

                            File.Delete(Application.StartupPath + "\\AreaChecker.exe");
                            File.Delete(Application.StartupPath + "\\VOXPatch.exe");

                            File.Create(Application.StartupPath + "\\_patch2.0_applied.sss");

                            Console.WriteLine("-- V's 2.0 Homemade Patcher succesfully used!");

                        }

                        catch
                        {
                            // UAC Blocked?
                            Ocelot.showUacWarning();
                            forbidStart = true;
                        }
                                     

                    }

                    // Is audio fix applied?
                    if (!(System.IO.File.Exists(Application.StartupPath + "\\_audio_fix_applied.sss")))
                    {
                        //Nope
                        Console.WriteLine("-- Audio not patched. Applying Audio Fix");

                        Unzip.UnZippa("2_audio_fix.zip");

                        File.Create(Application.StartupPath + "\\_audio_fix_applied.sss");

                        Console.WriteLine("-- Audio Fix succesfully applied!");

                    }

                    // Fixed exe has been applied?
                    if (!(System.IO.File.Exists(Application.StartupPath + "\\_fixed_exe_applied.sss")))
                    {
                        //Nope
                        Console.WriteLine("-- Fixed exe not applied. Applying it");

                        Unzip.UnZippa("3_fixed_exe.zip",true);

                        File.Create(Application.StartupPath + "\\_fixed_exe_applied.sss");

                        Console.WriteLine("-- Fixed Exe succesfully applied!");

                    }

                
                    // Boycott useless original setupper
                    // MGS2SConfig.exe

                    if (System.IO.File.Exists(Application.StartupPath + "\\MGS2SConfig.exe"))
                    {

                        if (System.IO.File.Exists(Application.StartupPath + "\\_MGS2SConfig.oldandcrappy"))
                        {
                            File.Delete(Application.StartupPath + "\\_MGS2SConfig.oldandcrappy");

                        }

                        System.IO.File.Move("MGS2SConfig.exe", "_MGS2SConfig.oldandcrappy");
                        Console.WriteLine("--- MSG2Config.exe nuked");
                    }

                    // Boycott useless original config .ini file
                    // MGS2SSET.ini

                    if (System.IO.File.Exists(Application.StartupPath + "\\MGS2SSET.ini"))
                    {

                        if (System.IO.File.Exists(Application.StartupPath + "\\_MGS2SSET.oldandcrappy"))
                        {
                            File.Delete(Application.StartupPath + "\\_MGS2SSET.oldandcrappy");

                        }

                        System.IO.File.Move("MGS2SSET.ini", "_MGS2SSET.oldandcrappy");
                        Console.WriteLine("--- MGS2SSET.ini nuked");
                    }

                    // Boycott useless non-sse executable
                    // mgs2.exe

                    if (System.IO.File.Exists(Application.StartupPath + "\\mgs2.exe"))
                    {

                        if (System.IO.File.Exists(Application.StartupPath + "\\_mgs2.exe.oldandcrappy"))
                        {
                            File.Delete(Application.StartupPath + "\\_mgs2.exe.oldandcrappy");

                        }

                        System.IO.File.Move("mgs2.exe", "_mgs2.exe.oldandcrappy");
                        Console.WriteLine("--- mgs2.exe nuked");
                    }

                    // V's Setupper has been used in the past?

                    if (!(System.IO.File.Exists(Application.StartupPath + "\\Configuration_file.ini")))
                    {
                        //Nope
                        // Deploying a basic .ini
                        Console.WriteLine("-- No Configuration_file.ini . Deploying it");

                        Unzip.UnZippa("Configuration_file.zip");

                        Console.WriteLine("-- Configuration_file.ini now it's there!");

                    }

                    // Someone has messed with Configuration_file.ini?
                    // Let's check it out

                    Ocelot.checkConfFileIntegrity();
	            }
	            catch (Exception)
	            {
                    Ocelot.showUacWarning();
	            }

                if (forbidStart == false)
                {
                    // All file cleaned.
                    // All patch applied.
                    // Now I'm sure there aren't old files inside the directory

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }

            }

        }

    }
}
