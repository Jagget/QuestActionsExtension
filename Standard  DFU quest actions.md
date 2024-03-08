# Standard DFU quest actions

> Incomplete. Just stubbing out action for now so quest will compile.
```
add (?<target>[a-zA-Z0-9_.-]+) as questor
```

> add dialog command used in quests to make talk options available.
```
add dialog for location (?<aPlace>\w+) person (?<anNPC>\w+) item (?<anItem>\w+)
add dialog for person (?<anNPC>\w+) item (?<anItem>\w+)
add dialog for location (?<aPlace>\w+) person (?<anNPC>\w+)
add dialog for location (?<aPlace>\w+) item (?<anItem>\w+)
add dialog for location (?<aPlace>\w+)
add dialog for person (?<anNPC>\w+)
add dialog for item (?<anItem>\w+)
```

> Adds an NPC or Foe portrait to HUD which indicates player is escorting this NPC or Foe.
```
add (?<anNPC>[a-zA-Z0-9_.-]+) face saying (?<sayingID>\d+)
add (?<anNPC>[a-zA-Z0-9_.-]+) face
add foe (?<aFoe>[a-zA-Z0-9_.-]+) face saying (?<sayingID>\d+)
add foe (?<aFoe>[a-zA-Z0-9_.-]+) face
```

> Executes target task when player readies a spell containing a single specific effect.
>
> Matches effect by key, so should support custom effects from mods.
```
cast (?<effectKey>[a-zA-Z0-9_.-]+) effect do (?<aTask>[a-zA-Z0-9_.-]+)
```

> Executes target task when player readies a spell containing specific effects.
>
> Classic only accepts standard versions of spell, not custom spells created by player.
>
> Daggerfall Unity makes no distinction between standard or custom spells and will instead match by effects.
```
cast (?<aSpell>[a-zA-Z0-9'_.-]+) spell do (?<aTask>[a-zA-Z0-9_.-]+)
```

> Queues a spell to be cast on a Foe resource.
>
> Can queue multiple spells at once or at different stages of quest.
>
> If the Foe has not been spawned: All spells currently in queue will be cast on Foe the moment they spawn.
>
> If the Foe has been spawned: The next spell(s) added to queue will be cast on spawned Foes on next quest tick.
>
> This allows quest author to cast spells on Foe both before and after spawn or at different stages of quest (e.g. after foe injured).
>
> Notes:
>
> - As spells have durations recommend casting spells on "place foe" dungeon foes after being injured or spell will likely expire by the time player locates foe.
> - For "create foe" foes spell can be queued at any time as spell is cast when foe is directly spawned near player.
```
cast (?<aSpell>[a-zA-Z0-9'_.-]+) spell on (?<aFoe>[a-zA-Z0-9_.-]+)
cast (?<aCustomSpell>[a-zA-Z0-9_.-]+) custom spell on (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Changes whether or not a quest foe is attackable by other NPCs.
```
change foe (?<anNPC>[a-zA-Z0-9_.-]+) infighting (?<isAttackableByAI>[a-zA-Z]+)
```

> Changes a foe's team.
```
change foe (?<anNPC>[a-zA-Z0-9_.-]+) team (?<teamNumber>\d+)
change foe (?<anNPC>[a-zA-Z0-9_.-]+) team (?<teamName>\w+)
```

> Changes reputation with an NPC by specified amount.
```
change repute with (?<target>[a-zA-Z0-9_.-]+) by (?<sign>[+-])(?<amount>\d+)
```

> Unsets 1 or more tasks so they can be triggered again
```
clear [a-zA-Z0-9_.]+
```

> Handles player clicking on Foe.
```
clicked foe (?<aFoe>[a-zA-Z0-9_.-]+) and at least (?<goldAmount>\d+) gold otherwise do (?<taskName>[a-zA-Z0-9_.]+)
clicked foe (?<aFoe>[a-zA-Z0-9_.-]+) say (?<id>\d+)
clicked foe (?<aFoe>[a-zA-Z0-9_.-]+) say (?<idName>\w+)
clicked foe (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Handles player clicking on Item.
```
clicked item (?<anItem>[a-zA-Z0-9_.-]+) say (?<id>\d+)
clicked item (?<anItem>[a-zA-Z0-9_.-]+) say (?<idName>\w+)
clicked item (?<anItem>[a-zA-Z0-9_.-]+)
```

> Handles player clicking on NPC.
>
> NOTES:
>  - Will clear click after handling if player clicks NPC.
>  - If used in combination with TotingItemAndClickedNpc on same NPC elsewhere in quest, always call TotingItemAndClickedNpc check BEFORE ClickedNpc.
```
clicked (?<anNPC>[a-zA-Z0-9_.-]+) and at least (?<goldAmount>\d+) gold otherwise do (?<taskName>[a-zA-Z0-9_.]+)
clicked npc (?<anNPC>[a-zA-Z0-9_.-]+) say (?<id>\d+)
clicked npc (?<anNPC>[a-zA-Z0-9_.-]+) say (?<idName>\w+)
clicked npc (?<anNPC>[a-zA-Z0-9_.-]+)
```

> DFU extension action. Triggers when the player is at a location with the right climate
```
climate (?<climate>desert|desert2|mountain|mountainwoods|rainforest|ocean|swamp|subtropical|woodlands|hauntedwoodlands)
climate (?<base>base) (?<climatebase>desert|mountain|temperate|swamp)
```

> Spawn a Foe resource into the world.
```
create foe (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<infinite>indefinitely) with (?<percent>\d+)% success
create foe (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<count>\d+) times with (?<percent>\d+)% success
(?<send>send) (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<count>\d+) times with (?<percent>\d+)% success
(?<send>send) (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes with (?<percent>\d+)% success
```

> Places a Person to a random building in their home town within current region.
```
create npc (?<anNPC>[a-zA-Z0-9_.-]+)
```

> Tipton calls this "create npc at" but its true function seems to reserve a quest site
> before linking resources. "create npc at" is usually followed by "place npc"
> but can also be followed by "place item" or "create foe" for example.
>
> This action likely initiates some book-keeping in Daggerfall's quest system.
> In Daggerfall Unity this creates a SiteLink in QuestMachine.
```
create npc at (?<aPlace>\w+)
```

> Cure specific disease on player through quest system.
```
cure vampirism
cure lycanthropy
cure (?<aDisease>[a-zA-Z0-9_.']+)
```

> Raise or lower task state based on time of day.
> Time must be in 24-hour time 00:00 to 23:59.
```
daily from (?<hours1>\d+):(?<minutes1>\d+) to (?<hours2>\d+):(?<minutes2>\d+)
```

> NPC will be be soft destroyed (permanently removed from world but othwerwise available for macro resolution).
>
> This is different to classic that will return BLANK once NPC is destroyed (probably beause resource is hard deleted).
>
> If there are any emulation issue with soft destruction then will change to hard destruction instead.
```
destroy npc (?<anNPC>[a-zA-Z0-9_.-]+)
destroy (?<anNPC>[a-zA-Z0-9_.-]+)
```

> dialog link command used in quests.
```
dialog link for location (?<aSite>\w+) person (?<anNPC>\w+) item (?<anItem>\w+)
dialog link for location (?<aSite>\w+) person (?<anNPC>\w+)
dialog link for location (?<aSite>\w+) item (?<anItem>\w+)
dialog link for location (?<aSite>\w+)
dialog link for person (?<anNPC>\w+) item (?<anItem>\w+)
dialog link for person (?<anNPC>\w+)
dialog link for item (?<anItem>\w+)
```

> Incomplete. Just stubbing out action for now so quest will compile.
```
drop (?<target>[a-zA-Z0-9_.-]+) as questor
```

> Drops an NPC or Foe portrait from HUD that player is currently escorting.
```
drop (?<anNPC>[a-zA-Z0-9_.-]+) face
drop foe (?<aFoe>[a-zA-Z0-9_.-]+) face
```

> Triggers when player dropped quest item
```
dropped (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) saying (?<id>\d+)
dropped (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+)
```

> End quest
```
end quest saying (?<id>\d+)
end quest
```

> Makes all foes hostile, or clears (removes) them all.
```
enemies (?<action>makehostile|clear)
```

> Give a quest Item to player.
>
> Currently ignoring "get item from" as it appears to behave identically to "get item".
```
get item (?<anItem>[a-zA-Z0-9_.]+) saying (?<id>\d+)
get item (?<anItem>[a-zA-Z0-9_.]+)
```

> Give a quest Item to other quest entity (NPC, Foe...)
```
give item (?<anItem>[a-zA-Z0-9_.]+) to (?<aResource>[a-zA-Z0-9_.]+)
```

> Give a quest Item to player. This has three formats:
>  * "give pc anItem" - Displays QuestComplete success message and opens loot window with reward. Could probably be called "give quest reward anItem".
>  * "give pc nothing" - Also displays QuestComplete success message but does not open loot window as no reward.
>  * "give pc anItem notify nnnn" - Places item directly into player's inventory and says message ID nnnn.
```
give pc (?<nothing>nothing)
give pc (?<anItem>[a-zA-Z0-9_.]+) notify (?<id>\d+)
give pc (?<anItem>[a-zA-Z0-9_.]+) (?<silently>silently)
give pc (?<anItem>[a-zA-Z0-9_.]+)
```

> Starts a task when player has a particular item resource in their inventory.
>
> This task continues to run and will start task when item present
```
have (?<targetItem>[a-zA-Z0-9_.-]+) set (?<targetTask>[a-zA-Z0-9_.-]+)
```

> Hide NPC from world temporarily.
```
hide npc (?<anNPC>[a-zA-Z0-9_.-]+)
hide (?<anNPC>[a-zA-Z0-9_.-]+)
```

> Triggers when a Foe has been injured.
>
> Will not fire if Foe dies immediately (e.g. player one-shots enemy).
```
injured (?<aFoe>[a-zA-Z0-9_.-]+) saying (?<textID>\d+)
injured (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Condition that fires when player equips an Item or clicks "Use" on item in inventory window.
>
> Seen in Sx006 when player equips the robe or in Sx017 when player uses painting.
```
(?<anItem>[a-zA-Z0-9_.-]+) used do (?<aTask>[a-zA-Z0-9_.-]+)
(?<anItem>[a-zA-Z0-9_.-]+) used saying (?<textID>\d+) do (?<aTask>[a-zA-Z0-9_.-]+)
```

> Adds a text entry to the player journal as a note.
```
journal note (?<id>\d+)
```

> Kills a specified foe instantly
```
kill foe (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Trigger when player kills a quest Foe
```
killed (?<kills>\d+) (?<aFoe>[a-zA-Z0-9_.-]+) (saying (?<sayingID>\d+))
killed (?<kills>\d+) (?<aFoe>[a-zA-Z0-9_.-]+)
killed (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Legal repute modifies player legal reputation in current region.
```
legal repute (?<amount>[+-]?\d+)
```

> Triggers when player has reached specified level or higher.
```
level (?<minLevelValue>\d+) completed
```

> Adds Qrc text message to player journal
>
> Note: Daggerfall groups journal entries together by quest (max 32 active at once)
> each quest can have up to 10 journal entries
> if message has already been added at stepID index, old message gets replaced
```
log (?<id>\d+)( step)? (?<step>\d+)
```

> Inflicts a disease on player through quest system.
```
make pc ill with (?<aDisease>[a-zA-Z0-9_.']+)
```

> Converts a quest item into a permanent item.
```
make (?<target>[a-zA-Z0-9_.-]+) permanent
```

> NPC will no longer respond to mouse clicks once muted.
```
mute npc (?<anNPC>[a-zA-Z0-9_.-]+)
```

> Take an amount from player, and start a task depending on if they could pay.
>
> Amount can be gold, letter of credit, or a combination.
```
pay (?<amount>\d+) (?<type>money|gold) do (?<paidTaskName>[a-zA-Z0-9_.]+) otherwise do (?<notTaskName>[a-zA-Z0-9_.]+)
```

> Condition which checks if player character at a specific place.
>
> Notes:
>
> Docs use form "pc at aPlace do aTask"
> But observed quests actually seem to use "pc at aPlace set aTask"
> Probably a change between writing of docs and Template v1.11.
> Supporting both variants as quest authors are working from docs
> Docs also missing "pc at aPlace set aTask saying nnnn"
> DFU extension: also adding "pc at any \<placeType>" so quests can check for any place of a certain type
```
pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)
pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+)
pc at (?<aPlace>\w+) do (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)
pc at (?<aPlace>\w+) do (?<aTask>[a-zA-Z0-9_.]+)
pc at any (?<placeType>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)
pc at any (?<placeType>\w+) set (?<aTask>[a-zA-Z0-9_.]+)
pc at any (?<placeType>\w+) do (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)
pc at any (?<placeType>\w+) do (?<aTask>[a-zA-Z0-9_.]+)
```

> This action triggers a random task
```
pick one of [a-zA-Z0-9_.]+
```

> Moves a foe into world at a reserved site.
```
place foe (?<aFoe>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) marker (?<marker>\d+)
place foe (?<aFoe>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+)
```

> Moves item into world at a reserved site.
```
place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) marker (?<marker>\d+)
place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) questmarker (?<questmarker>\d+)
place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) (?<anymarker>anymarker)
place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+)
```

> Moves NPC to a reserved site.
>
> Fixed NPCs always starts in their home location but quests can move them around as needed.
>
> Random NPCs are instantiated to target location only as they don't otherwise exist in world.
>
> Site must be reserved before moving NPC to that location.
```
place npc (?<anNPC>[a-zA-Z0-9_.-]+) at (?<aPlace>\w+) marker (?<marker>\d+)
place npc (?<anNPC>[a-zA-Z0-9_.-]+) at (?<aPlace>\w+)
```

> Plays a song from MIDI.BSA using SongFiles enum.
```
play song (?<song>[a-zA-Z0-9_-]+)
```

> Plays sound for quests (only used in Sx977...vengence)
>
> See Quests-Sounds.txt for valid sounds
>
> Unlike message posts, the play sound command performs until task is cleared
> can be in the form of:
>
> play sound (soundname) every x minutes y times
>
> or play sound (soundname) x y
>
> the second number is not currently used for anything - purpose is unkown.
```
play sound (?<sound>\w+) every (?<n1>\d+) minutes (?<count>\d+) times
play sound (?<sound>\w+) (?<n1>\d+) (?<n2>\d+)
```

> Plays one of the ANIMXXXX.VID videos for quest
```
play video (?<vidNum>\d+)
```

> Prompt which displays a yes/no dialog that executes a different task based on user input.
```
prompt (?<id>\d+) yes (?<yesTaskName>[a-zA-Z0-9_.]+) no (?<noTaskName>[a-zA-Z0-9_.]+)
prompt (?<idName>\w+) yes (?<yesTaskName>[a-zA-Z0-9_.]+) no (?<noTaskName>[a-zA-Z0-9_.]+)
```

> Prompt which displays a dialog with 2-4 buttons that each execute a different task based on user selection.
>
> Example usage:
>
> `promptmulti 1072 4:noChoice _dirRand_ 24:south _headS_ 25:west _headW_ 28:swest _headSW_`
```
promptmulti (?<id>\d+) (?<opt1>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt1TaskName>[a-zA-Z0-9_.]+) (?<opt2>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt2TaskName>[a-zA-Z0-9_.]+) (?<opt3>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt3TaskName>[a-zA-Z0-9_.]+) (?<opt4>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt4TaskName>[a-zA-Z0-9_.]+)
promptmulti (?<id>\d+) (?<opt1>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt1TaskName>[a-zA-Z0-9_.]+) (?<opt2>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt2TaskName>[a-zA-Z0-9_.]+) (?<opt3>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt3TaskName>[a-zA-Z0-9_.]+)
promptmulti (?<id>\d+) (?<opt1>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt1TaskName>[a-zA-Z0-9_.]+) (?<opt2>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt2TaskName>[a-zA-Z0-9_.]+)
```

> Removes foe
```
remove foe (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Removes Qrc text message from player journal
```
remove log step (?<step>\d+)
```

> Executes a task when reputation with NPC exceeds value.
>
> This is not a trigger condition and will not trigger parent task.
>
> Owning task must be made active before this action starts checking condition each tick.
```
repute with (?<npcSymbol>[a-zA-Z0-9_.-]+) exceeds (?<minReputation>\d+) do (?<taskSymbol>[a-zA-Z0-9_.]+)
```

> Restore NPC previously hidden from world.
```
restore npc (?<anNPC>[a-zA-Z0-9_.-]+)
restore (?<anNPC>[a-zA-Z0-9_.-]+)
```

> Makes a foe non-hostile until player strikes them again.
>
> Quest authors should also use a popup message to inform player that foe is no longer aggressive.
```
restrain foe (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Reveals location on travelmap
```
reveal (?<aPlace>\w+) (?<readMap>readmap)
reveal (?<aPlace>\w+)
```

> add dialog command used in quests to make talk options available.
```
rumor mill (?<id>\d+)
```

> Starts another quest and waits for its termination, then sets task for success or failure.
>
> Sets failure task immediately if target quest is not found.
>
> Will ensure that target quest is also terminated if still running when parent quest ends.
```
run quest (?<questName>\w+) then (?<successTask>[a-zA-Z0-9_.]+) or (?<failureTask>[a-zA-Z0-9_.]+)
```

> Displays a prompt which user can click to dismiss.
```
say (?<id>\d+)
say (?<idName>\w+)
```

> DFU extension action. Triggers when the calendar is at the right season
```
season (?<season>fall|summer|spring|winter)
```

> Sets a player's crime.
```
setplayercrime (?<crime>[a-zA-Z_]+)
```

> Spawns city guards using PlayerEntity.SpawnCityGuards().
```
spawncityguards (immediate)?
```

> Starts another quest. Classic 'start quest' can only start Sx000 value quests.
>
> Likely to create a Daggerfall Unity specific start quest variant that can start any named quest.
>
> In canonical quests always lists same quest index twice, e.g. "start quest 1 1".
>
> This implementation only uses first index, purpose of second index currently unknown.
```
start quest (?<questIndex1>\d+) (?<questIndex2>\d+)
start quest (?<questName>\w+)
```

> Starts and stops a Clock resource timer.
```
(?<start>start) timer (?<symbol>[a-zA-Z0-9_.-]+)
stop timer (?<symbol>[a-zA-Z0-9_.-]+)
```

> Starts a task by setting it active. Added alternate form "setvar taskname".
```
start task (?<taskName>[a-zA-Z0-9_.]+)
setvar (?<taskName>[a-zA-Z0-9_.]+)
```

> Remove a quest Item from player.
```
take (?<anItem>[a-zA-Z0-9_.]+) from pc saying (?<id>\d+)
take (?<anItem>[a-zA-Z0-9_.]+) from pc
```

> Partial implementation.
>
> Teleport player to a dungeon for dungeon traps, or as part of main quest.
>
> Does not exactly emulate classic for "transfer pc inside" variant. This is only used in Sx016.
```
teleport pc to (?<aPlace>[a-zA-Z0-9_.-]+)
transfer pc inside (?<aPlace>[a-zA-Z0-9_.-]+) marker (?<marker>\d+)
```

> Condition triggers when player clicks on NPC while holding a quest Item in their inventory.
>
> Superficially very similar to ClickedNpc but also requires item check to be true.
>
> NOTES:
>  - Will clear click after handling if player clicks NPC while holding specified item.
>  - If used in combination with ClickedNpc on same NPC elsewhere in quest, always call TotingItemAndClickedNpc check BEFORE ClickedNpc.
```
toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked saying (?<id>\d+)
toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked saying (?<idName>\w+)
toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked
```

> Train the player
```
train pc (?<skillName>\w+)
```

> Unrestrains a foe restrained by RestrainFoe
```
unrestrain foe (?<aFoe>[a-zA-Z0-9_.-]+)
```

> Unsets one or more tasks.
>
> Unlike clear, which simply rearms a task, unset will permanently disable that task.
```
unset [a-zA-Z0-9_.]+
```

> DFU extension action. Triggers when the current weather matches the condition
```
weather (?<weather>sunny|cloudy|overcast|fog|rain|thunder|snow)
```

> Triggers when character reaches minimum value for specified attribute.
```
when attribute (?<attributeName>\w+) is at least (?<minAttributeValue>\d+)
```

> Triggers when player clicks on an individual NPC that is not currently assigned to another quest.
>
> This is only used in a small number of canonical quests which refer to special individuals.
>
> Examples are King Gothryd, Queen Aubk-i, Prince Lhotun, etc.
```
when (?<individualNPCName>[a-zA-Z0-9_.-]+) is available
```

> Trigger for player entering or exiting an exterior type as defined in places table.
```
when pc (?<enters>enters) (?<exteriorType>\w+)
when pc (?<exits>exits) (?<exteriorType>\w+)
```

> Triggers when player reputation with a special named NPC equals or exceeds a minimum value.
>
> This is only used in a small number of canonical quests which refer to special individuals.
>
> Examples are King Gothryd, Queen Aubk-i, Prince Lhotun, etc.
```
when repute with (?<individualNPCName>[a-zA-Z0-9_.-]+) is at least (?<minRepValue>\d+)
```

> Triggers when player has reached specified skill level or higher.
```
when skill (?<skillName>\w+) is at least (?<minSkillValue>\d+)
```

> Handles a when|when not task performed condition chain.
```
when not (?<taskName>[a-zA-Z0-9_.]+)
when (?<taskName>[a-zA-Z0-9_.]+)
```

> Updates the world data system to use a specific variant for a given
> place (location, block, building) either for all instances or for a
> particular location.
>
> Specify a single dash for variant to remove any existing variant.
```
worldupdate (?<type>location) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)
worldupdate (?<type>locationnew) named (?<locationName>.+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)
worldupdate (?<type>block) (?<blockName>[a-zA-Z0-9_.-]+) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)
worldupdate (?<type>blockAll) (?<blockName>[a-zA-Z0-9_.-]+) variant (?<variant>[a-zA-Z0-9_.-]+)
worldupdate (?<type>building) (?<blockName>[a-zA-Z0-9_.-]+) (?<recordIndex>\d+) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)
worldupdate (?<type>buildingAll) (?<blockName>[a-zA-Z0-9_.-]+) (?<recordIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)
```
