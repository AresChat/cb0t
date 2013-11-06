using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMPLib;

namespace cb0t
{
    class AudioHelpers
    {
        public static AudioPlayerItem CreateAudioPlayerItem(WindowsMediaPlayer wmp, String path)
        {
            AudioPlayerItem item = new AudioPlayerItem();
            
            try
            {
                IWMPMedia media = wmp.newMedia(path);

                if (media.duration > 0)
                {
                    item.Duration = (int)media.duration;
                    item.SetDurationText(item.Duration);
                    item.Title = media.name.Trim();
                    item.Path = path;

                    for (int i = 0; i < media.attributeCount; i++)
                    {
                        switch (media.getAttributeName(i).ToUpper())
                        {
                            case "ALBUMIDALBUMARTIST":
                                String[] strs = media.getItemInfo(media.getAttributeName(i)).Split(new String[] { "*;*" }, StringSplitOptions.None);

                                if (strs.Length > 0)
                                    item.Album = strs[0].Trim();
                                if (strs.Length > 1)
                                    item.Artist = strs[1].Trim();
                                break;

                            case "AUTHOR":
                                item.Author = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "DISPLAYARTIST":
                                if (item.Artist.Length == 0)
                                    item.Artist = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "TITLE":
                                if (item.Title.Length == 0)
                                    item.Title = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "WM/ALBUMARTIST":
                                if (item.Artist.Length == 0)
                                    item.Artist = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "WM/ALBUMTITLE":
                                if (item.Album.Length == 0)
                                    item.Album = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;
                        }
                    }

                    if (String.IsNullOrEmpty(item.Artist))
                        item.Artist = item.Author;
                }
            }
            catch { return null; }

            return item;
        }
    }
}
