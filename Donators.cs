using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace MCForge {
    public class Donators {

        private readonly static Timer timer;
        private const int TEN_MINUTES = 600000;

        /// <summary>
        /// A list of donators
        /// </summary>
        public static readonly List<DonatorPlayers> DonatorList;

        static Donators() {
            timer = new Timer(CallBack, null, 0, TEN_MINUTES);
            DonatorList = new List<DonatorPlayers>();
        }

        private static void CallBack(object state) {
            using ( var client = new WebClient() ) {
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadString);
                client.DownloadStringAsync(new Uri("http://server.mcforge.net/donators.txt"));
            }
        }

        static void DownloadString(object sender, DownloadStringCompletedEventArgs e) {

            if ( e.Cancelled || e.Error != null ) {
                return;
            }

            DonatorList.Clear();

            string[] lines = e.Result.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach ( string line in lines ) {

                string[] sections = line.Split(':');

                if ( sections.Length != 3 )
                    continue;

                DonatorList.Add(new DonatorPlayers(sections[0], sections[1], sections[2][0]));
            }
        }

        /// <summary>
        /// Determines whether the specified player is in the list
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>
        ///   <c>true</c> if the specified player is in the list; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(Player player) {
            foreach ( var p in DonatorList ) {
                if ( p.Username.Equals(player.name) )
                    return true;
            }
            return false;
        }

        public static DonatorPlayers GetDonationAtribs(Player player) {
            foreach ( var donate in DonatorList ) {
                if(donate.Username.Equals(player.name))
                    return donate;
            }
            return null;
        }
    }

    /// <summary>
    /// Class containing username, title, and colors.
    /// </summary>
    public class DonatorPlayers {

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public char Color { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DonatorPlayers"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="title">The title.</param>
        /// <param name="color">The color.</param>
        public DonatorPlayers(string username, string title, char color) {
            this.Username = username;
            this.Title = title;
            this.Color = color;
        }
    }

}
