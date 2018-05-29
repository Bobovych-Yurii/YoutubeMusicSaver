from pytube import YouTube
import sys
import  threading
import re
from pytube.helpers import safe_filename
import os
import moviepy.editor as mp
gFilename = "";
gPath = "";

def downloaded(stream, file_handle):
    clip = mp.VideoFileClip(gPath+gFilename +"."+ stream.subtype).subclip()
    clip.audio.write_audiofile(gPath+gFilename + ".mp3", verbose=False, progress_bar=False)
    print(gFilename+".mp3")

def downloadAudio(link,path=sys.argv[2]):
    global gFilename
    global gPath
    gPath = path;

    yt = YouTube(link)
    charset = 'ascii'
    gFilename = yt.title.encode(charset, "replace").decode(charset).replace('?', ' ');
    gFilename = safe_filename(gFilename);
    yt.register_on_complete_callback(downloaded);

    if yt.streams.filter(progressive=True)!= None:
        yt.streams.filter(progressive=True).last().download(path, gFilename);
    else: print("error")

downloadAudio(sys.argv[1])

