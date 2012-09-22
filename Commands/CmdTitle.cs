/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using MCForge.SQL;
using System.Text.RegularExpressions;


namespace MCForge.Commands {
	public class CmdTitle : Command {
		public override string name { get { return "title"; } }
		public override string shortcut { get { return ""; } }
		public override string type { get { return "other"; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
		public CmdTitle() { }

		public override void Use(Player p, string message) {
			if ( message == "" ) { Help(p); return; }

			int pos = message.IndexOf(' ');
			Player who = Player.Find(message.Split(' ')[0]);
			if ( who == null ) { Player.SendMessage(p, "Could not find player."); return; }
			if ( p != null && who.group.Permission > p.group.Permission ) {
				if ( !Server.devs.Contains(p.name) ) {
					Player.SendMessage(p, "Cannot change the title of someone of greater rank");
					return;
				}
			}
			if ( Server.devs.Contains(who.name) ) {
				if ( !Server.devs.Contains(p.name) ) {
					Player.SendMessage(p, "You can't change the title of a developer!");
					return;
				}
			}
			if ( Server.gcmods.Contains(who.name) ) {
				if ( !Server.devs.Contains(p.name) && !Server.gcmods.Contains(p.name) ) {
					Player.SendMessage(p, "You can't change the title of a Global Chat Moderator!");
					return;
				}
			}
			string query;
			string newTitle = "";
			if ( message.Split(' ').Length > 1 ) newTitle = message.Substring(pos + 1);
			else {
				who.title = "";
				who.SetPrefix();
				Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " had their title removed.", false);
				query = "UPDATE Players SET Title = '' WHERE Name = '" + who.name + "'";
				Database.executeQuery(query);
				return;
			}

			if ( newTitle != "" ) {
				newTitle = newTitle.ToString().Trim().Replace("[", "");
				newTitle = newTitle.Replace("]", "");
				/* if (newTitle[0].ToString() != "[") newTitle = "[" + newTitle;
				if (newTitle.Trim()[newTitle.Trim().Length - 1].ToString() != "]") newTitle = newTitle.Trim() + "]";
				if (newTitle[newTitle.Length - 1].ToString() != " ") newTitle = newTitle + " "; */
			}

			if ( newTitle.Length > 17 ) { Player.SendMessage(p, "Title must be under 17 letters."); return; }
			if ( p != null && !Server.devs.Contains(p.name) ) {
				string title = newTitle.ToLower();
				foreach ( char c in Server.ColourCodesNoPercent ) {
					title = title.Replace("%" + c, "");
					title = title.Replace("&" + c, "");
				}
				if ( title.Contains("dev") ) { Player.SendMessage(p, "You're not a developer! Stop pretending you are!"); return; }
			}

			if ( newTitle != "" )
				Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " was given the title of &b[" + newTitle + "]", false);
			else Player.GlobalChat(who, who.color + who.prefix + who.name + Server.DefaultColor + " had their title removed.", false);

			if ( !Regex.IsMatch(newTitle.ToLower(), @"^[a-z0-9]*?$") ) {
				Player.SendMessage(p, "That is not allowed");
				return;
			}

			if ( newTitle == "" ) {
				query = "UPDATE Players SET Title = '' WHERE Name = '" + who.name + "'";
			}
			else {
				query = "UPDATE Players SET Title = '" + newTitle.Replace("'", "\'") + "' WHERE Name = '" + who.name + "'";
			}
			Database.executeQuery(query);
			who.title = newTitle;
			who.SetPrefix();
		}
		public override void Help(Player p) {
			Player.SendMessage(p, "/title <player> [title] - Gives <player> the [title].");
			Player.SendMessage(p, "If no [title] is given, the player's title is removed.");
		}
	}
}