using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Abuksigun.Piper
{
    public sealed unsafe class Piper : IDisposable
    {
        IntPtr configPtr;

        public IntPtr ConfigPtr => configPtr;

        Piper(IntPtr configPtr)
        {
            this.configPtr = configPtr;
        }

        ~Piper()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (configPtr != IntPtr.Zero)
            {
                PiperLib.terminatePiper(configPtr);
                PiperLib.destroy_PiperConfig(configPtr);
                configPtr = IntPtr.Zero;
            }
        }

        public static Piper LoadPiper(string fullEspeakDataPath)
        {
            Debug.Log("LoadPiper...");

            if (!Directory.Exists(fullEspeakDataPath))
                throw new DirectoryNotFoundException("Espeak data directory not found");


            var piperConfig = PiperLib.create_PiperConfig(fullEspeakDataPath);
            try
            {
                PiperLib.initializePiper(piperConfig);
                return new Piper(piperConfig);
            }
            catch
            {
                PiperLib.destroy_PiperConfig(piperConfig);
                throw;
            }
        }
    }
}