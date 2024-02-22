# Quest Actions Extension

QuestActionsExtension continues the work started by Actions Framework.

It includes the same 3 actions from Actions Framework and adds more new ones.

Here's the list of added actions:
* reduce player health by \<percent> // Actions Framework
* reduce player health on \<amount>
* reduce player fatigue by \<percent>
* reduce player fatigue on \<amount>
* reduce player magicka by \<percent>
* reduce player magicka on \<amount>
* infect player as vampire
* infect player as werewolf
* infect player as wereboar
* player handsover \<numberOfItems> items class \<itemClass> subclass \<itemSubClass>

Here's the list of added triggers: 
* player within \<distance> units of foe \<foe> // Actions Framework
* player within \<distance> units of item \<item> // Actions Framework
* player possesses \<numberOfItems> items class \<itemClass> subclass \<itemSubClass>

`player possesses` will check the player's inventory for specific amount of items.

`player handsover` will remove items from the inventory.

`player within` checks horizontal distance in in-game distance units (whatever they are).

`reduce player XXX` will change player's vitals. The vitals will not be reduced to the 0.
Also, keep in mind, there's is currently an issue with HUD https://github.com/Interkarma/daggerfall-unity/issues/2593
that might affect the user experience. 

Example quest of selling 10 arrows:

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
