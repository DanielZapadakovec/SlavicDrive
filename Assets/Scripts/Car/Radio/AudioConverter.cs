using System.IO;
using NAudio.Wave;
using UnityEngine;

public static class AudioConverter
{
    public static AudioClip ConvertMp3ToAudioClip(byte[] mp3Data)
    {
        using (MemoryStream memoryStream = new MemoryStream(mp3Data))
        using (Mp3FileReader mp3Reader = new Mp3FileReader(memoryStream))
        using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
        {
            byte[] rawAudioData = new byte[waveStream.Length];
            waveStream.Read(rawAudioData, 0, rawAudioData.Length);

            int sampleCount = rawAudioData.Length / 2;
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = (short)(rawAudioData[i * 2] | (rawAudioData[i * 2 + 1] << 8));
                samples[i] = sample / 32768.0f; // Normalizácia na rozsah -1.0f až 1.0f
            }

            AudioClip audioClip = AudioClip.Create("ConvertedAudio", sampleCount, 1, 44100, false);
            audioClip.SetData(samples, 0);

            return audioClip;
        }
    }
}
