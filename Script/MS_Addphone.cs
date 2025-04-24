using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using X_Vfarme.GPM;
using System;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using SeleniumKeys = OpenQA.Selenium.Keys;
using static System.Net.WebRequestMethods;
using System.IO;
using File = System.IO.File;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace X_Vframe_Tool
{
    public class Script_MS_AddPhone: Script_Sele
    {

        public ConcurrentBag<DataGridViewRow>? Index_MsAddPhone { get; set; }
        public ConcurrentBag<string>? IndexEmail ;
        Random random = new Random();
        public async Task Run(int Thread, CancellationToken token, string win_pos)
        {
            if (token.IsCancellationRequested)
            return;
            while (Index_MsAddPhone!.TryTake(out var row))
            {
                if (token.IsCancellationRequested)
                    return;
                row.Cells["Status"].Value = "Script Add Phone GPM";
                Profile_GPM profile = new Profile_GPM();
                IndexEmail!.TryTake(out string? dataEmail);
                string? profileId = null;
                try
                {
                    // Script Selenium);
                    // input
                    
                    //input id hotmail | pass hotmail | mkp | phone | 2fa
                    // laphamhaw737942@hotmail.com|Dz8g46mV9FZZAPm|laphamhaw737942@smvmail.com|362374507
                    string email = dataEmail.Split('|')[0];
                    row.Cells["Email"].Value = email;
                    string password = dataEmail.Split('|')[1];
                    row.Cells["Password"].Value = password;
                    string email_recovery = dataEmail.Split('|')[2];
                    row.Cells["EmailRecovery"].Value = email_recovery;
                    string phone = dataEmail.Split('|')[3];
                    row.Cells["Phone"].Value = phone;
                    string code_2fa = dataEmail.Split('|')[4];
                    string lastphone = new string(phone.TakeLast(4).ToArray());
                    string get_otp = $"http://otp.vocfor.site/get_otp?Brand=Microsoft&Token=c3580a8681a9bfa8663ae7c16b6def&Isdn={phone}"; //MS_Viettel
                    // Script create profile
                    row.Cells["Status"].Value = "Create Profile";
                    string profileName = $"Profile{Thread}";
                    profileId = await Profile_GPM.CreateProfile(profileName);
                    if (profileId != null)
                    {
                        row.Cells["Status"].Value = "Create Profile suscces";
                    }
                    else
                    {
                        row.Cells["Status"].Value = "Create Profile Fail";
                        return;
                    }
                    // open Profile
                    row.Cells["Status"].Value = "Open Chrome";
                    Profile_GPM profile_GPM = new Profile_GPM();
                    string ApiGPM_Open = $"http://127.0.0.1:19995/api/v3/profiles/start/{profileId}?win_scale=0.7&win_pos={win_pos}&win_size=500,300";
                    IWebDriver driver = await profile_GPM.OpenProfile(ApiGPM_Open);
                    if (driver != null)
                    {
                        row.Cells["Status"].Value = "Profile open suscces";
                    }
                    else
                    {
                        row.Cells["Status"].Value = "Profile open fail";
                        return;
                    }
                    // check ip
                    row.Cells["Status"].Value = "checkip";
                    driver.Navigate().GoToUrl("https://ifconfig.me/ip");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    string xpath_checkip = "//pre[text()]";
                    IWebElement element_checkip = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checkip), 10);
                    if (element_checkip != null)
                    {
                        row.Cells["Status"].Value = "checkip suscces";
                        row.Cells["Proxy"].Value = $"{element_checkip.Text}";
                    }
                    else
                    {
                        row.Cells["Status"].Value = "No internet";
                        return;
                    }
                    // mở trang microsoft
                    row.Cells["Status"].Value = "Login Microsoft";
                    driver.Navigate().GoToUrl("https://go.microsoft.com/fwlink/p/?LinkID=2125442");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500,2500));
                    // check element nhập mail
                    row.Cells["Status"].Value = "check element input mail";
                    string xpath_email = "//input[@type='email']";
                    IWebElement element_email = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_email), 10);
                    if (element_email != null)
                    {
                        row.Cells["Status"].Value = "Input email true";
                        element_email.SendKeys(email);
                        await Task.Delay(300);
                        element_email.SendKeys(SeleniumKeys.Enter);
                        await Task.Delay(300);
                    }
                    else
                    {
                        row.Cells["Status"].Value = "Input Fail";
                        return;
                    }
                    // check element nhập passw
                    row.Cells["Status"].Value = "Check Input Pass";
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    string xpath_passw = "//input[@name='passwd']";
                    row.Cells["Status"].Value = "check element passw";
                    IWebElement element_passw = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_passw), 10);
                    if (element_passw != null)
                    {
                        row.Cells["Status"].Value = "input passw true";
                        element_passw.SendKeys(password);
                        await Task.Delay(300);
                        element_passw.SendKeys(SeleniumKeys.Enter);
                        await Task.Delay(300);
                    }
                    else
                    {
                        row.Cells["Status"].Value = "find input passw fail";
                        return;
                    }
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500,2500));
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    //element lcok




                    // element submit
                    row.Cells["Status"].Value = "check stay login";
                    string xpath_staylogin = "//button[@type='submit' ]";
                    IWebElement element_staylogin = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_staylogin), 10);
                    int k_staylogin = 5;
                    while (k_staylogin >0) 
                    {
                        if(element_staylogin != null)
                        {
                            row.Cells["Status"].Value = "check stay login true";
                            element_staylogin.Click();
                            row.Cells["Status"].Value = "Delay 2s";
                            await Task.Delay(random.Next(1500,2500));
                            k_staylogin=0;
                        }
                        else
                        {
                            row.Cells["Status"].Value = $"check stay login fail: {k_staylogin}";
                            k_staylogin = k_staylogin - 1;
                            await Task.Delay(1000);
                        }
                    }
                    await Task.Delay(1000);
                    SeleniumHelper.WaitForPageLoad(driver, 15);
                    row.Cells["Status"].Value = "Chờ inbox";
                    string xpath_outlook = "//span[contains(text(), 'Home')]";
                    //string xpath_outlookphone = "(//span[contains(text(), '* 84')])[1]";
                    int k_element_outlook = 5;
                    while (k_element_outlook > 0)
                    {
                        IWebElement element_outlook = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_outlook), 10);
                        if (element_outlook != null)
                        {
                            row.Cells["Status"].Value = "Chờ inbox true switch activity";
                            k_element_outlook = 0;
                            driver.Navigate().GoToUrl("https://account.microsoft.com/activity");
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            row.Cells["Status"].Value = $"Chờ inbox fail: {k_element_outlook}";
                            k_element_outlook = k_element_outlook - 1;
                            await Task.Delay(1500);
                        }
                    }

                    row.Cells["Status"].Value = "check phương pháp xác minh ";
                    await Task.Delay(1000);
                    string xpath_moresecurity= "//span[text()='Hiển thị thêm phương pháp xác minh']";
                    int k_element_moresecurity = 5;
                    while(k_element_moresecurity>0)
                    {
                        IWebElement element_moresecurity = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_moresecurity), 10);
                        if(element_moresecurity != null)
                        {
                            row.Cells["Status"].Value = "check phương pháp xác minh suscces";
                            k_element_moresecurity = 0;
                            element_moresecurity.Click();
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            row.Cells["Status"].Value = "Delay 2s";
                            await Task.Delay(random.Next(1500, 2500));
                        }
                        else 
                        {
                            row.Cells["Status"].Value = "check phương pháp xác minh fail";
                            k_element_moresecurity = k_element_moresecurity - 1;
                            string xpath_nophone = "//span[contains(text(), \"Tôi không \")] ";
                            IWebElement element_nophone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_nophone), 10);
                            if (element_nophone != null)
                            {
                                row.Cells["Status"].Value = "Chưa add phone";
                                File.AppendAllText("output\\Nophone.txt", $"{email}|{password}|{email_recovery}|{phone}|{code_2fa}|no_phone" + Environment.NewLine);
                                return;

                            }
                                row.Cells["Status"].Value = "Delay 2s";
                            await Task.Delay(random.Next(1500, 2500));
                        }
                    }
                    row.Cells["Status"].Value = "check ô very phone";
                    string xpath_getphone = "//div[@data-testid=\"mainText\" and contains(text(), 'Nhắn tin đến')]";
                    int k_element_getphone = 5;
                    while (k_element_getphone > 0)
                    {
                        IWebElement element_getphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_getphone), 10);
                        if (element_getphone != null)
                        {
                            row.Cells["Status"].Value = "check ô very phone true";
                            k_element_getphone = 0;
                            element_getphone.Click();
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            row.Cells["Status"].Value = "check ô very phone fail";
                            k_element_getphone = k_element_getphone - 1;
                            await Task.Delay(2000);
                        }
                    }
                    row.Cells["Status"].Value = "check element 4 số cuối";
                    string xpath_4socuoi = "//input";
                    int k_element_4socuoi = 5;
                    while (k_element_4socuoi > 0)
                    {
                        IWebElement element_4socuoi = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_4socuoi), 10);
                        if (element_4socuoi != null)
                        {
                            row.Cells["Status"].Value = "check element 4 số cuối true";
                            k_element_4socuoi = 0;
                            element_4socuoi.SendKeys(lastphone);
                            await Task.Delay(300);
                            element_4socuoi.SendKeys(SeleniumKeys.Enter);
                            await Task.Delay(5000);
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            row.Cells["Status"].Value = "check element 4 số cuối fail";
                            k_element_4socuoi = k_element_4socuoi - 1;
                            await Task.Delay(4000);
                        }
                    }
                    row.Cells["Status"].Value = "check input otp";
                    string xpath_otp = "//input[@class=\"ext-input ext-text-box\"]";
                    int k_element_inputotp = 5;
                    while (k_element_inputotp > 0)
                    {
                        IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_otp), 10);
                        if (element_inputotp != null)
                        {
                            row.Cells["Status"].Value = "check input otp true";
                            k_element_inputotp = 0;
                            string get_otp1 = $"http://otp.vocfor.site/get_otp?Brand=Microsoft&Token=c3580a8681a9bfa8663ae7c16b6def&Isdn={phone}";
                            string otp = await OtpHelper.GetOtp(get_otp1, 30);
                            if (otp != null)
                            {
                                Console.WriteLine($"✅ OTP: {otp}");
                                row.Cells["Status"].Value = $"otp: {otp}";
                                element_inputotp.SendKeys(otp);
                                await Task.Delay(300);
                                element_inputotp.SendKeys(SeleniumKeys.Enter);
                                await Task.Delay(1000);
                                File.AppendAllText("output\\Data.txt", $"{email}|{password}|{email_recovery}|{phone}|{code_2fa}|{otp}" + Environment.NewLine);
                            }
                            else
                            {
                                row.Cells["Status"].Value = "check input fail";
                                return;
                            }
                        }
                        else
                        {
                            k_element_inputotp = k_element_inputotp - 1;
                            await Task.Delay(2000);
                        }
                    }

                    string xpath_checksucces = "//a[text()='Tài khoản Microsoft']";
                    int k_element_checksucces = 5;
                    while (k_element_checksucces > 0)
                    {
                        IWebElement element_checksucces= SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checksucces), 10);
                        if (element_checksucces != null)
                        {
                            k_element_checksucces = 0;
                            row.Cells["Status"].Value = "very otp succes";
                            await profile.CloseProfile(profileId);
                            await Task.Delay(1000);
                            await profile.Delete_Profile(profileId);
                        }
                        else
                        {
                            k_element_checksucces = k_element_checksucces - 1;
                            await Task.Delay(2000);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    row.Cells["Status"].Value = $"Lỗi: {ex.Message}";
                   
                    await profile.CloseProfile(profileId);
                    await profile.Delete_Profile(profileId);
                }
            }
        }
    }


}