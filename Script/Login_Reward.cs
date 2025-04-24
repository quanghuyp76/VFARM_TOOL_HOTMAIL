using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using static OpenQA.Selenium.BiDi.Modules.BrowsingContext.Locator;
using SeleniumKeys = OpenQA.Selenium.Keys;


namespace X_Vframe_Tool
{
    public class Script_Login_Reward : Script_Sele
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
                    string mkp_username = email_recovery.Split('@')[0];
                    string mkp_domain = "@"+email_recovery.Split('@')[1];
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
                    // mở trang microsoft
                    #region OpenLoginmicrosoft()
                    row.Cells["Status"].Value = "Login Microsoft";
                    driver.Navigate().GoToUrl("https://go.microsoft.com/fwlink/p/?LinkID=2125442");
                    NameCurrentTab(driver, "Tab0");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500, 2500));
                    int k_errror = 5;
                    #endregion
                    //check element nhập mail
                inputemail:
                    #region Inputemail()
                    string? xpath_inputemail = string.Empty;
                    row.Cells["Status"].Value = "Check Input Email";
                    int k_checkinput_email = 15;
                    while (k_checkinput_email > 0)
                    {
                        xpath_inputemail = "//input[@type='email']";
                        IWebElement element_inputemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemail), 1);
                        if (element_inputemail != null)
                        {
                            row.Cells["Status"].Value = "check element input email true";
                            k_checkinput_email = 0;
                            element_inputemail.SendKeys(email);
                            await Task.Delay(300);
                            element_inputemail.SendKeys(SeleniumKeys.Enter);
                            await Task.Delay(300);
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                        }
                        else
                        {
                            row.Cells["Status"].Value = "check element input email fail, again";
                            k_checkinput_email = k_checkinput_email - 1;
                            await Task.Delay(2000);
                            if (k_checkinput_email == 0)
                            {
                                row.Cells["Status"].Value = "check element input email fail";
                                goto EndLoop;
                            }
                        }
                    }
                    #endregion
                    // check element nhập passw
                    #region Inputpass()
                    row.Cells["Status"].Value = "Check Input Passs";
                    int k_checkinput_pass = 15;
                    while (k_checkinput_pass > 0)
                    {
                        string xpath_passw = "//input[@name='passwd']";
                        row.Cells["Status"].Value = "check element passw";
                        IWebElement element_passw = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_passw), 1);
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
                            string? xpath_error0 = "//div[@id='i0116Error' and text()='Nhập địa chỉ email, số điện thoại hoặc tên Skype hợp lệ.']";
                            IWebElement element_erroraccount = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_error0), 1);
                            if(element_erroraccount != null)
                            {
                                row.Cells["Status"].Value = "Email không hợp lệ i10116";
                                goto EndLoop;
                            }
                            IWebElement element_inputemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputemail), 1);
                            if (element_inputemail != null)
                            {
                                IWebElement element_erroraccount1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_error0), 1);
                                if (element_erroraccount1 != null)
                                {
                                    row.Cells["Status"].Value = "Email không hợp lệ i10116";
                                    goto EndLoop;
                                }
                                k_errror--;
                                row.Cells["Status"].Value = $"Error {k_errror}";
                                await Task.Delay(300);
                                if (k_errror==0)
                                {
                                    row.Cells["Status"].Value = "Email không hợp lệ error";
                                    goto EndLoop;
                                }    
                                goto inputemail;
                            }
                            await Task.Delay(2000);
                            if (k_checkinput_pass == 0)
                            {
                                row.Cells["Status"].Value = "Không tìm thấy nút nhập passw";
                                goto EndLoop;
                            }
                        }

                    }
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 1s";
                    IWebElement element_toomanyrequest = SeleniumHelper.WaitForElement(driver, By.XPath("//text[text()='Too Many Requests']"), 1);
                    if (element_toomanyrequest != null)
                    {
                        row.Cells["Status"].Value = "Too Many Requests";
                        goto EndLoop;
                    }

                    #endregion
                    // Bắt trường hợp sau đăng nhập
                    // Ghi chú nhanh về tài khoản Microsoft
                    #region logininbox()
                    string? xpath_note = "//span[text()='Ghi chú nhanh về tài khoản Microsoft']";
                    string? xpath_staylogin = "//h1[text()='Duy trì đăng nhập?']";
                    string? xpath_kmsi = "//div[@id='kmsiTitle']";
                    string? xpath_butttonkmsi = "//button[@type='submit' and @id='acceptButton']";
                    string? xpath_PIN = "//h1[text()='Đăng nhập nhanh hơn bằng khuôn mặt, dấu vân tay hoặc mã PIN của bạn']";
                    string? xpath_inbox = "//button[@data-automation-type='RibbonSplitButton' and @data-unique-id='Ribbon-588']";
                    string? xpath_PIN1 = "//div[text()='Đăng nhập nhanh hơn bằng khuôn mặt, dấu vân tay hoặc mã PIN của bạn']";
                    string? xpath_lock1 = "//div[@id='iPageTitle' and text()='Cho rằng chúng tôi bảo vệ quá mức...']";
                    string? xpath_security = "//div[@class='text-title' and text()='Giúp chúng tôi bảo vệ tài khoản của bạn']";
                    int k_inbox = 35;
                    while (k_inbox > 0)
                    {
                        IWebElement element_inbox = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inbox), 1);
                        if (element_inbox != null)
                        {
                            row.Cells["Status"].Value = "Login sussces";
                            await Task.Delay(3000);
                            break;


                        }
                        else
                        {

                            //ghi chú nhanh
                            IWebElement element_note = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_note), 1);
                            if (element_note != null)
                            {
                                row.Cells["Status"].Value = "find note";
                                string xpath_butttonnote = "//span[@data-automationid='splitbuttonprimary']";
                                IWebElement element_buttonnote = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_butttonnote), 1);
                                element_buttonnote.Click();
                                await Task.Delay(300);
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                            }
                            // staylogin
                            IWebElement element_staylogin = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_staylogin), 1);
                            if (element_staylogin != null)
                            {
                                string? xpath_buttonstaylogin = "//button[@type='submit' and @data-testid='primaryButton']";
                                IWebElement element_buttonstaylogin = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_buttonstaylogin), 1);
                                element_buttonstaylogin.Click();
                                await Task.Delay(300);
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                            }
                            //kmsi
                            IWebElement element_kmsi = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_kmsi), 1);
                            if (element_kmsi != null)
                            {
                                row.Cells["Status"].Value = "find kmsi";
                                xpath_butttonkmsi = "//button[@type='submit' and @id='acceptButton']";
                                IWebElement element_buttonkmsi = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_butttonkmsi), 1);
                                element_buttonkmsi.Click();
                                await Task.Delay(300);
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                            }
                            //PIN
                            IWebElement element_PIN = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_PIN), 1);
                            IWebElement element_PIN1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_PIN1), 1);
                            if (element_PIN != null || element_PIN1 != null)
                            {
                                row.Cells["Status"].Value = "find PIN";
                                string? xpath_butttonPIN = "//button[@type='button' and text()='Tạm bỏ qua']";
                                IWebElement element_buttonPIN = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_butttonPIN), 1);
                                element_buttonPIN.Click();
                                await Task.Delay(300);
                                SeleniumHelper.WaitForPageLoad(driver, 10);
                            }
                            //lock change security
                            IWebElement element_lock1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_lock1), 1);
                            if (element_lock1 != null)
                            {
                                row.Cells["Status"].Value = "Account lock, change security";
                                goto EndLoop;
                            }
                            // security 2fa
                            string checkdomain_mkp = string.Empty;
                            IWebElement element_security = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_security), 1);
                            if (element_security != null)
                            {
                                string? xpath_checkboxemail = "//input[@title=\"Để xác minh rằng đây là địa chỉ email của bạn, hãy hoàn thành phần bị ẩn và bấm vào gửi mã để nhận mã của bạn.\"]";
                                IWebElement element_checkboxemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_checkboxemail), 10);
                                if (element_checkboxemail != null)
                                {
                                    element_checkboxemail.Click();
                                    await Task.Delay(random.Next(300, 800));
                                    checkdomain_mkp = driver.FindElement(By.XPath("//label[@id='iConfirmProofEmailDomain']")).Text;
                                    if (checkdomain_mkp == mkp_domain)
                                    {
                                        string xpath_inputusermkp = "//input[@id=\"iProofEmail\"]";
                                        IWebElement element_inputusermkp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputusermkp), 10);
                                        if (element_inputusermkp != null)
                                        {
                                            element_inputusermkp.SendKeys(mkp_username);
                                            await Task.Delay(random.Next(300, 800));
                                            driver.FindElement(By.XPath("//input[@value='Gửi mã']")).Click();
                                            await Task.Delay(random.Next(300, 800));
                                            SeleniumHelper.WaitForPageLoad(driver, 10);
                                            string? xpath_inputotp = "//input[@placeholder=\"Mã\"]";
                                            IWebElement element_inputotp = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_inputotp), 10);
                                            string? otp = string.Empty;
                                            if (element_inputotp != null)
                                            {
                                                row.Cells["Status"].Value = "Get OTP Mail";
                                                otp = await OtpHelper.GETOTPSMV(email_recovery, 30);
                                                if (otp != null)
                                                {
                                                    Console.WriteLine("📩 Mã OTP: " + otp);
                                                }                                                       
                                                else
                                                {
                                                    Console.WriteLine("❌ Không tìm thấy mã OTP.");
                                                    goto EndLoop;
                                                }
                                                   
                                                element_inputotp.SendKeys(otp);
                                                await Task.Delay(random.Next(300, 800));
                                                driver.FindElement(By.XPath("//input[@type=\"submit\"]")).Click();
                                                await Task.Delay(random.Next(1000, 1500));
                                            }    
                                        }
                                    }
                                    else
                                    {
                                        row.Cells["Status"].Value = "Sai Email Recovery";
                                    }

                                }
                            }
                            IWebElement element_toomanyrequest1 = SeleniumHelper.WaitForElement(driver, By.XPath("//text[text()='Too Many Requests']"), 1);
                            if (element_toomanyrequest1 != null)
                            {
                                row.Cells["Status"].Value = "Too Many Requests";
                                goto EndLoop;
                            }
                            k_inbox = k_inbox - 1;
                            await Task.Delay(2000);
                            if (k_inbox == 0)
                            {
                                row.Cells["Status"].Value = "Login Fail";
                                goto EndLoop;
                            }
                        }
                    }

                    #endregion
                    //
                    row.Cells["Status"].Value = "Login Bing.com";
                    driver.Navigate().GoToUrl("https://www.bing.com/");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    await Task.Delay(random.Next(1500, 2500));
                    string? xpath_buttonreward = "//textarea";
                    IWebElement element_buttonreward = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_buttonreward), 10);
                    if (element_buttonreward != null)
                    {
                        element_buttonreward.Click();
                        await Task.Delay(random.Next(1500, 2500));
                        IWebElement element_buttonreward1 = SeleniumHelper.WaitForElement(driver, By.XPath("(//ul[@role=\"listbox\"]//li)[1]"), 10);
                        if(element_buttonreward1 != null)
                        {
                            element_buttonreward1.Click();
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            await Task.Delay(random.Next(1500, 2500));
                        }
                    }
                        //string? xpath_buttonreward = "//span[@class=\"sw_spd id_avatar\"]";
                        //IWebElement element_buttonreward = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_buttonreward), 10);
                        //if (element_buttonreward != null)
                        //{
                        //    //row.Cells["Status"].Value = "check element input email true";
                        //    //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element_buttonreward);
                        //    //await Task.Delay(300);
                        //    element_buttonreward.Click();
                        //    SeleniumHelper.WaitForPageLoad(driver, 10);
                        //    await Task.Delay(300);
                        //    IWebElement iframe1 = SeleniumHelper.WaitForElement(driver, By.XPath("//iframe[@style=\"border:none;\"]"), 10);
                        //    if (iframe1 != null)
                        //    {
                        //        driver.SwitchTo().Frame(iframe1);
                        //    }

                        //    var js = (IJavaScriptExecutor)driver;
                        //    var element = (IWebElement)js.ExecuteScript("return document.querySelector('a[title=\"Get started\"]');");

                        //    if (element != null)
                        //    {
                        //        Console.WriteLine("✅ Tìm thấy element qua JS, dù bị ẩn.");
                        //        // Có thể click bằng JS luôn nếu muốn
                        //        js.ExecuteScript("arguments[0].click();", element);
                        //    }

                        //    //IWebElement element_getstarted = SeleniumHelper.WaitForElement(driver, By.XPath("//a[@title=\"Get started\"]"), 10);
                        //    //if (element_getstarted != null)
                        //    //{
                        //    //    row.Cells["Status"].Value = "click getstart";
                        //    //    element_getstarted.Click();
                        //    //}

                        //    SeleniumHelper.WaitForPageLoad(driver, 10);
                        //    await Task.Delay(300);
                        //}




                        // Login Reward
                        #region LoginReward()
                        string? xpath_point = "/html/body/div[1]/div[2]/main/div/ui-view/mee-rewards-dashboard/main/mee-rewards-user-status-banner/div/div/div/div/div[2]/div[1]/mee-rewards-user-status-banner-item/mee-rewards-user-status-banner-balance/div/div/div/div/div/div/p/mee-rewards-counter-animation/span";
                    string? point = string.Empty;
                    row.Cells["Status"].Value = "Login Reward";
                    driver.Navigate().GoToUrl("https://rewards.bing.com/");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500, 2500));
                    int k_Kefir = 35;
                    while (k_Kefir > 0)
                    {
                        //string? xpath_Kefir = "//a[@aria-label='Kefir benefits   Discover the health benefits and how it can improve your diet   10 points']";
                        string? xpath_Kefir = "//p[@title='Điểm hiện có']";
                        IWebElement element_Kefir = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_Kefir), 1);
                        if (element_Kefir != null)
                        {
                            row.Cells["Status"].Value = "login reward sussces";
                            row.Cells["Status"].Value = "Delay 1s";
                            await Task.Delay(1000);
                            row.Cells["Status"].Value = "get point";
                            IWebElement element_point = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_point), 1);
                            if (element_point != null)
                            {
                                point = element_point.Text;
                                row.Cells["Status"].Value = $"point: {point}";
                                await Task.Delay(1000);
                                goto TaskReward;
                            }
                            else
                            {
                                row.Cells["Status"].Value = "Không tìm thấy point";
                            }

                        }
                        else
                        {
                            k_Kefir = k_Kefir - 1;
                            await Task.Delay(2000);
                        }
                    }
                #endregion




                TaskReward:
                    // check element Task Reward
                    #region Check Page Task Reward
                    int.TryParse(point, out int pointValue);
                    if (pointValue > 100)
                    {
                        row.Cells["Status"].Value = $"Đã đủ điểm {pointValue}";
                        goto EndLoop;
                    }
                    else
                    {
                        row.Cells["Status"].Value = $"Go to Task Reward: {pointValue}";

                    }
                    #endregion

                    // Run Task Daily 1
                    #region TaskDaily()
                    string originalWindow = driver.CurrentWindowHandle;
                    int counttaskdaily = driver.FindElements(By.XPath("//mee-card-group[@ng-hide=\"$ctrl.currentIndex !== 0\"]//mee-card")).Count;
                    row.Cells["Status"].Value = $"Tìm thấy {counttaskdaily} task daily";
                    // Tạo danh sách task chưa xử lý
                    List<int> pendingTasks = Enumerable.Range(1, counttaskdaily).ToList();
                    while (pendingTasks.Count > 0)
                    {
                        row.Cells["Status"].Value = $"Đang xử lý {pendingTasks.Count} task còn lại...";

                        for (int i = pendingTasks.Count - 1; i >= 0; i--) // Duyệt ngược để dễ xóa
                        {
                            int taskIndex = pendingTasks[i];
                            string xpath_task = $"//mee-card-group[@ng-hide=\"$ctrl.currentIndex !== 0\"]//mee-card[{taskIndex}]";
                            IWebElement taskElement = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_task), 3);

                            if (taskElement != null)
                            {
                                row.Cells["Status"].Value = $"Đang chạy task {taskIndex}";
                                try
                                {
                                    taskElement.Click();
                                    await Task.Delay(300);
                                    SeleniumHelper.WaitForPageLoad(driver, 10);
                                    await Task.Delay(random.Next(1000, 1500));

                                    // Đóng tab nếu có mở ra
                                    foreach (string window in driver.WindowHandles)
                                    {
                                        if (window != originalWindow)
                                        {
                                            driver.SwitchTo().Window(window);
                                            driver.Close(); // Đóng tab mới
                                            driver.SwitchTo().Window(originalWindow); // Quay về tab cũ
                                            break;
                                        }
                                    }

                                    // Refresh lại trang
                                    await Task.Delay(2000);
                                    driver.Navigate().Refresh();
                                    SeleniumHelper.WaitForPageLoad(driver, 10);
                                    await Task.Delay(1000);

                                    // ✅ Đánh dấu task đã hoàn thành → xóa khỏi danh sách
                                    pendingTasks.RemoveAt(i);
                                }
                                catch (Exception ex)
                                {
                                    row.Cells["Status"].Value = $"❌ Lỗi khi xử lý task {taskIndex}: {ex.Message}";
                                }
                            }
                            else
                            {
                                string xpath_welcome = "//h2[text()='Welcome to Microsoft Rewards!']";
                                IWebElement element_welcome = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_welcome), 1);
                                if (element_welcome != null)
                                {
                                    row.Cells["Status"].Value = "check element welcome reward true";
                                    string xpath_buttonwelcome = "//button[@aria-label='Đóng']";
                                    IWebElement element_buttonwelcome = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_buttonwelcome), 10);
                                    if (element_buttonwelcome != null)
                                    {
                                        element_buttonwelcome.Click();
                                        await Task.Delay(300);
                                        SeleniumHelper.WaitForPageLoad(driver, 10);
                                    }
                                }
                                else
                                {
                                    row.Cells["Status"].Value = "chưa tìm thấy task again";
                                }    
                            }
                        }

                        await Task.Delay(2000); // chờ chút trước vòng lặp tiếp theo
                    }
                    #endregion
                    await Task.Delay(2000);
                    string? point1 = string.Empty;
                    IWebElement element_point2 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_point), 1);
                    if (element_point2 != null)
                    {
                        point1 = element_point2.Text;
                        await Task.Delay(1000);
                    }

                    int.TryParse(point1, out int pointValue1);
                    if (pointValue1 > pointValue)
                    {
                        row.Cells["Status"].Value = $"Đã hoàn thành task daily {pointValue1}";
                        goto Task_onetime;
                    }
                    else
                    {
                        row.Cells["Status"].Value = $"Task daily fail : {pointValue1}";
                        goto EndLoop;
                    }
                    await Task.Delay(2000);
                Task_onetime:
                    #region Task_onetime()
                    int counttaskonetime = driver.FindElements(By.XPath("//mee-card-group[@id='more-activities']//mee-card")).Count;
                    row.Cells["Status"].Value = $"Tìm thấy {counttaskonetime} task one-time";

                    // Tạo danh sách task chưa xử lý
                    List<int> pendingTasksOT = Enumerable.Range(1, counttaskonetime).ToList();

                    while (pendingTasksOT.Count > 0)
                    {
                        row.Cells["Status"].Value = $"Đang xử lý {pendingTasksOT.Count} task one-time còn lại...";

                        for (int i = pendingTasksOT.Count - 1; i >= 0; i--)
                        {
                            int taskIndex = pendingTasksOT[i];
                            string xpath_task = $"//mee-card-group[@id='more-activities']//mee-card[{taskIndex}]";
                            IWebElement taskElement = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_task), 3);

                            if (taskElement != null)
                            {
                                row.Cells["Status"].Value = $"Đang chạy one-time task {taskIndex}";
                                try
                                {
                                    taskElement.Click();
                                    await Task.Delay(300);
                                    SeleniumHelper.WaitForPageLoad(driver, 10);
                                    await Task.Delay(random.Next(1000, 1500));

                                    foreach (string window in driver.WindowHandles)
                                    {
                                        if (window != originalWindow)
                                        {
                                            driver.SwitchTo().Window(window);
                                            driver.Close();
                                            driver.SwitchTo().Window(originalWindow);
                                            break;
                                        }
                                    }

                                    await Task.Delay(2000);
                                    driver.Navigate().Refresh();
                                    SeleniumHelper.WaitForPageLoad(driver, 10);
                                    await Task.Delay(1000);

                                    pendingTasksOT.RemoveAt(i);
                                }
                                catch (Exception ex)
                                {
                                    row.Cells["Status"].Value = $"❌ Lỗi task one-time {taskIndex}: {ex.Message}";
                                }
                            }
                            else
                            {
                                string xpath_welcome = "//h2[text()='Welcome to Microsoft Rewards!']";
                                IWebElement element_welcome = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_welcome), 1);
                                if (element_welcome != null)
                                {
                                    row.Cells["Status"].Value = "check element welcome reward true";
                                    string xpath_buttonwelcome = "//button[@aria-label='Đóng']";
                                    IWebElement element_buttonwelcome = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_buttonwelcome), 10);
                                    if (element_buttonwelcome != null)
                                    {
                                        element_buttonwelcome.Click();
                                        await Task.Delay(300);
                                        SeleniumHelper.WaitForPageLoad(driver, 10);
                                    }
                                }
                                else
                                {
                                    row.Cells["Status"].Value = "chưa tìm thấy task again";
                                }
                            }
                        }

                        await Task.Delay(2000);
                    }

                    IWebElement element_point1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_point), 1);
                    if (element_point1 != null)
                    {
                        point = element_point1.Text;
                        await Task.Delay(1000);
                    }
                    await Task.Delay(1000);
                    row.Cells["Status"].Value = $"✅ Đã hoàn thành task point ({point})";

                    #endregion



                    await Task.Delay(2000);
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