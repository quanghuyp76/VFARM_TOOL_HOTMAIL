using System;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace X_Vframe_Tool.Setting
{
    public static class ChromeConfig
    {
        public static ChromeOptions GetDefaulOptions()
        {
            // Seeting Chorme.exe
            ChromeOptions options = new ChromeOptions();
            string Path_BrowsersLocation = Directory.GetCurrentDirectory() + "\\Browsers\\App\\Chrome-bin\\chrome.exe";
            options.BinaryLocation = Path_BrowsersLocation;
            // Create Profile
            #region createprofile()
            string Path_ProfileLocation = Directory.GetCurrentDirectory() + "\\profile";
            if (!Directory.Exists(Path_ProfileLocation))
            {
                Directory.CreateDirectory(Path_ProfileLocation);
            }
            string uniqueProfileFolder = Guid.NewGuid().ToString();
            string profilePath = Path.Combine(Path_ProfileLocation, uniqueProfileFolder);
            Directory.CreateDirectory(profilePath);
            #endregion
            options.AddArgument($"--user-data-dir={profilePath}");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--ignore-certificate-errors");
            // Kiểm tra Setting Option
            SettingOption SettingOption_Chrome = new SettingOption();
            if (!Directory.Exists(Paths.Path_SettingOpption))
            {
                var setingchrome = File.ReadAllText(Paths.Path_SettingOpption);
                SettingOption_Chrome = JsonConvert.DeserializeObject<SettingOption>(setingchrome);  
            }
            else 
            {
                MessageBox.Show("Vui lòng Setting cấu hình");
            }
            if (SettingOption_Chrome.check_HideChrome)
            {
                options.AddArgument("--headless");
            }
            if(SettingOption_Chrome.check_Anonymous)
            {
                options.AddArgument("--incognito");
            }
            if (SettingOption_Chrome.check_DisableGpu)
            {
                options.AddArgument("--disable-gpu");
            }
            
                return options;
        }
    }

    
}