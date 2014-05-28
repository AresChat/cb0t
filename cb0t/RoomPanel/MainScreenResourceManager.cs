using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class MainScreenResourceManager : IResourceInterceptor
    {
        public ResourceResponse OnRequest(ResourceRequest request)
        {
            String path = Path.Combine(Settings.DataPath, "log2.txt");

            if (!String.IsNullOrEmpty(request.Url.Host))
            {
                switch (request.Url.Host)
                {
                    case "emotic.org":
                        return ResourceResponse.Create(Path.Combine(Settings.AniEmoticPath, "org", request.Url.LocalPath.Substring(1)));

                    case "emotic.ext":
                        return ResourceResponse.Create(Path.Combine(Settings.AniEmoticPath, "ext", request.Url.LocalPath.Substring(1)));

                    case "emotic.ui":
                        return ResourceResponse.Create(Path.Combine(Settings.AniEmoticPath, "ui", request.Url.LocalPath.Substring(1)));
                }
            }

            return null;
        }

        public bool OnFilterNavigation(NavigationRequest request)
        {
            if (!String.IsNullOrEmpty(request.Url.ToString()))
                if (RoomPool.Rooms != null)
                {
                    int id = request.ViewId;
                    Room r = RoomPool.Rooms.Find(x => x.Panel.ViewID == id);

                    if (r != null)
                    {
                        r.Panel.ProcessLinkClicked(request.Url.ToString());
                        return true;
                    }
                }

            return false;
        }
    }
}
