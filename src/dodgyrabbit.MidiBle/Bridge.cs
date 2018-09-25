using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class Bridge
{
    Stopwatch stopwatch = new Stopwatch();
    Action<byte[]> sendMidiMessage;
    TimeSpan activeSenseInterval = TimeSpan.FromMilliseconds(150);
    CancellationTokenSource cancellationTokenSource;
    byte lastStatusByte = 0;
    object sendLock = new object();

    /// <summary>
    /// Construct a new <see cref="Bridge"/> instance.
    /// </summary>
    /// <param name="sendMidiMessage">Provide a delegate that will be called when the bridge has a message to send.</param>
    public Bridge(Action<byte[]> sendMidiMessage) : this(sendMidiMessage, null)
    {
        if (sendMidiMessage == null)
        {
            throw new ArgumentNullException(nameof(sendMidiMessage), "Provide a delegate that will will be called when a MIDI message is ready to send.");
        }
        this.sendMidiMessage = sendMidiMessage;
        stopwatch.Start();
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

    public void StopActiveSense()
    {
        lock (sendLock)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
        }
    }

    public void StartActiveSense()
    {
        lock (sendLock)
        {
            if (cancellationTokenSource != null)
            {
                throw new InvalidOperationException("Can't start two active sense workers");
            }
            cancellationTokenSource = new CancellationTokenSource();
            StartMidiHeartbeat(cancellationTokenSource.Token);
        }
    }

    public void StartMidiHeartbeat(CancellationToken cancellationToken)
    {
        Task.Run(
            async () =>
            {
                // Active sensing message
                var buffer = new byte[] {0xFE};
                while (true)
                {
                    SendMidiMessage(AddTimestamp(buffer));
                    await Task.Delay(activeSenseInterval, cancellationToken);
                }
            },
            cancellationToken);
        Console.WriteLine("Exiting heartbeat");
    }

    // Internal, thread-safe method to call the callback when a MIDI message is ready to send
    void SendMidiMessage(byte[] message)
    {
        lock (sendLock)
        {
            sendMidiMessage(message);
        }
    }

    void ProcessMidiMessage(int length, byte[] data)
    {
        // TODO: Improve this to look for messages of this type anyhere in stream
        if (length == 1 && data[0] == 0xFE)
        {
            return;
        }
        bool hasStatusByte = false;

        List<byte> bytes = new List<byte>();
        for (int i = 0; i < length; i++)
        {
            bool isStatusByte = (data[i] & 0b1000_0000) > 0;

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
            SendMidiMessage(AddTimestamp(bytes.ToArray()));
        }
    }

    byte[] AddTimestamp(byte[] value)
    {
        var buffer = new byte[2 + value.Length];
        long milliseconds = stopwatch.ElapsedMilliseconds;

        buffer[0] = (byte)(((milliseconds >> 7) & 0x3F) | (long)0x80); // 6 bits plus MSB
        buffer[1] = (byte)((milliseconds & 0x7F) | 0x80); // 7 bits plus MSB

        Buffer.BlockCopy(value, 0, buffer, 2, value.Length);
        return buffer;
    }
}