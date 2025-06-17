using Reptile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

namespace TrueBRChaos
{
    internal static class ChaosAudioHandler
    {
        private static bool ResourcesLoaded = false;

        private const string HEADER = "RIFF";
        private const string TYPE   = "WAVE";
        private const string FMT    = "fmt ";
        private const string DATA   = "data";

        private static Dictionary<int, AudioClip> AudioClips = new Dictionary<int, AudioClip>();

        private static class ErrorMessages
        {
            private const string ERROR_Invalid      = "Invalid WAV file";
            private const string ERROR_Unsupported  = "Unsupported WAV file format";
            private const string ERROR_BadData      = "WAV Data Incorrect";
            private const string ERROR_BadDepth     = "Unsupported Bit Depth";

            internal enum ErrorMessage
            {
                Error_Invalid,
                Error_Unsupported,
                Error_BadData,
                Error_BadDepth
            }

            internal static void ThrowError(ErrorMessage errorType)
            {
                string error = string.Empty;
                switch(errorType)
                {
                    case ErrorMessage.Error_Invalid:        error = ERROR_Invalid;      break;
                    case ErrorMessage.Error_Unsupported:    error = ERROR_Unsupported;  break;
                    case ErrorMessage.Error_BadData:        error = ERROR_BadData;      break;
                    case ErrorMessage.Error_BadDepth:       error = ERROR_BadDepth;     break;
                }
                throw new System.Exception(error);
            }

            internal static void ThrowErrorIf(ErrorMessage errorType, bool isTrue)
            {
                if (isTrue) ThrowError(errorType);
            }
        }

        internal static void PlayClip(string audioName, int mixerValue = 0, float pitch = 0f, AudioSourceID audioSource = AudioSourceID.Gameplay)
        {
            if (TryGetClip(audioName, out AudioClip audioClip))
                PlayClip(audioClip, mixerValue, pitch, audioSource);
        }

        internal static void PlayClip(AudioClip audioClip, int mixerValue = 0, float pitch = 0f, AudioSourceID audioSource = AudioSourceID.UI)
        {
            AudioManager audioManager = Commons.AudioManager;
            if (audioManager == null)
                return;

            mixerValue  = Mathf.Clamp(mixerValue, 0, 3);
            pitch       = Mathf.Clamp(pitch, -2f, 2f);

            audioManager.InvokeMethod("PlayOneShotSfx", audioManager.GetValue<AudioMixerGroup[]>("mixerGroups")[mixerValue], audioClip, audioManager.GetValue<AudioSource[]>("audioSources")[(int)audioSource], pitch);
        }

        private static bool TryGetClip(string audioName, out AudioClip audioClip)
        {
            if (!ResourcesLoaded)
            {
                audioClip = null;
                return false;
            }

            int hash = audioName.GetHashCode();
            if (AudioClips.TryGetValue(hash, out AudioClip value))
            {
                audioClip = value;
                return true;
            }

            try
            {
                var stream = (UnmanagedMemoryStream)typeof(Properties.Resources).GetProperties(Extensions.flags).FirstOrDefault(x => x.Name == audioName)?.GetValue(null, null);
                if (stream != null)
                {
                    Debug.LogError($"Recreating {audioName}...");
                    audioClip = CreateAudioClipFromResource(stream, hash.ToString());
                    return true;
                }
            } catch { }

            audioClip = null;
            return false;
        }

        internal static void LoadAllResourceAudio()
        {
            if (ResourcesLoaded)
                return;

            foreach (var property in typeof(Properties.Resources).GetProperties(Extensions.flags).Where(x => x.PropertyType == typeof(UnmanagedMemoryStream)))
            {
                CreateAudioClipFromResource((UnmanagedMemoryStream)property.GetValue(null, null), property.Name);
                Debug.Log($"Audio File \"{property.Name}\" Loaded.");
            }
            ResourcesLoaded = true;
        }

        private static AudioClip CreateAudioClipFromResource(UnmanagedMemoryStream stream, string clipName)
        {
            int hash = clipName.GetHashCode();
            if (AudioClips.TryGetValue(hash, out AudioClip clip))
                return clip;

            using (BinaryReader reader = new BinaryReader(stream))
            {
                ErrorMessages.ThrowErrorIf(ErrorMessages.ErrorMessage.Error_Invalid, !IsValidFile(reader));

                reader.MoveStream(4);

                var     data        = GetAudioData(reader);
                float[] floatArray  = ConvertByteToFloat(data.Item4, data.Item3);

                AudioClip audioClip = AudioClip.Create(clipName, floatArray.Length / data.Item1, data.Item1, data.Item2, false);
                audioClip.SetData(floatArray, 0);

                AudioClips.Add(hash, audioClip);
                return audioClip;
            }
        }

        private static bool IsValidFile(BinaryReader reader)
        {
            if (!reader.ReadStream(HEADER))
                return false;

            reader.MoveStream(4);

            if (!reader.ReadStream(TYPE))
                return false;

            return reader.ReadStream(FMT);
        }

        private static (int, int, int, byte[]) GetAudioData(BinaryReader reader)
        {
            ErrorMessages.ThrowErrorIf(ErrorMessages.ErrorMessage.Error_Unsupported, reader.ReadInt16() != 1);

            int numChannels     = reader.ReadInt16();
            int sampleRate      = reader.ReadInt32();
            reader.MoveStream(6);
            int bitsPerSample   = reader.ReadInt16();

            ErrorMessages.ThrowErrorIf(ErrorMessages.ErrorMessage.Error_BadDepth, bitsPerSample != 16);
            ErrorMessages.ThrowErrorIf(ErrorMessages.ErrorMessage.Error_BadData, !reader.ReadStream(DATA));
            return (numChannels, sampleRate, bitsPerSample, reader.ReadBytes(reader.ReadInt32()));
        }

        private static float[] ConvertByteToFloat(byte[] byteArray, int bitsPerSample)
        {
            int     floatArrayLength    = byteArray.Length / (bitsPerSample / 8);
            float[] floatArray          = new float[floatArrayLength];

            for (int i = 0; i < floatArrayLength; i++)
            {
                short value = BitConverter.ToInt16(byteArray, i * 2);
                floatArray[i] = value / 32768f;
            }
            return floatArray;
        }

        private static void MoveStream(this BinaryReader reader, int value)
        {
            reader.BaseStream.Position = Math.Min(reader.BaseStream.Position + value, reader.BaseStream.Length - 1);
        }

        private static bool ReadStream(this BinaryReader reader, string expect)
        {
            return new string(reader.ReadChars(expect.Length)) == expect;
        }
    }
}
