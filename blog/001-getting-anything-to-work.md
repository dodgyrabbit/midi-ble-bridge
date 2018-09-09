# Getting anything to work

* Got the basic console app created and it can build.
* Added DBus library.
* Now need to figure out how to do stuff...

```console
me@machine:~/code/dotnetdbus/netmon$ dotnet dbus list services --bus system
com.redhat.NewPrinterNotification
com.redhat.PrinterDriversInstaller
...
fi.w1.wpa_supplicant1
org.bluez      <----------- GOOD!
...
org.freedesktop.thermald
org.freedesktop.UDisks2
org.freedesktop.UPower
org.gnome.DisplayManager
```
Create the D-Bus objects
```console
me@machine: dodgyrabbit.MidiBle$ dotnet dbus codegen --bus system --service org.bluez
Generated: src/dodgyrabbit.MidiBle/bluez.DBus.cs
```

