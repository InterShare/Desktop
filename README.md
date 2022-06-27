# InterShare Desktop

<div align="center">
    <img align="center" src="./design/macOS-screenshot.png" width="400" />
</div>

This is the official InterShare desktop app for Linux, macOS and Windows.

## Background

The desktop program for Linux, macOS and Windows is written in C# with the Eto.Forms GUI library. Since the library doesn't have the best documentation, many things in this project are a bit hacky. Mostly because I don't know any better. I originally wanted to program the GUI in xaml as well (which Eto.Forms actually supports), but since there is almost no documentation for it and I couldn't figure out necessary parts like bindings, I decided against it.

So with all these problems, why did I choose Eto.Forms?

I am a big fan of a native GUI experience per platform. Eto is cross-platform and uses the native GUI widgets per platform, and I couldn't find any other library that could do that. (At the time of writing .NET MAUI was still very unstable and without Linux support).

