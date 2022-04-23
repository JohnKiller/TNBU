# TNBU (Reverse UBNT Ubiquiti network protocol)

This repository contains:
- Library for decodifing and decrypting Ubiquiti proprietary network (discovery + inform)
- MitM tool to inspect traffic between devices and controller
- Fake device simulation
- Lightweight controller with very basic functionality

Pull requests are welcome.

## TNBU.Core

This project contains only the decryption and encryption utilities for discovery and inform traffic. You can use this to build other things like alternative controllers or custom devices.

## TNBU.MitM

This application will listen for discovery packets from devices and spoofs them to a controller. When you adopt the fake device in the controller, all the requests will be forwarded to the real device, logging all the commands and traffic to a file for later inspection.

This is useful for debugging and also for knowing what to implement to make the custom controller work.

## TNBU.Fake.AP

This will broadcast a fake AP that you can adopt. Useful for creating custom hardware and managin it on Unifi console.

## TNBU.Fake.SW

Same as Fake.AP but for switches.

## TNBU.Controller

Alternative controller implementation.
