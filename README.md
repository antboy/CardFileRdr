## CardFileRdr
A library module in the form of a .dll assembly, that reads a Microsoft Cardfile.  Written in C# (v6).

The MS Cardfile ([Cardfile](http://wikivisually.com/wiki/Cardfile)) is an old app from Windows NT and previous versions, however some still use Cardfile today.  The app was bundled in with the OS, and provided a simple GUI and 'database' in the manner of a manual indexed card file.  The MS app did not provide a means of exporting the data held within it.

Software which uses this .dll assembly include [CardFileExporter](http://github.com/antboy/CardFileExporter) which enables the Cardfile to be exported as XML, and [CardFileKPPlugin](http://github.com/antboy/CardFileKPPlugin) which imports the Cardfile into the [Keepass](http://keepass.info/) password manager.

A limitation of the CardFileRdr.dll is that it ignores contained OLEs and graphic objects such as bitmaps, which are theoretically allowed, so reads only the text contents, but it will process Unicode correctly.

## Installation
Being a library, CardFileRdr.dll is intended to be linked in with an app that uses it to read the cardfile and write it out as some other form.  The app needs to define a Writer object specific to the output form desired and pass it to the library.

So, download the .dll file from the repository Release to the app's project directory and link it as appropriate for your tools.

## Security
In case the cardfile being read contains sensitive information, the following steps have been taken to address security:

### Strongly Named Assembly
The DLL has been strongly named, primarily to get around the potential problems of "DLL Hell".  While this does not provide tight security in itself since it is vulnerable to certain compromises, it does present an obstacle to attackers and therefore contributes somewhat to a "defence in depth" strategy.

A strong name consists of the assembly's identity: its simple text name, version number, plus the author's public key and a digital signature, that together creates a unique DLL.

### Self-Signed X.509 Certificate
The DLL has been self-signed with an X.509 certificate, and a hash (SHA-256) taken of the signed result.

The two main aims of software security are:

Ensuring authenticity: Assures users that they know where the code came from.

Ensuring integrity:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Verifies that the code hasn't been tampered with since its publication.

While a self-signed certificate fails for the first case, since it cannot be independently verified (by a CA authority), it does succeed in ensuring the code has not been tampered with.

The SHA-256 hash of the .dll is:
f7feafa6de3ce4eea034fa6694d01719d2f19488dc3d960ff7ed448217f3e175
