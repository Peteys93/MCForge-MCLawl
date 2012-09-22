using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge
{
    public class OnConsoleCommandEvent
    {
        internal static List<OnConsoleCommandEvent> events = new List<OnConsoleCommandEvent>();
        Plugin plugin;
        Server.OnConsoleCommand method;
        Priority priority;
        internal OnConsoleCommandEvent(Server.OnConsoleCommand method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        public static void Call(string cmd, string message)
        {
            events.ForEach(delegate(OnConsoleCommandEvent p1)
            {
                try
                {
                    p1.method(cmd, message);
                }
                catch (Exception e) { Server.s.Log("The plugin " + p1.plugin.name + " errored when calling the LevelUnload Event!"); Server.ErrorLog(e); }
            });
        }
        static void Organize()
        {
            List<OnConsoleCommandEvent> temp = new List<OnConsoleCommandEvent>();
            List<OnConsoleCommandEvent> temp2 = events;
            OnConsoleCommandEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (OnConsoleCommandEvent p in temp2)
                {
                    if (temp3 == null)
                        temp3 = p;
                    else if (temp3.priority < p.priority)
                        temp3 = p;
                }
                temp.Add(temp3);
                temp2.Remove(temp3);
                temp3 = null;
                i++;
            }
            events = temp;
        }
        /// <summary>
        /// Find a event
        /// </summary>
        /// <param name="plugin">The plugin that registered this event</param>
        /// <returns>The event</returns>
        public static OnConsoleCommandEvent Find(Plugin plugin)
        {
            foreach (OnConsoleCommandEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        /// <summary>
        /// Register this event
        /// </summary>
        /// <param name="method">This is the delegate that will get called when this event occurs</param>
        /// <param name="priority">The priority (imporantce) of this call</param>
        /// <param name="plugin">The plugin object that is registering the event</param>
        /// <param name="bypass">Register more than one of the same event</param>
        public static void Register(Server.OnConsoleCommand method, Priority priority, Plugin plugin, bool bypass = false)
        {
            if (Find(plugin) != null)
                if (!bypass)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnConsoleCommandEvent(method, priority, plugin));
            Organize();
        }
        /// <summary>
        /// UnRegister this event
        /// </summary>
        /// <param name="plugin">The plugin object that has this event registered</param>
        public static void UnRegister(Plugin plugin)
        {
            if (Find(plugin) == null)
                throw new Exception("This plugin doesnt have this event registered!");
            else
                events.Remove(Find(plugin));
        }
    }
}
