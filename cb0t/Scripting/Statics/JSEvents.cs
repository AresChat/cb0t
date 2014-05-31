using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "Events")]
    class JSEvents : ObjectInstance
    {
        public JSEvents(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Events"; }
        }

        [JSFunction(Name = "onannouncereceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnAnnounceReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONANNOUNCERECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onannouncereceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnAnnounceReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONANNOUNCERECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "oncommandsending", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnCommandSending(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONCOMMANDSENDING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onconnected", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnConnected(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONCONNECTED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onconnecting", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnConnecting(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONCONNECTING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "oncustomdatareceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnCustomDataReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONCUSTOMDATARECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "ondisconnected", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnDisconnected(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONDISCONNECTED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onemotereceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnEmoteReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONEMOTERECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onemotereceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnEmoteReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONEMOTERECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onemotesending", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnEmoteSending(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONEMOTESENDING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onerror", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnError(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONERROR = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onhtmlreceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnHTMLReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONHTMLRECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onlinkclicked", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnLinkedClick(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONLINKCLICKED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onload", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnLoad(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONLOAD = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onnudgereceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnNudgeReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONNUDGERECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onpmreceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnPmReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONPMRECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onpmreceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnPmReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONPMRECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onpmsending", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnPmSending(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONPMSENDING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onredirecting", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnRedirecting(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONREDIRECTING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onscribblereceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnScribbleReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONSCRIBBLERECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onscribblereceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnScribbleReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONSCRIBBLERECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onsongchanged", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnSongChanged(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONSONGCHANGED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "ontextsending", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnTextSending(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONTEXTSENDING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "ontextreceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnTextReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONTEXTRECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "ontextreceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnTextReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONTEXTRECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "ontimer", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnTimer(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONTIMER = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "ontopicreceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnTopicReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONTOPICRECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onurlreceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUrlReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONURLRECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuseravatarreceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserAvatarReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERAVATARRECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserfontchanging", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserFontChanging(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERFONTCHANGING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserjoined", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserJoined(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERJOINED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserjoining", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserJoining(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERJOINING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserlevelchanged", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserLevelChanged(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERLEVELCHANGED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserlistreceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserListReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERLISTRECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onusermessagereceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserMessageReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERMESSAGERECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuseronlinestatuschanged", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserOnlineStatusChanged(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERONLINESTATUSCHANGED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserparted", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserParted(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERPARTED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserparting", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserParting(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERPARTING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuserwritingstatuschanged", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUserWritingStatusChanged(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUSERWRITINGSTATUSCHANGED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onvoiceclipreceived", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnVoiceClipReceived(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONVOICECLIPRECEIVED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onvoiceclipreceiving", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnVoiceClipReceiving(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONVOICECLIPRECEIVING = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onuiselected", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnUISelected(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONUISELECTED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onroomopened", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnRoomOpened(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONROOMOPENED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "onroomclosed", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool EventOnRoomClosed(ScriptEngine eng, object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    script.EVENT_ONROOMCLOSED = (UserDefinedFunction)a;
                    return true;
                }
            }

            return false;
        }
    }
}
