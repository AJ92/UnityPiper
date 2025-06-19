using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Abuksigun.Piper
{
    public unsafe static class PiperLib
    {
        private const string DllName = "piperlib"; // Replace with the actual name of your DLL

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalLong
        {
            [MarshalAs(UnmanagedType.I1)]  // ensures 1-byte boolean
            public bool hasValue;
            public long value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalPhonemeSilenceSecondsMap
        {
            [MarshalAs(UnmanagedType.I1)]  // ensures 1-byte boolean
            public bool hasValue;
            public PhonemeSilenceSecondsMap* value;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SynthesisConfig
        {
            public float noiseScale;
            public float lengthScale;
            public float noiseW;

            public int sampleRate;
            public int sampleWidth;
            public int channels;

//            public OptionalLong speakerId;

//            public float sentenceSilenceSeconds;
//            public OptionalPhonemeSilenceSecondsMap phonemeSilenceSeconds;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct PhonemeSilenceSecondsMap
        {
            public int phoneme;
            public float silenceSeconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SynthesisResult
        {
            public double inferSeconds;
            public double audioSeconds;
            public double realTimeFactor;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AudioCallback();


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr create_PiperConfig(string eSpeakDataPath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroy_PiperConfig(IntPtr config);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr create_Voice();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroy_Voice(IntPtr voice);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getSynthesisConfig(IntPtr voice);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern bool isSingleCodepoint(string s);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern char getCodepoint(string s);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getVersion();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void initializePiper(IntPtr config);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void terminatePiper(IntPtr config);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void loadVoice(IntPtr config, string modelPath, string modelConfigPath, IntPtr voice, Int64* speakerId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AudioCallbackDelegate(short* audioBuffer, int length);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void textToAudio(IntPtr config, IntPtr voice, string text, SynthesisResult* result, AudioCallbackDelegate audioCallback);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void textToWavFile(IntPtr config, IntPtr voice, string text, string audioFile, SynthesisResult* result);
    }
}