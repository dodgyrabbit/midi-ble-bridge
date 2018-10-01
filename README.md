[![Build Status](https://travis-ci.com/dodgyrabbit/midi-ble-bridge.svg?branch=master)](https://travis-ci.com/dodgyrabbit/midi-ble-bridge)

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

## TODO
- [x] Communicate with BlueZ via D-Bus
- [x] Get a basic GATT advertisement working
- [x] Advertise remaining GATT service properties
- [x] Create a BLE service
- [x] Register MIDI BLE service the mimics Yamaha device
- [x] Send a basic MIDI message (keep alive?)
- [x] Figure out timing/resolution
- [x] How to break "package" multiple midi messages as per the spec
- [x] Figure out how to read MIDI messages from USB port
- [x] Map incoming midi to outgoing midi
- [x] Filter out stuff you don't want to see
- [ ] Enumerate devices - disconnect if we find it's already connected (can't advertise otherwise)
- [ ] Better encapsulation/classes for MIDI
- [ ] Better abstraction for filtering
- [ ] Refactor properties out of DBus interfaces

