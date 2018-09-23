using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class Bridge
{
    Stopwatch stopwatch = new Stopwatch();
    bool isActiveSensing = true;
    Action<byte[]> sendMidiMessage;
    TimeSpan activeSenseInterval = TimeSpan.FromMilliseconds(200);

    byte lastStatusByte = 0;

    object sendLock = new object();

    /// <summary>
    /// Construct a new <see cref="Bridge"/> instance.
    /// </summary>
    /// <param name="sendMidiMessage">Provide a delegate that will be called when the bridge has a message to send.</param>
    public Bridge(Action<byte[]> sendMidiMessage) : this (sendMidiMessage, null)
    {
        if (sendMidiMessage == null)
        {
            throw new ArgumentNullException(nameof(sendMidiMessage), "Provide a delegate that will will be called when a MIDI message is ready to send.");
        }
        this.sendMidiMessage = sendMidiMessage;
    }

    public Bridge(Action<byte[]> sendMidiMessage, TimeSpan? activeSenseInterval)
    {
        if (sendMidiMessage == null)
        {
            throw new ArgumentNullException(nameof(sendMidiMessage), "Provide a delegate that will will be called when a MIDI message is ready to send.");
        }
        this.sendMidiMessage = sendMidiMessage;

        if (activeSenseInterval != null)
        {
            this.activeSenseInterval = activeSenseInterval.Value;
        }
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
        cancellationTokenSource.Cancel();
    }

    public void Start()
    {

        cancellationTokenSource = new CancellationTokenSource();
        StartMidiHeartbeat(cancellationTokenSource.Token);
    }

    CancellationTokenSource cancellationTokenSource;

    public void StartMidiHeartbeat(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var buffer = new byte[3];
                while (true)
                {
                        long millis = stopwatch.ElapsedMilliseconds;

                        buffer[0] = (byte)(((millis >> 7) & 0x3F) | (long)0x80); //6 bits plus MSB
                        buffer[1] = (byte)((millis & 0x7F) | 0x80); //7 bits plus MSB
                        // Active sending message
                        buffer[2] = 0xFE;

                        lock (sendLock)
                        {
                            sendMidiMessage(buffer);
                        }

                    await Task.Delay(activeSenseInterval, cancellationToken);
                }
            }, cancellationToken);

            Console.WriteLine("Exiting heartbeat");
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