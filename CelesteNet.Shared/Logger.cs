﻿using Celeste.Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.CelesteNet {
    // Based off of Everest's Logger.
    public static class Logger {

        public static LogLevel Level = LogLevel.INF;
        public static bool LogCelesteNetTag = false;

        private static TextWriter _Writer;
        public static TextWriter Writer {
            get => _Writer ?? Console.Out;
            set => _Writer = value;
        }

        /// <summary>
        /// Log a string to the console and to log.txt
        /// </summary>
        /// <param name="tag">The tag, preferably short enough to identify your mod, but not too long to clutter the log.</param>
        /// <param name="str">The string / message to log.</param>
        public static void Log(string tag, string str)
            => Log(LogLevel.VVV, tag, str);

        /// <summary>
        /// Log a string to the console and to log.txt
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="tag">The tag, preferably short enough to identify your mod, but not too long to clutter the log.</param>
        /// <param name="str">The string / message to log.</param>
        public static void Log(LogLevel level, string tag, string str) {
            if (level < Level)
                return;

            TextWriter w = Writer;
            w.Write("(");
            w.Write(DateTime.Now);
            w.Write(LogCelesteNetTag ? ") [CelesteNet] [" : ") [");
            w.Write(level.ToString());
            w.Write("] [");
            w.Write(tag);
            w.Write("] ");
            w.WriteLine(str);
        }

        /// <summary>
        /// Log a string to the console and to log.txt, including a call stack trace.
        /// </summary>
        /// <param name="tag">The tag, preferably short enough to identify your mod, but not too long to clutter the log.</param>
        /// <param name="str">The string / message to log.</param>
        public static void LogDetailed(string tag, string str) {
            Log(LogLevel.VVV, tag, str);
            Writer.WriteLine(new StackTrace(1, true).ToString());
        }
        /// <summary>
        /// Log a string to the console and to log.txt, including a call stack trace.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="tag">The tag, preferably short enough to identify your mod, but not too long to clutter the log.</param>
        /// <param name="str">The string / message to log.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogDetailed(LogLevel level, string tag, string str) {
            Log(level, tag, str);
            Writer.WriteLine(new StackTrace(1, true).ToString());
        }

        /// <summary>
        /// Print the exception to the console, including extended loading / reflection data useful for mods.
        /// </summary>
        public static void LogDetailedException(/*this*/ Exception e, string tag = null) {
            TextWriter w = Writer;

            if (tag == null) {
                w.WriteLine("--------------------------------");
                w.WriteLine("Detailed exception log:");
            }
            for (Exception e_ = e; e_ != null; e_ = e_.InnerException) {
                w.WriteLine("--------------------------------");
                w.WriteLine(e_.GetType().FullName + ": " + e_.Message + "\n" + e_.StackTrace);
                if (e_ is ReflectionTypeLoadException rtle) {
                    for (int i = 0; i < rtle.Types.Length; i++) {
                        w.WriteLine("ReflectionTypeLoadException.Types[" + i + "]: " + rtle.Types[i]);
                    }
                    for (int i = 0; i < rtle.LoaderExceptions.Length; i++) {
                        LogDetailedException(rtle.LoaderExceptions[i], tag + (tag == null ? "" : ", ") + "rtle:" + i);
                    }
                }
                if (e_ is TypeLoadException) {
                    w.WriteLine("TypeLoadException.TypeName: " + ((TypeLoadException) e_).TypeName);
                }
                if (e_ is BadImageFormatException) {
                    w.WriteLine("BadImageFormatException.FileName: " + ((BadImageFormatException) e_).FileName);
                }
            }
        }

    }
    public enum LogLevel {
        DEV = -1,
        VVV,
        DBG,
        INF,
        WRN,
        ERR,
        CRI
    }
}
