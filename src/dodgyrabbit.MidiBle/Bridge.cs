using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Bridge
{
    Stopwatch stopwatch = new Stopwatch();
    bool isActiveSensing = true;
    Action<byte[]> sendMidiMessage;

    byte lastStatusByte = 0;

    /// <summary>
    /// Construct a new <see cref="Bridge"/> instance.
    /// </summary>
    /// <param name="sendMidiMessage">Provide a delegate that will be called when the bridge has a message to send.</param>
    public Bridge(Action<byte[]> sendMidiMessage)
    {
        if (sendMidiMessage == null)
        {
            throw new ArgumentNullException(nameof(sendMidiMessage), "Provide a delegate that will will be called when a MIDI message is ready to send.");
        }
        this.sendMidiMessage = sendMidiMessage;
    }

    /// <summary>
    /// Called when raw MIDI bytes are received
    /// </summary>
    /// <param name="message"></param>
    public void ReceiveMidiMessage(int length, byte[] message)
    {
        ProcessMidiMessage(length, message);
    }

    void Run()
    {

    }

    public void Stop()
    {

    }

    public void Start()
    {

    }

    void ProcessMidiMessage(int length, byte[] data)
    {
        if (length == 1 && data[0] == 0xFE)
        {
            return;
        }
        bool hasStatusByte = false;

        List<byte> bytes = new List<byte>();
        for (int i = 0; i < length; i++)
        {
            bool isStatusByte = ((data[i] & (byte)0b1000_0000) > 0);

            if (!hasStatusByte && !isStatusByte)
            {
                // Continuation from previous message. Repeat status byte.
                if (lastStatusByte != 128)
                {
                    bytes.Add(lastStatusByte);
                    bytes.Add(data[i]);
                }
                hasStatusByte = true;
                continue;
            }
            if (!hasStatusByte && isStatusByte)
            {
                // New message, starting with status

                if (data[i] != 128)
                {
                    // Skip the note-off messages
                    bytes.Add(data[i]);
                }

                hasStatusByte = true;
                lastStatusByte = data[i];
                continue;
            }

            if (isStatusByte)
            {
                // Multiple status bytes in a row
                if (data[i] != 128)
                {
                    bytes.Add(data[i]);
                }
                lastStatusByte = data[i];
                continue;
            }

            // Must be a data byte then, add it
            if (lastStatusByte != 128)
            {
                bytes.Add(data[i]);
            }
        }
        if (bytes.Count > 0)
        {
            sendMidiMessage(bytes.ToArray());
        }
    }
}