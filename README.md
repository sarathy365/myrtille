# Myrtille
Myrtille provides a simple and fast access to remote desktops, applications and SSH servers through a web browser, without any plugin, extension or configuration.

Technically, Myrtille is an HTTP(S) to RDP and SSH gateway.

## How does it work?
User input (keyboard, mouse, touchscreen) is forwarded from a web browser to an HTTP(S) gateway, then up to an RDP (or SSH) client which maintains a session with an RDP (or SSH) server.

The display resulting (or not) of such actions is streamed back to the browser, from the RDP (or SSH) client and through the gateway.

The implementation is quite straightforward to maintain speed and stability.
Some optimizations, such as input buffering and display quality tweaking help mitigate latency and bandwidth issues.

More info in the DOCUMENTATION.md file.

## Features
- HTTP(S) to RDP and SSH gateway (new in version 2.0.0)
- Hyper-V VM direct connection
- Multi-factor authentication (MFA)
- Active Directory integration (management of hosts)
- Session sharing (collaborative mode)
- Start remote programs from their URLs
- File transfer (local and roaming accounts)
- PDF Virtual Printer
- Audio support
- HTML4 and HTML5 support
- Responsive design
- Clipboard syncing
- PNG, JPEG and WebP compression
- Realtime connection-info
- On-screen- console, logfile, debug-info
- On-screen keyboard (multiple languages)
- REST APIs (i.e.: hide connection info from the browser, tracks connections, monitor remote sessions, etc.)
- Fully parameterizable

## Requirements
- Browser: any HTML4 or HTML5 browser (starting from IE6!). No extension or administrative rights required. The clipboard synchronization requires Chrome (or async clipboard API support) and HTTPS connection
- Gateway (myrtille): Windows Server OS (2012 R2 or greater, with IIS 8.0+, .NET 4.5+ and WCF/HTTP activation enabled). CAUTION! IIS on Windows client OSes (7, 8, 10 - all versions) is limited to 10 simultaneous connections only - across all http sessions - and will hang after that!
- RDP server: any RDP enabled machine (preferably Windows Server but can also be Windows XP, 7, 8, 10 or Linux xRDP server)
- SSH server: any SSH server (tests were made using the built-in Windows 10 OpenSSH server)

## Resources
Myrtille supports multiple connections/tabs (can be disabled in web.config, as per the comments there).

The maximal number of concurrent users is not limited besides what the RDP (or SSH) server(s) can handle (number of CALs, CPU, RAM?).

Regarding the gateway, a simple dual-core CPU with 4 GB RAM can handle up to 50 simultaneous sessions (about 50MB RAM by rdp client process, even less for ssh).

A session uses about 200 KB/s of bandwidth on average. 1 MB/s per user is a good provision for most cases. What's really important to Myrtille is the outgoing bandwidth, as display updates will take up most of the traffic.

## Build
Microsoft Visual Studio 2017 or greater. See DOCUMENTATION.md.

## Installation
All releases here: https://github.com/cedrozor/myrtille/releases

See DOCUMENTATION.md for more details.

## Docker

From version 2.8.0, Myrtille is available as a Docker image.

You can pull it from Docker Hub with the following command (use a tag to get any specific version other than the latest)<br/>
```
docker pull cedrozor/myrtille(:tag)
```

Run the image in detached mode (optionally providing the resulting container a network adapter able to connect your hosts)<br/>
```
docker run -d (--network="<network adapter>") cedrozor/myrtille(:tag)
```

See DOCUMENTATION.md for more details.

## Remote Desktop Services

This is the primary requirement for RDP connections. Please read DOCUMENTATION.md for more on the RDS role and features, and how to best configure it for Myrtille.

## Usage
Once installed on your server, you can use Myrtile at http://myserver/myrtille. Set the RDP (or SSH) server address, user domain (if any, for RDP), name and password then click "Connect" to log in, and "Disconnect" to log out. You can pre-configure connections for one-click access from the dashboard for managing hosts.

Multi-factor authentication (MFA) and Active Directory integration (Enterprise Mode) are both off by default. The documentation will help you turn on these features.

You can connect a remote desktop and **start a program automatically from a URL** (see DOCUMENTATION.md). From version 1.5.0, Myrtille does support encrypted credentials (aka "password 51" into .rdp files) so the URLs can be distributed to third parties without compromising security.

The installer lets you optionally create a self-signed certificate for https://myserver/myrtille. Like for all self-signed certificates, you will have to add a security exception in your web browser (just ignore the warning message and proceed to the website). **Using HTTPS is recommended** to secure your remote connection.
Of course, you can avoid that by installing a certificate provided by a trusted Certification Authority (see DOCUMENTATION.md).

If you want connection info, turn on "Stat" (displayed on-screen or in the browser console). If you want debug info, turn on "Debug" (most traces are turned off (by being commented out) in the .js files, but can be turned on (by uncommenting them) as needed).

You can also choose HTML4 or HTML5 rendering mode, (HTML4 may be useful, for example, if websockets are blocked by a proxy or firewall).

On touchscreen devices, you can pop out the device keyboard with the "Keyboard" button.
Then enter some text and click "Send". This can be used, for example, to paste the local clipboard content and send it to the server (then it be copied from there, within the remote session).
Alternatively, you can run **osk.exe** (the Windows on screen keyboard, located into %SystemRoot%\System32) within the remote session.
It can be started automatically opon beginning a Windows session (https://www.cybernetman.com/kb/index.cfm/fuseaction/home.viewArticles/articleId/197).

The remote clipboard content can also be retrieved locally with the "Clipboard" button (text format only).

You can upload/download file(s) to/from the user documents folder with the "Files" button.
Note that it requires the RDP server to be localhost (same machine as the HTTP server) or a domain to be specified. Not available for SSH.

You can print any document on a local or network printer by using the "Myrtille PDF" (redirected) virtual printer.
Simply use the print feature of your application, then open/print the downloaded pdf.

From version 2.1.0, you can connect an Hyper-V VM directly (console session). It can be useful if remote desktop access is not enabled on the VM (i.e: Linux VMs), if the VM doesn't have a network connection (or is on a different network for security reasons, or use DHCP) or simply to be able to connect the VM during system startup or shutdown.
See [notes and limitations](https://github.com/cedrozor/myrtille/blob/master/DOCUMENTATION.md#notes-and-limitations) for info to connect an Hyper-V VM and the differences with a standard RDP connection.

## Third-party
Myrtille uses the following libre software:

- RDP client: [FreeRDP](https://github.com/FreeRDP/FreeRDP), [licensed Apache 2.0](https://github.com/FreeRDP/FreeRDP/blob/master/LICENSE). Myrtille uses a fork of FreeRDP (https://github.com/cedrozor/FreeRDP), to enforce a loose coupling architecture and always use the latest version of FreeRDP (the fork is periodically synced with the FreeRDP master branch).
- [OpenSSL toolkit 1.0.2n](https://github.com/openssl/openssl), [licensed Apache 2.0](https://github.com/openssl/openssl/blob/master/LICENSE.txt). Precompiled versions of OpenSSL can be obtained here: https://wiki.openssl.org/index.php/Binaries.
- WebP encoding: libWebP 0.5.1 (https://developers.google.com/speed/webp/), licensed in a [BSD-like fashion](https://chromium.googlesource.com/webm/libwebp/+/refs/heads/main/COPYING).
- Logging: [Log4net 2.0.8](https://logging.apache.org/log4net/), [licensed Apache 2.0](https://github.com/apache/logging-log4j2/blob/master/LICENSE.txt).
- Multi-factor authentication: [OASIS.Integration 1.6.1](https://www.nuget.org/packages/OASIS.Integration/1.6.1), [licensed Apache 2.0](https://github.com/OliveInnovations/OASIS/blob/master/LICENSE). Source code available at https://github.com/OliveInnovations/OASIS. Copyright Olive Innovations Ltd 2017.
- PDF Virtual Printer: [PdfScribe 1.0.5](https://github.com/stchan/PdfScribe), [licensed AGPLv3](https://github.com/stchan/PdfScribe/blob/master/Common/agpl-3.0.rtf).
- Redirection Port Monitor: RedMon 1.9 (http://pages.cs.wisc.edu/~ghost/redmon/index.htm), [licensed GPL v3](https://github.com/ARLM-Keller/redmon/blob/master/LICENCE.TXT).
- SSH client: SSH.NET 2016.1.0 (https://github.com/sshnet/SSH.NET/), [licensed MIT](https://github.com/sshnet/SSH.NET/blob/develop/LICENSE).
- HTML Terminal Emulator: [xtermjs](https://github.com/xtermjs/xterm.js/), [licensed MIT](https://github.com/xtermjs/xterm.js/blob/master/LICENSE).
- WAV audio support: [NAudio](https://github.com/naudio/NAudio), [licensed MIT](https://github.com/naudio/NAudio/blob/master/license.txt).
- MP3 audio support: [NAudio.Lame](https://github.com/Corey-M/NAudio.Lame), [licensed MIT](https://github.com/Corey-M/NAudio.Lame/blob/master/LICENSE.txt).
- MP3 audio support: [Lame](https://lame.sourceforge.net/), [licensed LGPLv2](https://svn.code.sf.net/p/lame/svn/trunk/lame/COPYING).
- Remote Desktop Services API wrapper: [Cassia](https://github.com/danports/cassia), [licensed MIT](https://github.com/danports/cassia/blob/master/LICENSE).
- On-Screen keyboard: [Simple-Keyboard](https://github.com/hodgef/simple-keyboard), [licensed MIT](https://github.com/hodgef/simple-keyboard/blob/master/LICENSE).
- Draggable popups: [interact.js](https://github.com/taye/interact.js), [licensed MIT](https://github.com/taye/interact.js/blob/main/LICENSE).

Proprietary (un-libre), conflicting or unknown terms:

- Postscript Printer Drivers: [Microsoft Postscript Printer Driver V3](https://docs.microsoft.com/en-us/windows-hardware/drivers/print/microsoft-postscript-printer-driver), copyright (c) Microsoft Corporation. All rights reserved.
- Postscript and PDF interpreter/renderer: [Ghostscript 9.23](https://www.ghostscript.com/download/gsdnld.html), [licensed AGPLv3 and GPLv3](https://git.ghostscript.com/?p=ghostpdl.git;a=blob_plain;f=LICENSE;hb=HEAD), and the incompatible terms in [a custom license](https://git.ghostscript.com/?p=ghostpdl.git;a=blob_plain;f=jpegxr/COPYRIGHT.txt;hb=HEAD).
- HTML5 websockets: Microsoft.WebSockets 0.2.3.1 (https://www.nuget.org/packages/Microsoft.WebSockets/0.2.3.1), licensed MS-.NET-Library-JS License ](https://www.microsoft.com/web/webpi/eula/net_library_eula_enu.htm). Pending replacement in Myrtile by [SignalR](https://github.com/SignalR/SignalR), [Licensed Apache 2.o](https://github.com/SignalR/SignalR/blob/main/LICENSE.txt).


See DISCLAIMERS.md file.

The Myrtille code in FreeRDP is surrounded by region tags "#pragma region Myrtille" and "#pragma endregion".

libWebP is supplied as the official Google's WebP precompiled binaries, left unmodified.

## License
Myrtille is licensed Apache 2.0.
See the LICENSE file.

## Author
Cedric Coste.
- Website:  https://www.cedric-coste.com
- LinkedIn:	https://fr.linkedin.com/in/cedric-coste-a1b9194b
- Twitter:	https://twitter.com/cedrozor
- Facebook:	https://www.facebook.com/profile.php?id=100011710352840

## Contributors
- Catalin Trifanescu (AppliKr developer: application server. Steemind cofounder)
- Fabien Janvier (AppliKr developer: website css, clipping algorithm, websocket server)
- UltraSam (AppliKr developer: rdp client, http gateway)
- Paul Oliver (Olive Innovations Ltd: MFA, enterprise mode, SSH terminal)

## Sponsors

- Blackfish Software (http://www.blackfishsoftware.com/) — the makers of IE Tab - swipe on touchscreen devices
- ElasticServer (http://www.elasticserver.co/) — print a remote document using the browser print dialog
- Coduct GmbH (https://www.coduct.com/) - reconnect on browser resize, keeping the display aspect ratio
- Practice-Labs (https://practice-labs.com/) — audio support, REST APIs, improved iframes integration
- Schleupen AG (https://www.schleupen.de/) — clipboard synchronization, disconnect API, drain of disconnected sessions
- Microarea SpA (https://www.microarea.it/) — application pool API, reduced memory usage
- Arkafort (https://www.arkafort.com) — improved Hyper-V console support, on-screen keyboard
- Your company here (contact me!)

## Fun

Ever wanted to run Myrtille in your Tesla supercar? :) https://www.youtube.com/watch?v=YwNlf6Bm_so

## Links
- Website:	https://www.myrtille.io (support & consulting services)
- Source:	https://github.com/cedrozor/myrtille
- Tracker:	https://github.com/cedrozor/myrtille/issues
- Wiki:		https://github.com/cedrozor/myrtille/wiki
- Forum:	https://groups.google.com/forum/#!forum/myrtille_rdp (community)
- Donate:	https://www.paypal.me/costecedric
