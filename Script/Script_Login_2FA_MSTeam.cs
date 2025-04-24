using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.Concurrent;
using File = System.IO.File;
using SeleniumKeys = OpenQA.Selenium.Keys;
using OpenQA.Selenium.Interactions;
using System.Runtime.InteropServices;
using OpenQA.Selenium.Interactions;
using System;
using System.Windows.Forms;

  

namespace X_Vframe_Tool
{
    
    public class Script_Login_2FA_MSTeam : Script_Sele
    {

        public ConcurrentBag<DataGridViewRow>? Index_Login_2FA { get; set; }
        public ConcurrentBag<string>? IndexEmail;
        Random random = new Random();
        public async Task Run(int Thread, CancellationToken token, string win_pos)
        {

            if (token.IsCancellationRequested)
                return;
            while (Index_Login_2FA!.TryTake(out var row))
            {
                if (token.IsCancellationRequested)
                    return;
                row.Cells["Status"].Value = "Script Login 2FA GPM";
                Profile_GPM profile = new Profile_GPM();
                IndexEmail!.TryTake(out string? dataEmail);
                string? profileId = null;
                IWebDriver? driver = null;
                try
                {
                    string[] parts = dataEmail.Split('|');
                    if (parts.Length < 2)
                    {
                        row.Cells["Status"].Value = "Sai Định Dạng Dữ Liệu";
                        goto EndLoop;
                    }
                    string phone = dataEmail.Split('|')[0];
                    row.Cells["Phone"].Value = phone;
                    string get_otp = dataEmail.Split('|')[1];
                    row.Cells["Password"].Value = get_otp;
                    string otp_oldss = "";
                    
                       
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
                        goto EndLoop;
                    }
                    // open Profile
                    row.Cells["Status"].Value = "Open Chrome";
                    Profile_GPM profile_GPM = new Profile_GPM();
                    int k_OpenGPM = 5;
                    string ApiGPM_Open = $"http://127.0.0.1:19995/api/v3/profiles/start/{profileId}?win_scale=0.7&win_pos={win_pos}&win_size=500,300";
                    int k_OpenProfileGPM = 5;

                    while (k_OpenGPM > 0)
                    {
                        IWebDriver driver1 = await profile_GPM.OpenProfile(ApiGPM_Open);
                        if (driver1 != null)
                        {
                            k_OpenGPM = 0;
                            row.Cells["Status"].Value = "Profile open suscces";
                            driver = driver1;
                        }
                        else
                        {
                            k_OpenGPM = k_OpenGPM - 1;
                            row.Cells["Status"].Value = "Profile open fail changeip delay 1s";
                            await Task.Delay(1000);
                            await profile_GPM.UpdateProxy(profileId);
                            if (k_OpenGPM == 0)
                            {
                                row.Cells["Status"].Value = "Open Profile Fail";
                                goto EndLoop;
                            }

                        }
                    }
                    //Kịch Bản
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                        goto EndLoop;
                    }
                    // mở trang microsoft
                    row.Cells["Status"].Value = "Login Microsoft";
                    string url_1 = "https://entra.microsoft.com/signin/index/";
                    string url_2 = "https://www.office.com/login?es=UnauthClick";
                    int x = random.Next(1, 2);
                    if (x == 1)
                    {
                        driver.Navigate().GoToUrl(url_1);
                    }
                    else
                    {
                        driver.Navigate().GoToUrl(url_2);
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    //////

                    string get_otp_old = get_otp + phone;
                    row.Cells["Status"].Value = " get otp old";

                    string otp_old = await OtpHelper.GetOtp(get_otp_old, 1);
                    if (otp_old != null)
                    {
                        otp_oldss = otp_old;
                        row.Cells["Status"].Value = $"otp old: {otp_old}";
                    }
               






                    // check element nhập mail
                    int k_checkinput_email = 5;
                    await Task.Delay(3000);
                    while (k_checkinput_email > 0)
                    {
                        string xpath_inputemail = "//input[@type='email']";
                        IWebElement element_inputemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemail), 10);
                        if (element_inputemail != null)
                        {
                            row.Cells["Status"].Value = "check element input phone true, nhập phone";
                            k_checkinput_email = 0;
                            element_inputemail.SendKeys(phone);
                            await Task.Delay(random.Next(500,1000));
                            element_inputemail.SendKeys(SeleniumKeys.Enter);
                            row.Cells["Status"].Value = "Delay1s";
                            await Task.Delay(random.Next(1000, 1100));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy nút nhập email, again";
                            k_checkinput_email = k_checkinput_email - 1;
                            await Task.Delay(2000);
                            if (k_checkinput_email == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy nút nhập email";
                                goto EndLoop;
                            }
                        }
                    }
                    row.Cells["Status"].Value = "Check element nhập code";
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    // check element input code
                    
                    row.Cells["Status"].Value = "check input otp";
                    string xpath_otp = "//input[@name=\"piotc\"]";
                    int k_element_inputotp = 5;
                    string toast_otp = "";
                    while (k_element_inputotp > 0)
                    {
                        IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_otp), 3);
                        if (element_inputotp != null)
                        {
                            row.Cells["Status"].Value = "check input otp true";
                            k_element_inputotp = 0;
                            string get_otp1 = get_otp ;
                            row.Cells["Status"].Value = "bắt đầu get otp";
                            int k_checkotp = 20;
                            while (k_checkotp>0)
                            {
                                string otp = await OtpHelper.GetOtp(get_otp1, 1);
                                if (otp != null && otp != otp_oldss)
                                {
                                    toast_otp = otp;
                                    Console.WriteLine($"✅ OTP: {otp}");
                                    row.Cells["Status"].Value = $"otp: {otp}";
                                    element_inputotp.SendKeys(otp);
                                    await Task.Delay(300);
                                    element_inputotp.SendKeys(SeleniumKeys.Enter);
                                    await Task.Delay(1000);
                                    File.AppendAllText("output\\Data.txt", $"{phone}|{otp}" + Environment.NewLine);
                                    await Task.Delay(random.Next(2500, 3000));
                                    goto EndLoop;
                                }
                                else
                                {
                                    k_checkotp= k_checkotp - 1;
                                    row.Cells["Status"].Value = $"Không có OTP, checkj lại {k_checkotp}";
                                    File.AppendAllText("output\\Nootp.txt", $"{phone}|" + Environment.NewLine);
                                    otp = await OtpHelper.GetOtp(get_otp1, 1);
                                    await Task.Delay(1000);
                                    if (k_checkotp == 0)
                                    {
                                        row.Cells["Status"].Value = "Không có OTP";
                                        goto EndLoop;
                                    }
                                }
                            }    
                            
                        }
                        else
                        {
                            k_element_inputotp = k_element_inputotp - 1;
                            string xpath22 = "//div[@id=\"usernameError\"]";
                            IWebElement element_inputemailfail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath22), 1);
                            if (element_inputemailfail != null)
                            {
                                row.Cells["Status"].Value = "Login Fail";
                                goto EndLoop;
                            }
                            string xpath33 = "//div[@id=\"loginHeader\"]";
                            IWebElement element_inputemailfail1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath33), 1);
                            if (element_inputemailfail1 != null)
                            {
                                row.Cells["Status"].Value = "Login Fail";
                                goto EndLoop;
                            }
                            // cơ quan trường học
                            string xpath66 = "//div[@id='aadTileTitle' and contains(text(), 'cơ quan')]";
                            IWebElement element_coquan = SeleniumHelper.WaitForElement(driver, By.XPath(xpath66), 1);
                            if (element_coquan != null)
                            {
                                element_coquan.Click();
                                row.Cells["Status"].Value = "Click Cơ quan";
                                await Task.Delay(2000);
                            }

                            await Task.Delay(2000);
                        }
                    }

                    string xpath_checksucces = "//div[text()='Duy trì đăng nhập?']";
                    int k_element_checksucces = 5;
                    while (k_element_checksucces > 0)
                    {
                        IWebElement element_checksucces = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checksucces), 10);
                        if (element_checksucces != null)
                        {
                            k_element_checksucces = 0;
                            row.Cells["Status"].Value = $"login succes otp {toast_otp}";
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
                //Kịch Bản
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                EndLoop:
                    await profile.CloseProfile(profileId);
                    await profile.Delete_Profile(profileId);
                    driver.Quit();
                }
                catch (Exception ex)
                {
                    row.Cells["Status"].Value = $"Lỗi: {ex.Message}";

                    await profile.CloseProfile(profileId);
                    await profile.Delete_Profile(profileId);
                    driver.Quit();
                }
                finally
                {
                    // Đảm bảo luôn đóng profile ngay cả khi có lỗi
                    if (!string.IsNullOrEmpty(profileId))
                    {
                        await profile.CloseProfile(profileId);
                        await profile.Delete_Profile(profileId);
                        driver.Quit();
                    }
                }
            }
        }
    }


}