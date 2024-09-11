using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ContinuousPianoSound : MonoBehaviour
{
    public int midiNote = 60; // Nota MIDI corrispondente al "Do centrale"
    public float amplitude = 0.5f;
    public int sampleRate = 44100;

    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        float frequency = MidiNoteToFrequency(midiNote);
        float increment = frequency * 2 * Mathf.PI / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float t = (Time.time - startTime) * Mathf.PI * 2; // Utilizza il tempo memorizzato

            // Aggiungi armonici o variazioni per rendere la forma d'onda più complessa
            float harmonic1 = Mathf.Sin(t);
            float harmonic2 = 0.5f * Mathf.Sin(2 * t);
            float harmonic3 = 0.2f * Mathf.Sin(3 * t);

            float sample = amplitude * (harmonic1 + harmonic2 + harmonic3);

            // Ripeti la stessa forma d'onda su tutti i canali
            for (int channel = 0; channel < channels; channel++)
            {
                data[i + channel] = sample;
            }
        }
    }

    float MidiNoteToFrequency(int midiNote)
    {
        return 440f * Mathf.Pow(2f, (midiNote - 69) / 12f);
    }
}