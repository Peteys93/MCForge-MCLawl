using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MCForge.Commands
{
    class CmdGcrules : Command
    {
        public override string name { get { return "gcrules"; } }
        public override string shortcut { get { return "gcr"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdGcrules() { }
        //bla
        public override void Use(Player p, string message)
        {
            RulesMethod(p);
            p.Readgcrules = true;
            p.Timereadgcrules = DateTime.Now;
        }
        public void RulesMethod(Player p)
        {
            Player.SendMessage(p, "&cBy using the Global Chat you agree to the following rules:");
            Player.SendMessage(p, "1. No Spamming");
            Player.SendMessage(p, "2. No Advertising (Trying to get people to come to your server)");
            Player.SendMessage(p, "3. No links");
            Player.SendMessage(p, "4. No Excessive Cursing (You are allowed to curse, but not pointed at anybody)");
            Player.SendMessage(p, "5. No use of $ Variables.");
            Player.SendMessage(p, "6. English only. No exceptions.");
            Player.SendMessage(p, "7. Be respectful");
            Player.SendMessage(p, "8. Do not ask for ranks");
            Player.SendMessage(p, "9. Do not ask for a server name");
            Player.SendMessage(p, "10. Use common sense.");
            Player.SendMessage(p, "11. Don't say any server name");
            
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/gcrules - Shows global chat rules");
            Player.SendMessage(p, "To chat in global chat, use /global");
        }
    }
}
