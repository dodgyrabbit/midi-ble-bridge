# org.freedesktop.DBus.Error.AccessDenied

This error has taken me a long time to understand.

The cryptic error message is
```
org.freedesktop.DBus.Error.AccessDenied
```

It happens as soon as my program responds to 
```
Type=method_call  Endian=l  Flags=1  Version=1  Priority=0 Cookie=435
  Sender=:1.534  Destination=:1.592  Path=/org/bluez/example/service0/characteristic0  Interface=org.bluez.GattCharacteristic1  Member=StartNotify
  UniqueName=:1.534
  MESSAGE "" {
  };
```

with 
```
Type=method_return  Endian=l  Flags=3  Version=1  Priority=0 Cookie=8  ReplyCookie=435
  Sender=:1.592  Destination=:1.534
  UniqueName=:1.592
  MESSAGE "" {
  };
<<<<<<< Updated upstream
```

The error is
```
Type=error  Endian=l  Flags=1  Version=1  Priority=0 Cookie=3  ReplyCookie=8
  Sender=org.freedesktop.DBus  Destination=:1.592
  ErrorName=org.freedesktop.DBus.Error.AccessDenied  ErrorMessage="Rejected send message, 1 matched rules; type="method_return", sender=":1.592" (uid=1000 pid=16755 comm="/usr/bin/dotnet /home/pieterventer/code/midi-ble-b" label="unconfined") interface="(unset)" member="(unset)" error name="(unset)" requested_reply="0" destination=":1.534" (uid=0 pid=5898 comm="/usr/lib/bluetooth/bluetoothd -d --experimental " label="unconfined")"
  MESSAGE "s" {
          STRING "Rejected send message, 1 matched rules; type="method_return", sender=":1.592" (uid=1000 pid=16755 comm="/usr/bin/dotnet /home/pieterventer/code/midi-ble-b" label="unconfined") interface="(unset)" member="(unset)" error name="(unset)" requested_reply="0" destination=":1.534" (uid=0 pid=5898 comm="/usr/lib/bluetooth/bluetoothd -d --experimental " label="unconfined")";
  };
```

What could be the problem here?
Did it not actually expect a reply?
=======
```
>>>>>>> Stashed changes
