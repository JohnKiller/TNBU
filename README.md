# TNBU (Reverse UBNT Ubiquiti network protocol)

This repository contains:
- Library for decoding and decrypting Ubiquiti proprietary network (discovery + inform)
- MitM tool to inspect traffic between devices and controller
- Lightweight controller (with very basic functionality)

Pull requests are welcome.

## TNBU.Core

This is the library that contains the decryption/encryption utilities for discovery and inform traffic. You can use this to build other things like alternative controllers or custom devices.

## TNBU.MitM

This application will listen for discovery packets from physical devices and spoofs them to a real Unifi controller. When you adopt the fake device in the controller, all the requests will be forwarded to the real device, logging all the commands and traffic to a file for later inspection.

This is useful for debugging and also for knowing what to implement to make the custom controller work.

Help in this area is very appreciated: if you have a device not currently supported, please submit a bug report with the logs, so I can take a look.

## TNBU.GUI

Alternative controller implementation. At this time it can only adopt access points (tested with UAP-AC-LR, UAP-nanoHD and U6-LR) and set up a WPA2 network. *Please don't adopt a switch*: although you will see the device listed, the adoption will actually work but the configuration will be completely borked, and you will need to manually reset the device.
