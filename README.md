# MIDI to BLE bridge
## Overview
A MIDI to Bluetooth Low Energy bridge written in C# running on Linux using .Net Core.
cIt uses the [BlueZ](https://git.kernel.org/pub/scm/bluetooth/bluez.git) protocol stack to communicate with the Bluetooth device on your machine.
BlueZ exposes the required functionality via [D-Bus](https://www.freedesktop.org/wiki/Software/dbus/).
Access to D-Bus has been simplified and modernised (async model) by [Tmds.DBus](https://github.com/tmds/Tmds.DBus).

The goal is to access a USB midi device on one side and expose it on the other via BTL, creating a bridge.

## Assumptions
For now, I'm simply documenting my own setup to get things working. Hopefully I can make this more generic in future.
* The system runs on Ubuntu 1604.
* USB interface to Midi.
* Using a laptop with Bluetooth built in or a dongle. Mine is a System76 laptop.

## Installation
Follow the instructions in the [installation guide](install.md)

## Blog
My crude collection of notes on how this is progressing, what I tried, what failed and so forth.

* 10 Sep 2018 [Who wants to ride DBus?](blog/002-understanding-dbus.md)
* 9 Sep 2018 [Basics](blog/001-getting-anything-to-work.md)

## TODO
- [x] Communicate with BlueZ via D-Bus
- [x] Get a basic GATT advertisement working
- [x] Advertise remaining GATT service properties
- [x] Create a BLE service
- [ ] Register MIDI BLE service the mimics Yamaha device
- [ ] Send a basic MIDI message (keep alive?)
- [ ] Figure out timing/resolution
- [ ] How to break "package" multiple midi messages as per the spec
- [ ] Figure out how to read MIDI messages from USB port
- [ ] Map incoming midi to outgoing midi
- [ ] Filter out stuff you don't want to see
      https://github.com/atsushieno/managed-midi
      https://dev.to/atsushieno/managed-midi-the-truly-cross-platform-net-midi-api-56hk