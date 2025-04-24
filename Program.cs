using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace X_Vframe_Tool
{
    public interface Script_Sele
    {
            Task Run(int Thread1, CancellationToken token, string win_pos);
    }
    

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}