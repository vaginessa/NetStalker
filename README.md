<p align="center">
  <img align="center" src="https://i.imgur.com/4NdcRHF.png" alt="NetStalker">
</p>

![GitHub All Releases](https://img.shields.io/github/downloads/hmz777/NetStalker/total?color=orange&label=downloads&style=flat-square) 
![GitHub release (latest by date)](https://img.shields.io/github/v/release/hmz777/NetStalker?color=yellow&label=latest%20release&style=flat-square) 
![GitHub Release Date](https://img.shields.io/github/release-date/hmz777/NetStalker?color=yellow&style=flat-square)
![GitHub](https://img.shields.io/github/license/hmz777/NetStalker?color=blue&style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/hmz777/NetStalker?color=black&style=flat-square)

### There is a new version in the works with new features, written with Avalonia, MVVM and Reactive UI.
### [NetStalker Avalonia](https://github.com/hmz777/NetStalker.Avalonia)

# NetStalker
A network tool to control the bandwidth over your local network, it can block internet access form any selected device, or limit its speed using packet redirection, in addition, it can log web activity for the targeted device using a built in packet sniffer.

# Features
- Background Scanning for newly connected devices.
- Bandwidth limitation for better distribution of internet speed across devices, both upload and download speeds can be controlled for each device separately.
- A Packet Sniffer that is intended to log addresses that each device on the network visits with the ability to decode Http headers for HTTP packets and resolve domains for HTTPS packets, also the packet direction can be chosen in order to capture requests only or requests and responses.
- A packet viewer to view the properties of a selected packet in the Packet Sniffer with the ability to expand the viewer for better visibility.
- Export the captured packets as a log file with the resolved domains included along with the timestamp for each packet.
- Spoof protection in order not to get spoofed by the same tool or any other spoofing software.
- Dark and light modes.
- Get network card vendor for every device using MacVendors API for better device identification.
- Can be locked with a password.
- Can be minimized to tray if the option is chosen.
- Integration with windows 10 notification system (works from build 17763).
- When minimized it notifies the user of newly discovered devices using Windows 10 notification system with the ability to choose from multiple options on what actions to take.
- Track disconnected devices with a timeout for each device. 

# Changelog
### v4.1-beta
- Fully working dark mode + taking advantage of the immersive dark mode toggle in Windows 10 (See updated images below).
- Add Block All/Redirect All toggles (both of them will also apply to all future detections).
- Add the ability to set friendly names for discovered devices, in order to better identify them (Saved as JSON in the app directory).
- Code refactorings.
- Minor visual improvements.
- Packaged setup included.

### v4.0
- Drop all the UI dependencies and re-create the whole UI from the original controls, in order for the app to be scaled properly by Windows on high DPI screens.
- Remove most icon resource files and store them as ImageList objects.
- Add `MessageBoxForm` dialog for displaying errors and info messages.
- Move hardcoded messages to Resources.

Note: Dark mode isn't fully working yet, due to the UI change.

### v3.0.2
- DPI awareness added (Beta).
- Small visual updates.
- Add the ability to change the MAC Vendors API token to your own in order to avoid getting a 429 while fetching the device's manufacturer.

### v3.0.1
- Fix a KeyNotFoundException when the notification arguments didn't have a Choice key.

### v3.0
- Major performance improvements from upgrading to the new SharpPcap v6.0 that uses the new libpcap driver,
(For a noticeable performance boost, upgrade to the latest Npcap driver (uses the new libpcap driver) (May be required for NetStalker to work properly)).
- Implement the new Microsoft toast notifications API (ver 7.0.2).
- Some code refactorings here and there for better performance and code clarity.
- Give most of the controls meaningful names for better code readability.

### v2.2
- Switched driver from Winpcap to Npcap.
- Npcap driver check added.
- Code refactorings.

### V2.0
- Major bug fixes and performance improvements.
- Updated dependencies.
- Cleaner codebase.
- Most of the codebase is now documented.
- Better threading work.
- Lower CPU consumption.
- Windows 10 toast notifications updated.
- Faster device discovery.
- No need for loading dialogs anymore.
- Packet sniffer can keep running while other devices are being blocked/redirected.
- Upgraded the project to .Net Framework 4.7.2.
- Included a packaged installer.

# Binaries
***Make sure you have the latest Npcap driver before installing NetStalker.***
- [Npcap](https://nmap.org/download.html).
- The latest stable version of NetStalker: [Download Setup Package](https://github.com/hmz777/NetStalker/releases/download/v4/NetStalker_Installer.exe). ***(v4.0)***
- The latest packaged version of NetStalker: [Download Setup Package](https://github.com/hmz777/NetStalker/releases/download/v4.1-beta/NetStalkerInstaller.exe). ***(v4.1 beta)*** (Beta, as in there are some features that i might change later but has the same stability as the non-beta one)

# Notes
- The app uses the [Mac Vendors API](https://macvendors.com/) to retrieve the device's manufacturer, but it only uses the OUI (Organizational Unique Identifier) aka, the first 6 digits of the MAC address.
- The app is tested only on a small amount of network cards, so I can't guarantee it will work on yours.
- The source code may contain experimental features, if you're looking for a stable version, refer to the binaries or the releases section. 

# Caveats
- When releasing a device from redirection, it takes a couple of seconds for this device to regain internet access (Rediscovering the original gateway).

# To Do
- <strike>Add the ability to set friendly names for discovered devices.</strike>
- Redirect IPV6 traffic.
- Make a Linux version. Maybe? 🤔

# Pictures (Might be different depending on your version)

### Main Dark Mode
![Main Dark Mode](https://user-images.githubusercontent.com/38891601/136677615-fb391a51-48c0-4f07-ada9-db284304bd7f.png)

### Main Light Mode
![Main Light Mode](https://user-images.githubusercontent.com/38891601/136677624-c7ad3033-8f56-4292-beb5-a68025744635.png)

### Network Card Selection
![Network Card Selection](https://user-images.githubusercontent.com/38891601/136677683-eea14309-8663-46c6-9e3d-02d1f45cc1b1.png)

### Packet Sniffer
![Packet Sniffer Dark](https://user-images.githubusercontent.com/38891601/136677636-8c7189a7-5e2d-40df-959f-5e025e7c654d.png)

### Speed Limiter
![Speed Limiter Dark](https://user-images.githubusercontent.com/38891601/136677648-a7fcd9de-56f7-4e48-9005-e0d5ac3d6d1d.png)

### Options
![Options](https://user-images.githubusercontent.com/38891601/136677697-38bbfdc7-3856-48aa-b290-db45286de751.png)

# Development Notes
- ~~The project references a modified version of the Windows API Code Pack (see [here](https://stackoverflow.com/questions/54390709/toastactivatorclsid-missing-from-appusermodel)) in order to register the app for the Windows Notifications API.~~
No longer needed since v3.0 (Uses the new toast notifications API).

# Contributions
- Before submitting a pull request, please take sometime to understand how NetStalker and the SharpPcap library work. 

# Issues
- Before creating a new issue please make sure you have the latest release version or the latest compiled version if you're compiling the app your self.
- Include the app version.
- Include the full stacktrace.
- I do not issue setup installers for every release, so if you're having a problem with a previous version try compiling the app your self first.
- Follow the issue template.

# Disclaimer
NetStalker is provided by Hamzi Alsheikh "as is" and "with all faults". The provider makes no representations or warranties of any kind concerning the safety, suitability, lack of viruses, inaccuracies, typographical errors, or other harmful components of this software. There are inherent dangers in the use of any software, and you are solely responsible for determining whether NetStalker is compatible with your equipment and other software installed on your equipment. You are also solely responsible for the protection of your equipment and backup of your data, and the provider will not be liable for any damages you may suffer in connection with using, modifying, or distributing this software.

# Additional Information

If you notice any errors or have a suggestion, you're free to email me or submit a pull request.

# Author
**Hamzi Alsheikh**

**Website: [https://www.hamzialsheikh.tk](https://www.hamzialsheikh.tk)**
