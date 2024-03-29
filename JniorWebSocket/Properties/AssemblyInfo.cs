﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("JniorWebSocket")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("JniorWebSocket")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d319d09e-38f1-499b-80b5-719f87b2ee1e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

// 2.0
//    added MessageReceived event 
//    added PostMessage Message Class
//    fixed issue where Authenticated event was getting called every time a Montior packet was received.
//    added a RegistryRead Message
//    added a Query method that will wait for a response.This will check to see if the object being sent contains a Meta Hash value
//    made the Query method an async method so that it does not block on the UI thread

[assembly: AssemblyVersion("2.0.*")]
[assembly: AssemblyFileVersion("2.0.0.0")]
