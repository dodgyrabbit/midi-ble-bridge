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

## What useful objects do we see?

```console
me@machine: src/dodgyrabbit.MidiBle$ dotnet dbus list objects --bus system --service org.bluez
/ : org.freedesktop.DBus.ObjectManager
/org/bluez : org.bluez.AgentManager1 org.bluez.ProfileManager1
/org/bluez/hci0 : org.bluez.Adapter1 org.bluez.GattManager1 org.bluez.LEAdvertisingManager1 org.bluez.Media1 org.bluez.NetworkServer1
/org/bluez/hci0/dev_65_3D_FA_DE_2F_B3 : org.bluez.Device1
/org/bluez/hci0/dev_67_49_10_09_B7_A0 : org.bluez.Device1
/org/bluez/hci0/dev_6E_BD_4D_C2_CC_94 : org.bluez.Device1
/org/bluez/hci0/dev_77_A9_45_16_57_1A : org.bluez.Device1
/org/bluez/hci0/dev_7A_73_C4_42_58_09 : org.bluez.Device1
/org/bluez/hci0/dev_A0_6F_AA_3A_76_57 : org.bluez.Device1
```

I guess **org.bluez.GattManager1** looks interesting...


