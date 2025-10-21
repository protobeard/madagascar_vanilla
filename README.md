# Madagascar Vanilla

The best vanilla.

## Why?

For those of us with literally thousands of hours in RimWorld, it can be a pain to reconfigure each new game. This mod aims to reduce the overhead of starting new games by adding settings for things like: choosing manual work priorites, auto-expanding home zone, auto-rebuilding destroyed buildings, etc..

As well, there were a number of quality of life and "house rules" small changes which I wanted to make to RimWorld.

Note: everything is set to RimWorld defaults and must be enabled in the mod settings to take effect.

## What does it do exactly?

### General

* Verbose Mode: make this mod log a lot. Probably you should leave this off.

### Bug Fixes (Well, Kinda...)

* Display Correct Milk Type on Animal Fullness Labels: Boomables produce chemfuel, not milk.
* Show Traits in Outfit Assignment: just like for Drug Policies.

### Quality of Life

* Stand Your Ground: set a default value for pawn hostility response.
* Just Say No: set a default value for drug policy. FIXME: this should be removed in favor of just persisting drug policies.
* Allow Goodwill Reward: enable or disable goodwill rewards from factions.
* Allow Honor Reward: enable of distable honot rewards from factions.
* Override Default Schedules: enable a better (according to protobeard) default schedule for pawns.
* Special Night Owl Schedule: enable night shift schedule for Night Owl pawns.
* Special UV Sensitive Schedule: enable night shift schedule for UV Sensistive pawns.
* Special Sleep Gene Schedules: enable special schedules for pawns with sleep modifying genes. Sleepy and Very Sleepy pawns get biphasic schedules, etc. See tooltip for more info.
* Special Body Mastery Schedule: enable special schedule with no sleep time for Body Mastery pawns.
* Reduce Sleep in Schedules for Quick Sleepers: modify special schedules to have less sleep time for Quick Sleeper pawns.
* Avoid Scheduled Mood Debuffs: Do not reduce sleep in schedules for Night Owl/UV Sensitive even if the pawn has Quick Sleeper
* Automatically Expand Home: enable or disable automatically expanding the Home area.
* Auto Rebuild in Home: enable or disable automatically rebuilding in the Home area.
* Starting Areas: provide a list of areas which will be automatically created on each new game (e.g., "Work, Inside, Safe").
* Use Manual Priorities: enable or disable using manual work priorities.
* Bill Storage: enable Drop on Floor or Take to Best Stockpile default for new production bills.
* Bill Ingredient Search Radius: set a default ingredient search radius for new production bills.
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

### House Rules

* Make all Mechanitor Chips Nonflammable: instead of just the Signal Chip
* Extend Odyssey Quest Expiration time: nice for 100% worlds. Amount can be configured.
* Extend Odyssey Quest Spawn Distance: nice for 100% worlds. Amount can be configured.
* Allow Additional Gravship Subquests: nice for 100% worlds. Amount can be configured.
* Toxic Fallout Immune Toxipotatos: seems on brand.
* Toxic Fallout Immune Devilstrand: it takes so long to grow already.
* Hydroponic Devilstrand: allow devilstrand to be grown in hydroponics.


## Balance

I'd say that 95% of this mod is simple quality of life, with no implact on balance. The other 5% has some small impact, such as allowing players to configure Toxipotatos to life through toxic fallout, or extending the duration of Odyssey quests to more easily accomodate a 100% planet coverage playstyle. Please see the "What does it do exactly" second for a more complete list of all changes, and remember that every change is opt-in.

## Known Issues

* Errors will be thrown on game launch if mods which add persisted items have been removed. For example, if persist storytellers is enabled and a custom storyteller such as Perry Persistent is selected, then the mod which adds that custom storyteller is removed, on next game launch you will see an error like:

`Could not load reference to RimWorld.StorytellerDef named VSE_PerryPersistent`

This means that this mod attempted to load up the persisted storyteller and couldn't find it (since the mod which adds it has been removed). When a new game is setup, the default RimWorld values for storyteller will be used.

This will occur when any mod which adds custom content that has been persisted is removed: foods in food policies, apparel/weapons in apparel policies, drugs in drug policies, books in reading policies, custom storytellers, custom factions, etc.

## Great Minds Think Alike

* [New Game Plus][new_game_plus]
* [Export Agency][export_agency]

## How to Install

* Unzip the contents and place them in your RimWorld/Mods folder.
* Activate the mod in the mod menu in the game.

## Compatibity

* Can be added and removed to existing saves.
* Not extensively tested, but this should be compatible with most other mods.

## License

* [MIT][license] -- feel free to use as you see fit, including not honoring the following requests. If you want to include this mod in a mod pack, or base a larger mod off of it, I'd appreciate a heads up. Please don't upload standalone translated versions on Steam; if you wish to translate it, please create a pull request on GitHub and I'll merge it in.

## Thanks To

* Pardeike for [Harmony][harmonylib]
* Marnador for [RimWorld Font][font]

## Authors

* [protobeard][protobeard]


## Downloads

* [GitHub][github]

[license]: https://github.com/protobeard/madagascar_vanilla/blob/master/LICENSE
[harmonylib]: https://github.com/pardeike/Harmony
[font]: https://ludeon.com/forums/index.php?topic=11022.0

[steam]: http://steamcommunity.com/sharedfiles/filedetails/?id={mod_steam_id}
[github]: https://github.com/protobeard/madagascar_vanilla/releases

[protobeard]: https://github.com/protobeard
[patreon]: https://www.patreon.com/protobeard

[new_game_plus]: https://steamcommunity.com/sharedfiles/filedetails/?id=2909126210
[export_agency]: https://steamcommunity.com/sharedfiles/filedetails/?id=1467209473
