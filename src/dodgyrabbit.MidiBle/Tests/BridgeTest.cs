using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace dodgyrabbit.MidiBle.Tests
{
    public class BridgeTest
    {
        byte[] RemoveTimestamp(byte[] message)
        {
            if (message.Length < 2)
            {
                Assert.True(false, "Message too short for timestamp");
            }
            return message.Skip(2).Take(message.Length-2).ToArray();
        }

        [Fact]
        public void NoteOn()
        {
            byte[] noteOn = new byte[] {0x90, 0x24, 0x0};
            Bridge bridge = new Bridge(data =>
            {
                Assert.Equal(noteOn, RemoveTimestamp(data));
            });
            bridge.ReceiveMidiMessage(noteOn.Length, noteOn);
        }

        // Filter out Note Off messages
        [Fact]
        public void NoteOff()
        {
            byte[] noteOff = new byte[] {0x80, 0x24, 0x0};
            Bridge bridge = new Bridge(data =>
            {
                Assert.True(false, "Expected message to be filtered, yet here we are");
            });
            bridge.ReceiveMidiMessage(noteOff.Length, noteOff);
        }

        [Fact]
        public void MultipleNoteOnInOneMessage()
        {
            byte[] noteOn = new byte[] {0x90, 0x24, 0x0, 0x25, 0x0};
            Bridge bridge = new Bridge(data =>
            {
                Assert.Equal(noteOn, RemoveTimestamp(data));
            });
            bridge.ReceiveMidiMessage(noteOn.Length, noteOn);
        }

        [Fact]
        public void MultipleNoteOnInTwoMessages()
        {
            // First message includes the Status byte (0x90 - Note on)
            byte[] noteOn1 = new byte[] {0x90, 0x24, 0x0};
            // Second message is a continuation, and does not
            byte[] noteOn2 = new byte[] {0x25, 0x0};

            List<byte[]> messages = new List<byte[]>();
            Bridge bridge = new Bridge(data =>
            {
                messages.Add(data);
            });
            // Send the two messages seperately
            bridge.ReceiveMidiMessage(noteOn1.Length, noteOn1);
            bridge.ReceiveMidiMessage(noteOn2.Length, noteOn2);

            // First message includes header byte
            Assert.Equal(noteOn1, RemoveTimestamp(messages[0]));
            // This is the important bit - the second message should also have header byte
            Assert.Equal(new byte[] {0x90, 0x25, 0x0}, RemoveTimestamp(messages[1]));
        }

        /// <summary>
        /// Tests that in a given time interval we receive active sense messages.
        /// This is not a great test because it depends on the execution and timing of threads.
        /// Ideally something where the test can control the loop
        /// </summary>
        [Fact]
        public void ActiveSense()
        {
            int activeSenseCount = 0;
            Bridge bridge = new Bridge(data =>
            {
                Assert.Equal(data.Length, 3);
                Assert.Equal(data[2], 0xFE);
                activeSenseCount++;
            }, TimeSpan.FromMilliseconds(100));

            bridge.StartActiveSense();
            Thread.Sleep(250);
            bridge.StopActiveSense();

            Assert.Equal(3, activeSenseCount);
        }
    }
}