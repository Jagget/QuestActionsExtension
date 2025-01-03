# Quest Actions Extension

QuestActionsExtension continues the work started by Actions Framework.

It includes the same 3 actions from Actions Framework and adds more new ones.

## Here's the list of added actions:

* reduce player health by \<percent> // Actions Framework
* reduce player health by \<percent> every \<interval> seconds
* reduce player health by \<percent> every \<interval> seconds can kill
* reduce player health by \<percent> every \<interval> seconds \<repeats> times
* reduce player health by \<percent> every \<interval> seconds \<repeats> times can kill
* reduce player health on \<amount>
* reduce player fatigue by \<percent>
* reduce player fatigue on \<amount>
* reduce player magicka by \<percent>
* reduce player magicka on \<amount>
* increase player health by \<percent>
* increase player health on \<amount>
* increase player fatigue by \<percent>
* increase player fatigue on \<amount>
* increase player magicka by \<percent>
* increase player magicka on \<amount>
* infect player as [vampire|werewolf|wereboar]
* player handsover \<numberOfItems> items class \<itemClass> subclass \<TemplateIndex>
* raise time by \<hours>:\<minutes>
* raise time by \<hours>:\<minutes> saying \<sayingID>
* raise time to \<hours>:\<minutes>
* raise time to \<hours>:\<minutes> saying \<sayingID>
* update-quest-item \<item> set-material \<material>
* update-quest-item \<anItem> add-enchantment type \<enchantmentType>
* update-quest-item \<anItem> add-enchantment type \<enchantmentType> spell \<spellId>
* update-quest-item \<anItem> apply-magic-template \<templateIndex>

## Here's the list of added triggers:

* player within \<distance> units of foe \<foe> // Actions Framework
* player within \<distance> units of item \<item> // Actions Framework
* player possesses \<numberOfItems> items class \<itemClass> subclass \<TemplateIndex>
* player equipped with item class \<itemClass> subclass \<TemplateIndex>
* player fatigue is less than \<minPoints> pt
* player fatigue is less than \<minPercent>%
* player magicka is less than \<minPoints> pt
* player magicka is less than \<minPercent>%
* player health is less than \<minPoints> pt
  * pchealth lower than \<minPoints> // Health Quest Actions is also supported
* player health is less than \<minPercent>%
  * pchealthp lower than \<minPercent> // Health Quest Actions is also supported
* player currentmappixel x \<xCoord> y \<yCoord> 
* player currentmappixel x \<xCoord> y \<yCoord> delta \<distance>
* player inblock position x \<xCoord> y \<yCoord> 
* player inblock position x \<xCoord> y \<yCoord> delta \<distance>
* player guild rank in \<guildGroupName> at least \<minRank>
* enemy \<aFoe> health is lower than \<highLimit>%
* enemy \<aFoe> health is lower than \<highLimit>% and higher than \<lowLimit>% 
* player slain \<amount> enemies of class \<aFoe>
* player slain \<amount> enemies of class \<aFoe> at \<aPlace>
* player slain \<amount> enemies of class \<aFoe> at any \<placeType>
* magic-effect key \<effectKey> is on foe \<aFoe>
* magic-effect key \<effectKey> is on player
* player current-state is [god mode|no clip mode|no target mode|resting|loitering|ready to level up|arrested|in prison|in beast form|vampire|wereboar|werewolf]
* player legal-repute is (lower | higher) than \<amount>
* player faction-repute with \<individualNpcName> is (lower | higher) than \<amount>
* player faction-repute with-faction-id \<anyFactionID> is (lower | higher) than \<amount>
* player current-region-index is \<index>

## Here's the list of added console commands:

* `qae_inblockposition`
  * Output current position coordinates inside the current block.
* `qae_getcurrentpixel`
  * Output pixel coordinates for a current location.
* `qae_player_possesses (inventory|wagon)`
  * Output list of items that player possesses in the inventory or in the wagon.
* `qae_getcurrentregionindex`
  * Outputs the integer index of the current region.

## Details

* `player possesses` will check the player's inventory for specific amount of items.
  Works with mundane items that you can equip or pick up from regular loot.
  For quest items, created by quest command, use other command, like `clicked item`, `take item`, or `toting` 
* `player handsover` will remove items from the inventory.
* `player within` checks horizontal distance in in-game distance units (whatever they are).
* `reduce player XXX` will change player's vitals. The vitals will not be reduced to the 0.
* `increase player XXX` will change player's vitals. The vitals will not be increased past maximum values.
* `can kill` - adding it will bypass 1 point limit and will allow to kill a player 
* `player possesses`, `player equipped with`, and `player currentmappixel` will be checked constantly. Works the same way as the "weather" or "climate" triggers.
* The delta distance in `player currentmappixel`, and `player inblock position` is $`\sqrt{(x_1-x_2)^2+(y_1-y_2)^2}`$
* `player inblock position` X and Y coordinates are from (0, 0) Southwest to (128, 128) Northeast. It ignores height.
* `player guild rank` For a guild group, see [FactionFile.GuildGroups enum](https://github.com/Interkarma/daggerfall-unity/blob/master/Assets/Scripts/API/FactionFile.cs#L568).
* `player [vitals] is less than` triggers will fire only once. Author will need to create several stages if they need it to fire more than once.
* `player fatigue is less than <minPoints> pt` is applying FatigueMultiplier inside, so authors will need to set normal value as on the character page.
  
  ```
  player fatigue is less than 30 pt // NOT 1920! 😁
  ```
* `raise time by` will skip time by defined hours and minutes. Time increases should always be positive.
* `raise time to` will skip time to the desired time. If it is now 11:00, and you ask for 10:45, it will be the next day.
* `enemy <aFoe> health is lower than` if player hit too hard and skip a lower limit (1% by default), the trigger will not spring. It makes sense to add fail-safe option. I guess an easy fail-safe option would just be to have a "killed foe" action.
* `player slain <amount> enemies` will keep track of enemies killed. For enemy class check Quests-Foes.txt table, for Place type check Quests-Places.txt table. This trigger condition can only be used with building types (p1=0) and dungeon types (p1=1) in Quests-Places table.
* `magic-effect key` should be an existing magic effect key, like "Shadow-True", "Slowfall", "Damage-Health", "Fortify-Luck", "SpellAbsorption", "Invisibility-Normal" or any other
* `player current-state is` will be set to true whenever the requested condition is met.
* `update-quest-item` material works for quest created items, the material value is as follows:
  * for weapons: Iron Steel Silver Elven Dwarven Mithril Adamantium Ebony Orcish Daedric Leveled
  * for armor: Leather Chain Iron Steel Silver Elven Dwarven Mithril Adamantium Ebony Orcish Daedric Leveled
  * Using `Leveled` will assign random material, available on the current player level
* `update-quest-item add-enchantment` will add up to 10 enchantments to the item.
  * The list of possible param combinations [can be found here](https://github.com/Jagget/QuestActionsExtension/wiki/UpdateQuestItem-AddEnchantment)
* `update-quest-item apply-magic-template` will apply ready magic template to an item.
  * The list of magic templates [can be found here](https://github.com/Interkarma/daggerfall-unity/blob/master/Assets/Resources/MagicItemTemplates.txt)
  * You can use only templates with type=RegularMagicItem
  * You can use `add-enchantment` after `apply-magic-template`, this will result in additional capabilities
  * If you use `apply-magic-template` after `add-enchantment`, all magic powers will be overwritten
* `player legal-repute` is "always on" trigger and might and will change when the player crosses the border
* `faction-repute with individualNpcName` only works with a special named NPCs. Examples are King Gothryd, Queen Aubk-i, Prince Lhotun, etc.
* `player current-region-index` compares not the region name but region index, to know the region index travel there and use the `qae_getcurrentregionindex` console command. 

## Example quest of selling 10 arrows

```
Quest: SELLINGARROWS
DisplayName: Selling Arrows

QRC:
QuestComplete:  [1004]
I sold arrows.

Message:  1013
I'll buy 10 arrows from you. Here's the _reward_ coins.

QBN:
Item _reward_ gold

_possesses_ task:
  player possesses 10 items class 3 subclass 131

_handover_ task: 
  when _possesses_
  player handsover 10 items class 3 subclass 131  
  say 1013

_damaged_ task:
  when _handover_
  reduce player health on 5

_sold_ task:
  when _damaged_
  give pc _reward_
  end quest
```

## Example quest to find a center of the city

_(that is if the quest is started in the city)_

```
QRC:
QuestComplete:  [1004]
I found a CENTER of the city and stood there for 45 minutes, contemplating my life!

QBN:

_found_ task:
  player inblock position x 64 y 64

_close_ task:
  when _found_
  raise time by 0:45 saying 1004
  end quest
```
