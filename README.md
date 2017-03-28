# Chihya.Tempo

Tempo detector, compatible with .NET core.

## Features

- Energy-based tempo detector.

Supports:

- .NET Standard 1.6+
- .NET Core 1.0+

## Usage

An example is given in `Chihya.Tempo.Test`.

```csharp
// Read the audio data and metadata from a wave audio.
var wav = WaveReader.ReadWaveFile(fileName);
// Use a config preset.
var config = EnergyTempoDetectorConfig.For44KHz;
// Create a tempo detector using known information.
var detector = new EnergyTempoDetector(wav.data, wav.properties, config);
// Detect tempo.
var tempo = detector.Detect();
// Print out the result. BPM = 0 means detection failed.
Console.WriteLine($"Starts from {tempo.BeatStart}, BPM is {tempo.BeatsPerMinute:0.00}");
```

Example output:

```plain
File: /mnt/c/Users/Administrator/Desktop/no9/No.9.wav
Starts from 00:00:00.0230000, BPM is 144.00
```

## Building

Visual Studio 2017 is required.

Or, use the `dotnet` command:

```bash
# Restore dependencies.
dotnet restore

# $runtime can be: win7-x86 win7-x64 osx.10.10-x64 ...
# Ref: https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog
# For a runtime list, please see the .csproj files.
dotnet build -c Release -r $runtime

# $framework can be: netcoreapp1.0
# See the .csproj files.
dotnet publish -f $framework -c Release -r $runtime
```

`Chihya.Tempo` also supports .NET Standard. If you only need this library, you can run:

```bash
dotnet publish -c Release -r $runtime -f netstandard1.6 Chihya.Tempo/Chihya.Tempo.csproj
```

Then you will see built binaries at `bin/Release/$framework`, and published binaries at `bin/Release/$framework/$runtime` of each project.

You should probably restore and publish on Windows. `dotnet build` and `dotnet publish` caused several problems on over platforms. Don't worry,
once published with native binaries, it can run on target platform. For example, publish on Windows and run on Ubuntu 14.04 (x64).

It is tested on Windows 10 and [WSL](https://en.wikipedia.org/wiki/Windows_Subsystem_for_Linux).

## What is "Chihya"?

![ちひゃー](http://www.project-imas.com/w/images/1/15/Chihya-Puchi.jpg)

A [petit idol](http://www.project-imas.com/wiki/Chihyaa) who loves singing as much as its
owner [Chihaya](http://www.project-imas.com/wiki/Chihaya_Kisaragi).

Please come and crouch on my head! (No, I'm not talking to you, [headcrab](https://en.wikipedia.org/wiki/Headcrab)!)

Maybe you'll ask, where is the missing "a" (the standard romaji spelling is "Chihyaa")?
Forget about it. The pronouciation does not differ much with or without that "a".

## License

GNU LGPL 2.1 (as in Accord.NET)

See [LICENSE.md](LICENSE.md) for more information.

> **Chihya.Tempo**
> 
> Copyright (C) 2017 George Wu &lt;uiharu@buaa.edu.cn&gt;
>
> This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.
>
> This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.
> 
> You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

## TODO

As you can see, this project has no traditional .NET framework support yet.
This is because I am new to .NET Core / .NET Framework hybrid development.
It is very appreciated if someone can help me out.
