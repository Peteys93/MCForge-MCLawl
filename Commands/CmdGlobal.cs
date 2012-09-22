using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MCForge.Commands
{
    class CmdGlobal : Command
    {
        public override string name { get { return "global"; } }
        public override string shortcut { get { return "gc"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdGlobal() { }
        //bla
        public override void Use(Player p, string message)
        {
            if (String.IsNullOrEmpty(message)) { 
                p.InGlobalChat = !p.InGlobalChat;
                Player.SendMessage(p, p.InGlobalChat ? "%aGlobal Chat enabled" : "%cGlobal Chat Disabled");
                return;
             }

            if (!Server.UseGlobalChat) { Player.SendMessage(p, "Global Chat is disabled."); return; }
            if (p != null && p.muted) { Player.SendMessage(p, "You are muted."); return; }
            if (p != null && p.muteGlobal) { Player.SendMessage(p, "You cannot use Global Chat while you have it muted."); return; }
            if (p != null && !Server.gcaccepted.Contains(p.name.ToLower())) { RulesMethod(p); return; }
            if (p != null)
            {
                foreach (string line in Server.gcnamebans)
                {
                    if (line.Split('|')[0] == p.name)
                    {
                        Player.SendMessage(p, "You have been banned from the global chat by " + line.Split('|')[2] + " because of the following reason: " + line.Split('|')[1] + ". You can apply a ban appeal at www.mcforge.net. Keep yourself to the rules.");
                        return;
                    }
                }
                foreach (string line in Server.gcipbans)
                {
                    if (line.Split('|')[0] == p.ip)
                    {
                        Player.SendMessage(p, "You have been ip banned from the global chat by " + line.Split('|')[2] + " because of the following reason: " + line.Split('|')[1] + ". You can apply a ban appeal at www.mcforge.net. Keep yourself to the rules.");
                        return;
                    }
                }
            }            //Server.GlobalChat.Say((p != null ? p.name + ": " : "Console: ") + message, p);
            Server.GlobalChat.Say(p == null ? "Console: " + message : p.name + ": " + message, p);
            Player.GlobalMessage(Server.GlobalChatColor + "<[Global] " + (p != null ? p.name + ": " : "Console: ") + "&f" + (Server.profanityFilter ? ProfanityFilter.Parse(message) : message), true);

        }
        public void RulesMethod(Player p)
        {
            Player.SendMessage(p, "&cBy using the Global Chat you agree to the following rules:");
            Player.SendMessage(p, "1. No Spamming");
            Player.SendMessage(p, "2. No Advertising (Trying to get people to your server)");
            Player.SendMessage(p, "3. No links");
            Player.SendMessage(p, "4. No Excessive Cursing (You are allowed to curse, but not pointed at anybody)");
            Player.SendMessage(p, "5. No use of $ Variables.");
            Player.SendMessage(p, "6. English only. No exceptions.");
            Player.SendMessage(p, "7. Be respectful");
            Player.SendMessage(p, "8. Do not ask for ranks");
            Player.SendMessage(p, "9. Do not ask for a server name");
            Player.SendMessage(p, "10. Use common sense.");
            Player.SendMessage(p, "11. Don't say any server name");
            Player.SendMessage(p, "&3Type /gcaccept to accept these rules");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/global [message] - Send a message to Global Chat.");
        }
    }
}
