namespace X_Vframe_Tool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            TabControl1 = new TabControl();
            Start = new TabPage();
            button_InputPhone = new Button();
            button_OutputFail = new Button();
            textBox_Process = new TextBox();
            button_FileEmail = new Button();
            button_Proxy = new Button();
            label_UrlGetOtp = new Label();
            textBox_UrlGetOtp = new TextBox();
            label_Script = new Label();
            comboBox_Script = new ComboBox();
            numericUpDown_NumberThread = new NumericUpDown();
            label_code = new Label();
            comboBox_Code = new ComboBox();
            label_Browser = new Label();
            comboBox_Browser = new ComboBox();
            button_OutputSuscces = new Button();
            label_NumberProcess = new Label();
            label_ChromeArrange = new Label();
            textBox_ChromeArrange = new TextBox();
            dataGridView_Start = new DataGridView();
            STT = new DataGridViewTextBoxColumn();
            Email = new DataGridViewTextBoxColumn();
            Password = new DataGridViewTextBoxColumn();
            EmailRecovery = new DataGridViewTextBoxColumn();
            Phone = new DataGridViewTextBoxColumn();
            Proxy = new DataGridViewTextBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            button_DeleteTable = new Button();
            label_ChromeSize = new Label();
            textBox_ChromeSize = new TextBox();
            label_NumberThread = new Label();
            Button_Close_Chrome = new Button();
            Button_Close_ChromeDrive = new Button();
            Button_Update_ChromeDrive = new Button();
            button_Stop = new Button();
            button_Start = new Button();
            Setting = new TabPage();
            groupBox_Captcha = new GroupBox();
            button_CaptchaBalance = new Button();
            textBox_ApiCaptcha = new TextBox();
            label_ApiCaptcha = new Label();
            comboBox1 = new ComboBox();
            label_TypeCapcha = new Label();
            groupBox_SettingChrome = new GroupBox();
            checkBox_DisableGpu = new CheckBox();
            checkBox_Anonymous = new CheckBox();
            checkBox_HideChrome = new CheckBox();
            groupBox_Proxy = new GroupBox();
            checkBox_ProxyNon = new CheckBox();
            checkBox_ProxyLocal = new CheckBox();
            textBox_Proxy = new TextBox();
            groupBox_Input = new GroupBox();
            checkBox_InputUrl = new CheckBox();
            checkBox_InputLocalFile = new CheckBox();
            textBox_input = new TextBox();
            button_InputFile = new Button();
            button_outputphone = new Button();
            TabControl1.SuspendLayout();
            Start.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_NumberThread).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_Start).BeginInit();
            Setting.SuspendLayout();
            groupBox_Captcha.SuspendLayout();
            groupBox_SettingChrome.SuspendLayout();
            groupBox_Proxy.SuspendLayout();
            groupBox_Input.SuspendLayout();
            SuspendLayout();
            // 
            // TabControl1
            // 
            TabControl1.Controls.Add(Start);
            TabControl1.Controls.Add(Setting);
            TabControl1.Location = new Point(-2, 1);
            TabControl1.Name = "TabControl1";
            TabControl1.SelectedIndex = 0;
            TabControl1.Size = new Size(1280, 625);
            TabControl1.TabIndex = 0;
            // 
            // Start
            // 
            Start.Controls.Add(button_outputphone);
            Start.Controls.Add(button_InputPhone);
            Start.Controls.Add(button_OutputFail);
            Start.Controls.Add(textBox_Process);
            Start.Controls.Add(button_FileEmail);
            Start.Controls.Add(button_Proxy);
            Start.Controls.Add(label_UrlGetOtp);
            Start.Controls.Add(textBox_UrlGetOtp);
            Start.Controls.Add(label_Script);
            Start.Controls.Add(comboBox_Script);
            Start.Controls.Add(numericUpDown_NumberThread);
            Start.Controls.Add(label_code);
            Start.Controls.Add(comboBox_Code);
            Start.Controls.Add(label_Browser);
            Start.Controls.Add(comboBox_Browser);
            Start.Controls.Add(button_OutputSuscces);
            Start.Controls.Add(label_NumberProcess);
            Start.Controls.Add(label_ChromeArrange);
            Start.Controls.Add(textBox_ChromeArrange);
            Start.Controls.Add(dataGridView_Start);
            Start.Controls.Add(button_DeleteTable);
            Start.Controls.Add(label_ChromeSize);
            Start.Controls.Add(textBox_ChromeSize);
            Start.Controls.Add(label_NumberThread);
            Start.Controls.Add(Button_Close_Chrome);
            Start.Controls.Add(Button_Close_ChromeDrive);
            Start.Controls.Add(Button_Update_ChromeDrive);
            Start.Controls.Add(button_Stop);
            Start.Controls.Add(button_Start);
            Start.Location = new Point(4, 24);
            Start.Name = "Start";
            Start.Padding = new Padding(3);
            Start.Size = new Size(1272, 597);
            Start.TabIndex = 0;
            Start.Text = "Start";
            // 
            // button_InputPhone
            // 
            button_InputPhone.Location = new Point(915, 71);
            button_InputPhone.Name = "button_InputPhone";
            button_InputPhone.Size = new Size(106, 23);
            button_InputPhone.TabIndex = 32;
            button_InputPhone.Text = "Input_Phone";
            button_InputPhone.UseVisualStyleBackColor = true;
            button_InputPhone.Click += button_InputPhone_Click;
            // 
            // button_OutputFail
            // 
            button_OutputFail.Location = new Point(230, 6);
            button_OutputFail.Name = "button_OutputFail";
            button_OutputFail.Size = new Size(58, 90);
            button_OutputFail.TabIndex = 31;
            button_OutputFail.Text = "Output_Fail";
            button_OutputFail.UseVisualStyleBackColor = true;
            button_OutputFail.Click += button_OutputFail_Click;
            // 
            // textBox_Process
            // 
            textBox_Process.Location = new Point(294, 28);
            textBox_Process.Name = "textBox_Process";
            textBox_Process.Size = new Size(85, 23);
            textBox_Process.TabIndex = 30;
            // 
            // button_FileEmail
            // 
            button_FileEmail.Location = new Point(803, 70);
            button_FileEmail.Name = "button_FileEmail";
            button_FileEmail.Size = new Size(106, 23);
            button_FileEmail.TabIndex = 29;
            button_FileEmail.Text = "File Data Email";
            button_FileEmail.UseVisualStyleBackColor = true;
            button_FileEmail.Click += button_FileEmail_Click;
            // 
            // button_Proxy
            // 
            button_Proxy.Location = new Point(691, 71);
            button_Proxy.Name = "button_Proxy";
            button_Proxy.Size = new Size(106, 23);
            button_Proxy.TabIndex = 28;
            button_Proxy.Text = "File Proxy";
            button_Proxy.UseVisualStyleBackColor = true;
            button_Proxy.Click += button_Proxy_Click;
            // 
            // label_UrlGetOtp
            // 
            label_UrlGetOtp.AutoSize = true;
            label_UrlGetOtp.Location = new Point(687, 11);
            label_UrlGetOtp.Name = "label_UrlGetOtp";
            label_UrlGetOtp.Size = new Size(66, 15);
            label_UrlGetOtp.TabIndex = 27;
            label_UrlGetOtp.Text = "Url Get Otp";
            // 
            // textBox_UrlGetOtp
            // 
            textBox_UrlGetOtp.Location = new Point(691, 29);
            textBox_UrlGetOtp.Name = "textBox_UrlGetOtp";
            textBox_UrlGetOtp.Size = new Size(422, 23);
            textBox_UrlGetOtp.TabIndex = 26;
            // 
            // label_Script
            // 
            label_Script.AutoSize = true;
            label_Script.Location = new Point(294, 55);
            label_Script.Name = "label_Script";
            label_Script.Size = new Size(37, 15);
            label_Script.TabIndex = 25;
            label_Script.Text = "Script";
            // 
            // comboBox_Script
            // 
            comboBox_Script.FormattingEnabled = true;
            comboBox_Script.Items.AddRange(new object[] { "UnlockHotmail" });
            comboBox_Script.Location = new Point(294, 71);
            comboBox_Script.Name = "comboBox_Script";
            comboBox_Script.Size = new Size(179, 23);
            comboBox_Script.TabIndex = 24;
            comboBox_Script.SelectedIndexChanged += comboBox_Script_SelectedIndexChanged;
            // 
            // numericUpDown_NumberThread
            // 
            numericUpDown_NumberThread.Location = new Point(434, 28);
            numericUpDown_NumberThread.Name = "numericUpDown_NumberThread";
            numericUpDown_NumberThread.Size = new Size(39, 23);
            numericUpDown_NumberThread.TabIndex = 22;
            // 
            // label_code
            // 
            label_code.AutoSize = true;
            label_code.Location = new Point(585, 53);
            label_code.Name = "label_code";
            label_code.Size = new Size(35, 15);
            label_code.TabIndex = 21;
            label_code.Text = "Code";
            // 
            // comboBox_Code
            // 
            comboBox_Code.FormattingEnabled = true;
            comboBox_Code.Items.AddRange(new object[] { "Selenium", "PlayWright" });
            comboBox_Code.Location = new Point(585, 71);
            comboBox_Code.Name = "comboBox_Code";
            comboBox_Code.Size = new Size(100, 23);
            comboBox_Code.TabIndex = 20;
            // 
            // label_Browser
            // 
            label_Browser.AutoSize = true;
            label_Browser.Location = new Point(479, 55);
            label_Browser.Name = "label_Browser";
            label_Browser.Size = new Size(49, 15);
            label_Browser.TabIndex = 19;
            label_Browser.Text = "Browser";
            // 
            // comboBox_Browser
            // 
            comboBox_Browser.FormattingEnabled = true;
            comboBox_Browser.Items.AddRange(new object[] { "Chrome Drive", "GPM", "Gologin" });
            comboBox_Browser.Location = new Point(479, 70);
            comboBox_Browser.Name = "comboBox_Browser";
            comboBox_Browser.Size = new Size(100, 23);
            comboBox_Browser.TabIndex = 18;
            // 
            // button_OutputSuscces
            // 
            button_OutputSuscces.Location = new Point(162, 5);
            button_OutputSuscces.Name = "button_OutputSuscces";
            button_OutputSuscces.Size = new Size(62, 90);
            button_OutputSuscces.TabIndex = 15;
            button_OutputSuscces.Text = "Output Suscces";
            button_OutputSuscces.UseVisualStyleBackColor = true;
            button_OutputSuscces.Click += button_Output_Click;
            // 
            // label_NumberProcess
            // 
            label_NumberProcess.AutoSize = true;
            label_NumberProcess.Location = new Point(294, 11);
            label_NumberProcess.Name = "label_NumberProcess";
            label_NumberProcess.Size = new Size(47, 15);
            label_NumberProcess.TabIndex = 14;
            label_NumberProcess.Text = "Process";
            // 
            // label_ChromeArrange
            // 
            label_ChromeArrange.AutoSize = true;
            label_ChromeArrange.Location = new Point(471, 11);
            label_ChromeArrange.Name = "label_ChromeArrange";
            label_ChromeArrange.Size = new Size(95, 15);
            label_ChromeArrange.TabIndex = 12;
            label_ChromeArrange.Text = "Chrome Arrange";
            // 
            // textBox_ChromeArrange
            // 
            textBox_ChromeArrange.Location = new Point(479, 29);
            textBox_ChromeArrange.Name = "textBox_ChromeArrange";
            textBox_ChromeArrange.PlaceholderText = "Row X Colum";
            textBox_ChromeArrange.Size = new Size(100, 23);
            textBox_ChromeArrange.TabIndex = 11;
            // 
            // dataGridView_Start
            // 
            dataGridView_Start.AllowUserToAddRows = false;
            dataGridView_Start.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dataGridView_Start.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView_Start.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_Start.Columns.AddRange(new DataGridViewColumn[] { STT, Email, Password, EmailRecovery, Phone, Proxy, Status });
            dataGridView_Start.Location = new Point(5, 113);
            dataGridView_Start.Name = "dataGridView_Start";
            dataGridView_Start.RowHeadersVisible = false;
            dataGridView_Start.Size = new Size(1271, 484);
            dataGridView_Start.TabIndex = 10;
            dataGridView_Start.CellContentClick += dataGridView_Start_CellContentClick;
            // 
            // STT
            // 
            STT.HeaderText = "STT";
            STT.Name = "STT";
            STT.Resizable = DataGridViewTriState.False;
            STT.SortMode = DataGridViewColumnSortMode.Programmatic;
            STT.Width = 50;
            // 
            // Email
            // 
            Email.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Email.HeaderText = "Email";
            Email.Name = "Email";
            Email.ReadOnly = true;
            Email.Resizable = DataGridViewTriState.False;
            Email.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Password
            // 
            Password.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Password.HeaderText = "Password";
            Password.Name = "Password";
            Password.ReadOnly = true;
            Password.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // EmailRecovery
            // 
            EmailRecovery.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmailRecovery.HeaderText = "Email Recovery";
            EmailRecovery.Name = "EmailRecovery";
            EmailRecovery.ReadOnly = true;
            EmailRecovery.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Phone
            // 
            Phone.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Phone.HeaderText = "Phone";
            Phone.Name = "Phone";
            Phone.ReadOnly = true;
            Phone.SortMode = DataGridViewColumnSortMode.NotSortable;
            Phone.Width = 123;
            // 
            // Proxy
            // 
            Proxy.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Proxy.HeaderText = "Proxy";
            Proxy.Name = "Proxy";
            Proxy.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            Status.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Status.HeaderText = "Status";
            Status.Name = "Status";
            Status.ReadOnly = true;
            // 
            // button_DeleteTable
            // 
            button_DeleteTable.Location = new Point(99, 6);
            button_DeleteTable.Name = "button_DeleteTable";
            button_DeleteTable.Size = new Size(57, 90);
            button_DeleteTable.TabIndex = 9;
            button_DeleteTable.Text = "Delete Table";
            button_DeleteTable.UseVisualStyleBackColor = true;
            button_DeleteTable.Click += button_DeleteTable_Click;
            // 
            // label_ChromeSize
            // 
            label_ChromeSize.AutoSize = true;
            label_ChromeSize.Location = new Point(581, 11);
            label_ChromeSize.Name = "label_ChromeSize";
            label_ChromeSize.Size = new Size(73, 15);
            label_ChromeSize.TabIndex = 8;
            label_ChromeSize.Text = "Chrome Size";
            // 
            // textBox_ChromeSize
            // 
            textBox_ChromeSize.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox_ChromeSize.Location = new Point(585, 28);
            textBox_ChromeSize.Name = "textBox_ChromeSize";
            textBox_ChromeSize.PlaceholderText = "Width x Height";
            textBox_ChromeSize.Size = new Size(100, 23);
            textBox_ChromeSize.TabIndex = 7;
            // 
            // label_NumberThread
            // 
            label_NumberThread.AutoSize = true;
            label_NumberThread.Location = new Point(385, 30);
            label_NumberThread.Name = "label_NumberThread";
            label_NumberThread.Size = new Size(43, 15);
            label_NumberThread.TabIndex = 6;
            label_NumberThread.Text = "Thread";
            // 
            // Button_Close_Chrome
            // 
            Button_Close_Chrome.Location = new Point(1119, 80);
            Button_Close_Chrome.Name = "Button_Close_Chrome";
            Button_Close_Chrome.Size = new Size(147, 26);
            Button_Close_Chrome.TabIndex = 4;
            Button_Close_Chrome.Text = "Close Chrome";
            Button_Close_Chrome.UseVisualStyleBackColor = true;
            Button_Close_Chrome.Click += Button_Close_Chrome_Click;
            // 
            // Button_Close_ChromeDrive
            // 
            Button_Close_ChromeDrive.Location = new Point(1119, 44);
            Button_Close_ChromeDrive.Name = "Button_Close_ChromeDrive";
            Button_Close_ChromeDrive.Size = new Size(147, 26);
            Button_Close_ChromeDrive.TabIndex = 3;
            Button_Close_ChromeDrive.Text = "Close Chrome Drive";
            Button_Close_ChromeDrive.UseVisualStyleBackColor = true;
            Button_Close_ChromeDrive.Click += Button_Close_ChromeDrive_Click;
            // 
            // Button_Update_ChromeDrive
            // 
            Button_Update_ChromeDrive.Location = new Point(1119, 5);
            Button_Update_ChromeDrive.Name = "Button_Update_ChromeDrive";
            Button_Update_ChromeDrive.Size = new Size(147, 26);
            Button_Update_ChromeDrive.TabIndex = 2;
            Button_Update_ChromeDrive.Text = "Update Chrome Drive";
            Button_Update_ChromeDrive.UseVisualStyleBackColor = true;
            // 
            // button_Stop
            // 
            button_Stop.Location = new Point(6, 51);
            button_Stop.Name = "button_Stop";
            button_Stop.Size = new Size(87, 46);
            button_Stop.TabIndex = 1;
            button_Stop.Text = "Stop";
            button_Stop.UseVisualStyleBackColor = true;
            button_Stop.Click += button_Stop_Click;
            // 
            // button_Start
            // 
            button_Start.Location = new Point(6, 3);
            button_Start.Name = "button_Start";
            button_Start.Size = new Size(87, 42);
            button_Start.TabIndex = 0;
            button_Start.Text = "Start";
            button_Start.UseVisualStyleBackColor = true;
            button_Start.Click += button_Start_Click;
            // 
            // Setting
            // 
            Setting.Controls.Add(groupBox_Captcha);
            Setting.Controls.Add(groupBox_SettingChrome);
            Setting.Controls.Add(groupBox_Proxy);
            Setting.Controls.Add(groupBox_Input);
            Setting.Location = new Point(4, 24);
            Setting.Name = "Setting";
            Setting.Padding = new Padding(3);
            Setting.Size = new Size(1272, 597);
            Setting.TabIndex = 1;
            Setting.Text = "Setting";
            Setting.UseVisualStyleBackColor = true;
            // 
            // groupBox_Captcha
            // 
            groupBox_Captcha.Controls.Add(button_CaptchaBalance);
            groupBox_Captcha.Controls.Add(textBox_ApiCaptcha);
            groupBox_Captcha.Controls.Add(label_ApiCaptcha);
            groupBox_Captcha.Controls.Add(comboBox1);
            groupBox_Captcha.Controls.Add(label_TypeCapcha);
            groupBox_Captcha.Location = new Point(303, 85);
            groupBox_Captcha.Name = "groupBox_Captcha";
            groupBox_Captcha.Size = new Size(305, 100);
            groupBox_Captcha.TabIndex = 3;
            groupBox_Captcha.TabStop = false;
            groupBox_Captcha.Text = "Captcha";
            // 
            // button_CaptchaBalance
            // 
            button_CaptchaBalance.Location = new Point(215, 19);
            button_CaptchaBalance.Name = "button_CaptchaBalance";
            button_CaptchaBalance.Size = new Size(75, 66);
            button_CaptchaBalance.TabIndex = 4;
            button_CaptchaBalance.Text = "Check Balance";
            button_CaptchaBalance.UseVisualStyleBackColor = true;
            // 
            // textBox_ApiCaptcha
            // 
            textBox_ApiCaptcha.Location = new Point(84, 62);
            textBox_ApiCaptcha.Name = "textBox_ApiCaptcha";
            textBox_ApiCaptcha.Size = new Size(121, 23);
            textBox_ApiCaptcha.TabIndex = 3;
            // 
            // label_ApiCaptcha
            // 
            label_ApiCaptcha.AutoSize = true;
            label_ApiCaptcha.Location = new Point(6, 65);
            label_ApiCaptcha.Name = "label_ApiCaptcha";
            label_ApiCaptcha.Size = new Size(72, 15);
            label_ApiCaptcha.TabIndex = 2;
            label_ApiCaptcha.Text = "Captcha Api";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "OMO CAPTCHA", "EZ CAPTCHA" });
            comboBox1.Location = new Point(84, 22);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 1;
            // 
            // label_TypeCapcha
            // 
            label_TypeCapcha.AutoSize = true;
            label_TypeCapcha.Location = new Point(6, 25);
            label_TypeCapcha.Name = "label_TypeCapcha";
            label_TypeCapcha.Size = new Size(78, 15);
            label_TypeCapcha.TabIndex = 0;
            label_TypeCapcha.Text = "Type Captcha";
            // 
            // groupBox_SettingChrome
            // 
            groupBox_SettingChrome.Controls.Add(checkBox_DisableGpu);
            groupBox_SettingChrome.Controls.Add(checkBox_Anonymous);
            groupBox_SettingChrome.Controls.Add(checkBox_HideChrome);
            groupBox_SettingChrome.Location = new Point(303, 3);
            groupBox_SettingChrome.Name = "groupBox_SettingChrome";
            groupBox_SettingChrome.Size = new Size(305, 76);
            groupBox_SettingChrome.TabIndex = 2;
            groupBox_SettingChrome.TabStop = false;
            groupBox_SettingChrome.Text = "Setting Chrome";
            // 
            // checkBox_DisableGpu
            // 
            checkBox_DisableGpu.AutoSize = true;
            checkBox_DisableGpu.Location = new Point(6, 47);
            checkBox_DisableGpu.Name = "checkBox_DisableGpu";
            checkBox_DisableGpu.Size = new Size(90, 19);
            checkBox_DisableGpu.TabIndex = 2;
            checkBox_DisableGpu.Text = "Disable GPU";
            checkBox_DisableGpu.UseVisualStyleBackColor = true;
            // 
            // checkBox_Anonymous
            // 
            checkBox_Anonymous.AutoSize = true;
            checkBox_Anonymous.Location = new Point(109, 22);
            checkBox_Anonymous.Name = "checkBox_Anonymous";
            checkBox_Anonymous.Size = new Size(91, 19);
            checkBox_Anonymous.TabIndex = 1;
            checkBox_Anonymous.Text = "Anonymous";
            checkBox_Anonymous.UseVisualStyleBackColor = true;
            // 
            // checkBox_HideChrome
            // 
            checkBox_HideChrome.AutoSize = true;
            checkBox_HideChrome.Location = new Point(6, 22);
            checkBox_HideChrome.Name = "checkBox_HideChrome";
            checkBox_HideChrome.Size = new Size(97, 19);
            checkBox_HideChrome.TabIndex = 0;
            checkBox_HideChrome.Text = "Hide Chrome";
            checkBox_HideChrome.UseVisualStyleBackColor = true;
            // 
            // groupBox_Proxy
            // 
            groupBox_Proxy.Controls.Add(checkBox_ProxyNon);
            groupBox_Proxy.Controls.Add(checkBox_ProxyLocal);
            groupBox_Proxy.Controls.Add(textBox_Proxy);
            groupBox_Proxy.Location = new Point(6, 88);
            groupBox_Proxy.Name = "groupBox_Proxy";
            groupBox_Proxy.Size = new Size(282, 506);
            groupBox_Proxy.TabIndex = 1;
            groupBox_Proxy.TabStop = false;
            groupBox_Proxy.Text = "Proxy";
            // 
            // checkBox_ProxyNon
            // 
            checkBox_ProxyNon.AutoSize = true;
            checkBox_ProxyNon.Location = new Point(113, 21);
            checkBox_ProxyNon.Name = "checkBox_ProxyNon";
            checkBox_ProxyNon.Size = new Size(90, 19);
            checkBox_ProxyNon.TabIndex = 4;
            checkBox_ProxyNon.Text = "Never Proxy";
            checkBox_ProxyNon.UseVisualStyleBackColor = true;
            // 
            // checkBox_ProxyLocal
            // 
            checkBox_ProxyLocal.AutoSize = true;
            checkBox_ProxyLocal.Location = new Point(7, 21);
            checkBox_ProxyLocal.Name = "checkBox_ProxyLocal";
            checkBox_ProxyLocal.Size = new Size(100, 19);
            checkBox_ProxyLocal.TabIndex = 3;
            checkBox_ProxyLocal.Text = "Proxy IP:PORT";
            checkBox_ProxyLocal.UseVisualStyleBackColor = true;
            // 
            // textBox_Proxy
            // 
            textBox_Proxy.Location = new Point(4, 46);
            textBox_Proxy.Multiline = true;
            textBox_Proxy.Name = "textBox_Proxy";
            textBox_Proxy.PlaceholderText = "IP:PORT:USER:PASS";
            textBox_Proxy.Size = new Size(276, 460);
            textBox_Proxy.TabIndex = 2;
            // 
            // groupBox_Input
            // 
            groupBox_Input.BackColor = Color.Transparent;
            groupBox_Input.Controls.Add(checkBox_InputUrl);
            groupBox_Input.Controls.Add(checkBox_InputLocalFile);
            groupBox_Input.Controls.Add(textBox_input);
            groupBox_Input.Controls.Add(button_InputFile);
            groupBox_Input.Location = new Point(6, 6);
            groupBox_Input.Name = "groupBox_Input";
            groupBox_Input.Size = new Size(238, 76);
            groupBox_Input.TabIndex = 0;
            groupBox_Input.TabStop = false;
            groupBox_Input.Text = "Input";
            // 
            // checkBox_InputUrl
            // 
            checkBox_InputUrl.AutoSize = true;
            checkBox_InputUrl.Location = new Point(6, 50);
            checkBox_InputUrl.Name = "checkBox_InputUrl";
            checkBox_InputUrl.Size = new Size(99, 19);
            checkBox_InputUrl.TabIndex = 2;
            checkBox_InputUrl.Text = "Input Website";
            checkBox_InputUrl.UseVisualStyleBackColor = true;
            // 
            // checkBox_InputLocalFile
            // 
            checkBox_InputLocalFile.AutoSize = true;
            checkBox_InputLocalFile.Location = new Point(6, 25);
            checkBox_InputLocalFile.Name = "checkBox_InputLocalFile";
            checkBox_InputLocalFile.Size = new Size(106, 19);
            checkBox_InputLocalFile.TabIndex = 0;
            checkBox_InputLocalFile.Text = "Input File Local";
            checkBox_InputLocalFile.UseVisualStyleBackColor = true;
            // 
            // textBox_input
            // 
            textBox_input.Location = new Point(121, 48);
            textBox_input.Name = "textBox_input";
            textBox_input.PlaceholderText = "Url Buy Account";
            textBox_input.Size = new Size(100, 23);
            textBox_input.TabIndex = 1;
            // 
            // button_InputFile
            // 
            button_InputFile.Location = new Point(120, 22);
            button_InputFile.Name = "button_InputFile";
            button_InputFile.Size = new Size(101, 23);
            button_InputFile.TabIndex = 1;
            button_InputFile.Text = "Input File";
            button_InputFile.UseVisualStyleBackColor = true;
            // 
            // button_outputphone
            // 
            button_outputphone.Location = new Point(1027, 71);
            button_outputphone.Name = "button_outputphone";
            button_outputphone.Size = new Size(86, 23);
            button_outputphone.TabIndex = 33;
            button_outputphone.Text = "filter_phone";
            button_outputphone.UseVisualStyleBackColor = true;
            button_outputphone.Click += button_outputphone_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1276, 623);
            Controls.Add(TabControl1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "VFarmTool HotMail V0";
            Load += MainForm_Load;
            TabControl1.ResumeLayout(false);
            Start.ResumeLayout(false);
            Start.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_NumberThread).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_Start).EndInit();
            Setting.ResumeLayout(false);
            groupBox_Captcha.ResumeLayout(false);
            groupBox_Captcha.PerformLayout();
            groupBox_SettingChrome.ResumeLayout(false);
            groupBox_SettingChrome.PerformLayout();
            groupBox_Proxy.ResumeLayout(false);
            groupBox_Proxy.PerformLayout();
            groupBox_Input.ResumeLayout(false);
            groupBox_Input.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl TabControl1;
        private TabPage Start;
        private TabPage Setting;
        private Label label_NumberThread;
        private Button Button_Close_Chrome;
        private Button Button_Close_ChromeDrive;
        private Button Button_Update_ChromeDrive;
        private Label label_ChromeSize;
        private GroupBox groupBox_Input;
        private Button button_InputFile;
        private TextBox textBox_input;
        private GroupBox groupBox_Proxy;
        private GroupBox groupBox_SettingChrome;
        private GroupBox groupBox_Captcha;
        private Label label_TypeCapcha;
        private Label label_ApiCaptcha;
        private Button button_CaptchaBalance;
        private Label label_NumberProcess;
        private Label label_ChromeArrange;
        private Label label_Browser;
        private Label label_code;
        private DataGridView dataGridView_Start;
        private DataGridViewTextBoxColumn STT;
        private DataGridViewTextBoxColumn Email;
        private DataGridViewTextBoxColumn Password;
        private DataGridViewTextBoxColumn EmailRecovery;
        private DataGridViewTextBoxColumn Phone;
        private DataGridViewTextBoxColumn Proxy;
        private DataGridViewTextBoxColumn Status;
        private Label label_Script;
        private Label label_UrlGetOtp;
        private Button button_Proxy;
        private Button button_FileEmail;
        public Button button_Start;
        public Button button_Stop;
        public Button button_DeleteTable;
        public Button button_OutputSuscces;
        public Button button_OutputFail;
        public TextBox textBox_ChromeArrange;
        public ComboBox comboBox_Browser;
        public NumericUpDown numericUpDown_NumberThread;
        public ComboBox comboBox_Script;
        public TextBox textBox_Process;
        public TextBox textBox_Proxy;
        public CheckBox checkBox_InputUrl;
        public CheckBox checkBox_InputLocalFile;
        public CheckBox checkBox_HideChrome;
        public CheckBox checkBox_ProxyNon;
        public CheckBox checkBox_ProxyLocal;
        public ComboBox comboBox1;
        public TextBox textBox_ApiCaptcha;
        public CheckBox checkBox_Anonymous;
        public CheckBox checkBox_DisableGpu;
        public ComboBox comboBox_Code;
        public TextBox textBox_UrlGetOtp;
        public TextBox textBox_ChromeSize;
        private Button button_InputPhone;
        private Button button_outputphone;
    }
}