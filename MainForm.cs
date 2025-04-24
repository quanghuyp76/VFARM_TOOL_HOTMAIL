using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace X_Vframe_Tool
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public CancellationTokenSource? cancellationTokenSource;
        public static ConcurrentBag<string>? List_IdGPM;
        public ConcurrentBag<DataGridViewRow>? Index;
        public ConcurrentBag<DataGridViewRow>? Index_MsAddPhone;
        public ConcurrentBag<DataGridViewRow>? Index_Login_2FA;
        public ConcurrentBag<string>? IndexEmail;
        public ConcurrentBag<string>? IndexPhone;
        private static ConcurrentBag<string> listPhone = new ConcurrentBag<string>();
        // update setting đã lưu
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(Paths.Path_SettingOpption))
            {
                string json = File.ReadAllText(Paths.Path_SettingOpption);
                if (!string.IsNullOrEmpty(json))
                {
                    //SettingOption settingOption = new SettingOption();
                    SettingOption? settingOption = JsonConvert.DeserializeObject<SettingOption>(json);
                    if (settingOption != null)
                    {
                        //check chrome
                        checkBox_HideChrome.Checked = settingOption.check_HideChrome;
                        checkBox_Anonymous.Checked = settingOption.check_Anonymous;
                        checkBox_DisableGpu.Checked = settingOption.check_DisableGpu;
                        //check input
                        checkBox_InputLocalFile.Checked = settingOption.check_Input_File_Location;
                        checkBox_InputUrl.Checked = settingOption.check_Input_Website;
                        //check proxy
                        checkBox_ProxyLocal.Checked = settingOption.check_Proxy_IP_PORT;
                        checkBox_ProxyNon.Checked = settingOption.check_Proxy_Never;
                        //check browser
                        comboBox_Browser.SelectedItem = settingOption.check_Browser;
                        comboBox_Code.SelectedItem = settingOption.check_Code;
                        comboBox_Script.SelectedItem = settingOption.check_Script;
                        //check design chrome
                        textBox_ChromeArrange.Text = settingOption.check_ChormeArrange;
                        textBox_ChromeSize.Text = settingOption.check_ChromeSize;
                        //check run
                        textBox_Process.Text = settingOption.check_NumberProcess.ToString();
                        numericUpDown_NumberThread.Value = settingOption.check_NumberThread;
                        textBox_UrlGetOtp.Text = settingOption.check_UrlGetOtp;
                    }
                }
                else
                {
                    SettingOption settingOption = new SettingOption();
                    //setting chrome
                    settingOption.check_HideChrome = false;
                    settingOption.check_Anonymous = false;
                    settingOption.check_DisableGpu = false;
                    // setting input
                    settingOption.check_Input_File_Location = false;
                    settingOption.check_Input_Website = false;
                    // setting proxy
                    settingOption.check_Proxy_Never = false;
                    settingOption.check_Proxy_IP_PORT = false;
                    // setting browser
                    settingOption.check_Browser = "GPM";
                    settingOption.check_Code = "Selenium";
                    settingOption.check_Script = "Reg Mail Office";
                    // setting design chrome
                    settingOption.check_ChromeSize = "300x400";
                    settingOption.check_ChormeArrange = "1x1";
                    //setting run
                    settingOption.check_NumberThread = 1;
                    settingOption.check_NumberProcess = 1;
                    File.WriteAllText(Paths.Path_SettingOpption, JsonConvert.SerializeObject(settingOption, Newtonsoft.Json.Formatting.Indented));
                }
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); // Dừng việc mở luồng mới
                MessageBox.Show("Đã dừng việc tạo luồng mới, các luồng đang chạy sẽ tiếp tục hoàn thành.");
            }
        }

        private void dataGridView_Start_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async Task RunMain(int thread, Script_Sele scriptRunner, CancellationToken token)
        {
            List<Task> tasks = new List<Task>();
            int column = 5;  // Số cột tối đa
            int rowHeight = 300;
            int colWidth = 500;
            for (int i = 0; i < thread; i++)
            {
                if (token.IsCancellationRequested) // Kiểm tra nếu yêu cầu dừng
                    break;
                int k = i;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        int x = (k % column) * colWidth + 20;
                        int y = (k / column) * rowHeight;
                        string win_pos = $"{x},{y}";
                        await scriptRunner.Run(k, token, win_pos);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi trong luồng {i}: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        private async void button_Start_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource(); // Tạo token mới mỗi khi Start
            CancellationToken token = cancellationTokenSource.Token;
            // Đọc Setting
            SettingOption settingOption = SettingOption.GetSettingOption(this);
            File.WriteAllText(Paths.Path_SettingOpption, JsonConvert.SerializeObject(settingOption, Newtonsoft.Json.Formatting.Indented));
            //check setting
            string json = File.ReadAllText(Paths.Path_SettingOpption);
            GlobalSetting.SettingOption_Run = JsonConvert.DeserializeObject<SettingOption>(json);
            // check file json
            if (GlobalSetting.SettingOption_Run.check_Code == null)
            {
                MessageBox.Show("Không đọc được file cấu hình hoặc file JSON không hợp lệ.");
                return;
            }
            // check Code
            if (GlobalSetting.SettingOption_Run.check_Code == "Selenium")
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region Reg Mail Auther
                if (GlobalSetting.SettingOption_Run.check_Script == "Reg Mail Auther")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        var listPhone = File.ReadAllLines("input\\phone.txt").ToList();
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        IndexPhone = new ConcurrentBag<string>(listPhone.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Script_RegMail_Auther script = new Script_RegMail_Auther();
                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            script.IndexPhone = IndexPhone;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ Chorme Drive");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");
                    }
                    ;
                }
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region MS_AddPhone
                else if (GlobalSetting.SettingOption_Run.check_Script == "MS_Addphone")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0)
                        {
                            MessageBox.Show("Số lượng Process và Thread phải lớn hơn 0");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        Index_MsAddPhone = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());

                        try
                        {
                            Script_MS_AddPhone script = new Script_MS_AddPhone();
                            script.Index_MsAddPhone = Index_MsAddPhone;
                            script.IndexEmail = IndexEmail;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ trình duyệt này");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");

                    }
                }
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region Login_2FA
                else if (GlobalSetting.SettingOption_Run.check_Script == "Login_2FA")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        // hàm run
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }

                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string dataFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");      // File cần lọc dữ liệu
                        string searchFile = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt"); // File chứa danh sách email cần tìm
                        string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt"); // File kết quả sau khi lọc
                        string searchFile2 = Path.Combine(Directory.GetCurrentDirectory(), "output", "Nootp.txt");
                        EmailFilter.FilterEmails(dataFile, searchFile, outputFile);
                        EmailFilter.FilterEmails(dataFile, searchFile2, outputFile);
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Script_Login_2FA script = new Script_Login_2FA();
                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Chưa hỗ trợ trình duyệt này");
                        return;
                    }
                }
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region Login_Auther
                else if (GlobalSetting.SettingOption_Run.check_Script == "Login_Auther")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string dataFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");      // File cần lọc dữ liệu
                        string searchFile = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt"); // File chứa danh sách email cần tìm
                        string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt"); // File kết quả sau khi lọc
                        string searchFile2 = Path.Combine(Directory.GetCurrentDirectory(), "output", "Nootp.txt");
                        EmailFilter.FilterEmails(dataFile, searchFile, outputFile);
                        EmailFilter.FilterEmails(dataFile, searchFile2, outputFile);
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Script_Login_Auther script = new Script_Login_Auther();
                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ Chorme Drive");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");
                    }
                }
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region Login_2FA_MSTeam
                else if (GlobalSetting.SettingOption_Run.check_Script == "Login_2FA_Microsoft Teams")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string dataFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");      // File cần lọc dữ liệu
                        string searchFile = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt"); // File chứa danh sách email cần tìm
                        string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt"); // File kết quả sau khi lọc
                        string searchFile2 = Path.Combine(Directory.GetCurrentDirectory(), "output", "Nootp.txt");
                        EmailFilter.FilterEmails(dataFile, searchFile, outputFile);
                        EmailFilter.FilterEmails(dataFile, searchFile2, outputFile);
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Script_Login_2FA_MSTeam script = new Script_Login_2FA_MSTeam();
                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ Chorme Drive");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");
                    }
                }
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region TaskReward()
                else if (GlobalSetting.SettingOption_Run.check_Script == "Login_Reward")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string dataFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");      // File cần lọc dữ liệu
                        string searchFile = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt"); // File chứa danh sách email cần tìm
                        string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt"); // File kết quả sau khi lọc
                        string searchFile2 = Path.Combine(Directory.GetCurrentDirectory(), "output", "Nootp.txt");
                        EmailFilter.FilterEmails(dataFile, searchFile, outputFile);
                        EmailFilter.FilterEmails(dataFile, searchFile2, outputFile);
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Script_Login_Reward script = new Script_Login_Reward();
                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ Chorme Drive");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");
                    }
                }
                #endregion
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region Reg_Alibaba()
                else if (GlobalSetting.SettingOption_Run.check_Script == "Reg_Alibaba")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        ////string dataFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");      // File cần lọc dữ liệu
                        ////string searchFile = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt"); // File chứa danh sách email cần tìm
                        ////string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt"); // File kết quả sau khi lọc
                        ////string searchFile2 = Path.Combine(Directory.GetCurrentDirectory(), "output", "Nootp.txt");
                        //EmailFilter.FilterEmails(dataFile, searchFile, outputFile);
                        //EmailFilter.FilterEmails(dataFile, searchFile2, outputFile);
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Reg_Alibaba script = new Reg_Alibaba();
                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ Chorme Drive");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");
                    }
                }

                #endregion
                #region TaskUnlockHotmail()
                else if (GlobalSetting.SettingOption_Run.check_Script == "UnlockHotmail")
                {
                    if (GlobalSetting.SettingOption_Run.check_Browser == "GPM")
                    {
                        int numberProcess = GlobalSetting.SettingOption_Run.check_NumberProcess;
                        int numberThread = GlobalSetting.SettingOption_Run.check_NumberThread;
                        if (numberProcess <= 0 || numberThread <= 0 || numberThread > numberProcess)
                        {
                            MessageBox.Show("Number Process và Thread không hợp lệ");
                            return;
                        }
                        dataGridView_Start.Rows.Clear();
                        for (int i = 0; i < numberProcess; i++)
                        {
                            dataGridView_Start.Rows.Add(i, "");
                        }
                        if (File.ReadAllLines("input\\Email.txt").Length == 0)
                        {
                            MessageBox.Show("Vui lòng thêm Email", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string dataFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");      // File cần lọc dữ liệu
                        string searchFile = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt"); // File chứa danh sách email cần tìm
                        string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt"); // File kết quả sau khi lọc
                        string searchFile2 = Path.Combine(Directory.GetCurrentDirectory(), "output", "AccountBlock.txt");
                        EmailFilter.FilterEmails(dataFile, searchFile, outputFile);
                        EmailFilter.FilterEmails(dataFile, searchFile2, outputFile);

                        string dataphone = Path.Combine(Directory.GetCurrentDirectory(), "input", "phone.txt");      // File cần lọc dữ liệu
                        string searchFilephone = Path.Combine(Directory.GetCurrentDirectory(), "output", "Phoneused.txt"); // File chứa danh sách email cần tìm
                        string outputFilePhone = Path.Combine(Directory.GetCurrentDirectory(), "input", "phone.txt"); // File kết quả sau khi lọc
                        EmailFilter.FilterPhone(dataphone, searchFilephone, outputFilePhone);
                        var listdataEmail = File.ReadAllLines("input\\Email.txt").ToList();
                        var listphone = File.ReadAllLines("input\\phone.txt").ToList();
                        listPhone = new ConcurrentBag<string>(File.ReadAllLines("input\\phone.txt").Select(p => p.Trim()));
                        IndexEmail = new ConcurrentBag<string>(listdataEmail.ToArray());
                        IndexPhone = new ConcurrentBag<string>(listphone.ToArray());
                        Index_Login_2FA = new ConcurrentBag<DataGridViewRow>(dataGridView_Start.Rows.OfType<DataGridViewRow>());
                        try
                        {
                            Script_UnlockHotmail script = new Script_UnlockHotmail();

                            script.Index_Login_2FA = Index_Login_2FA;
                            script.IndexEmail = IndexEmail;
                            script.IndexPhone = IndexPhone;
                            await RunMain(numberThread, script, token);
                        }
                        catch { }
                        finally
                        {
                            MessageBox.Show("Đã hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (GlobalSetting.SettingOption_Run.check_Browser == "Chrome Drive" || GlobalSetting.SettingOption_Run.check_Browser == "Gologin")
                    {
                        MessageBox.Show("Chưa hỗ trợ Chorme Drive");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn Browser");
                    }
                }
                #endregion



                else
                {
                    MessageBox.Show("Vui lòng chọn Script");
                    return;
                }
            }
            else if (GlobalSetting.SettingOption_Run.check_Code == "PlayWright")
            {
                MessageBox.Show("Chưa hỗ trợ PlayWright");
                return;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Code");
            }
            if (GlobalSetting.SettingOption_Run.check_Code == null)
            {
                MessageBox.Show("Không đọc được file cấu hình hoặc file JSON không hợp lệ.");
                return;
            }
        }


        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void comboBox_Script_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedScript = comboBox_Script.SelectedItem.ToString() ?? string.Empty;

        }

        private void button_DeleteTable_Click(object sender, EventArgs e)
        {
            dataGridView_Start.Rows.Clear();
        }

        private void button_Proxy_Click(object sender, EventArgs e)
        {
            string Path_FileProxy = Path.Combine(Directory.GetCurrentDirectory(), "input", "Proxy.txt");
            if (File.Exists(Path_FileProxy))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path_FileProxy,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_FileEmail_Click(object sender, EventArgs e)
        {
            string Path_Email = Path.Combine(Directory.GetCurrentDirectory(), "input", "Email.txt");
            if (File.Exists(Path_Email))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path_Email,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_Output_Click(object sender, EventArgs e)
        {
            string Path_OutPut = Path.Combine(Directory.GetCurrentDirectory(), "output", "Data.txt");
            if (File.Exists(Path_OutPut))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path_OutPut,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Button_Close_Chrome_Click(object sender, EventArgs e)
        {
            var chromeProcesses = Process.GetProcessesByName("chrome");
            foreach (var process in chromeProcesses)
            {
                if (!process.HasExited)
                {
                    process.CloseMainWindow();
                    if (!process.WaitForExit(5000)) // Chờ 5 giây để tiến trình thoát
                    {
                        process.Kill();
                    }
                    process.WaitForExit(); // Đảm bảo tiến trình đã kết thúc
                    process.Dispose(); // Giải phóng tài nguyên
                }
            }
        }

        private void button_OutputFail_Click(object sender, EventArgs e)
        {
            string Path_OutPut = Path.Combine(Directory.GetCurrentDirectory(), "output", "AccountBlock.txt");
            if (File.Exists(Path_OutPut))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path_OutPut,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Button_Close_ChromeDrive_Click(object sender, EventArgs e)
        {
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            int closedCount = 0;
            foreach (Process process in chromeDriverProcesses)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit();
                        closedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Không thể đóng tiến trình ChromeDriver (PID: {process.Id}): {ex.Message}");
                }
            }
            MessageBox.Show($"Đã đóng {closedCount} ChromeDriver.");
        }

        private void button_InputPhone_Click(object sender, EventArgs e)
        {
            string Path_Phone = Path.Combine(Directory.GetCurrentDirectory(), "input", "phone.txt");
            if (File.Exists(Path_Phone))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path_Phone,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_outputphone_Click(object sender, EventArgs e)
        {
            string Path_Phone = Path.Combine(Directory.GetCurrentDirectory(), "output", "Phoneused.txt");
            if (File.Exists(Path_Phone))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path_Phone,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    public static class Paths
    {
        public static string Path_SettingOpption = Path.Combine(Application.StartupPath, "save", "OptionSetting.txt");
        public static string Path_Output = Path.Combine(Application.StartupPath, "output");
    }
}
