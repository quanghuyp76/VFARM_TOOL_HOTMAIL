using OpenQA.Selenium;
using System.Collections.Concurrent;
using SeleniumKeys = OpenQA.Selenium.Keys;


namespace X_Vframe_Tool
{
    public class Script_Login_Auther : Script_Sele
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
                row.Cells["Status"].Value = "Script Login Authen GPM";
                Profile_GPM profile = new Profile_GPM();
                IndexEmail!.TryTake(out string? dataEmail);
                string? profileId = null;
                IWebDriver? driver = null;
                string auther_email = null;
                string auther_pass = null;
                string auther_phone = "";
                string auther_otp = "";
                string auther_newpass = "";
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
                    string passnew = password + "11a";
                    string phone = dataEmail.Split('|')[2];
                    row.Cells["Phone"].Value = phone;
                    string lastphone = new string(phone.TakeLast(4).ToArray());
                    string firtphone = new string(phone.Take(2).ToArray());
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
                    row.Cells["Status"].Value = "Bắt đầu Open Chrome";
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
                    //Kịch Bản
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // mở trang microsoft
                    #region Login Microsoft
                    //Login Microsoft
                    row.Cells["Status"].Value = "Login Microsoft Entra";
                    driver.Navigate().GoToUrl("https://entra.microsoft.com/signin/index/");
                    SeleniumHelper.WaitForPageLoad(driver, 15);
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
                            await Task.Delay(random.Next(500,1000));
                            element_inputemail.SendKeys(SeleniumKeys.Enter);
                            await Task.Delay(random.Next(1000, 1500));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy nút nhập email, again 2s";
                            k_checkinput_email = k_checkinput_email - 1;
                            await Task.Delay(random.Next(2000, 3000));
                            if (k_checkinput_email == 3)
                            {
                                driver.Navigate().Refresh();
                                SeleniumHelper.WaitForPageLoad(driver, 15);
                            }
                            if (k_checkinput_email == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy nút nhập email";
                                goto EndLoop;
                            }
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 15);
                    // check element nhập passw
                    int k_checkinput_pass = 5;
                    while (k_checkinput_pass > 0)
                    {
                        row.Cells["Status"].Value = "check element passw";
                        string xpath_passw = "//input[@name='passwd']";
                        IWebElement element_passw = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_passw), 10);
                        if (element_passw != null)
                        {
                            k_checkinput_pass = 0;
                            row.Cells["Status"].Value = "input passw true";
                            element_passw.SendKeys(password);
                            await Task.Delay(300);
                            element_passw.SendKeys(SeleniumKeys.Enter);
                            await Task.Delay(300);
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy nút nhập pass, again 2s";
                            k_checkinput_pass = k_checkinput_pass - 1;
                            await Task.Delay(2000);
                            if (k_checkinput_pass == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy nút nhập passw";
                                goto EndLoop;
                            }
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 15);
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                // check element cần thêm thông tin, add số
                CheckAddPhone:
                    int k_checkinfomation = 30;
                    while (k_checkinfomation > 0)
                    {
                        // Add Phone
                        row.Cells["Status"].Value = "check element add infomation";
                        string xpath_infomation = "//div[@id='ProofUpDescription' and contains(text(),'Tổ chức của bạn cần thêm thông tin để bảo mật tài khoản của bạn')]";
                        IWebElement element_action = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_infomation), 10);
                        if (element_action != null)
                        {
                            k_checkinfomation = 0;
                            row.Cells["Status"].Value = "Tìm thấy element add thông tin, next";
                            string xpath_click_action = "//input[@type=\"submit\"]";
                            IWebElement element_actionsubmit = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_click_action), 10);
                            element_actionsubmit.Click();
                            goto Addphone;
                        }
                        else
                        {
                            // Login 2FA
                            k_checkinfomation = k_checkinfomation - 1;
                            string xpath_veryaction = "//div[contains(text(), '+XX XXXXXX')]";
                            row.Cells["Status"].Value = "check element Verify your identity ";
                            IWebElement element_veryphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_veryaction), 1);
                            if (element_veryphone != null)
                            {
                                goto veryphone;
                            }
                            // tiếng việt chưa bắt được
                            string xpath_action = "//div[@id='ProofUpDescription' and contains(text(), 'Tổ chức của bạn yêu cầu thông tin bảo mật bổ sung. Làm theo lời nhắc để tải xuống và thiết lập ứng dụng Microsoft Authenticator này')]";
                            row.Cells["Status"].Value = "check element action";
                            IWebElement element_action1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_action), 1);
                            if (element_action1 != null)
                            {
                                goto ActionRequired;
                            }
                            // change pass
                            string xpath_changepass = "//input[@id=\"currentPassword\"]";
                            IWebElement element_changepass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_changepass), 1);
                            if (element_changepass != null)
                            {
                                goto changepass;
                            }
                            string xpath_dontshowagain0 = "//span[text()=\"Không hiển thị lại thông báo này\"]";
                            IWebElement element_dontshowagain0 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_dontshowagain0), 1);
                            if (xpath_dontshowagain0 != null)
                            {
                                row.Cells["Status"].Value = "Tìm thấy xpath_dontshowagain0";
                                // nhấn nút next
                                string xpath_submit = "//input[@type=\"submit\"]";
                                IWebElement element_yes = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_submit), 1);
                                row.Cells["Status"].Value = "Click yes";
                                element_yes.Click();
                                row.Cells["Status"].Value = "Dealy 3s";
                                await Task.Delay(3000);
                            }
                            string xpath_saipass = "//div[@id=\"passwordError\"]";
                            IWebElement element_saipass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_saipass), 1);
                            if(element_saipass != null)
                            {
                                row.Cells["Status"].Value = "Account sai pass";
                                goto EndLoop;
                            }    
                            if (k_checkinfomation == 0)
                            {
                                row.Cells["Status"].Value = "Chưa tìm thấy kịch bản";
                                goto EndLoop;
                            }
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 15);
                changepass:
                    row.Cells["Status"].Value = "check element change pass";
                    int k_checkelement_changepass = 5;
                    while (k_checkelement_changepass >0)
                    {
                        string xpath_changepass = "//input[@id=\"currentPassword\"]";
                        IWebElement element_changepass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_changepass), 5);
                        if(element_changepass !=null)
                        {
                            row.Cells["Status"].Value = "đang change pass";
                            string passold = password;
                            string passnew1 = passnew;
                            element_changepass.SendKeys(passold);
                            await Task.Delay(random.Next(500, 1000));
                            string xpath_newpass1 = "//input[@id=\"newPassword\"]";
                            IWebElement element_newpass1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_newpass1), 5);
                            element_newpass1.SendKeys(passnew1);
                            await Task.Delay(random.Next(500, 1000));
                            string xpath_newpass2 = "//input[@id=\"confirmNewPassword\"]";
                            IWebElement element_newpass2 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_newpass2), 5);
                            element_newpass2.SendKeys(passnew1);
                            await Task.Delay(random.Next(500, 1000));
                            string xpath_submitx = "//input[@type=\"submit\"]";
                            IWebElement element_submitxx = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_submitx), 5);
                            element_submitxx.Click();
                            await Task.Delay(random.Next(500, 1000));
                            password = passnew1;
                            row.Cells["Password"].Value = password;
                            goto CheckAddPhone;
                        }    
                    }    



                Addphone:
                    #region Add Phone
                    // check element add phone
                    int k_checkelement_addphone = 5;
                    while (k_checkelement_addphone > 0)
                    {
                        row.Cells["Status"].Value = "check element add phone 11";
                        string xpath_addphone = "//input[@placeholder=\"Nhập số điện thoại\"]";
                        IWebElement element_addphonen = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_addphone), 10);
                        if (element_addphonen != null)
                        {
                            k_checkelement_addphone = 0;
                            row.Cells["Status"].Value = "Tìm thấy element add phone, nhập phone";
                            // chọn country
                            string xpath_clickcountry = "//button[@role=\"presentation\"]";
                            IWebElement element_clickcountry = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_clickcountry), 10);
                            element_clickcountry.Click();
                            await Task.Delay(random.Next(1000,2000));
                            string xpath_clickcountry1 = "//span[text()=\"Vietnam (+84)\"]";
                            IWebElement element_clickcountry1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_clickcountry1), 10);
                            element_clickcountry1.Click();
                            await Task.Delay(random.Next(1000, 2000));
                            element_addphonen.SendKeys(phone);
                            await Task.Delay(random.Next(2000, 3000));
                            string xpath_nextotp = "//span[text()=\"Tiếp theo\"]";
                            IWebElement element_nextotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_nextotp), 10);
                            await Task.Delay(random.Next(2000, 3000));
                            element_nextotp.Click();
                            row.Cells["Status"].Value = "Delay 10s";
                            await Task.Delay(random.Next(15000,15100));

                        }
                        else
                        {
                            k_checkelement_addphone = k_checkelement_addphone - 1;
                            string xpath_veryaction = "//div[contains(text(), '+XX XXXXXX')]";
                            row.Cells["Status"].Value = "check element Verify your identity ";
                            IWebElement element_veryphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_veryaction), 1);
                            if (element_veryphone != null)
                            {
                                goto veryphone;
                            }
                            string xpath_action = "//div[@id='ProofUpDescription' and contains(text(), 'Tổ chức của bạn yêu cầu thông tin bảo mật bổ sung. Làm theo lời nhắc để tải xuống và thiết lập ứng dụng Microsoft Authenticator này')]";
                            row.Cells["Status"].Value = "check element action";
                            IWebElement element_action1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_action), 1);
                            if (element_action1 != null)
                            {
                                goto ActionRequired;
                            }
                            if (k_checkelement_addphone == 0)
                            {
                                row.Cells["Status"].Value = "Chưa thấy element add thông tin";
                                goto EndLoop;
                            }
                            row.Cells["Status"].Value = "Chưa thấy element add thông tin, again, 3s";
                            await Task.Delay(random.Next(3000, 4000));
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 15);
                    // check element very code                    
                    int k_checkelement_verycode = 5;
                    while (k_checkelement_verycode > 0)
                    {
                        string xpath_inputcode2 = "//input[@placeholder=\"Nhập mã\"]";
                        row.Cells["Status"].Value = "check element very code";
                        IWebElement element_verycode = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputcode2), 10);
                        if (element_verycode != null)
                        {
                            k_checkelement_verycode = 0;
                            row.Cells["Status"].Value = $"Tìm thấy element very code, get code {phone}";
                            string get_otp2 = get_otp + phone;
                            string otp2 = await OtpHelper.GetOtp(get_otp2, 30);
                            if (otp2 != null)
                            {
                                row.Cells["Status"].Value = $"otp add phone: {otp2}";
                                element_verycode.SendKeys(otp2);
                                await Task.Delay(random.Next(2000,3000));
                                //element_verycode.SendKeys(SeleniumKeys.Enter);
                                await Task.Delay(random.Next(2000, 3000));
                                string xpath_xx = "//span[text()='Tiếp theo']";
                                IWebElement element_xx = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_xx), 10);
                                element_xx.Click();
                                await Task.Delay(random.Next(10000, 11000));
                                //File.AppendAllText("output\\Data.txt", $"{email}|{password}|{phoneadd}|{otp2}|" + Environment.NewLine);
                                // check addphonesussces
                                int k_checkxpathsuscces = 10 ;
                                while (k_checkxpathsuscces>0)
                                {
                                    string xpath_checkaddphonesussces = "//span[@aria-label=\"Đã xác minh xong. Điện thoại của bạn đã được đăng ký.\"]";
                                    IWebElement element_checksuscces = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checkaddphonesussces), 10);
                                    if(element_checksuscces != null)
                                    {
                                        row.Cells["Phone"].Value = $"{phone} add phone suscces";
                                        row.Cells["Status"].Value = $"Add phone thành công {phone}";
                                        File.AppendAllText("output\\Data.txt", $"{email}|{password}|84{phone}" + Environment.NewLine);
                                        goto EndLoop;
                                    }
                                    else
                                    {
                                        row.Cells["Status"].Value = " Chưa find giao diện suscces, again";
                                        await Task.Delay(3000);
                                        string xpath_xx1 = "//span[text()='Tiếp theo']";
                                        IWebElement element_xx1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_xx1), 2);
                                        {
                                            element_xx.Click();
                                            await Task.Delay(random.Next(2000, 3000));
                                        }
                                    }
                                }

                                goto EndLoop;
                            }
                            else
                            {
                                row.Cells["Status"].Value = $"Không có OTP add phone";
                                File.AppendAllText("output\\Nootp.txt", $"{email}|{password}" + Environment.NewLine);
                                goto EndLoop;
                            }
                        }
                        else
                        {
                            k_checkelement_verycode = k_checkelement_verycode - 1;
                            row.Cells["Status"].Value = "Không tìm thấy element very code add phone, again 2s";
                            await Task.Delay(2000);
                            if (k_checkelement_verycode == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy element very code add phone";
                                goto EndLoop;
                            }
                        }
                    }
                #endregion
                // 

                ActionRequired:
                    #region Action Required
                    // check element Action Required
                    int k_checkelement_action = 10;
                    while (k_checkelement_action > 0)
                    {
                        string xpath_action = "//div[@id='ProofUpDescription' and contains(text(), 'Your organization requires additional security information. Follow the prompts to download and set up the Microsoft Authenticator app.')]";
                        row.Cells["Status"].Value = "check element action";
                        IWebElement element_action = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_action), 1);
                        if (element_action != null)
                        {
                            row.Cells["Status"].Value = "Tìm thấy Action Required";
                            k_checkelement_action = 0;
                            // nhấn nút next
                            row.Cells["Status"].Value = "Click submit";
                            string xpath_click_action = "//input[@type=\"submit\"]";
                            IWebElement element_actionsubmit = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_action), 10);
                            element_actionsubmit.Click();
                            row.Cells["Status"].Value = "Dealy 3s";
                            await Task.Delay(3000);
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy element action, again 2s";
                            k_checkelement_action = k_checkelement_action - 1;
                            await Task.Delay(2000);
                            string xpath_veryaction = "//div[contains(text(), '+XX XXXXXX')]";
                            row.Cells["Status"].Value = "check element Verify your identity ";
                            IWebElement element_veryphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_veryaction), 1);
                            if (element_veryphone != null)
                            {
                                goto veryphone;
                            }

                            if (k_checkelement_action == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy element action";
                            }
                        }
                    }
                #endregion
                // check element very phone
                #region veryphone login
                veryphone:
                    int k_checkelement_veryphone = 5;
                    while (k_checkelement_veryphone > 0)
                    {
                        string xpath_veryaction = "//div[contains(text(), '+XX XXXXXX')]";
                        row.Cells["Status"].Value = "check element Verify your identity ";
                        IWebElement element_veryphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_veryaction), 10);
                        if (element_veryphone != null)
                        {
                            row.Cells["Status"].Value = "Tìm thấy Action Very Phone";
                            k_checkelement_veryphone = 0;
                            // nhấn nút next
                            row.Cells["Status"].Value = "Click submit";
                            element_veryphone.Click();
                            row.Cells["Status"].Value = "Dealy 3s";
                            await Task.Delay(3000);
                            goto OtpVeryLogin;
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy element Verify your identity, again 2s";
                            k_checkelement_veryphone = k_checkelement_veryphone - 1;
                            await Task.Delay(2000);
                            if (k_checkelement_veryphone == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy element Verify your identity";
                                goto EndLoop;
                            }
                        }
                    }
                #endregion
                OtpVeryLogin:
                    // check code otp very
                    int k_element_inputotp = 15;
                    string toast_otp = "";
                    while (k_element_inputotp > 0)
                    {
                        row.Cells["Status"].Value = "check input otp";
                        string xpath_otp = "//input[@aria-label=\"Mã\"]";
                        IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_otp), 1);
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
                                File.AppendAllText("output\\Data.txt", $"{email}|{password}|{phone}|{otp}" + Environment.NewLine);
                                goto EndLoop;
                            }
                            else
                            {
                                row.Cells["Status"].Value = $"Không có OTP";
                                File.AppendAllText("output\\Nootp.txt", $"{email}|{password}|{phone}" + Environment.NewLine);
                                goto EndLoop;

                            }
                        }
                        else
                        {
                            k_element_inputotp = k_element_inputotp - 1;
                            row.Cells["Status"].Value = "Không tìm thấy element very otp,  again 2s";
                            await Task.Delay(1000);
                            string xpath_veryotpfail = "//span[contains(text(), 'Rất tiếc, chúng tôi gặp sự cố')]";
                            IWebElement element_veryotpfail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_veryotpfail), 1);
                            if(element_veryotpfail != null)
                            {
                                row.Cells["Status"].Value = "Login very otp fail";
                                goto EndLoop;

                            }    
                            if (k_checkelement_veryphone == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy element very otp";
                                goto EndLoop;
                            }
                        }
                    }
                    // check element don't show again
                    int k_checkelement_dontshowagain = 5;
                    while (k_checkelement_dontshowagain > 0)
                    {
                        row.Cells["Status"].Value = "check element Don't show again ";
                        string xpath_dontshowagain = "//span[text()=\"Không hiển thị lại thông báo này\"]";
                        IWebElement element_dontshowagain = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_dontshowagain), 1);
                        if (element_dontshowagain != null)
                        {
                            row.Cells["Status"].Value = "Tìm thấy Action Very Phone";
                            k_checkelement_dontshowagain = 0;
                            // nhấn nút next
                            string xpath_submit = "//input[@type=\"submit\"]";
                            IWebElement element_yes = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_submit), 1);
                            row.Cells["Status"].Value = "Click yes";
                            element_yes.Click();
                            row.Cells["Status"].Value = "Dealy 3s";
                            await Task.Delay(300);
                            row.Cells["Status"].Value = "Login sussces";
                            await profile.CloseProfile(profileId);
                            await profile.Delete_Profile(profileId);
                            driver.Quit();
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Không tìm thấy element don't show again, again 2s";
                            k_checkelement_dontshowagain = k_checkelement_dontshowagain - 1;
                            await Task.Delay(2000);
                            if (k_checkelement_veryphone == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy element don't show again";
                            }
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