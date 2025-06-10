using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Collections.Concurrent;
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
                    if (parts.Length < 3)
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
                    // phone
                    string phone = dataEmail.Split('|')[2];
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
                        if (driver1 != null)
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
                    driver.Navigate().GoToUrl("https://mysignins.microsoft.com/security-info?tenant=7212a37c-41a9-4402-9f69-ac32c6f76e1a");
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
                        IWebElement element_lock0 = SeleniumHelper.WaitForElement(driver, By.XPath("//div[text()='More information required']"), 1);
                        if (element_lock0 != null)
                        {
                            row.Cells["Status"].Value = "Add Phone";
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
                            //stay is login
                            IWebElement element_staylogin = SeleniumHelper.WaitForElement(driver, By.XPath("//h1[text()='Stay signed in?']"), 1);
                            if (element_staylogin != null)
                            {
                                row.Cells["Status"].Value = $"stay login";
                                IWebElement element_click = SeleniumHelper.WaitForElement(driver, By.XPath("//button[@type=\"submit\" and text()='No']"), 10);
                                if (element_click != null)
                                {
                                    element_click.Click();
                                    SeleniumHelper.WaitForPageLoad(driver, 10);
                                    await Task.Delay(1000);
                                }
                            }
                            // acept lời mời
                            IWebElement element_aceptinvite = SeleniumHelper.WaitForElement(driver, By.XPath("//div[text()='Permissions requested by:']"), 1);
                            if (element_aceptinvite != null)
                            {
                                row.Cells["Status"].Value = $"Accept Invite";
                                IWebElement element_click1 = SeleniumHelper.WaitForElement(driver, By.XPath("//input[@value=\"Accept\"]"), 1);
                                element_click1.Click();
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
                    await Task.Delay(1000);
                    driver.FindElement(By.XPath("//input[@type='submit']")).Click();
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    await Task.Delay(1000);
                    int k_unlockaccount = 10;
                    string? xpath_inputphonelock = string.Empty;
                    IWebElement element_differentmethod = SeleniumHelper.WaitForElement(driver, By.XPath("//h1[text()='Keep your account secure']"), 10000);
                    if (element_differentmethod != null)
                    {
                        await Task.Delay(1000);
                        row.Cells["Status"].Value = "input security";
                        driver.FindElement(By.XPath("//button[@class=\"ms-Link rsPg_4zyyqx3UY6NzZci root-249\"]")).Click();
                        await Task.Delay(1000);
                    }
                    else
                    {
                        row.Cells["Status"].Value = "No find security";
                        goto EndLoop;
                    }
                    IWebElement element_differentmethod1 = SeleniumHelper.WaitForElement(driver, By.XPath("//div[text()='Get a call or text to sign in with a code']"), 15000);
                    if (element_differentmethod1 != null)
                    {
                        row.Cells["Status"].Value = "Start add phone";
                        await Task.Delay(1000);
                        element_differentmethod1.Click();
                        await Task.Delay(1000);
                        SeleniumHelper.WaitForPageLoad(driver, 15);
                        await Task.Delay(2000);
                    }
                    row.Cells["Status"].Value = "Start add phone";
                    IWebElement element_addphone = SeleniumHelper.WaitForElement(driver, By.XPath("//h2[text()='Phone']"), 15000);
                    if (element_addphone != null)
                    {
                        await Task.Delay(1000);
                        driver.FindElement(By.XPath("//i[@data-icon-name=\"ChevronDown\"]")).Click();
                        await Task.Delay(1000);
                        var element = driver.FindElement(By.XPath("//span[contains(text(), 'Vietnam (+84)')]"));
                        element.Click();
                        await Task.Delay(3000);
                        var element1 = driver.FindElement(By.XPath("//input[@placeholder=\"Enter phone number\"]"));
                        string? phone1 = phone.Substring(2);
                        element1.SendKeys(phone1);
                        await Task.Delay(1000);
                        var element11 = driver.FindElement(By.XPath("//span[@class=\"ms-Button-label label-256\"]"));
                        element11.Click();
                        await Task.Delay(1000);
                        SeleniumHelper.WaitForPageLoad(driver, 15);
                    }
                    else
                    {
                        row.Cells["Status"].Value = "start add phone fail";
                        goto EndLoop;
                    }
                    IWebElement element_getcode = SeleniumHelper.WaitForElement(driver, By.XPath("//input[@aria-label=\"Enter code\"]"), 15000);
                    string otp = string.Empty;
                    if (element_getcode != null)
                    {
                        row.Cells["Status"].Value = "getotp";
                        goto getotp;
                    }
                    else
                    {
                        row.Cells["Status"].Value = "no find inputotp";
                        goto EndLoop;
                    }

                getotp:
                    string? getotp1 = get_otp + phone;
                    string otp1 = await OtpHelper.GetOtp(getotp1, 30);
                    if (otp1 != null)
                    {
                        row.Cells["Status"].Value = $"input OTP {otp1}";
                        IWebElement element_getcode1 = SeleniumHelper.WaitForElement(driver, By.XPath("//input[@aria-label=\"Enter code\"]"), 15000);
                        element_getcode1.SendKeys(otp1);
                        await Task.Delay(1000);
                        driver.FindElement(By.XPath("//button[@class=\"ms-Button ms-Button--primary root-255\"]")).Click();
                        SeleniumHelper.WaitForPageLoad(driver, 15);
                        await Task.Delay(10000);
                        IWebElement element_addphoness = SeleniumHelper.WaitForElement(driver, By.XPath("//span[@aria-label=\"Verification complete. Your phone has been registered.\"]"), 15);
                        if (element_addphoness != null)
                        {
                            row.Cells["Status"].Value = "very phone succes";
                            driver.FindElement(By.XPath("//button[@class=\"ms-Button ms-Button--primary root-356\"]")).Click();
                            await Task.Delay(1000);
                            SeleniumHelper.WaitForPageLoad(driver, 15);
                            await Task.Delay(1000);
                        }
                        IWebElement element_addphoness1 = SeleniumHelper.WaitForElement(driver, By.XPath("//b[text()=\"Default sign-in method:\"]"), 15000);
                        if (element_addphoness1 != null)
                        {
                            row.Cells["Status"].Value = "very phone succes 1";
                            driver.FindElement(By.XPath("//span[text()=\"Done\"]")).Click();
                            await Task.Delay(3000);
                            SeleniumHelper.WaitForPageLoad(driver, 15);
                            await Task.Delay(4000);

                        }
                    }
                    else if (otp1 == null)
                    {
                        row.Cells["Status"].Value = "Get OTP Fail";
                        goto EndLoop;
                    }
                    row.Cells["Status"].Value = "Login Microsoft";
                    driver.Navigate().GoToUrl("https://mysignins.microsoft.com/security-info?tenant=7212a37c-41a9-4402-9f69-ac32c6f76e1a");
                    row.Cells["Status"].Value = "Await Page Login Microsoft 10s";
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    await Task.Delay(1500);
                    // bật 2fa
                    IWebElement element_add2fa = SeleniumHelper.WaitForElement(driver, By.XPath("//span[text()='These are the methods you use to sign into your account or reset your password.']"), 25000);
                    if (element_add2fa != null)
                    {
                        row.Cells["Status"].Value = "bật 2fa";
                        IWebElement element4 = SeleniumHelper.WaitForElement(driver, By.XPath("//span[@class=\"ms-Button-label label-331\"]"), 10000);
                        if (element4 != null)
                        {
                            await Task.Delay(1500);
                            element4.Click();
                            await Task.Delay(1500);
                        }
                        IWebElement element3 = SeleniumHelper.WaitForElement(driver, By.XPath("//p[text()='Microsoft Authenticator']"), 10000);
                        if (element3 != null)
                        {
                            element3.Click();
                            await Task.Delay(3000);
                        }
                        IWebElement element2 = SeleniumHelper.WaitForElement(driver, By.XPath("//button[@class=\"ms-Link d_hxfHpJiF_9Hwnz7WNw root-336\"]"), 10000);
                        if (element2 != null)
                        {
                            await Task.Delay(1500);
                            element2.Click();
                            await Task.Delay(1500);
                        }
                        IWebElement element1 = SeleniumHelper.WaitForElement(driver, By.XPath("//span[@class=\"ms-Button-label label-271\" and text()='Next']"), 10000);
                        if (element1 != null)
                        {
                            element1.Click();
                            await Task.Delay(2000);
                        }
                        await Task.Delay(3000);
                        IWebElement element = SeleniumHelper.WaitForElement(driver, By.XPath("//h2[text()=\"Scan the QR code\"]"), 10000);
                        if (element != null)
                        {
                            driver.FindElement(By.XPath("//span[text()=\"Can't scan image?\"]")).Click();
                            await Task.Delay(3000);
                        }
                        IWebElement element5 = SeleniumHelper.WaitForElement(driver, By.CssSelector("span.ms-pii[aria-labelledby='secretKeyLabel']"), 10000);
                        if (element5 != null)
                        {
                            row.Cells["Status"].Value = "get 2FA";
                            string secretKey = element5.Text;
                            row.Cells["SecretKey"].Value = $"2FA: {secretKey}";
                            lock (lockFile)
                            {
                                File.AppendAllText("output\\Data.txt", $"{email}|{password}|{phone}|{secretKey}" + Environment.NewLine);
                            }
                            await Task.Delay(1500);
                            driver.FindElement(By.XPath("//span[text()=\"Next\"]")).Click();
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            await Task.Delay(1500);
                        }
                        else
                        {
                            row.Cells["Status"].Value = "bật 2fa fail";
                            goto EndLoop;
                        }
                        IWebElement element6 = SeleniumHelper.WaitForElement(driver, By.XPath("//input[@placeholder=\"Enter code\"]"), 10000);
                        if (element6 != null) 
                        {
                            row.Cells["Status"].Value = "get 2fa code";
                            string? otp2fa = string.Empty;
                            otp2fa = "111111";
                            //string otp2fa = await OtpHelper.GetOtp($"2fa_{email}", 30);
                            if (otp2fa != null)
                            {
                                element6.SendKeys(otp2fa);
                                await Task.Delay(1000);
                                driver.FindElement(By.XPath("//span[text()='Next']")).Click();
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                                await Task.Delay(15000);
                            }
                            else
                            {
                                row.Cells["Status"].Value = "Get OTP 2FA Fail";
                                goto EndLoop;
                            }
                        }
                        else
                        {
                            row.Cells["Status"].Value = "No find input otp 2fa";
                            goto EndLoop;
                        }
                    
                    }
                //Kịch Bản
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                EndLoop:
                    await profile.CloseProfile(profileId);
                    await Task.Delay(1000);
                    await profile.Delete_Profile(profileId);
                    if (driver != null)
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