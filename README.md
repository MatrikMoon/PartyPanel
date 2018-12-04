# PartyPanel
## A Beat Saber plugin which spawns a control panel the user can use to launch songs

Feel free to contribute by adding miscellaneous options (such as setting the player height from the command window)!
PR's accepted! :)

TODO:
1. Label the OSTs. Right now it's impossible to tell between OneSaber and NoArrows and standard.

Known issues:
1. When you drag the window, the game will look like it's frozen to your friend in VR. Blame Windows Forms.
2. Large song libraries will take a long time to load, thanks to having to load all the images. This also takes RAM. We can *possibly* circumvent this if we use whatever SongLoader already has loaded, but I haven't looked into it yet.
3. It is currently requiring some unnecessary dependencies.
