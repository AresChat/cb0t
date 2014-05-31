using Awesomium.Core;
using cb0t.Scripting;
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

                    case "scribble.image":
                        return ResourceResponse.Create(Path.Combine(Settings.ScribblePath, request.Url.LocalPath.Substring(1)));

                    default:
                        String[] parts = request.Url.Host.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                            if (parts[1] == "script")
                            {
                                String script_name = parts[1] + ".js";
                                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == script_name);

                                if (script != null)
                                    return ResourceResponse.Create(Path.Combine(script.DataPath, request.Url.LocalPath.Substring(1)));
                            }

                        break;
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
