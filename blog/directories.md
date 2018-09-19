# Interesting directories
Even though I've used Linux for years, it still feels like I struggle to know what all the directories are for. This is to document at least what I'm seeing or where I found interesting bits.

## /etc/dbus1/system.d
Inside this folder is a bunch of .conf files. These are DBus configuration files and seem to control DBus communcation policies for the most part.
```console
/etc/dbus1/system.d/bluetooth.conf
```

## /usr/bin
Here lies mostly executable files by the look of it. Some are symbolic links (term used correctly?) to other files.
I.e. they are just pointers to the real files that live elsewhere.
The useful ones I see are:
```
/usr/bin/dotnet
/usr/bin/btmon
/usr/bin/apt-get
/usr/bin/bluetoothctl
/usr/bin/btmon
/usr/bin/busctl
/usr/bin/btmgmt ??
/usr/bin/code -> /usr/share/code/bin/code
/usr/bin/csharp ??
/usr/bin/dbus-*
/usr/bin/d-feet
/usr/bin/docker
/usr/bin/dotnet -> /usr/share/dotnet/dotnet
/usr/bin/gattool ??

```
## /usr/share/code/bin/code
Looks like more executables - perhaps these are shared between different users

## /usr/lib
A bunch of directories with files in it, mostly "*.so" files. My understanding is .so file is like a Windows DLL. So these are fittingly libraries.

## /usr/lib/bluetooth
The location for your bluetooth daemon. 
If you execute the command 
```
ps -ax | grep blue
```
you should see the bluetoothd process running.
```
/usr/lib/bluetooth/bluetoothd
```

 #/lib/systemd/system 
 Insider this folder you'd find a long list of `.service` files. This location seems to be where Linux looks to figure out which "services" (daemons?) to start.
 
 The file `/lib/systemd/system/bluetooth.service` looks like this:
 ```ini
 [Unit]
Description=Bluetooth service
Documentation=man:bluetoothd(8)
ConditionPathIsDirectory=/sys/class/bluetooth

[Service]
Type=dbus
BusName=org.bluez
ExecStart=/usr/lib/bluetooth/bluetoothd -d --experimental
NotifyAccess=main
#WatchdogSec=10
#Restart=on-failure
CapabilityBoundingSet=CAP_NET_ADMIN CAP_NET_BIND_SERVICE
LimitNPROC=1
ProtectHome=true
ProtectSystem=full

[Install]
WantedBy=bluetooth.target
Alias=dbus-org.bluez.service
 ```

 Some useful commands that relate to this:
 ```
 systemctl status bluetooth
 systemctl start bluetooth
 systemctl stop bluetooth
 systemctl restart bluetooth
 ```
 If you change one of these files and restart, nothing will happen. You have to manually tell it that the configuration files changed
 ```
 sudo systemctl daemon-reload
 ```