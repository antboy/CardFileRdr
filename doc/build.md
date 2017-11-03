## Notes on how to build the DLL

The code was created in Microsoft's Visual Studio 2015 (community edition) and
is provided here as a VS2015 project (i.e with both C# code and VS2015
configuration files), so the simplest thing to do is to import it into VS2015
or later and build it from there.

## Dependencies
The VS2015 project refers to an snpvtkey.snk file, which is an "Assembly Signature Key Attribute" containing a *private* RSA key, which is used to do the signing for the Strong Naming of the .dll.  This file is not provided with the source code.

To create your own equivalent file, see [Sn.exe (Strong Name Tool)](https://docs.microsoft.com/en-us/dotnet/framework/tools/sn-exe-strong-name-tool).

## Security
### Why sign an assembly?
Apart from the main reason, which is to prevent "dll hell",there is also a small [security contribution](http://www.csharp411.com/net-assembly-faq-part-3-strong-names-and-signing/).

A malicious user would have to replace both the signed assembly, as well as recompile all the assemblies that use it. E.g. sign an assembly called utility.dll to use in our program called myapp.exe. A pirate could acquire the dll and re-sign it, but then myapp.exe and all other applications would fail to bind to the utility.dll because of a signature mismatch. Also, once utility.dll is installed in the [GAC](https://en.m.wikipedia.org/wiki/Global_Assembly_Cache), it cannot be modified or re-signed. Finally, you can use Authenticode or oher certification to prevent re-signing.


### What causes signed assemblies to need to be recompiled?
When myapp.exe calls a method in the utility.dll assembly that has been signed, myapp.exe references not only the method and library names, but the full library signature including the public key token, for example: “Utility, Version=2.0.3855.22908, Culture=neutral, PublicKeyToken=eba64b46725f21db”. Therefore, utility.dll must be resigned with the same private key, else the binding will fail, and myapp.exe will throw an exception.

## References
Sources of information used in creating the source code:

[http://wikivisually.com/wiki/Cardfile](http://wikivisually.com/wiki/Cardfile)

[https://support.microsoft.com/en-us/help/99340/windows-3-1-card-file-format](https://support.microsoft.com/en-us/help/99340/windows-3-1-card-file-format)

[http://lindholm.jp/chpro_sof.html](http://lindholm.jp/chpro_sof.html)

[http://donaldkenney.x10.mx/CARDFILE.HTM](http://donaldkenney.x10.mx/CARDFILE.HTM)
