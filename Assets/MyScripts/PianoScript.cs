using UnityEngine;

public class PianoScript : MonoBehaviour
{
    public AudioSource audioSource;
    public int midiNote = 60; // Nota MIDI corrispondente al "Do centrale"

    void Start()
    {
        // Assicurati di avere un componente AudioSource associato al GameObject
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Imposta la frequenza della nota MIDI corrispondente
        float frequency = MidiNoteToFrequency(midiNote);

        // Imposta le caratteristiche audio
        audioSource.pitch = frequency / 440f; // Normalizza rispetto al La a 440Hz
        audioSource.clip = GeneratePianoSound(frequency, 1.0f, 1.0f, 44100);
       
    }

    AudioClip GeneratePianoSound(float frequency, float amplitude, float duration, int sampleRate)
    {
        int sampleLength = (int)(duration * sampleRate);
        float[] samples = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            float t = i / (float)sampleRate;

            // Aggiungi armonici o variazioni per rendere la forma d'onda più complessa
            float harmonic1 = Mathf.Sin(2 * Mathf.PI * frequency * t);
            float harmonic2 = 0.5f * Mathf.Sin(2 * Mathf.PI * 2 * frequency * t);
            float harmonic3 = 0.2f * Mathf.Sin(2 * Mathf.PI * 3 * frequency * t);

            samples[i] = amplitude * (harmonic1 + harmonic2 + harmonic3);
        }

        AudioClip audioClip = AudioClip.Create("PianoNote", sampleLength, 1, sampleRate, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }

    float MidiNoteToFrequency(int midiNote)
    {
        return 440f * Mathf.Pow(2f, (midiNote - 69) / 12f);
    }
}