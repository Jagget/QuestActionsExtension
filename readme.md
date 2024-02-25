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
* infect player as vampire
* infect player as werewolf
* infect player as wereboar
* player handsover \<numberOfItems> items class \<itemClass> subclass \<TemplateIndex>

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

## Details

* `player possesses` will check the player's inventory for specific amount of items.
  Works with mundane items that you can equip or pick up from regular loot.
  For quest items, created by quest command, use other command, like `clicked item`, `take item`, or `toting` 
* `player handsover` will remove items from the inventory.
* `player within` checks horizontal distance in in-game distance units (whatever they are).
* `reduce player XXX` will change player's vitals. The vitals will not be reduced to the 0.
Also, keep in mind, there's is currently an issue with HUD https://github.com/Interkarma/daggerfall-unity/issues/2593
that might affect the user experience. 
* `can kill` - adding it will bypass 1 point limit and will allow to kill a player 
* `player possesses`, `player equipped with`, and `player currentmappixel` will be checked constantly. Works the same way as the "weather" or "climate" triggers.
* `player [vitals] is less than` triggers will fire only once. Author will need to create several stages if they need it to fire more than once.
* `player fatigue is less than <minPoints> pt` is applying FatigueMultiplier inside, so authors will need to set normal value as on the character page.
  ```
  player fatigue is less than 30 pt // NOT 1920! 😁
  ```

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
