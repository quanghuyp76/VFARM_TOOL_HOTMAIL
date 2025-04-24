using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Concurrent;


namespace X_Vframe_Tool
{
    public class Reg_Alibaba : Script_Sele
    {
        public static Dictionary<string, string> NamedTabs = new();
        public ConcurrentBag<DataGridViewRow>? Index_Login_2FA { get; set; }
        public ConcurrentBag<string>? IndexEmail;
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
                row.Cells["Status"].Value = "Script Login Reward";
                Profile_GPM profile = new Profile_GPM();
                IndexEmail!.TryTake(out string? dataEmail);
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
                    string email = dataEmail.Split('|')[0];
                    row.Cells["Email"].Value = email;
                    string password = dataEmail.Split('|')[1];
                    row.Cells["Password"].Value = password;
                    string phone = dataEmail.Split('|')[2];
                    row.Cells["Phone"].Value = phone;
                    string get_otp = GlobalSetting.SettingOption_Run.check_UrlGetOtp; //MS_Viettel
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
                    #region checkip()
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
                    #endregion
                    // mở trang alibaba
                    #region   Go to alibabacloud()
                    row.Cells["Status"].Value = "Go to alibabacloud";
                    driver.Navigate().GoToUrl("https://account.alibabacloud.com");
                    NameCurrentTab(driver, "Tab0");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500, 2500));
                    #endregion
                    // vào trang đăng kí
                    #region Signin()

                    string? xpath_Signup = "//a[@id='login-signup-link']";
                    IWebElement element_signup = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_Signup), 10);
                    if (element_signup != null)
                    {
                        row.Cells["Status"].Value = "click signup";
                        element_signup.Click();
                        await Task.Delay(random.Next(300, 500));
                        SeleniumHelper.WaitForPageLoad(driver, 10);
                        row.Cells["Status"].Value = "Delay 2s";
                        await Task.Delay(random.Next(1000, 2000));

                    }
                    else
                    {
                        row.Cells["Status"].Value = "Go to Signin Fail";
                        goto EndLoop;
                    }
                    #endregion
                    // Chuyển iframe
                    IWebElement iframe = driver.FindElement(By.XPath("//iframe[@id='alibaba-register-box']"));
                    driver.SwitchTo().Frame(iframe);


                    // click individual account
                    #region indivodial_account()
                    await Task.Delay(random.Next(1000, 2000));
                    string? xpath_individual = "//label[@class=\"next-radio-wrapper \"]";
                    IWebElement element_individual = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_individual), 10);
                    if (element_individual != null)
                    {
                        row.Cells["Status"].Value = "click individual account";
                        element_individual.Click();
                        await Task.Delay(random.Next(300, 500));
                        string? xpath_next0 = "//a[@class=\"entity__btn-next\"]";
                        IWebElement element_next0 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_next0), 10);
                        if (element_next0 != null)
                        {
                            element_next0.Click();
                        }
                        SeleniumHelper.WaitForPageLoad(driver, 10);
                        row.Cells["Status"].Value = "Delay 2s";
                        await Task.Delay(random.Next(1000, 2000));

                    }
                    else
                    {
                        row.Cells["Status"].Value = "indivodial_account Fail";
                        goto EndLoop;
                    }
                    #endregion
                    await Task.Delay(random.Next(1000, 2000));
                    // input email
                    string? xpath_inputemail = "//input[@id=\"email\"]";
                    string? xpath_password = "//input[@id=\"password\"]";
                    string? xpath_configpass = "//input[@id=\"confirmPwd\"]";
                    IWebElement element_inputemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemail), 10);
                    if (element_inputemail != null)
                    {
                        row.Cells["Status"].Value = "input email";
                        element_inputemail.SendKeys(email);
                        await Task.Delay(random.Next(300, 500));
                        row.Cells["Status"].Value = "input pass";
                        IWebElement element_inputpass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_password), 10);
                        element_inputpass.SendKeys(password);
                        await Task.Delay(random.Next(300, 500));
                        row.Cells["Status"].Value = "input configpass";
                        IWebElement element_inputconfigpass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_configpass), 10);
                        element_inputconfigpass.SendKeys(password);
                        await Task.Delay(random.Next(300, 500));
                        string? xpath_next1 = "//button[@class=\"next-btn next-medium next-btn-primary account__submit\"]";
                        IWebElement element_next1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_next1), 10);
                        if (element_next1 != null)
                        {
                            element_next1.Click();
                        }
                        SeleniumHelper.WaitForPageLoad(driver, 10);
                        row.Cells["Status"].Value = "Delay 2s";
                        await Task.Delay(random.Next(1000, 2000));

                    }
                    else
                    {
                        row.Cells["Status"].Value = "input email Fail";
                        goto EndLoop;
                    }
                    await Task.Delay(random.Next(1000, 2000));
                    IWebElement element_select = SeleniumHelper.WaitForElement(driver, By.XPath("//span[@class=\"next-select-arrow\"]"), 10);
                    if (element_select != null)
                    {
                        element_select.Click();
                        await Task.Delay(random.Next(1000, 2000));
                        IWebElement element_country = SeleniumHelper.WaitForElement(driver, By.XPath("//span[text()='Mozambique']"), 10);
                        if (element_country != null)
                        {
                            element_country.Click();
                        }    
                    }
                   




                        string? xpath_inputphone = "//input[@id=\"mobile\"]";
                    IWebElement element_inputphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputphone), 10);
                    if (element_inputphone != null)
                    {
                        row.Cells["Status"].Value = "input phone";
                        element_inputphone.SendKeys(phone);
                        row.Cells["Status"].Value = "Delay 2s";
                        await Task.Delay(random.Next(1000, 2000));
                        string? xpath_sendphone = "//button[@class=\"next-btn next-medium next-btn-primary\"]";
                        IWebElement element_sendphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_sendphone), 10);
                        if (element_sendphone != null)
                        {
                            element_sendphone.Click();
                            row.Cells["Status"].Value = "Delay 2s";
                            await Task.Delay(random.Next(1000, 2000));
                            goto GetOTP;
                        }
                    }


                GetOTP:
                    string? getotp1 = get_otp + phone;
                    string otp = await OtpHelper.GetOtpTest(getotp1, 30);
                    if (otp != null)
                    {
                        row.Cells["Status"].Value = $"input OTP {otp}";
                        string? xpath_inputotp = "//input[@id=\"mobileCaptcha\"]";
                        IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputotp), 10);
                        if (element_inputotp != null)
                        {
                            // nhập otp
                            element_inputotp.SendKeys(otp);
                            await Task.Delay(random.Next(300, 500));
                            // click next
                            row.Cells["Status"].Value = "click policy";
                            //string? xpath_policy = "//span[@class=\"next-checkbox-inner\"]";
                            //IWebElement element_policy = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_policy), 10);
                            //if (element_policy != null)
                            //{
                            //    element_policy.Click();
                            //    await Task.Delay(random.Next(300, 500));
                            //}
                            driver.FindElement(By.XPath("//span[@class='next-checkbox']")).Click();

                            row.Cells["Status"].Value = "click next";
                            string? xpath_next2 = "//button[@class=\"next-btn next-medium next-btn-primary verify__submit\"]";
                            IWebElement element_next2 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_next2), 10);
                            if (element_next2 != null)
                            {
                                element_next2.Click();
                                row.Cells["Status"].Value = "Delay 2s";
                                await Task.Delay(random.Next(1000, 2000));
                                goto checkedsuscces;
                            }
                        }
                    }
                    else
                    {
                        row.Cells["Status"].Value = "Get OTP Fail";
                        goto EndLoop;
                    }
                checkedsuscces:
                    string? xpath_checksuscces = "//h2[text()=\"Account Successfully Created!\"]";
                    string? xpath_checksuscces1 = "//div[text()='Successfully registered! ']";
                    IWebElement element_checksuscces = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checksuscces), 10);
                    IWebElement element_checksuscces1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checksuscces1), 10);
                    if (element_checksuscces != null || element_checksuscces1 != null)
                    {
                        row.Cells["Status"].Value = "Signin suscces";
                        File.AppendAllText("output\\Data.txt", $"{email}|{password}|{phone}" + Environment.NewLine);
                        goto EndLoop;
                    }
                    await Task.Delay(600000);
                //Kịch Bản
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                EndLoop:
                    await profile.CloseProfile(profileId);
                    await Task.Delay(1000);
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