# Registering Application Trace

The following is a breakdown of the example-gatt-server.py trace call that happens **after** the call to `RegisterApplication`. The object at Path / is being called on method `GetManagedObjects`.

## GetManagedObjects

```json
    ‣ Type=method_call  Endian=l  Flags=0  Version=1  Priority=0 Cookie=1316
    Sender=:1.228  Destination=:1.524  Path=/  Interface=org.freedesktop.DBus.ObjectManager  Member=GetManagedObjects
    UniqueName=:1.228
    MESSAGE "" {
    };
```
## Response

The simplified reponse


```
Dictionary[ObjectPath, Dictionary]
                        |
                      [string, Dictionary]
                                   |
                                [string, Variant]
```

Using this we can contstruct a simplified view:

["/org/bluez/example/service0/char0",]

```json
{
    "/org/bluez/example/service0/char0" : {
        "org.bluez.GattCharacteristic1" : {
            "Descriptors" : [],
            "Flags" : ["Notify"],
            "UUID" : ["00002a37-0000-1000-8000-00805f9b34fb"],
            "Service" : "/org/bluez/example/service0"
        }
    }
}
```

```json
    ‣ Type=method_return  Endian=l  Flags=1  Version=1  Priority=0 Cookie=8  ReplyCookie=1316
    Sender=:1.524  Destination=:1.228
    UniqueName=:1.524
    MESSAGE "a{oa{sa{sv}}}" {
            ARRAY "{oa{sa{sv}}}" {
                    DICT_ENTRY "oa{sa{sv}}" {
                            OBJECT_PATH "/org/bluez/example/service0/char0";
                            ARRAY "{sa{sv}}" {
                                    DICT_ENTRY "sa{sv}" {
                                            STRING "org.bluez.GattCharacteristic1";
                                            ARRAY "{sv}" {
                                                    DICT_ENTRY "sv" {
                                                            STRING "Descriptors";
                                                            VARIANT "ao" {
                                                                    ARRAY "o" {
                                                                    };
                                                            };
                                                    };
                                                    DICT_ENTRY "sv" {
                                                            STRING "Flags";
                                                            VARIANT "as" {
                                                                    ARRAY "s" {
                                                                            STRING "notify";
                                                                    };
                                                            };
                                                    };
                                                    DICT_ENTRY "sv" {
                                                            STRING "UUID";
                                                            VARIANT "s" {
                                                                    STRING "00002a37-0000-1000-8000-00805f9b34fb";
                                                            };
                                                    };
                                                    DICT_ENTRY "sv" {
                                                            STRING "Service";
                                                            VARIANT "o" {
                                                                    OBJECT_PATH "/org/bluez/example/service0";
                                                            };
                                                    };
                                            };
                                    };
                            };
                    };
                    DICT_ENTRY "oa{sa{sv}}" {
                            OBJECT_PATH "/org/bluez/example/service0";
                            ARRAY "{sa{sv}}" {
                                    DICT_ENTRY "sa{sv}" {
                                            STRING "org.bluez.GattService1";
                                            ARRAY "{sv}" {
                                                    DICT_ENTRY "sv" {
                                                            STRING "Characteristics";
                                                            VARIANT "ao" {
                                                                    ARRAY "o" {
                                                                            OBJECT_PATH "/org/bluez/example/service0/char0";
                                                                    };
                                                            };
                                                    };
                                                    DICT_ENTRY "sv" {
                                                            STRING "UUID";
                                                            VARIANT "s" {
                                                                    STRING "0000180d-0000-1000-8000-00805f9b34fb";
                                                            };
                                                    };
                                                    DICT_ENTRY "sv" {
                                                            STRING "Primary";
                                                            VARIANT "b" {
                                                                    BOOLEAN true;
                                                            };
                                                    };
                                            };
                                    };
                            };
                    };
            };
    };
```