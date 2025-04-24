using System.Collections.Concurrent;
using Newtonsoft.Json;
using OpenQA.Selenium;
using File = System.IO.File;
using SeleniumKeys = OpenQA.Selenium.Keys;


namespace X_Vframe_Tool
{
    public class Script_Login_2FA : Script_Sele
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
                    if (parts.Length < 4)
                    {
                        row.Cells["Status"].Value = "Sai Định Dạng Dữ Liệu";
                        goto EndLoop;
                    }
                    string email = dataEmail.Split('|')[0];
                    row.Cells["Email"].Value = email;
                    string password = dataEmail.Split('|')[1];
                    row.Cells["Password"].Value = password;
                    string email_recovery = dataEmail.Split('|')[2];
                    row.Cells["EmailRecovery"].Value = email_recovery;
                    string phone = dataEmail.Split('|')[3];
                    row.Cells["Phone"].Value = phone;
                    string lastphone = new string(phone.TakeLast(4).ToArray());
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
                                }
                                catch (Exception ex)
                                {
                                    row.Cells["Status"].Value = $"Unexpected Error on attempt {attempt}: {ex.Message}";
                                    await Task.Delay(1000);
                                }
                            }
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
                    driver.Navigate().GoToUrl("https://go.microsoft.com/fwlink/p/?LinkID=2125442");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500, 2500));
                    // check element nhập mail
                    int k_checkinput_email = 5;
                    while (k_checkinput_email > 0)
                    {
                        string xpath_inputemail = "//input[@type='email']";
                        IWebElement element_inputemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemail), 10);
                        if (element_inputemail != null)
                        {
                            row.Cells["Status"].Value = "check element input mail true";
                            k_checkinput_email = 0;
                            element_inputemail.SendKeys(email);
                            await Task.Delay(300);
                            element_inputemail.SendKeys(SeleniumKeys.Enter);
                            await Task.Delay(300);
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
                    row.Cells["Status"].Value = "Check Input Pass";
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    // check element nhập passw
                    int k_checkinput_pass = 5;
                    while (k_checkinput_pass > 0)
                    {
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
                            k_checkinput_pass = 0;
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy nút nhập pass, again";
                            k_checkinput_pass = k_checkinput_pass - 1;
                            await Task.Delay(2000);
                            if (k_checkinput_pass == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy nút nhập passw";
                                goto EndLoop;
                            }
                        }

                    }

                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500, 2500));
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    // check phương thức xác minh
                    row.Cells["Status"].Value = "check phương pháp xác minh ";
                    await Task.Delay(1000);
                    string xpath_moresecurity = "//span[text()='Hiển thị thêm phương pháp xác minh']";
                    int k_element_moresecurity = 5;
                    while (k_element_moresecurity > 0)
                    {
                        IWebElement element_moresecurity = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_moresecurity), 10);
                        if (element_moresecurity != null)
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
                            string xpath_nophone = "//span[contains(text(), \"Tôi không \")]";
                            IWebElement element_nophone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_nophone), 10);
                            if (element_nophone != null)
                            {
                                row.Cells["Status"].Value = "Chưa add phone";
                                File.AppendAllText("output\\Nophone.txt", $"{email}|{password}|{email_recovery}|{phone}|no_phone" + Environment.NewLine);
                                goto EndLoop;

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
                    string toast_otp = "";
                    while (k_element_inputotp > 0)
                    {
                        IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_otp), 10);
                        if (element_inputotp != null)
                        {
                            row.Cells["Status"].Value = "check input otp true";
                            k_element_inputotp = 0;
                            string get_otp1 = get_otp + phone;
                            row.Cells["Status"].Value = "bắt đầu get otp";

                            string otp = await OtpHelper.GetOtp(get_otp1, 30);
                            if (otp != null)
                            {
                                toast_otp = otp;
                                Console.WriteLine($"✅ OTP: {otp}");
                                row.Cells["Status"].Value = $"otp: {otp}";
                                element_inputotp.SendKeys(otp);
                                await Task.Delay(300);
                                element_inputotp.SendKeys(SeleniumKeys.Enter);
                                await Task.Delay(1000);
                                File.AppendAllText("output\\Data.txt", $"{email}|{password}|{email_recovery}|{phone}|{otp}" + Environment.NewLine);
                            }
                            else
                            {
                                row.Cells["Status"].Value = $"Không có OTP";
                                File.AppendAllText("output\\Nootp.txt", $"{email}|{password}|{email_recovery}|{phone}" + Environment.NewLine);
                                goto EndLoop;

                            }
                        }
                        else
                        {
                            k_element_inputotp = k_element_inputotp - 1;
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