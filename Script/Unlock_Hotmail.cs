using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Concurrent;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using SeleniumKeys = OpenQA.Selenium.Keys;


namespace X_Vframe_Tool
{
    public class Script_UnlockHotmail : Script_Sele
    {
        private static readonly object lockFile = new object();

        public static Dictionary<string, string> NamedTabs = new();
        public ConcurrentBag<DataGridViewRow>? Index_Login_2FA { get; set; }
        public ConcurrentBag<string>? IndexEmail;
        public ConcurrentBag<string>? IndexPhone;
        public static void NameCurrentTab(IWebDriver driver, string name)
        {
            string handle = driver.CurrentWindowHandle;
            if (!NamedTabs.ContainsKey(name))
                NamedTabs[name] = handle;
        }

        public static void SwitchToTab(IWebDriver driver, string name)
        {
            if (NamedTabs.ContainsKey(name))
            {
                driver.SwitchTo().Window(NamedTabs[name]);
            }
        }
        Random random = new Random();
        public async Task Run(int Thread, CancellationToken token, string win_pos)
        {

            if (token.IsCancellationRequested)
                return;
            while (Index_Login_2FA!.TryTake(out var row))
            {
                if (token.IsCancellationRequested)
                    return;
                row.Cells["Status"].Value = "Script Unlock Hotmail";
                Profile_GPM profile = new Profile_GPM();
                IndexEmail!.TryTake(out string? dataEmail);
                IndexPhone!.TryTake(out string? dataPhone);
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
                    // email
                    string email = dataEmail.Split('|')[0];
                    row.Cells["Email"].Value = email;
                    // password
                    string password = dataEmail.Split('|')[1];
                    row.Cells["Password"].Value = password;
                    //email recovery
                    string mkp_username = email.Split('@')[0];
                    string mkp_domain = "@smvmail.com";
                    string email_recovery = $"{mkp_username}{mkp_domain}";
                    // phone
                    string? phone = dataPhone?.Trim();
                    row.Cells["Phone"].Value = phone;
                    string lastphone = new string(phone.TakeLast(4).ToArray());
                    // link getotp
                    string get_otp = GlobalSetting.SettingOption_Run.check_UrlGetOtp; //MS_Viettel
                    // Script create profile
                    row.Cells["EmailRecovery"].Value = "GET DATA SUSCCESS";
                    row.Cells["Status"].Value = "Create Profile";
                    string profileName = $"Profile{Thread}";
                    int maxRetryCreate = 3;

                    for (int attempt = 1; attempt <= maxRetryCreate; attempt++)
                    {
                        try
                        {
                            profileId = await Profile_GPM.CreateProfile(profileName);

                            if (!string.IsNullOrEmpty(profileId))
                            {
                                row.Cells["EmailRecovery"].Value = $"Create Profile Success (Try {attempt})";
                                Console.WriteLine($"Tạo profile thành công: {profileId}");
                                break;
                            }
                            else
                            {
                                row.Cells["EmailRecovery"].Value = $"Create Profile Fail (Try {attempt})";
                                Console.WriteLine("Tạo profile thất bại, thử lại...");
                                await Task.Delay(300); // Delay 300ms như bạn muốn
                            }
                        }
                        catch (JsonReaderException ex)
                        {
                            row.Cells["EmailRecovery"].Value = $"JSON Error: {ex.Message}";
                            Console.WriteLine($"Lỗi JSON khi tạo profile (Try {attempt}): {ex.Message}");
                            await Task.Delay(300); // Delay 300ms
                        }
                        catch (Exception ex)
                        {
                            row.Cells["EmailRecovery"].Value = $"Other Error: {ex.Message}";
                            Console.WriteLine($"Lỗi khác khi tạo profile (Try {attempt}): {ex.Message}");
                            await Task.Delay(300); // Delay 300ms
                        }
                    }

                    if (string.IsNullOrEmpty(profileId))
                    {
                        row.Cells["EmailRecovery"].Value = "Create Profile Fail - Max Retry Reached";
                        goto EndLoop;  // Chuyển đến EndLoop nếu tạo profile thất bại hoàn toàn
                    }

                    // open Profile
                    row.Cells["Status"].Value = "Open Chrome 1";
                    Profile_GPM profile_GPM = new Profile_GPM();
                    int k_OpenGPM = 5;
                    string ApiGPM_Open = $"http://127.0.0.1:19995/api/v3/profiles/start/{profileId}?win_scale=0.7&win_pos={win_pos}&win_size=500,400";
                    int k_OpenProfileGPM = 5;
                    row.Cells["Status"].Value = "Open Chrome 2";
                    while (k_OpenGPM > 0)
                    {
                        IWebDriver? driver1 = null;
                        try
                        {
                            if (profile_GPM == null)
                            {
                                profile_GPM = new Profile_GPM();
                            }
                            driver1 = await profile_GPM.OpenProfile(ApiGPM_Open);
                        }
                        catch (Exception ex)
                        {
                            row.Cells["EmailRecovery"].Value = $"Other Error login : {ex.Message}";
                            await Task.Delay(300); // Delay 300ms
                            goto EndLoop;
                        }
                        if (driver1 != null )
                        {
                            row.Cells["Status"].Value = "Open Chrome 2.1";
                            k_OpenGPM = 0;
                            row.Cells["Status"].Value = "Profile open suscces";
                            driver = driver1;
                            row.Cells["EmailRecovery"].Value = "Profile open suscces";
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Open Chrome 3";
                            k_OpenGPM = k_OpenGPM - 1;
                            row.Cells["Status"].Value = "Profile open fail changeip delay 1s";
                            int maxRetry = 3;
                            bool proxyUpdated = false;
                            for (int attempt = 1; attempt <= maxRetry; attempt++)
                            {
                                try
                                {
                                    proxyUpdated = await profile_GPM.UpdateProxy(profileId);
                                    if (proxyUpdated)
                                    {
                                        row.Cells["Status"].Value = "Update Proxy Suscces";
                                        break; // Thành công, thoát retry
                                    }
                                    else
                                    {
                                        row.Cells["Status"].Value = "Update Proxy Fail, Retry";
                                        await Task.Delay(300); // Delay giữa các lần thử
                                    }
                                }
                                catch (JsonReaderException ex)
                                {
                                    row.Cells["Status"].Value = "Json errror";
                                    await Task.Delay(300);
                                    goto EndLoop;
                                }
                                catch (Exception ex)
                                {
                                    row.Cells["Status"].Value = $"Unexpected Error on attempt {attempt}: {ex.Message}";
                                    await Task.Delay(1000);
                                    goto EndLoop;
                                }
                            }





                            if (k_OpenGPM == 0)
                            {
                                row.Cells["Status"].Value = "Open Profile Fail";
                                row.Cells["EmailRecovery"].Value = "Profile open fail";
                                goto EndLoop;
                            }

                        }
                    }
                    //Kịch Bản
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // check ip
                    #region checkip()
                    row.Cells["Status"].Value = "checkip";
                    driver.Navigate().GoToUrl("https://ifconfig.me/ip");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    string xpath_checkip = "//pre[text()]";
                    IWebElement element_checkip = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checkip), 10);
                    if (element_checkip != null)
                    {
                        row.Cells["Status"].Value = "checkip suscces";
                        await Task.Delay(4000);
                        row.Cells["Proxy"].Value = $"{element_checkip.Text}";
                    }
                    else
                    {
                        row.Cells["Status"].Value = "No internet";
                        goto EndLoop;
                    }
                    #endregion
                    // mở trang microsoft
                    #region Login Microsoft()
                    row.Cells["Status"].Value = "Login Microsoft";
                    driver.Navigate().GoToUrl("https://go.microsoft.com/fwlink/p/?LinkID=2125442");
                    row.Cells["Status"].Value = "Await Page Login Microsoft 10s";
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    string? xpath_accountlock0 = "//div[@role='heading' and text() ='Your account has been locked']";
                    string? xpath_inputemail = "//input[@type='email']";
                    string? xpath_inputpass = "//input[@name='passwd']";
                    string? xpath_addemailrecovery = "//div[@id='iPageTitle' and text()=\"Let's protect your account\"]";
                    string? xpath_helpusyouraccount = "//div[@id=\"iPageTitle\" and text()='Help us secure your account']";
                    string? xpath_helpusyouraccount1 = "//div[@id=\"iSelectProofTitle\" and text()=\"Help us protect your account\"]";
                    //check account lock
                    int k_accountlock0 = 30;
                    while (k_accountlock0 > 0)
                    {
                        IWebElement element_lock0 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_accountlock0), 1);
                        if (element_lock0 != null)
                        {
                            row.Cells["Status"].Value = "Account lock, start unlock";
                            goto Unlock;
                        }
                        else
                        {
                            k_accountlock0 = k_accountlock0 - 1;
                            // input email
                            IWebElement element_inputemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemail), 1);
                            if (element_inputemail != null)
                            {
                                row.Cells["Status"].Value = $"input email k:{k_accountlock0}";
                                element_inputemail.SendKeys(email);
                                await Task.Delay(300);
                                element_inputemail.SendKeys(SeleniumKeys.Enter);
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                                await Task.Delay(1000);
                            }
                            // input pass
                            IWebElement element_inputpass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputpass), 1);
                            if (element_inputpass != null)
                            {
                                row.Cells["Status"].Value = $"input passw {k_accountlock0}";
                                element_inputpass.SendKeys(password);
                                await Task.Delay(300);
                                element_inputpass.SendKeys(SeleniumKeys.Enter);
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                                await Task.Delay(1000);
                            }
                            // add email recovery
                            string? otpold = string.Empty;
                            IWebElement element_addemailrecovery = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_addemailrecovery), 1);
                            if (element_addemailrecovery != null)
                            {
                                row.Cells["Status"].Value = $"ADD email recovery k:{k_accountlock0}";
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\Data.txt", $"{email}|{password}|addmkp" + Environment.NewLine);
                                }
                                goto EndLoop;
                                //string? xpath_inputemailrecovery = "//input[@aria-label='Alternate email address']";
                                //IWebElement element_inputemailrecovery = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemailrecovery), 1);
                                //if (element_inputemailrecovery != null)
                                //{
                                //    row.Cells["Status"].Value = "check email old";
                                //    otpold = await OtpHelper.GETOTPSMV(email_recovery, 10);
                                //    element_inputemailrecovery.SendKeys(email_recovery);
                                //    await Task.Delay(300);
                                //    driver.FindElement(By.XPath("//input[@type='submit']")).Click();
                                //    await Task.Delay(300);
                                //    SeleniumHelper.WaitForPageLoad(driver, 10);
                                //    string? xpath_inputotp = "//input[@placeholder='Code']";
                                //    IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputotp), 10);
                                //    string? otp = string.Empty;
                                //    if (element_inputotp != null)
                                //    {
                                //        int k_getpotp = 30;
                                //        while (k_getpotp > 0)
                                //        {
                                //            row.Cells["Status"].Value = $"Get OTP Mail {k_getpotp}";
                                //            otp = await OtpHelper.GETOTPSMV(email_recovery, 1);
                                //            if (otp != null && otp != otpold)
                                //            {
                                //                row.Cells["Status"].Value = $"otp: {otp}";
                                //                element_inputotp.SendKeys(otp);
                                //                await Task.Delay(random.Next(300, 800));
                                //                driver.FindElement(By.XPath("//input[@type='submit']")).Click();
                                //                SeleniumHelper.WaitForPageLoad(driver, 10);
                                //                await Task.Delay(random.Next(1000, 1500));

                                //                break;
                                //            }
                                //            else
                                //            {
                                //                row.Cells["Status"].Value = "không tìm thấy otp";
                                //                k_getpotp = k_getpotp - 1;
                                //                if (k_getpotp == 0)
                                //                {
                                //                    row.Cells["Status"].Value = "Get OTP Mail Fail";
                                //                    goto EndLoop;
                                //                }
                                //                else
                                //                {
                                //                    await Task.Delay(1000);
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                            }
                            // too many request
                            IWebElement element_toomanyrequest = SeleniumHelper.WaitForElement(driver, By.XPath("//[text()='Too Many Requests']"), 1);
                            if (element_toomanyrequest != null)
                            {
                                row.Cells["Status"].Value = $"Too Many Requests k:{k_accountlock0}";
                                goto EndLoop;
                            }
                            // Account live
                            string? xpath_kmsi = "//div[@id='kmsiTitle']";
                            IWebElement element_kmsi = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_kmsi), 1);
                            if (element_kmsi != null)
                            {
                                row.Cells["Status"].Value = $"Account live k:{k_accountlock0}";
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\Data.txt", $"{email}|{password}|live" + Environment.NewLine);
                                }
                                goto EndLoop;
                            }
                            // error pass
                            string xpath_saipass = "//div[text()=\"Your account or password is incorrect. If you don't remember your password, \"]";
                            IWebElement element_saipass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_saipass), 1);
                            if (element_saipass != null)
                            {
                                row.Cells["Status"].Value = $"sai passworld k: {k_accountlock0}";
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|saipass" + Environment.NewLine);
                                }
                                goto EndLoop;
                            }
                            // error pass1
                            string? xpath_saipass1 = "//span[text()=\"That password is incorrect for your Microsoft account.\"]";
                            IWebElement element_saipass1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_saipass1), 1);
                            if (element_saipass1 != null)
                            {
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|saipass" + Environment.NewLine);
                                }
                                row.Cells["Status"].Value = $"sai passworld k:{k_accountlock0}";
                                goto EndLoop;
                            }
                            // helpusyouraccount
                            IWebElement element_helpusyouraccount = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_helpusyouraccount), 1);
                            if (element_helpusyouraccount != null)
                            {
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|helpusyouraccount" + Environment.NewLine);
                                }
                                row.Cells["Status"].Value = $"helpusyouraccount k: {k_accountlock0}";
                                goto EndLoop;
                            }
                            // helpusyouraccount1
                            IWebElement element_helpusyouraccount1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_helpusyouraccount1), 1);
                            if (element_helpusyouraccount1 != null)
                            {
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|helpusyouraccount1" + Environment.NewLine);
                                }
                                row.Cells["Status"].Value = $"helpusyouraccount k: {k_accountlock0}";
                                goto EndLoop;
                            }
                            // Very your email
                            string? xpath_saipassverymkp = "//h1[text()=\"Verify your email\"]";
                            IWebElement element_saipassverymkp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_saipassverymkp), 1);
                            if (element_saipassverymkp != null)
                            {
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|saipassverrymkp" + Environment.NewLine);
                                }
                                row.Cells["Status"].Value = $"sai pass verry mkp: {k_accountlock0}";
                                goto EndLoop;
                            }
                            //unavailable
                            string xpath_unavailable = "//div[@id=\"idPageTitle\" and text()=\"This site is temporarily unavailable\"]";
                            IWebElement element_unavailable = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_unavailable), 1);
                            if (element_unavailable != null)
                            {
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|block" + Environment.NewLine);
                                }
                                row.Cells["Status"].Value = $"unavailable: {k_accountlock0}";
                                goto EndLoop;
                            }
                            // refresh page
                            if (k_accountlock0 == 5)
                            {
                                row.Cells["Status"].Value = "Refresh Page";
                                driver.Navigate().Refresh();
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                                await Task.Delay(1500);
                            }
                            //delay
                            await Task.Delay(1000);
                            // fail
                            if (k_accountlock0 == 0)
                            {
                                row.Cells["Status"].Value = "Login Microsoft Fail";
                                goto EndLoop;
                            }
                        }
                    }
                #endregion

                //
                Unlock:
                    // 
                    driver.FindElement(By.XPath("//button[@type='button']")).Click();
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    await Task.Delay(1000);
                    int k_unlockaccount = 10;
                    string? xpath_inputphonelock = string.Empty;
                    while (k_unlockaccount > 0)
                    {
                        xpath_inputphonelock = "//input[@aria-labelledby='enterPhoneNumberTitle']";
                        IWebElement elemt_inputphonelock = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputphonelock), 1);
                        if (elemt_inputphonelock != null)
                        {
                            row.Cells["Status"].Value = "input phone unlock";
                            var dropdown = new SelectElement(driver.FindElement(By.Id("phoneCountry")));
                            dropdown.SelectByValue("VN");
                            await Task.Delay(3000);
                            string? phone1 = phone.Substring(2);
                            elemt_inputphonelock.SendKeys(phone1);
                            await Task.Delay(300);
                            driver.FindElement(By.XPath("//button[@id='nextButton']")).Click();
                            SeleniumHelper.WaitForPageLoad(driver, 15);
                            await Task.Delay(300);
                            goto Captcha;
                        }
                        else
                        {
                            k_unlockaccount = k_unlockaccount - 1;
                            await Task.Delay(1000);
                            IWebElement element_lock0 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_accountlock0), 1);
                            if (element_lock0 != null)
                            {
                                row.Cells["Status"].Value = "Account lock, next";
                                driver.FindElement(By.XPath("//button[@type='button']")).Click();
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                                await Task.Delay(1000);
                            }
                            if (k_unlockaccount == 0)
                            {
                                row.Cells["Status"].Value = "Unlock Account Fail";
                                goto EndLoop;
                            }
                        }
                    }
                Captcha:
                    string? bypasscapthca = string.Empty;
                    await Task.Delay(3000);
                    string? xpath_inputotpphone = "//input[@type='phone' and @id='enter-code-input']";
                    string? xpath_captcha = "//div[text()='Help us beat the robots']";
                    string? xpath_unlockfail = "//div[@role='heading' and text()='Try another verification method']";
                    int k_bypasscapthca = 30;
                    while (k_bypasscapthca >0)
                    {
                        IWebElement element_inputotpphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputotpphone), 1);
                        if (element_inputotpphone != null)
                        {
                            row.Cells["Status"].Value = "bypasscaptcha suscces, get otp"; 
                            await Task.Delay(300);
                            goto getotp;
                        }
                        else
                        {
                            k_bypasscapthca = k_bypasscapthca - 1;
                            await Task.Delay(4000);
                            IWebElement element_bypasscaptcha = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_captcha), 1);
                            if(element_bypasscaptcha != null )
                            {
                                row.Cells["Status"].Value = $"await bypass captcha, delay5s k:{k_bypasscapthca}";

                            }
                            IWebElement element_unlockfail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_unlockfail), 1);
                            if( element_unlockfail != null )
                            {
                                row.Cells["Status"].Value = "Unlock Fail Try another verification method";
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|block" + Environment.NewLine);
                                }
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\Phoneused.txt", $"{phone}" + Environment.NewLine);
                                }
                                goto EndLoop;
                            }

                            if (k_bypasscapthca == 0)
                            {
                                row.Cells["Status"].Value = "By Pass Captcha Fail";
                                lock (lockFile)
                                {
                                    File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|captchafail" + Environment.NewLine);
                                }
                                goto EndLoop;
                            }
                           
                        }
                    }
                getotp:
                    string? url = get_otp;
                    string? xpath_unlocksuscces = string.Empty;
                    xpath_unlocksuscces = "//div[@role=\"heading\" and text()= 'Your account has been unblocked']";
                    string? otpphone = string.Empty;
                    int k_checkunlocksuscces = 30;
                    while(k_checkunlocksuscces>0)
                    {
                        IWebElement element_unlocksuscces = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_unlocksuscces), 1);
                        if (element_unlocksuscces != null)
                        {
                            row.Cells["Status"].Value = "Unlock Suscces";
                            goto EndLoop;
                        }
                        else
                        {
                            k_checkunlocksuscces= k_checkunlocksuscces-1;
                            IWebElement element_inputotpphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputotpphone), 1);
                            if (element_inputotpphone != null)
                            {
                                int k_getcode = 30;
                                while (k_getcode > 0)
                                {
                                    otpphone = await GETOTPVFARM.GetOTPFarmAsync(url, phone);
                                    if (otpphone != null)
                                    {
                                        row.Cells["Status"].Value = $"OTP Phone: {otpphone}";
                                        element_inputotpphone.SendKeys(otpphone);
                                        await Task.Delay(300);
                                        element_inputotpphone.SendKeys(SeleniumKeys.Enter);
                                       SeleniumHelper.WaitForPageLoad(driver, 10);
                                        int k_checksusscesc = 5;
                                        while (k_checksusscesc >0)
                                        {
                                            IWebElement element_unlocksuscces1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_unlocksuscces), 1);
                                            if (element_unlocksuscces1 != null)
                                            {
                                                lock (lockFile)
                                                {
                                                    File.AppendAllText("output\\Data.txt", $"{email}|{password}|{phone}" + Environment.NewLine);
                                                }
                                                lock (lockFile)
                                                {
                                                    File.AppendAllText("output\\Phoneused.txt", $"{phone}|{otpphone}" + Environment.NewLine);
                                                }
                                                row.Cells["Status"].Value = "Unlock Suscces";
                                                goto EndLoop;
                                            }
                                            else
                                            {
                                                k_checksusscesc = k_checksusscesc - 1;
                                                await Task.Delay(2000);
                                                if(k_checksusscesc ==0 )
                                                {
                                                    lock (lockFile)
                                                    {
                                                        File.AppendAllText("output\\Data.txt", $"{email}|{password}|{phone}" + Environment.NewLine);
                                                    }
                                                    lock (lockFile)
                                                    {
                                                        File.AppendAllText("output\\Phoneused.txt", $"{phone}|{otpphone}" + Environment.NewLine);
                                                    }
                                                    row.Cells["Status"].Value = "Get Otp Suscces, Không Xác định";
                                                    goto EndLoop;
                                                }    
                                            }
                                        }
                                           

                                    }
                                    else
                                    {
                                        row.Cells["Status"].Value = $"không tìm thấy otp, again {k_getcode}";
                                        k_getcode = k_getcode - 1;
                                        if (k_getcode == 0)
                                        {
                                            row.Cells["Status"].Value = "Không có OTP";
                                            lock (lockFile)
                                            {
                                                File.AppendAllText("output\\AccountBlock.txt", $"{email}|{password}|NOOTP" + Environment.NewLine);
                                            }
                                            goto EndLoop;
                                        }
                                        else
                                        {
                                            await Task.Delay(1000);
                                        }
                                    };
                                } 
                            }
                        }
                    }
                //Kịch Bản
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                EndLoop:
                    await profile.CloseProfile(profileId);
                    await Task.Delay(1000);
                    await profile.Delete_Profile(profileId);
                    if( driver != null)
                    {
                        driver.Quit();
                    }
                }
                catch (Exception ex)
                {
                    row.Cells["EmailRecovery"].Value = $"Lỗi catch: {ex.Message}";
                    await profile.CloseProfile(profileId);
                    await Task.Delay(300);
                    await profile.Delete_Profile(profileId);
                    driver.Quit();
                }
                finally
                {
                    // Đảm bảo luôn đóng profile ngay cả khi có lỗi
                    if (!string.IsNullOrEmpty(profileId))
                    {
                        try
                        {
                            await profile.CloseProfile(profileId);
                            await profile.Delete_Profile(profileId);
                        }
                        catch (Exception ex)
                        {
                            row.Cells["EmailRecovery"].Value = $"Lỗi finaaly: {ex.Message}";
                        }
                    }

                    if (driver != null)
                    {
                        try
                        {
                            driver.Quit();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi Quit driver: {ex.Message}");
                        }
                    }
                }
            }
        }
    }


}