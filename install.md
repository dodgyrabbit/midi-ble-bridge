# Installation

## Install the latest .Net Core SDK

https://www.microsoft.com/net/download/linux-package-manager/ubuntu16-04/sdk-current

## Install a newer version of BlueZ

This was probably the most complicated part and hopefully will become more simple in time.
The main problem is that BlueZ that currently ships with Ubuntu (even 1804) does not fully support BTLE MIDI.

BlueZ provides a download on their website. I honestly don't know exaclty how to use it.
The [Launchpad] (https://launchpad.net/ubuntu/cosmic/+source/bluez) has the code and additional files required for building on Ubuntu.
You could look at this site and find newer versions and try build them using the instructions below and making small modifications as required.

Install the prerequisites for Bluez compilation.

```console
sudo apt install build-essential debhelper fakeroot wget \
dh-autoreconf flex bison libdbus-glib-1-dev libglib2.0-dev \
libcap-ng-dev udev libudev-dev libreadline-dev libical-dev \
check dh-systemd libebook1.2-dev libasound2-dev
```

Next download the tar files.

```console
wget https://launchpad.net/ubuntu/+archive/primary/+sourcefiles/bluez/5.48-0ubuntu3/bluez_5.48.orig.tar.xz
wget https://launchpad.net/ubuntu/+archive/primary/+sourcefiles/bluez/5.48-0ubuntu3/bluez_5.48-0ubuntu3.debian.tar.xz
tar xJf bluez_5.48.orig.tar.xz
cd bluez-5.48
tar xJf ../bluez_5.48-0ubuntu3.debian.tar.xz
```

Before we biuld, we need to change the debian/rules file and add --enable-midi to it. My understanding is that is because MIDI support in BlueZ is still experimental.

```console
sed -i 's/\(--enable-usb\)/\1 --enable-midi/' debian/rules
```

Time to create the build. It looks like this command compiles and then packages up everything in deb files.

```console
dpkg-buildpackage -rfakeroot -b
```

Lastly time to install the new version and restart the bluetooth service.
``` console
cd ..
sudo dpkg -i bluez*.deb libbluetooth*.deb
sudo service bluetooth restart
```