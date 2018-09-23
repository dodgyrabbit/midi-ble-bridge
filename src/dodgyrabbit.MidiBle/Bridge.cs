using System;
using System.Diagnostics;

public class Bridge
{
    Stopwatch stopwatch = new Stopwatch();
    bool isActiveSensing = true;
    Action<byte[]> sendMidiMessage;

    /// <summary>
    /// Construct a new <see cref="Bridge"/> instance.
    /// </summary>
    /// <param name="sendMidiMessage">Provide a delegate that will be called when the bridge has a message to send.</param>
    public Bridge(Action<byte[]> sendMidiMessage)
    {

    }

    void ReceiveMidiMessage(byte[] message)
    {

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
}