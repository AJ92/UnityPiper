using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Abuksigun.Piper
{
    public sealed unsafe class PiperVoice : IDisposable
    {
        Piper piper;
        //PiperLib.Voice* voice;
        IntPtr voicePtr;

        public IntPtr VoicePtr => voicePtr;

        public Piper Piper => piper;

        PiperVoice(Piper piper, IntPtr voice)
        {
            this.piper = piper;
            this.voicePtr = voice;
        }

        ~PiperVoice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (voicePtr != IntPtr.Zero)
            {
                PiperLib.destroy_Voice(voicePtr);
                voicePtr = IntPtr.Zero;
            }
        }

        public static PiperVoice LoadPiperVoice(Piper piper, string fullModelPath)
        {
            Debug.Log("LoadPiperVoice...");

            if (!File.Exists(fullModelPath))
                throw new FileNotFoundException("Model file not found", fullModelPath);
            if (!File.Exists(fullModelPath + ".json"))
                throw new FileNotFoundException("Model descriptor not found (Make sure it has the same name as model + .json)", fullModelPath);

            var newVoice = PiperLib.create_Voice();
            try
            {
                PiperLib.loadVoice(piper.ConfigPtr, fullModelPath, fullModelPath + ".json", newVoice, null);
                return new PiperVoice(piper, newVoice);
            }
            catch
            {
                PiperLib.destroy_Voice(newVoice);
                throw;
            }
        }

        public float[] TextToPCMAudio(string text)
        {
            float[] audioData = new float[0];
            TextToAudioStream(text, piper.ConfigPtr, voicePtr, (short* data, int length) =>
            {
                int writeIndex = audioData.Length;
                Array.Resize(ref audioData, audioData.Length + length);
                for (int i = writeIndex; i < audioData.Length; i++)
                    audioData[i] = data[i] / 32768f;
            });

            return audioData;
        }

        public static void TextToAudioStream(string text, IntPtr config, IntPtr voice, PiperLib.AudioCallbackDelegate audioCallback)
        {
            PiperLib.SynthesisResult result = new PiperLib.SynthesisResult();
            PiperLib.textToAudio(config, voice, text, &result, audioCallback);
        }
    }
}
