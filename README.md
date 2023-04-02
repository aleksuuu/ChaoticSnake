# Chaotic Snake!

A Windows-98-themed snake game with portals, power-ups, and scary (and very real!) pop-up warnings! Also my MTEC-340 mid-term project. You can download a build for macOS here: [https://aleksuuu.itch.io/chaotic-snake](https://aleksuuu.itch.io/chaotic-snake). 

## Notes for David

The AudioManager is only implemented in the TitleScene with a DoNotDestroy, so in order to hear the soundtrack and sound effects, the game must be played from the TitleScene. I am using one AudioManager for both scenes because I want the audio to fade out when the player switches from the TitleScene to PlayScene (and not cut off, which produces a click), meaning that I cannot destroy the AudioManager object that is part of the TitleScene when I am in PlayScene.

The fake pop-ups rarely show up. If you’d like to experience them, you might have to wait a while.