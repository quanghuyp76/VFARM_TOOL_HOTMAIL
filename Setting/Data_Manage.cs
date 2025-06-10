using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OtpNet;
using SeleniumExtras.WaitHelpers;
using System.Text;
using System.Text.RegularExpressions;
using X_Vfarme.GPM;

namespace X_Vframe_Tool
{
    public static class Utils
    {
        public static readonly HttpClient SharedHttpClient = new HttpClient();
    }

    public class GETOTPVFARM
    {
        public static async Task<string?> GetOTPFarmAsync(string url, string phone)
        {
            try
            {
                var responseMessage = await Utils.SharedHttpClient.GetAsync($"{url}{phone}");

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await responseMessage.Content.ReadAsStringAsync();

                string? otp = Regex.Match(content, "\"otp\":\"(.*?)\"").Groups[1].Value;
                string? message = Regex.Match(content, "\"message\":\"(.*?)\"").Groups[1].Value;

                if (!string.IsNullOrEmpty(otp))
                {
                    return otp;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine($"Message from server: {message}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching OTP: {ex.Message}");
                return null;
            }
        }
    }

    public static class EmailFilter
    {
        public static void FilterEmails(string dataFile, string searchFile, string outputFile)
        {
            // Bước 1: Đọc tất cả các dòng của file tìm kiếm và lấy danh sách email (giả sử email là phần tử đầu tiên)
            HashSet<string> searchEmails = new HashSet<string>(
                File.ReadAllLines(searchFile)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split('|')[0].Trim())
            );

            // Bước 2: Đọc file dữ liệu cần lọc, và loại bỏ các dòng mà email xuất hiện trong searchEmails
            var filteredLines = File.ReadAllLines(dataFile)
                .Where(line =>
                {
                    if (string.IsNullOrWhiteSpace(line))
                        return false;
                    // Tách dòng theo ký tự '|' và lấy phần tử đầu tiên làm email
                    string email = line.Split('|')[0].Trim();
                    // Nếu email không có trong danh sách tìm kiếm thì giữ lại dòng đó
                    return !searchEmails.Contains(email);
                })
                .ToList();

            // Bước 3: Ghi kết quả đã lọc vào file kết quả
            File.WriteAllLines(outputFile, filteredLines);
        }

        public static void FilterPhone(string dataFile, string searchFile, string outputFile)
        {
            // Bước 1: Đọc tất cả các số điện thoại đã dùng
            HashSet<string> usedPhones = new HashSet<string>(
                File.ReadAllLines(searchFile)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split('|')[0].Trim())  // Nếu searchFile có dạng phone|otp
            );

            // Bước 2: Lọc bỏ những số đã dùng khỏi dataFile
            var filteredLines = File.ReadAllLines(dataFile)
                .Where(line =>
                {
                    if (string.IsNullOrWhiteSpace(line))
                        return false;
                    string phone = line.Trim();  // Nếu dataFile chỉ chứa phone
                    return !usedPhones.Contains(phone);
                })
                .ToList();

            // Bước 3: Ghi lại những số chưa dùng vào file mới
            File.WriteAllLines(outputFile, filteredLines);
        }


    }

    public class ProxyManager
    {
        public static List<string> LoadProxies(string filePath)
        {
            List<string> proxyList = new List<string>();

            if (File.Exists(filePath))
            {
                proxyList = new List<string>(File.ReadAllLines(filePath)); // Đọc tất cả proxy từ file
            }
            else
            {
                Console.WriteLine("File proxy.txt không tồn tại!");
            }

            return proxyList;
        }
    }

    public class OtpResponse
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string otp { get; set; }
    }

    public class OtpHelper
    {


        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetOtp(string apiUrl, int timeoutSeconds)
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var token = cancellationTokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    HttpResponseMessage response_otp = await client.GetAsync(apiUrl, token);
                    response_otp.EnsureSuccessStatusCode();

                    string json_otp = await response_otp.Content.ReadAsStringAsync();
                    var result_otp = JsonConvert.DeserializeObject<OtpResponse>(json_otp);

                    if (result_otp.ErrorCode == 0 && !string.IsNullOrEmpty(result_otp.otp))
                    {
                        return result_otp.otp;
                    }

                    await Task.Delay(3000, token); // Chờ 3 giây rồi thử lại
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("⏳ Timeout sau 30 giây, không có OTP.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi gọi API: {ex.Message}");
            }

            return null;
        }

        public static async Task<string> GetOtpTest(string apiUrl, int timeoutSeconds)
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var token = cancellationTokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    HttpResponseMessage response_otp = await client.GetAsync(apiUrl, token);
                    response_otp.EnsureSuccessStatusCode();

                    string responseBody = await response_otp.Content.ReadAsStringAsync();
                    Match match = Regex.Match(responseBody, @"\""message\""\s*:\s*\"".*?(\d{4,8}).*?\""");

                    if (match.Success)
                    {
                        string otp = match.Groups[1].Value;
                        return otp;
                    }

                    await Task.Delay(3000, token); // Chờ 3 giây rồi thử lại
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("⏳ Timeout sau 30 giây, không có OTP.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi gọi API: {ex.Message}");
            }

            return null;
        }

        public static async Task<string> GETOTPSMV(string email, int timeoutSeconds)
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var token = cancellationTokenSource.Token;
            try
            {
                string? apiUrl = "https://www.smvmail.com/api/email?page=1&q=&email=" + email;
                while (!token.IsCancellationRequested)
                {
                    HttpResponseMessage response_otp = await client.GetAsync(apiUrl, token);
                    response_otp.EnsureSuccessStatusCode();
                    string responseBody = await response_otp.Content.ReadAsStringAsync();
                    string? otp = Regex.Match(responseBody, @"Security code: (\d{6})").Groups[1].Value;
                    if (!string.IsNullOrEmpty(otp))
                    {
                        Console.WriteLine($"✅ OTP tìm thấy: {otp}");
                        return otp;
                    }
                    await Task.Delay(3000, token); // Chờ 3 giây rồi thử lại
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("⏳ Timeout sau 30 giây, không có OTP.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi gọi API: {ex.Message}");
            }

            return null;
        }

        public static string GenerateOTP(string secretKey)
        {
            // Chuyển khóa từ base32 sang byte[]
            byte[] secretBytes = Base32Encoding.ToBytes(secretKey);

            // Tạo đối tượng TOTP (mã theo thời gian)
            Totp totp = new Totp(secretBytes);

            // Sinh mã OTP hiện tại
            return totp.ComputeTotp(); // Ví dụ: "123456"
        }


    }

    public class SeleniumHelper
    {
        public static IWebElement WaitForElement(IWebDriver driver, By by, int timeoutInSeconds, int pollingIntervalInMillis = 100)
        {
            try
            {
                // Tạo FluentWait với driver
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver)
                {
                    Timeout = TimeSpan.FromMilliseconds(timeoutInSeconds),                      // Thời gian chờ tối đa
                    PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalInMillis)   // Kiểm tra lại sau mỗi khoảng thời gian
                };

                // Bỏ qua các ngoại lệ có thể xảy ra khi chưa tìm thấy phần tử
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(StaleElementReferenceException));

                // Chờ cho đến khi phần tử xuất hiện và có thể thao tác
                return fluentWait.Until(drv =>
                {
                    try
                    {
                        IWebElement element = drv.FindElement(by);
                        return element.Displayed && element.Enabled ? element : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null; // Nếu DOM thay đổi, thử lại
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Lỗi không mong muốn khi tìm phần tử {by}: {ex.Message}");
                        return null;
                    }
                });
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"❌ Không tìm thấy phần tử: {by}");
                return null;  // Hoặc có thể throw exception nếu muốn bắt lỗi
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi không mong muốn khi tìm phần tử {by}: {ex.Message}");
                return null;
            }
        }

        public static void WaitForPageLoad(IWebDriver driver, int timeoutInSeconds = 10)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
        }


    }

    public class Profile_GPM
    {
        private static Random random = new Random();
        private static readonly HttpClient client = new HttpClient();
        private const string ApiGPM_Create = "http://127.0.0.1:19995/api/v3/profiles/create";
        //private const string proxyFilePath =""
        //List<string> proxyList = new List<string>();
        //public var proxyList = new List<string>(File.ReadAllLines(proxyFilePath));
        public static async Task<string?> CreateProfile(string profileName)
        {
            string proxyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "input", "Proxy.txt");
            List<string> proxies = ProxyManager.LoadProxies(proxyFilePath);
            if (proxies.Count == 0)
            {
                Console.WriteLine("Proxy list is empty.");
                return null;
            }
            string selectedProxy = proxies[random.Next(proxies.Count)];
            selectedProxy = selectedProxy.Replace("\\", "\\\\")
                                         .Replace("\n", "")
                                         .Replace("\r", "")
                                         .Replace("\t", "")
                                         .Trim();

            var data = new
            {
                profile_name = profileName,
                //browser_version = "124.0.6367.29",
                //raw_proxy = selectedProxy,
                is_noise_canvas = true,
                is_noise_webgl = true,
                is_noise_client_rect = true,
                is_noise_audio_context = true,
                is_random_screen = true,
                is_masked_webgl_data = true,
                is_masked_media_device = true,
                is_random_os = true,


            };

            string json = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(ApiGPM_Create, content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
            string responseString = await response.Content.ReadAsStringAsync();
            try
            {  
                JObject jsonResponse = JObject.Parse(responseString);
                string? profileId = jsonResponse["data"]?["id"]?.ToString();
                return profileId;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                Console.WriteLine($"Response String: {responseString}");
                return null;
            }
        }

        public async Task<IWebDriver> OpenProfile(string apiUrl)
        {
            // Bước 1: Gọi API mở profile
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ProfileInfo_GPM>>(json);
            if (apiResponse == null || !apiResponse.success || apiResponse.data == null)
            {
                Console.WriteLine("❌ API mở profile thất bại.");
                return null; // Trả về null nếu không thành công
            }

            ProfileInfo_GPM profileInfo = apiResponse.data;

            // Bước 2: Cấu hình ChromeOptions để kết nối đến Chrome instance đang chạy (theo remote_debugging_port)
            ChromeOptions options = new ChromeOptions();
            // Sử dụng remote debugging port mà API trả về
            options.DebuggerAddress = profileInfo.remote_debugging_address;
            string driverDirectory = System.IO.Path.GetDirectoryName(profileInfo.driver_path);
            string driverExecutable = System.IO.Path.GetFileName(profileInfo.driver_path);
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverDirectory, driverExecutable);
            service.HideCommandPromptWindow = true; // Ẩn cửa sổ console
            service.SuppressInitialDiagnosticInformation = true; // Ẩn log khởi động
            // Tạo đối tượng WebDriver với options đã cấu hình
            IWebDriver driver = new ChromeDriver(service, options);
            return driver;
        }
        public async Task<bool> CloseProfile(string profileId)
        {
            string ApiGPM_CloseProfile = $"http://127.0.0.1:19995/api/v3/profiles/close/{profileId}";
            try
            {
                HttpResponseMessage response1 = await client.GetAsync(ApiGPM_CloseProfile);
                string responseBody = await response1.Content.ReadAsStringAsync();
                response1.EnsureSuccessStatusCode();
                if (!response1.IsSuccessStatusCode)
                {
                    return false;
                }
                else
                {
                    var json = JObject.Parse(responseBody);
                    return json["success"]?.ToObject<bool>() == true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Delete_Profile(string profileId)
        {
            string ApiGPM_DeleteProfile = $"http://127.0.0.1:19995/api/v3/profiles/delete/{profileId}?mode=2";
            try
            {
                HttpResponseMessage respons2 = await client.GetAsync(ApiGPM_DeleteProfile);
                string responseBody = await respons2.Content.ReadAsStringAsync();
                respons2.EnsureSuccessStatusCode();
                if (!respons2.IsSuccessStatusCode)
                {
                    return false;
                }
                var json = JObject.Parse(responseBody);
                return json["success"]?.ToObject<bool>() == true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> UpdateProxy(string ProfileID)
        {
            string proxyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "input", "Proxy.txt");
            List<string> proxies = ProxyManager.LoadProxies(proxyFilePath);

            if (proxies.Count == 0)
            {
                Console.WriteLine("Proxy list is empty.");
                return false;
            }

            string selectedProxy = proxies[random.Next(proxies.Count)];
            selectedProxy = selectedProxy.Replace("\\", "\\\\")
                                         .Replace("\n", "")
                                         .Replace("\r", "")
                                         .Replace("\t", "")
                                         .Trim();

            string UrlUpdateGPM = $"http://127.0.0.1:19995/api/v3/profiles/update/{ProfileID}";

            var data = new
            {
                profile_name = "change profile",
                raw_proxy = selectedProxy
            };

            string json = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(UrlUpdateGPM, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"Response Body: {responseBody}");
                    return false;
                }

                try
                {
                    var json1 = JObject.Parse(responseBody);
                    bool success = json1["success"]?.ToObject<bool>() == true;
                    return success;
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"JSON Parse Error: {ex.Message}");
                    Console.WriteLine($"Response Body: {responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during HTTP request: {ex.Message}");
                return false;
            }
        }

    }
}