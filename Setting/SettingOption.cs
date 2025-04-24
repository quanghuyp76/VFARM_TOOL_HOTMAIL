using System;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Windows.Forms;

namespace X_Vframe_Tool
{
    public static class GlobalSetting
    {
        public static SettingOption? SettingOption_Run {  get; set; }
    }
    public class SettingOption
    {
        
        // Các tùy chọn của chrome
        public bool check_HideChrome { get; set; } = false;
        public bool check_Anonymous { get; set; } = false;
        public bool check_DisableGpu { get; set; } = false;
        // Các tùy chọn input
        public bool check_Input_File_Location { get; set; } = false;
        public bool check_Input_Website { get; set; } = false;
        // Các tùy chọn của Proxy
        public bool check_Proxy_IP_PORT { get; set; } = false;
        public bool check_Proxy_Never { get; set; } = false;
        // chọn browser
        public string check_Browser { get; set; } = "GPM";// GPM,...
        // chọn script Code
        public string check_Code { get; set; } = "Selenium";    // Selenium, Playwright,....
        public string check_Script { get; set; } = "Reg Mail Office";    // Script,....
        // chọn design Chrome
        public string check_ChromeSize { get; set; } = "1920x1080";
        public string check_ChormeArrange { get; set; } = "1x1";
        // check run
        public int check_NumberProcess { get; set; } = 1;
        public int check_NumberThread { get; set; } = 1;
        // check link getotp
        public string check_UrlGetOtp {  get; set; } = "";

        public static SettingOption GetSettingOption(MainForm form)
        {
            return new SettingOption
            {
                check_HideChrome = form.checkBox_HideChrome.Checked,
                check_Anonymous = form.checkBox_Anonymous.Checked,
                check_DisableGpu = form.checkBox_DisableGpu.Checked,
                check_Input_File_Location = form.checkBox_InputLocalFile.Checked,
                check_Input_Website = form.checkBox_InputUrl.Checked,
                check_Proxy_IP_PORT = form.checkBox_ProxyLocal.Checked,
                check_Proxy_Never = form.checkBox_ProxyNon.Checked,
                check_Browser = form.comboBox_Browser.SelectedItem?.ToString() ?? "Default",
                check_Code = form.comboBox_Code.SelectedItem?.ToString() ?? "Default",
                check_Script = form.comboBox_Script.SelectedItem?.ToString() ?? "Default",
                check_ChormeArrange = form.textBox_ChromeArrange.Text,
                check_ChromeSize = form.textBox_ChromeSize.Text,
                check_NumberProcess = int.TryParse(form.textBox_Process.Text, out int process) ? process : 1,
                check_NumberThread = (int)form.numericUpDown_NumberThread.Value,
                check_UrlGetOtp = form.textBox_UrlGetOtp.Text,
            };
        }
    }

}