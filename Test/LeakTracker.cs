using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public static class LeakTracker
    {
        static List<WeakReference> _tracker = new List<WeakReference>();

        /// <summary>
        /// should be called only at object construction/instanciation
        /// </summary>
        public static void Add(object objToTrack)
        {
            Prune();
            _tracker.Add(new WeakReference(objToTrack));
        }

        public static void Prune()
        {
            _tracker = _tracker.Where(o => o.IsAlive).ToList();
        }

        public static string Dump()
        {
            Prune();
            return string.Join("\r\n", _tracker.Select(o => o.Target).Where(o => o != null).Select(o => o.GetType().ToString()));
        }
    }
}
