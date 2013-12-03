using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class ScriptEvents
    {
        public static void OnDisconnected(Room room) { }

        public static void OnConnecting(Room room) { }

        public static void OnConnected(Room room) { }

        public static bool OnRedirecting(Room room, Redirect redirect) { return true; }

        public static bool OnCommand(Room room, String text) { return true; }

        public static bool OnTextReceiving(Room room, String name, String text)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(name);

            if (ai == AutoIgnoreType.All || ai == AutoIgnoreType.Text)
                return false;

            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Text, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Block)
                            return false;
                    }
                }

            return true;
        }

        public static void OnTextReceived(Room room, String name, String text)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Text, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", name).Replace("+t", text), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", name).Replace("+t", text));
                        }
                    }
                }

            if (room.RoomIsVisible)
                if (Settings.GetReg<bool>("can_narrate", false))
                    Narrator.Say(text);                    
        }

        public static bool OnEmoteReceiving(Room room, String name, String text)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(name);

            if (ai == AutoIgnoreType.All || ai == AutoIgnoreType.Text)
                return false;

            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Emote, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Block)
                            return false;
                    }
                }

            return true;
        }

        public static void OnEmoteReceived(Room room, String name, String text)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Emote, name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", name).Replace("+t", text), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", name).Replace("+t", text));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", name).Replace("+t", text));
                        }
                    }
                }

            if (room.RoomIsVisible)
                if (Settings.GetReg<bool>("can_narrate", false))
                    Narrator.Say(text);
        }

        public static bool OnAnnounceReceiving(Room room, String text) { return true; }

        public static void OnAnnounceReceived(Room room, String text) { }

        public static void OnUserlistReceived(Room room) { }

        public static bool OnUserJoining(Room room, User user) { return true; }

        public static void OnUserJoined(Room room, User user)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Join, user.Name, null);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name));
                        }
                    }
                }
        }

        public static bool OnUserParting(Room room, User user) { return true; }

        public static void OnUserParted(Room room, User user)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.Part, user.Name, null);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name));
                        }
                    }
                }
        }

        public static void OnUserLevelChanged(Room room, User user) { }

        public static bool OnUserAvatarReceiving(Room room, User user) { return true; }

        public static bool OnUserMessageReceiving(Room room, User user, String text) { return true; }

        public static bool OnTopicReceiving(Room room, String text) { return true; }

        public static bool OnUrlReceiving(Room room, String text, String address) { return true; }

        public static bool OnUserFontChanging(Room room, User user) { return true; }

        public static void OnUserWritingStatusChanged(Room room, User user) { }

        public static void OnUserOnlineStatusChanged(Room room, User user) { }

        public static bool OnPmReceiving(Room room, User user, String text)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(user.Name);

            if (ai == AutoIgnoreType.All || ai == AutoIgnoreType.PM)
                return false;

            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.PM, user.Name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Block)
                            return false;
                    }
                }

            return true;
        }

        public static void OnPmReceived(Room room, User user, String text)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.PM, user.Name, text);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name).Replace("+t", text), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name).Replace("+t", text));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name).Replace("+t", text));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name).Replace("+t", text));
                        }
                    }
                }
        }

        public static void OnPmFirstReceived(Room room, User user)
        {
            if (Settings.GetReg<bool>("filter_on", false))
                if (user.Name != room.MyName)
                {
                    FilterResult[] filters = Filter.GetFilters(room.Credentials.Name, FilterEvent.PMFirst, user.Name, null);

                    foreach (FilterResult f in filters)
                    {
                        if (f.Task == FilterTask.Notify)
                            room.ShowPopup("cb0t :: " + StringTemplate.Get(STType.Messages, 2), f.Text.Replace("+n", user.Name), PopupSound.Notify);
                        else if (f.Task == FilterTask.Send)
                        {
                            if (f.Text.StartsWith("/me ") && f.Text.Length > 4)
                                room.SendEmote(f.Text.Substring(4).Replace("+n", user.Name));
                            else if (f.Text.StartsWith("/") && f.Text.Length > 1)
                                room.SendCommand(f.Text.Substring(1).Replace("+n", user.Name));
                            else if (f.Text.Length > 0)
                                room.SendText(f.Text.Replace("+n", user.Name));
                        }
                    }
                }
        }

        public static bool OnNudgeReceiving(Room room, User user)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(user.Name);

            if (ai == AutoIgnoreType.All)
                return false;

            return true;
        }

        public static bool OnScribbleReceiving(Room room, User user)
        {
            AutoIgnoreType ai = AutoIgnores.IgnoreType(user.Name);

            if (ai == AutoIgnoreType.All)
                return false;

            return true;
        }

        public static void OnScribbleReceived(Room room, User user) { }

        public static void OnSongChanged(Room room, String song)
        {
            if (Settings.GetReg<bool>("show_song", true))
            {
                if (!String.IsNullOrEmpty(song))
                    room.SendPersonalMessage("\x0007" + song);
                else
                    room.SendPersonalMessage();
            }
        }

        public static void OnTimer(Room room) { }

        public static bool OnVoiceClipReceiving(Room room, User user) { return true; }

        public static void OnVoiceClipReceived(Room room, User user) { }

        public static void OnCustomDataReceived(Room room, User user, String ident, String data) { }
    }
}
