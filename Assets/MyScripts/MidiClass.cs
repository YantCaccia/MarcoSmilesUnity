using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Sanford.Multimedia.Midi;
using System;


public class MidiClass
{


    /*
     * Per eseguire il programma si deve scaricare ed installare LoopMidi
     * --------------------------------------------------------------------
     * https://www.tobias-erichsen.de/software/loopmidi.html
     * --------------------------------------------------------------------
     * Successivamente creare con LoopMidi una porta Midi virtuale con il nome "MarcoSmiles"
     */
    private OutputDevice outD;
    private ChannelMessageBuilder builder;

    public MidiClass(){
       
    }


    public void SendEvent(int note, int octave)
    {
        //Per stampare a video la nota suonata
        String Nota = "";
        String Ottava = "";
        String[] NomeNote = new String[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };



        // C C# D D# E F F# G G# A A# B
        // 0 1  2 3  4 5 6  7 8  9 10 11 ... 23
        //aggiungo 12 per ogni ottava
        //MarcoSmiles utilizza una tastiera con 2 ottave numerando le note selezionate da 0 a 23
        //inoltre offre la possibilità di selezionare le ottave partendo dal Range C0-C1
        //C0-C1  C1-C2  C2-C3 ....
        //  0      1      2





        //Per disabilitare tutti gli eventi MIDI in caso di errore
        /*for (int i = 0; i < 128; i++)
        {
            //-----costruisco l'evento midi da inviare ---

            //Data1 rappresenta la Nota

            builder.Data1 = i;


            //Data2 è la velocity
            builder.Data2 = 120;

            builder.MidiChannel = 0;

            //Suono la nota
            builder.Command = ChannelCommand.NoteOff;
            builder.Build();
            outD.Send(builder.Result);
        }
        */


        //TO DO
        // per integrare questo modulo in MarcoSmiles faccio corrispondere il Note On nel momento in cui viene selezionata la nota
        // invio il note Off nel momento in cui si seleziona un'altra nota


        //-----costruisco l'evento midi da inviare ---

        Debug.Log("Note ON" + (note + (octave * 12)));
        //Data1 rappresenta la Nota
        builder.Data1 = note + (octave * 12);

        //Data2 è la velocity
        builder.Data2 = 105;

        builder.MidiChannel = 0;

        //Suono la nota
        builder.Command = ChannelCommand.NoteOn;
        builder.Build();
        outD.Send(builder.Result);

        //Stampo sul terminale la nota
        Nota = NomeNote[note % 12];
        if (note > 11)
        {
            Ottava = "" + (octave + 1);
        }
        else
        {
            Ottava = "" + octave;
        }
        Debug.Log("\n" + Nota + Ottava);

    }


    public void FindMidi()
    {
        int DevId = 0;

        //Identifico la porta relativa a MarcoSmiles
        int numDevice = OutputDevice.DeviceCount;
        Debug.Log("\n Output Devices Disponibili: " + numDevice);

        for (int i = 0; i < numDevice; i++)
        {
            MidiOutCaps dev = OutputDevice.GetDeviceCapabilities(i);
            Debug.Log("\n" + dev.name);
            if (dev.name == "MarcoSmiles")
            {
                DevId = i;
                Debug.Log("\nPort Marco " + DevId);

            }
        }


        //seleziono L'output Device e inizializzo Il MessageBuilder
        outD = new OutputDevice(DevId);
        builder = new ChannelMessageBuilder();

    }

    

    public void SendMidiOff(int note, int octave)
    {


        Debug.Log("Note OFF" + (note + (octave * 12)));
        //Data1 rappresenta la Nota
        builder.Data1 = (note + (octave * 12));

        //Data2 è la velocity
        builder.Data2 = 105;
        builder.MidiChannel = 0;
        //Invio il segnale di Note Off
        builder.Command = ChannelCommand.NoteOff;
        builder.Build();
     
        outD.Send(builder.Result);


    
        
        


    }

}