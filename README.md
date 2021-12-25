# files.javiercampos.info
This is the source code for the software used to run the site at [files.javiercampos.info](https://files.javiercampos.info) (a dump of random personal files with no interest whatsoever).

It's a small project made in a few hours during Xmas boredom, to subsitute a very old Apache2 FancyIndexing site I had before.

The source code is presented as-is, and you can use it for any purposes you want. It's made in ASP.NET Core using Razor Pages, in .NET 6.
It uses icons from [https://fileicons.org/ by Daniel Hendriks](https://fileicons.org/), [highlight.js](https://highlightjs.org/) for the source code viewer syntax highlighter, and [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) for an optional simple sqlite access log database. 

Otherwise all code is mine.

## Warning
I've made my best effort to prevent leaking data (using relative folders, etc.), however the program access the files directly and streams down, so there **might** be some security issues I haven't thought of.

On my site, I'm running it inside a docker container on an alpine distribution with an unprivileged account, so other than the files you can actually see there (and the precompiled binaries), there's little you could leak from it, but your mileage may vary: make sure you test for broken stuff if you happen to use any of this code in production anywhere.
