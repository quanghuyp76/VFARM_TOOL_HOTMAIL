using Microsoft.VisualBasic.ApplicationServices;
using OpenQA.Selenium;
using System.Collections.Concurrent;


namespace X_Vframe_Tool
{
    public class Script_RegMail_Auther : Script_Sele
    {

        public ConcurrentBag<DataGridViewRow>? Index_Login_2FA { get; set; }
        public ConcurrentBag<string>? IndexEmail;
        public ConcurrentBag<string>? IndexPhone;
        Random random = new Random();
        public async Task Run(int Thread, CancellationToken token, string win_pos)
        {

            if (token.IsCancellationRequested)
                return;
            while (Index_Login_2FA!.TryTake(out var row))
            {
                if (token.IsCancellationRequested)
                    return;
                row.Cells["Status"].Value = "Script Reg Mail Auther";
                string tkuser = "ngochais12";
                Profile_GPM profile = new Profile_GPM();
                IndexEmail!.TryTake(out string? dataEmail);
                string? profileId = null;
                IWebDriver? driver = null;
                string auther_email = null;
                string auther_pass = null;
                string auther_phone = "+258 866866506";
                try
                {
                    // lấy profile GPM
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
                    string Id = dataEmail.Split('|')[2];
                    row.Cells["EmailRecovery"].Value = Id;
                    // open Profile
                    row.Cells["Status"].Value = "Open Chrome";
                    Profile_GPM profile_GPM = new Profile_GPM();
                    int k_OpenGPM = 5;
                    string ApiGPM_Open = $"http://127.0.0.1:19995/api/v3/profiles/start/{Id}?win_scale=0.7&win_pos={win_pos}&win_size=1400,1200";
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
                    // mở trang microsoft entra
                    row.Cells["Status"].Value = "Login Microsoft";
                    driver.Navigate().GoToUrl("https://entra.microsoft.com/signin/index/");
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    row.Cells["Status"].Value = "Delay 2s";
                    await Task.Delay(random.Next(1500, 2500));

                    SeleniumHelper.WaitForPageLoad(driver, 15);
                    // check login sussces
                    string xpath_loginsuscces = "//a[@title=\"Microsoft Entra admin center\"]";
                    int k_checkloginsuscces = 5;
                    while (k_checkloginsuscces > 0)
                    {
                        IWebElement element_loginsuscces = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_loginsuscces), 10);
                        if (element_loginsuscces != null)
                        {
                            k_checkloginsuscces = 0;
                            row.Cells["Status"].Value = "Login Microsoft Entra Suscces";
                            string xpath_user = "//button[@aria-label=\"Expand Users section\"]";
                        //switch farme
                        EndLoop1:
                            string xpath_frame1 = "//iframe[@id='_react_frame_0']";
                            IWebElement element_frame1 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_frame1), 10);
                            if (element_frame1 != null)
                            {
                                row.Cells["Status"].Value = "Tìm thấy frame, switch";
                                driver.SwitchTo().Frame(element_frame1);
                                IWebElement element_user = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_user), 10);
                                element_user.Click();
                                row.Cells["Status"].Value = "Dealy 3s";
                                await Task.Delay(random.Next(3000, 4500));
                            }
                            else
                            {
                                row.Cells["Status"].Value = "No Element Frame";
                                goto EndLoop1;
                            }
                        }
                        else
                        {
                            k_checkloginsuscces = k_checkloginsuscces - 1;
                            row.Cells["Status"].Value = $"Check Login Fail Again: {k_checkloginsuscces}";
                            driver.Navigate().Refresh();
                            SeleniumHelper.WaitForPageLoad(driver, 10);
                            await Task.Delay(random.Next(3000, 4500));
                        }
                    }
                    await Task.Delay(random.Next(2000, 3000));
                    int k_checkalluser = 5;
                    row.Cells["Status"].Value = "Check element all users";
                    string xpath_alluser = "//a[@title='All users']";
                    while (k_checkalluser > 0)
                    {
                        IWebElement element_alluser = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_alluser), 10);
                        if (element_alluser != null)
                        {
                            k_checkalluser = 0;
                            row.Cells["Status"].Value = "Tìm thấy allusers";
                            element_alluser.Click();
                            row.Cells["Status"].Value = "Dealy 3s";
                            await Task.Delay(random.Next(3000, 4500));

                        }
                        else
                        {
                            k_checkalluser = k_checkalluser - 1;
                            await Task.Delay(random.Next(2000));
                        }
                    }
                    await Task.Delay(random.Next(1000, 2000));
                    driver.SwitchTo().DefaultContent();
                    await Task.Delay(random.Next(500, 1000));
                    string xpath_frame2 = "//iframe[@id=\"_react_frame_2\"]";
                    int k_checkframe2 = 5;
                    while (k_checkframe2 > 0)
                    {
                        IWebElement element_frame2 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_frame2), 10);
                        if (element_frame2 != null)
                        {
                            k_checkframe2 = 0;
                            row.Cells["Status"].Value = "Tìm thấy frame 2, switch";
                            driver.SwitchTo().Frame(element_frame2);
                            string xpath_newuser = "//button[@name='New user']";
                            IWebElement element_newuser = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_newuser), 10);
                            element_newuser.Click();
                            await Task.Delay(random.Next(3000, 4500));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy frame";
                            k_checkframe2 = k_checkframe2 - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    await Task.Delay(random.Next(1000, 2000));
                    int k_checkcreateuser = 5;
                    while (k_checkcreateuser>0)
                    {
                        string xpath_createuser = "//button[@data-testid='createNewUser']";
                        IWebElement element_createuser = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_createuser), 10);
                        if(element_createuser != null)
                        {
                            k_checkcreateuser = 0;
                            element_createuser.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));
                        }
                        else
                        {
                            k_checkcreateuser = k_checkcreateuser - 1;
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    // switch về mặt định
                    await Task.Delay(random.Next(1000, 3000));
                    driver.SwitchTo().DefaultContent();
                    await Task.Delay(random.Next(500, 1000));
                    string xpath_frame3 = "//iframe[@id='_react_frame_3']";
                    int k_checkframe3 = 5;
                    while (k_checkframe3 > 0)
                    {
                        IWebElement element_frame3 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_frame3), 10);
                        if (element_frame3 != null)
                        {
                            k_checkframe3 = 0;
                            row.Cells["Status"].Value = "Tìm thấy frame 3, switch";
                            driver.SwitchTo().Frame(element_frame3);
                            string xpath_tkuser = "//input[@id='TextField23']";
                            IWebElement element_tkuser = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_tkuser), 10);
                            element_tkuser.SendKeys(tkuser);
                            await Task.Delay(random.Next(1500, 2500));
                            string xpath_name = "//input[@aria-label='Display name']";
                            IWebElement element_name = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_name), 10);
                            element_name.SendKeys(tkuser);
                            await Task.Delay(random.Next(1500, 2500));
                            string xpath_buttoncreate = "//button[@data-testid=\"review + create\"]";
                            IWebElement element_createclick = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_buttoncreate), 10);
                            element_createclick.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));

                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy frame";
                            k_checkframe3 = k_checkframe3 - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 10);




                    await Task.Delay(random.Next(1000, 3000));
                    // get mail|pass
                    string xpath_gettemail = "//div[@class='textStyle-303']";
                    int k_checkgetemail = 5;
                    while (k_checkgetemail > 0)
                    {
                        IWebElement element_getemail = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_gettemail), 10);
                        if (element_getemail != null)
                        {
                            k_checkgetemail = 0;
                            row.Cells["Status"].Value = "Tìm thấy email";
                            row.Cells["Status"].Value = "Bắt đầu lấy username|pass";
                            auther_email = element_getemail.Text;
                            row.Cells["Email"].Value = auther_email;
                            string xpath_getpass = "//input[@type='password']";
                            IWebElement element_getpass = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_getpass), 10);
                            auther_pass = element_getpass.GetAttribute("value");
                            row.Cells["Password"].Value = auther_pass;
                            string xpath_submit = "//button[@type='submit']";
                            IWebElement element_submit = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_submit), 10);
                            element_submit.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy email|pass";
                            k_checkgetemail = k_checkgetemail - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    // switch về mặc định
                    await Task.Delay(random.Next(1000, 3000));
                
                    driver.SwitchTo().DefaultContent();
                    await Task.Delay(random.Next(500, 1000));
                    // switch về frame 2
                    k_checkframe2 = 5;
                    while (k_checkframe2 > 0)
                    {
                        IWebElement element_frame2 = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_frame2), 10);
                        if (element_frame2 != null)
                        {
                            k_checkframe2 = 0;
                            row.Cells["Status"].Value = "Tìm thấy frame 2, switch";
                            driver.SwitchTo().Frame(element_frame2);
                            string xpath_refresh = "//button[@data-telemetryname='CommandBar - Refresh']";
                            IWebElement element_refresh = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_refresh), 10);
                            element_refresh.Click();
                            await Task.Delay(random.Next(1000, 2000));
                            string xpath_search = "//input[@id=\"SearchBox4\"]";
                            await Task.Delay(random.Next(3000, 4500));
                            IWebElement element_search = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_search), 10);
                            element_search.SendKeys(tkuser);
                            await Task.Delay(random.Next(1000, 2000));
                            string xpath_disnameveryphone = $"//div[contains(@class, 'displayNameStyle-') and text()='{tkuser}']";
                            IWebElement element_disnameveryphone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_disnameveryphone), 10);
                            if(element_disnameveryphone != null)
                            {
                                element_disnameveryphone.Click();
                                await Task.Delay(random.Next(1000, 2000));
                            }    
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy frame";
                            k_checkframe2 = k_checkframe2 - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    SeleniumHelper.WaitForPageLoad(driver, 10);
                    await Task.Delay(random.Next(1000, 2000));
                    driver.SwitchTo().DefaultContent();
                    await Task.Delay(random.Next(1000, 2000));

                    string xpath_auther = "//div[@data-telemetryname='Menu-UserAuthMethods']";
                    int k_checkauther = 5;
                    while (k_checkauther > 0)
                    {
                        IWebElement element_auther = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_auther), 10);
                        if (element_auther != null)
                        {
                            k_checkauther = 0;
                            row.Cells["Status"].Value = "Tìm thấy auther";
                            element_auther.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy auther methods";
                            k_checkauther = k_checkauther - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    await Task.Delay(random.Next(1000, 2000));

                    string xpath_add_auther = "//div[@data-telemetryname='Command-AddAuthenticationMethod']";
                    int k_checkaddauther = 5;
                    while (k_checkaddauther > 0)
                    {
                        IWebElement element_addauther = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_add_auther), 10);
                        if (element_addauther != null)
                        {
                            k_checkaddauther = 0;
                            row.Cells["Status"].Value = "Tìm thấy add auther";
                            element_addauther.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy add auther methods";
                            k_checkaddauther = k_checkaddauther - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    await Task.Delay(random.Next(1000, 2000));
                    String xpath_add_phone = "//span[@aria-label='Toggle']";
                    int k_check_add_phone = 5;
                    while (k_check_add_phone > 0)
                    {
                        IWebElement element_add_phone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_add_phone), 10);
                        if (element_add_phone != null)
                        {
                            k_check_add_phone = 0;
                            row.Cells["Status"].Value = "Tìm thấy add phone";
                            element_add_phone.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));
                            string xpath_add_phone1a = "//span[@class='fxs-portal-svg' and text()='Phone number']";
                            IWebElement element_add_phone1a = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_add_phone1a), 10);
                            element_add_phone1a.Click();
                            row.Cells["Status"].Value = "Dealy 1-3s";
                            await Task.Delay(random.Next(1000, 3000));

                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy add phone";
                            k_check_add_phone = k_check_add_phone - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    await Task.Delay(random.Next(1000, 2000));
                    String xpath_input_phone = "//input[@name='__azc-textBox-tsx1']";
                    int k_check_input_phone = 5;
                    while (k_check_input_phone > 0)
                    {
                        IWebElement element_input_phone = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_input_phone), 10);
                        if (element_input_phone != null)
                        {
                            k_check_input_phone = 0;
                            element_input_phone.SendKeys(auther_phone);
                            await Task.Delay(random.Next(1500, 2500));
                            string xpath_add = "//div[@title=\"Add\"]";
                            IWebElement element_aadd = SeleniumHelper.WaitForElement(driver, By.XPath(xpath_add), 10);
                            element_aadd.Click();
                            await Task.Delay(random.Next(1500, 2500));
                        }
                        else
                        {
                            row.Cells["Status"].Value = "Chưa tìm thấy input phone";
                            k_check_input_phone = k_check_add_phone - 1;
                            await Task.Delay(random.Next(1000, 2000));
                        }
                    }
                    await Task.Delay(random.Next(1000, 2000));
                // check element đăng nhập susccess
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