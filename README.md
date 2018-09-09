# MIDI to BLE bridge
## Overview
A MIDI to Bluetooth Low Energy bridge written in C# running on Linux using .Net Core.
It uses the [BlueZ](www.bluez.org) protocol stack to communicate with the Bluetooth device on your machine.
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

[Getting basics to work](blog/001-getting-anything-to-work.md)

## TODO
- [ ] Communicate with BlueZ via D-Bus
- [ ] Create a BLE service
- [ ] Send basic MIDI command via BLE service
- [ ] Figure out how to read MIDI messages from USB port
      https://github.com/atsushieno/managed-midi
      https://dev.to/atsushieno/managed-midi-the-truly-cross-platform-net-midi-api-56hk





