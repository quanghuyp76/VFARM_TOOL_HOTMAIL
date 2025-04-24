using System;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Text;

namespace X_Vfarme.GPM
{
    public class Create_GPM
    {
        public string? profile_name {get; set;}
        public string? group_name { get; set; }
        public string? browser_core { get; set; }
        public string? browser_name { get; set; }
        public string? browser_version { get; set; }
        public bool? is_random_browser_version { get; set; }
        public string? raw_proxy { get; set; }
        public string? startup_urls { get; set; }
        public bool? is_masked_font { get; set; }
        public bool? is_noise_canvas { get; set; }
        public bool? is_noise_webgl { get; set; }
        public bool? is_noise_client_rect { get; set; }
        public bool? is_noise_audio_context { get; set; }
        public bool? is_random_screen { get; set; }
        public bool? is_masked_webgl_data { get; set; }
        public bool? is_masked_media_device { get; set; }
        public bool? is_random_os { get; set; }
        public string? os { get; set; }
        public int? webrtc_mode { get; set; }
        public string? user_agent { get; set; }
    }



    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public T data { get; set; }
        public string message { get; set; }
    }

    public class ProfileInfo_GPM
    {
        public string profile_id { get; set; }
        public string browser_location { get; set; }
        // Lưu ý: API trả về "remote_debugging_address", nên đặt tên tương ứng:
        public string remote_debugging_address { get; set; }
        public string driver_path { get; set; }
    }

    public class LoginController_GPM
    {
        public static readonly HttpClient httpClient = new HttpClient();
        public async Task<IWebDriver> OpenProfileAndGetDriverAsync(string apiUrl)
        {
            // Bước 1: Gọi API mở profile
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ProfileInfo_GPM>>(json);
            ProfileInfo_GPM profileInfo = apiResponse.data;

            // Bước 2: Cấu hình ChromeOptions để kết nối đến Chrome instance đang chạy (theo remote_debugging_port)
            ChromeOptions options = new ChromeOptions();
            // Sử dụng remote debugging port mà API trả về
            options.DebuggerAddress = profileInfo.remote_debugging_address;
            string driverDirectory = System.IO.Path.GetDirectoryName(profileInfo.driver_path);
            string driverExecutable = System.IO.Path.GetFileName(profileInfo.driver_path);
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverDirectory, driverExecutable);
            // Tạo đối tượng WebDriver với options đã cấu hình
            IWebDriver driver = new ChromeDriver(service, options);
            return driver;
        }
        private static readonly string[] windowsVersions =
        {
            "Windows 10"//, "Windows 8", "Windows 8.1", "Windows 7",
            //"Windows 11", "Windows Server 2012", "Windows Server 2012 R2"
        }; 

    }


}