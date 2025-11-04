# Biomonitor (WinForms)

This repository contains a Windows Forms application called "Biomonitor" used to read EMG data sent over a serial port (from an Arduino) and present/process it on a PC.

## Contents
- `Biomonitor.sln`, `Biomonitor.csproj` — Visual Studio solution and project for the WinForms app.
- `Form1.*` — main form files.
- `bin/`, `obj/` — build artifacts (ignored by .gitignore).
- `../kod_do_sterowania/kod_do_sterowania.ino` — Arduino sketch that samples EMG, smooths it and sends values via serial (if you keep the Arduino folder next to this repository root).

## Prerequisites
- Windows
- Visual Studio (2015 or newer recommended) with .NET desktop development workload
- An Arduino (Uno/Nano/compatible) connected to the PC via USB

## Build & Run
1. Open `Biomonitor.sln` in Visual Studio.
2. Build the solution (Build → Build Solution).
3. Connect the Arduino and determine which COM port it uses (Device Manager → Ports (COM & LPT)).
4. Configure the COM port in the application (either via UI or application settings). The Arduino sketch uses 9600 baud by default.
5. Run the application (Debug → Start Debugging or run the built executable in `bin/Debug/`).

## Serial protocol
- The Arduino sketch prints one integer per line to the serial port (baud: 9600). The integer is the smoothed EMG reading (moving average).
- The PC app should read these newline-terminated integers and map/control UI or actuators as required.

## Arduino sketch
See `h:\Magisterka\kod_do_sterowania\kod_do_sterowania.ino` for the Arduino source. The sketch reads an EMG sensor on analog pin A0, uses a moving average filter, maps values to a servo angle and prints the averaged reading via Serial.

## Troubleshooting
- If no data appears, check the COM port and baud rate (9600).
- Ensure the Arduino is programmed with the included sketch or a compatible one.
- If the UI is unresponsive, make sure serial reading runs on a background thread or the DataReceived event is handled correctly (avoid blocking the UI thread).

## License
Add project license information here if applicable.
