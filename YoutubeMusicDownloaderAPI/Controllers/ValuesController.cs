using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Apis.Util.Store;
using System.Threading;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Security.Claims;
using YoutubeMusicDownloaderAPI.Middlewares;
using Microsoft.AspNetCore.Cors;


namespace YoutubeMusicDownloaderAPI.Controllers
{
     [EnableCors("default")]
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        private IDownloadMIddleware download;
        private IOauthMIddleware oauth;
        public ValuesController(IDownloadMIddleware download,IOauthMIddleware oauth){
            this.download = download;
            this.oauth = oauth;
        }
        // GET api/values
        [HttpPost]
        public async  Task<ActionResult> Save([FromForm]string videoUrl)        
        {            
            byte[] temp;
            string folderId = HttpContext.Session.TryGetValue("folderId",out temp) ? System.Text.Encoding.UTF8.GetString(temp) : "";
            string email = getSessionUser();
            Console.WriteLine(email+" download "+videoUrl);
             folderId = await oauth.GetFolderID(getSessionUser(),"YoutubeMusicDownloader");
            if (folderId == "") { folderId = await oauth.CreateForlder(getSessionUser(),"YoutubeMusicDownloader"); }
            
            HttpContext.Session.Set("folderId",System.Text.Encoding.UTF8.GetBytes(folderId));

            if(videoUrl != "")
                download.Dowload(videoUrl,email,folderId,Directory.GetCurrentDirectory()+"\\downlaod");
            return new OkObjectResult("done");
        }

        [HttpGet]
        public ActionResult IsAuth(){
            return new BadRequestResult();
        }
        [HttpGet]   
        public async Task<ActionResult> Test(){
            await oauth.CreateForlder(getSessionUser(),"YoutubeMusicDownloader");
            await oauth.GetFolderID(getSessionUser(),"YoutubeMusicDownloader");
            return new OkResult();
        }
        [HttpGet]
        public ActionResult Auth(){            
            Console.WriteLine(HttpContext.Session.Id.ToString());
            Console.WriteLine(getSessionUser());
            return new OkResult();
        }
        public ActionResult SignOut(){
            oauth.SingOut(getSessionUser());
            return new OkResult();
        }

        private string getSessionUser(){
            byte[] temp;
            return HttpContext.Session.TryGetValue("userId",out temp) ? System.Text.Encoding.UTF8.GetString(temp) : "";
        }
        public class dowloadSchema{
           public string videoUrl;
        }
    }
}
