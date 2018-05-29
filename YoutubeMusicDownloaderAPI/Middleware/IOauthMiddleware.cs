using System.Threading.Tasks;
namespace YoutubeMusicDownloaderAPI.Middlewares
{    
    public interface IOauthMIddleware
    {  
        Task<string> Auth();
        void Upload(string email,string audioName,string folderId);
        Task<bool> SingOut(string userEmail);
        Task<string> GetFolderID(string email,string folderName);
        Task<string> CreateForlder(string email,string folderName);
    } 
}