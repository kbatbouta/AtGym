using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace PumpingSteel
{
    public static class Logging
    {
        private static readonly StringBuilder buffer = new StringBuilder();

        private static StringBuilder Header
        {
            get
            {
                buffer.Clear();
                return buffer.Append(
                    "[").Append(
                    Finder.packageID.ToLower()).Append(
                    "]: ");
            }
        }

        public static void List(IEnumerable<object> aList, string title = "List", bool force = false)
        {
            Header.Append(title);

            foreach (var obj in aList) buffer.AppendInNewLine(obj.ToString());

            if (force)
            {
                Log.Message(buffer.ToString());
            }
            else
            {
#if DEBUG
                Log.Message(buffer.ToString());
#endif
            }
        }

        public static void Line(string message, bool force = false)
        {
            Header.Append(message);

            if (force)
            {
                Log.Message(buffer.ToString());
            }
            else
            {
#if DEBUG
                Log.Message(buffer.ToString());
#endif
            }
        }

        public static void Warning(string message)
        {
            Line(new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                true);
            Log.Warning(Header.Append(message).ToString());
        }

        public static void Error(string message)
        {
            Line(new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                true);
            Log.Error(Header.Append(message).ToString());
        }
    }
}