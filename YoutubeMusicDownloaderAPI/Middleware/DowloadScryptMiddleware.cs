using System;
using System.Diagnostics;
using System.IO;      

namespace YoutubeMusicDownloaderAPI.Middlewares
{
    public class DowloadScryptMIddleware:IDownloadMIddleware
    {
        private IOauthMIddleware oauth;
        public DowloadScryptMIddleware(IOauthMIddleware oauth){
            this.oauth = oauth;
        }
        public void Dowload(string url,string email,string folderId,string fromPath="") { 
            ProcessStartInfo start = new ProcessStartInfo("python",Directory.GetCurrentDirectory()+"\\test.py "+url+" "
                                                            + Directory.GetCurrentDirectory()+"\\download\\");//" https://youtu.be/-gciLRKnVYk"
            Console.WriteLine("python"+" "+Directory.GetCurrentDirectory()+"\\test.py "+url+" "
                                                            + Directory.GetCurrentDirectory()+"\\download");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            using( Process process = Process.Start(start))
            {
            process.OutputDataReceived += (object sender,DataReceivedEventArgs e) => {
                if(e?.Data?.Length > 1 )
                    oauth.Upload(email,e.Data,folderId);
                Console.WriteLine(e.Data+"edata");
            };
            process.ErrorDataReceived += (object sender,DataReceivedEventArgs e) => {
                Console.WriteLine(e.Data+"edataError"); 
            };
            process.BeginOutputReadLine();
            process.WaitForExit();
            }
        }
    }
}