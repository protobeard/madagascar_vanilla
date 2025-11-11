# Madagascar Vanilla

The best vanilla.

## Why?

For those of us with literally thousands of hours in RimWorld, it can be a pain to reconfigure each new game. This mod aims to reduce the overhead of starting new games by adding settings for things like: choosing manual work priorites, auto-expanding home zone, auto-rebuilding destroyed buildings, etc..

As well, there were a number of quality of life and "house rules" small changes which I wanted to make to RimWorld.

Note: everything is set to RimWorld defaults and must be enabled in the mod settings to take effect.

## What does it do exactly?

### General

* Verbose Mode: make this mod log a lot. Probably you should leave this off. Almost certainly. Unless you're trying to debug something.

### Bug Fixes (Well, Kinda...)

* Disable pawns changing clothes while bleeding
* Display Correct Milk Type on Animal Fullness Labels: Boomables produce chemfuel, not milk.

### (Very Small) Features

* Add Production Specialist Only work restriction option to bills
* Add Inspired Pawn Only work restriction option to bills
* Show Traits in Outfit Assignment: just like for Drug Policies. Modders note: what traits count as "outfit relevant" can be extended via XML patches.
* Alert for pawns sleeping alone who don't want to be.

### Quality of Life

* Default Hostility Response: set a default value for pawn hostility response.
* Allow Goodwill Reward: enable or disable goodwill rewards from factions.
* Allow Honor Reward: enable or disable honor rewards from factions.
* Override Default Schedules: enable custom schedules to be assigned to different types of pawns on recruitment: Never Sleep, Night Shift, Biphasic, and Day Shift. These schedules are assigned like so:
    * Never Sleep to BodyMastery and Never Sleep gene pawns
    * Night Shift to NightOwl and UV Sensitive pawns
    * Biphasic to Very Sleepy and Sleepy gene pawns
    * Day Shift to Low Sleep gene and "Normal" pawns
* Automatically Expand Home: enable or disable automatically expanding the Home area.
* Auto Rebuild in Home: enable or disable automatically rebuilding in the Home area.
* Starting Areas: provide a list of areas which will be automatically created on each new game (e.g., "Work, Inside, Safe").
* Use Manual Priorities: enable or disable using manual work priorities.
* Bill Repeat Mode: enable Repeat Count (Do X Times), Target Count (Do Until X), and Forever default for new production bills.
    * If Target Count is selected, enable default hitpoint range for items to count.
    * If Target Count is selected, enable default quality range for items to count.
* Bill Storage: enable Drop on Floor or Take to Best Stockpile default for new production bills.
* Bill Ingredient Search Radius: set a default ingredient search radius for new production bills.
* Tailoring:
    * Disable cloth in recipes
    * Disable valuble textiles in recipes (devilstrand, hyperweave, synthread, thrumbofur, thrumbomane). Modders note: what counts as "valuable" can be extended via XML patches.
    * Disable mood impacting textiles in recipes (human leather, dread leather). Modders note: what counts as "mood altering" can be extended via XML patches.
* Crematorium: disable cremating colonist corpses by default.
* Enable Mech Repair by Default: Yup.
* Landmark Visibility: enable or disable Odyssey landmark visibility on the world map.
* Auto Cut by Default: enable auto cutting on windmills and animal pens.
* Auto Strip Prisoners: enable or disable automatically stripping prisoners upon capture.
* Auto Strip Arrested Colonists: enable or disable automatically stripping colonists upon arrest.
* Hide Learning Helper Button: In case you don't want to look at it.
* Medical Defaults Persist Across Games: In case you don't want to set these up every game.
* Apparel Policies Persist Across Games: In case you don't want to set these up every game.
* Drug Policies Persist Across Games: In case you don't want to set these up every game.
* Food Policies Persist Across Games: In case you don't want to set these up every game.
* Reading Policies Persist Across Games: In case you don't want to set these up every game.
* Persist New Game Configuration: save new game setup (storyteller, difficulty, world size, etc.) and reuse it for the next new game.
* Shelf-like Storage:
    * Set default storage to be empty and priority normal for shelves, outfit stands, bookcases, and hoppers (each individually toggleable).
    * Disable storing rotten items on shelves/outfit stands by default
    * Disable storing tainted items on shelves/outfit stands by default
    * Disable storing biocoded weapons and apparel on shelves/outfit stands by default
* Stockpile Storage:
    * Disable storing rotten items in stockpiles by default
    * Disable storing tainted items in stockpiles by default
    * Disable storing biocoded weapons and apparel in stockpiles by default
* Dumping Stockpile Storage:
    * Disable storing rotten items in dumping stockpiles by default (prevent pawns from bringing in rotting animal corpses)

## Balance

This mod only contains strict quality of life features. By that I mean that I don't believe any reasonable person could argue that any feature included in this mod gives the player any advantage. Most options simply reduce tedious tasks that must be done each new game, or make tasks which are possible simpler to execute (e.g., the "Inspired Pawn Only" option on production bills -- this can be accomplished manually in vanilla by waiting until a pawn is inspired, then assigning the bill to that pawn. My feature just makes that process a bit less tedious.).

Please let me know if you disagree about any feature. We may agree to disagree, but that's life.

Also, note that I have nothing against mods which give the player advantages, I just want *this* mod to be focused on things that don't. Please see my mod House Rules for options that range a bit further outside quality of life.

## Known Issues

* Errors will be thrown on game launch if mods which add persisted items have been removed. For example, if persist storytellers is enabled and a custom storyteller such as Perry Persistent is selected, then the mod which adds that custom storyteller is removed, on next game launch you will see an error like:

`Could not load reference to RimWorld.StorytellerDef named VSE_PerryPersistent`

This means that this mod attempted to load up the persisted storyteller and couldn't find it (since the mod which adds it has been removed). When a new game is setup, the default RimWorld values for storyteller will be used.

This will occur when any mod which adds custom content that has been persisted is removed: foods in food policies, apparel/weapons in apparel policies, drugs in drug policies, books in reading policies, custom storytellers, custom factions, etc.


## Great Minds Think Alike

* [New Game Plus][new_game_plus]
* [Export Agency][export_agency]
* [Other Strictly Quality of Life Mods][quality_of_life_collection]

## Compatibity

* Can be added to and removed from existing saves.
* Not extensively tested. Likely compatible with most content mods; likely incompatible with most "tweak" mods. As I use several of those mods myself, I am planning to add better compatibility support.

## Thanks To

* Pardeike for [Harmony][harmonylib]
* Marnador for [RimWorld Font][font]
* The community in the RimWorld mod development discord server.

## License

* [Unlicense][license] -- feel free to use as you see fit. Go ahead and make a copy, cross out my name, write in yours, and publish it. Go wild, but remember to follow the [RimWorld TOS][rimworld_tos].

Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.

## Authors

* [protobeard][protobeard]


## Downloads

* [GitHub][github]

[license]: https://github.com/protobeard/madagascar_vanilla/blob/master/UNLICENSE
[rimworld_tos]: https://store.steampowered.com/eula/294100_eula_1
[harmonylib]: https://github.com/pardeike/Harmony
[font]: https://ludeon.com/forums/index.php?topic=11022.0

[steam]: http://steamcommunity.com/sharedfiles/filedetails/?id={mod_steam_id}
[github]: https://github.com/protobeard/madagascar_vanilla/releases

[protobeard]: https://github.com/protobeard

[quality_of_life_collection]: https://steamcommunity.com/sharedfiles/filedetails/?id=3592953821
[new_game_plus]: https://steamcommunity.com/sharedfiles/filedetails/?id=2909126210
[export_agency]: https://steamcommunity.com/sharedfiles/filedetails/?id=1467209473
