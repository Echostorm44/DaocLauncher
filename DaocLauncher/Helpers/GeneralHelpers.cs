using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DaocLauncher.Helpers
{
    public static class GeneralHelpers
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        public enum ConfigFileType
        {
            GeneralSettings,
            Accounts,
            Characters,
            Teams,
            AdvancedSettings,
            Hotkeys
        }

        private const string EntropyValue = " superdupersekritsarewhatyouneedforthiskindathingielol";

        public static string Encrypt(this string stringToEncrypt)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.Unicode.GetBytes(stringToEncrypt), Encoding.Unicode.GetBytes(EntropyValue), DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(this string stringToDecrypt)
        {
            byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(stringToDecrypt), Encoding.Unicode.GetBytes(EntropyValue), DataProtectionScope.LocalMachine);
            return Encoding.Unicode.GetString(decryptedData);
        }

        public static List<DaocCharacter> LoadCharactersFromDisk()
        {
            string fileName = "characters.dat";
            var myAccounts = new List<DaocCharacter>();

            var rawCharacters = GetFileContents(fileName, true);
            if (string.IsNullOrEmpty(rawCharacters))
            {
                var serial = JsonSerializer.Serialize<List<DaocCharacter>>(myAccounts);
                WriteFile(fileName, serial, true);
            }
            else
            {
                myAccounts = JsonSerializer.Deserialize<List<DaocCharacter>>(rawCharacters) ?? new List<DaocCharacter>() { };
            }
            return myAccounts;
        }

        public static Dictionary<string, string> GetAllCharacterClasses()
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            results.Add("Armsman", "1");
            results.Add("Cabalist", "1");
            results.Add("Cleric", "1");
            results.Add("Friar", "1");
            results.Add("Heretic", "1");
            results.Add("Infiltrator", "1");
            results.Add("Mauler (Alb) ", "1");
            results.Add("Mercenary", "1");
            results.Add("Minstrel", "1");
            results.Add("Necromancer", "1");
            results.Add("Paladin", "1");
            results.Add("Reaver", "1");
            results.Add("Scout", "1");
            results.Add("Sorcerer", "1");
            results.Add("Theurg", "1");
            results.Add("Wizard", "1");
            results.Add("Animist", "3");
            results.Add("Bainshee", "3");
            results.Add("Bard", "3");
            results.Add("Blademaster", "3");
            results.Add("Champion", "3");
            results.Add("Druid", "3");
            results.Add("Eldritch", "3");
            results.Add("Enchanter", "3");
            results.Add("Hero", "3");
            results.Add("Mauler (Hib) ", "3");
            results.Add("Mentalist", "3");
            results.Add("Nightshade", "3");
            results.Add("Ranger", "3");
            results.Add("Valewalker", "3");
            results.Add("Vampiir", "3");
            results.Add("Warden", "3");
            results.Add("Berserker", "2");
            results.Add("Bonedancer", "2");
            results.Add("Healer", "2");
            results.Add("Hunter", "2");
            results.Add("Mauler (Mid) ", "2");
            results.Add("Runemaster", "2");
            results.Add("Savage", "2");
            results.Add("Shadowblade", "2");
            results.Add("Shaman", "2");
            results.Add("Skald", "2");
            results.Add("Spiritmaster", "2");
            results.Add("Thane", "2");
            results.Add("Valkyrie", "2");
            results.Add("Warlock", "2");
            results.Add("Warrior", "2");
            return results;
        }

        public static void SaveCharactersToDisk(List<DaocCharacter> characters)
        {
            var charsToSave = characters.Where(a => !string.IsNullOrEmpty(a.Name) && !string.IsNullOrEmpty(a.Server) &&
                !string.IsNullOrEmpty(a.ParentAccountName) && !string.IsNullOrEmpty(a.Class)).ToList();
            
            string fileName = "characters.dat";
            var serialA = JsonSerializer.Serialize<List<DaocCharacter>>(charsToSave);
            WriteFile(fileName, serialA, true);
        }

        public static GeneralSettings LoadGeneralSettingsFromDisk()
        {
            string fileName = "generalsettings.dat";
            string defaultGameDLLPath = "C:\\Program Files(x86)\\Electronic Arts\\Dark Age of Camelot\\game.dll";
            
            var settings = GetFileContents(fileName, false);
            GeneralSettings mySettings = new GeneralSettings();
            if (string.IsNullOrEmpty(settings))
            {
                mySettings = new GeneralSettings() { PathToGameDll = defaultGameDLLPath };
                var serialGS = JsonSerializer.Serialize<GeneralSettings>(mySettings);
                WriteFile(fileName, serialGS, false);
            }
            else
            {
                mySettings = JsonSerializer.Deserialize<GeneralSettings>(settings) ?? new GeneralSettings() { PathToGameDll = defaultGameDLLPath };
            }
            return mySettings;
        }

        public static void SaveGeneralSettingsToDisk(GeneralSettings settings)
        {
            string fileName = "generalsettings.dat";
            var serialA = JsonSerializer.Serialize<GeneralSettings>(settings);
            WriteFile(fileName, serialA, false);
        }

        public static ServerList LoadServerListFromDisk()
        {
            string fileName = "serverList.ini";
            var rawServers = GetFileContents(fileName, false);
            ServerList servers = new ServerList();
            if (string.IsNullOrEmpty(rawServers))
            {
                servers = new ServerList() { Servers = new List<Server>() };
                servers.Servers.Add(new Server() { ID = "41", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain1" });
                servers.Servers.Add(new Server() { ID = "49", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain2" });
                servers.Servers.Add(new Server() { ID = "50", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain3" });
                servers.Servers.Add(new Server() { ID = "51", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain4" });
                servers.Servers.Add(new Server() { ID = "52", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain5" });
                servers.Servers.Add(new Server() { ID = "53", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain6" });
                servers.Servers.Add(new Server() { ID = "54", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain7" });
                servers.Servers.Add(new Server() { ID = "55", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain8" });
                servers.Servers.Add(new Server() { ID = "56", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain9" });
                servers.Servers.Add(new Server() { ID = "57", IP = "107.23.173.143", IsOfficial = true, Name = "Ywain10" });
                servers.Servers.Add(new Server() { ID = "23", IP = "107.21.60.95", IsOfficial = true, Name = "Gaheris" });
                var serialSL = JsonSerializer.Serialize<ServerList>(servers);
                WriteFile(fileName, serialSL, false);
            }
            else
            {
                servers = JsonSerializer.Deserialize<ServerList>(rawServers) ?? new ServerList() { Servers = new List<Server>() };
            }
            return servers;
        }
        public static AllDaocAccounts LoadAccountListFromDisk()
        {
            string fileName = "accountList.ini";
            var rawAccounts = GetFileContents(fileName, true);
            AllDaocAccounts accounts = new AllDaocAccounts();
            if (string.IsNullOrEmpty(rawAccounts))
            {
                accounts = new AllDaocAccounts() { MyAccounts = new List<DaocAccount>() };
                accounts.MyAccounts.Add(new DaocAccount() { Name = "SampleName", Password= "SamplePW", DefaultTag = "" });
                var serialA = JsonSerializer.Serialize<AllDaocAccounts>(accounts);
                WriteFile(fileName, serialA, true);
            }
            else
            {
                accounts = JsonSerializer.Deserialize<AllDaocAccounts>(rawAccounts) ?? new AllDaocAccounts() { MyAccounts = new List<DaocAccount>() };
            }
            return accounts;
        }

        public static void SaveAccountListToDisk(List<DaocAccount> accounts)
        {
            string fileName = "accountList.ini";
            AllDaocAccounts final = new AllDaocAccounts();
            var accountsToSave = accounts.Where(a => !string.IsNullOrEmpty(a.Name) && !string.IsNullOrEmpty(a.Password) && !string.IsNullOrEmpty(a.DefaultTag)).ToList();
            final.MyAccounts = accountsToSave;
            var serialA = JsonSerializer.Serialize<AllDaocAccounts>(final);
            WriteFile(fileName, serialA, true);
        }

        static string GetFileContents(string fileName, bool isEncrypted)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaocLauncher\\";
            if (!File.Exists(basePath + fileName))
            {
                return "";
            }
            else
            {
                var fileText = File.ReadAllText(basePath + fileName);
                if (isEncrypted)
                {
                    fileText = fileText.Decrypt();
                }
                return fileText;
            }
        }

        static void WriteFile(string filename, string contents, bool isEncrypted)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaocLauncher\\";
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (isEncrypted)
            {
                contents = contents.Encrypt();
            }
            File.WriteAllText(basePath + filename, contents);
        }


        /// <summary>
        /// Launch DAOC
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="serverIP"></param>
        /// <param name="serverID"></param>
        /// <param name="characterName"></param>
        /// <param name="realmNumber">1 Alb; 2 Mid; 3 Hib</param>
        public static void LaunchDaoc(string windowTitle, string userName, string password, string serverIP, string serverID, string characterName, string realmNumber)
        {
            string gameFolder = LoadGeneralSettingsFromDisk().PathToGameDll.Replace("game.dll", "");
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.WorkingDirectory = gameFolder;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.Arguments = "/K game.dll " + serverIP + " 10622 " + serverID + " " + userName + " " + password + " " + characterName + " " + realmNumber;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.Start();
                    Thread.Sleep(500);
                    var win = FindWindow("DAoCMWC", "Dark Age of Camelot © 2001-2021 Electronic Arts Inc. All Rights Reserved.");
                    SpinWait.SpinUntil(() => win != IntPtr.Zero);
                    SetWindowText(win, windowTitle);                    
                }
                // We look for the mutant handles that prevents more than 2 instances and kill them
                MutantHunter hunt = new MutantHunter();
                var results = hunt.KillMutants();
            }
            catch (Exception)
            {  
                
            }
        }

        #region TODO
        //public static void UpdateTaskBarProgress(object sender, double value)
        //{
        //    var foo = ((MainWindow)System.Windows.Window.GetWindow((System.Windows.Controls.UserControl)sender));
        //    if (value >= 1)
        //    {
        //        foo.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
        //        foo.TaskbarItemInfo.ProgressValue = 0;
        //        return;
        //    }
        //    if (value < 0)
        //        value = 0;
        //    foo.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        //    foo.TaskbarItemInfo.ProgressValue = value;
        //}
        #endregion

    }
}
