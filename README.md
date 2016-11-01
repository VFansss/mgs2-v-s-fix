mgs2 v's fix
=========================================

What is :
-------------------

Amateur fix that aim to correct all minor (and mayor) flaws in Metal Gear Solid 2 for PC.

Features :
-------------------

* Setupper builded from scratch!
* Automatic 2.0 official patcher! (thanks year-2000 Konami!)
* Set up your custom resolution in a matter of seconds
* 16:9 and 16:10 widescreen support (thanks to nemesis2000)
* Audio fix automatically applied (Thanks Creative labs!)
* Automatic fix for every model of vga adapter (Nvidia or ATI or Intel)
* Semi-automatic laptop fix needed (there are some conflicts with internal gpu adapter)
* Xbox360 controller supported **(Please check below section)**
* Graphic setup, if you have a crappy pc and can't affort to max out a 15 years old game.
* Less than 9 MB.
* You don't like it? Change it yourself!

[Screen 1](http://i.imgur.com/7JDZxfi.jpg)
[Screen 2](http://i.imgur.com/NgkerMJ.jpg)

Download Link :
-------------------

**.net Framework 4.5 required**

[Compiled version in 1 November '16 by VFansss](http://bit.ly/2fzYPaI)

**Xbox 360 Gamepad Support**:

I can't publish library to do automatic support of it (Can't contact the author of the library).

- Download [Xinput Plus](https://sites.google.com/site/0dd14lab/xinput-plus) (First blue link on top)

- Open XInputPlus.exe

- Select "Target Program" and select mgs2_see from inside the target program

- Press apply

- Enable '360 Controller' inside V's Fix and press save. All should work like expected.

Concise Guide :
-------------------

- FOR BETTER RESULT: DO A CLEAN INSTALLATION OF THE GAME! DON'T APPLY **ANYTHING** AFTER INSTALLING!
Isn't 100% important BUT BETTER SAFE THAN SORRY!

- Download it (or compile it, your choice).
- Overwrite MGS2SSetup.exe inside the "GAME DIRECTORY/BIN" folder
- Open it!

ToDo List :
-------------------
- Add anti-aliasing
- Add dualshock ps2/ps3 analog button
- Restore original Konami Leaderboard
- Add graphical button layout
- Add Mouse/Keyboard key setupper
- VR Mission First Person support

Developer Clarifications :
-------------------

- Code is heavily commented.

- Code can be optimized a little, in some case. I didn't do that for reasons of time.

- Inside MANUAL folder you will find other useful guidelines.

- Project builded with Visual Studio Pro 2013.

Known Bug :
-------------------

- With Widescreen Fix can happen strange video bug with VR Mission select. Unfortunatelly I can't fix it.

- With Widescreen Fix, bitmap element will remeain stretched. (It's obliviously, but better say it)

- On Tanker chapter the marine commandant speaking video isn't visible or has a green screen on it. It's caused by some Dx8 api that don't work good on recent OS. Even original game has it. The only way to fix it is to play on Windows XP without SP2 (cause SP2 I think will upgrade some Dx library and break something). I didn't have found a way to fix it. If someone know how to deal with that please let me know.

License :
-------------------
Copyrights of the launcher belong to... me.
Copyright © 2016 Alessio Di Giacomo (VFansss)

Copyrights for each of these extenal library belong to the respective owner:

nemesis2000
[http://ps2wide.net/pc.html](http://ps2wide.net/pc.html)
[http://forums.pcsx2.net/User-nemesis2000](http://forums.pcsx2.net/User-nemesis2000)

You made a big effort.
Kudos to you.
