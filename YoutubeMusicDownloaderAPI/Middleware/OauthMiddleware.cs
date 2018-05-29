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
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Linq;

namespace YoutubeMusicDownloaderAPI.Middlewares
{
    
    public class UserData {
        public UserCredential credential;
        public string folderId;
    }

    public class OauthMIddleware:IOauthMIddleware
    {
        static string ApplicationName = "YoutubeMusicDownloaderAPI";
        private static string[] Scopes = {DriveService.Scope.Drive,Google.Apis.Oauth2.v2.Oauth2Service.Scope.UserinfoProfile,Google.Apis.Oauth2.v2.Oauth2Service.Scope.UserinfoEmail};
        private static Dictionary<string,UserData> creditinalStorage = new Dictionary<string, UserData>();
        public async Task<string> Auth() {
        UserCredential tempCreditinal = await getCredential();
       
        var oauthSerivce =
            new Google.Apis.Oauth2.v2.Oauth2Service(new BaseClientService.Initializer {HttpClientInitializer = tempCreditinal,
            ApplicationName = ApplicationName});
        var UserInfo = await oauthSerivce.Userinfo.Get().ExecuteAsync();
        var userData = new UserData();
        if(!creditinalStorage.ContainsKey(UserInfo.Email)){
            userData.credential = tempCreditinal;
            creditinalStorage.Add(UserInfo.Email,userData);
        } else {
            creditinalStorage[UserInfo.Email].credential = tempCreditinal;
        }
        return UserInfo.Email;
        
        }
        public async Task<string> GetFolderID(string email,string folderName){
            var credential = creditinalStorage[email].credential;
            var service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});                 
            var filemetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            
            string pageToken = null;
            var request = service.Files.List();

            request.Q = "mimeType='application/vnd.google-apps.folder' and name='"+folderName+"'";
            request.Spaces = "drive";
            request.Fields = "nextPageToken, files(id, name)";
            request.PageToken = pageToken;
            var result = request.Execute();
            return  result.Files.FirstOrDefault() != null ? result.Files.FirstOrDefault().Id: "";
        }
        public async Task<string> CreateForlder(string email,string folderName){
            var credential = creditinalStorage[email].credential;
            var service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});                 
            var filemetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            var request = service.Files.Create(filemetadata);
            request.Fields = "id";
            var file = request.Execute();
            
            Console.WriteLine("file loaded:" + file.Id);
            return file.Id;
        }
        public async void Upload(string email,string audioName,string folderId){
            var credential = creditinalStorage[email].credential;
            var service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
			fileMetadata.Name = audioName;
			fileMetadata.Description = audioName;
            fileMetadata.Parents = new List<string>{ folderId};
			fileMetadata.MimeType = GetMimeType(getPathToFile(audioName));	 
            Console.WriteLine(fileMetadata.MimeType+"metadata");
            //FilesResource.CreateMediaUpload request;   
			using (var stream = new System.IO.FileStream(getPathToFile(audioName), System.IO.FileMode.Open,System.IO.FileAccess.Read))
			{	
			    var request = service.Files.Create(
					fileMetadata, stream, fileMetadata.MimeType);
				request.Fields = "id";
                var error = request.Upload();
                Console.WriteLine(error+"error");
                var fileres = request.ResponseBody;
                Console.WriteLine("uploaded");
			    Console.WriteLine("file loaded:" + fileres.Id);
			}
			
        }

        public async Task<bool> SingOut(UserCredential credential){
            using(var cts = new CancellationTokenSource()){
                await  credential.RevokeTokenAsync(cts.Token);
            }
            return true;
        }

        public async Task<bool> SingOut(string userEmail){
            var credential =  creditinalStorage[userEmail];
            using(var cts = new CancellationTokenSource()){
                await credential.credential.RevokeTokenAsync(cts.Token);
            }	
            return true;
        }
        private string getPathToFile(string audioName){
            Console.WriteLine(audioName+"getpath");
            Console.WriteLine(Directory.GetCurrentDirectory()+"\\download\\"+audioName);
            return Directory.GetCurrentDirectory()+"\\download\\"+audioName;
        }
        private string GetMimeType(string fileName){
			string mimeType = "application/unknown";
			string ext = System.IO.Path.GetExtension(fileName).ToLower();
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (regKey != null && regKey.GetValue("Content Type") != null)
				mimeType = regKey.GetValue("Content Type").ToString();
			return mimeType;

		}
        private async Task<UserCredential> getCredential(){
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read)){
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
			    GoogleClientSecrets.Load(stream).Secrets,
			    Scopes,
			    "user", CancellationToken.None);	
                }
            return credential;	
        }
    }
}