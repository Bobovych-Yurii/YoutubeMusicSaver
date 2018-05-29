using System.Threading.Tasks;
namespace YoutubeMusicDownloaderAPI.Middlewares
{    
    public interface IDownloadMIddleware
    {  
        void Dowload(string url,string email,string folderId,string fromPath="");
        
    } 
}