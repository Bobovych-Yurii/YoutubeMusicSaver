document.addEventListener('DOMContentLoaded', () => {  
    var urlText = document.getElementById('urlText');
    var submitelem = document.getElementById('submitInput');  
    var singOut = document.getElementById('singOut');
      
    chrome.tabs.getSelected(null,function(tab) {
    var tablink = tab.url;
    document.getElementById('urlText').value = tablink.indexOf("youtube.com")
    if(tablink.indexOf("youtube.com") !== -1) {
        var linkStart = tablink.indexOf("v=")+2;
        var linkEnd = tablink.indexOf('&',linkStart) == -1 ? tablink.length : tablink.indexOf('&',linkStart)
      document.getElementById('urlText').value = "https://youtu.be/"+ tablink.substring(linkStart, linkEnd); 
   }
    });
  
    submitelem.addEventListener('click',()=>{     
         $.ajax({
            type: "POST",
            url: "http://localhost:5000/api/Values/save",            
            data: {videoUrl:document.getElementById('urlText').value},
             success: function(data) {
            }
        }); 
    });
    singOut.addEventListener('click',()=>{        
        $.ajax({
            type: "POST",
            url: "http://localhost:5000/api/Values/singOut",            
            success: function(data) {
            }
        });  
    });  
      
      
});
  