W okolicy spawnerów wrogów będą rozmieszczone skrzynie z lootem. Otwierając je bohater może zdobyć złoto oraz przedmioty. Każda skrzynia ma różny poziom. Skrzynie biorą informacja o tym jakie przedmioty i złoto mają być losowane z loot tables. Jest to dataobject który przechowuje informacje o szansie danego itemu na spawn w skrzyni danego poziomu.

Przykład loot table
1. Jeden losowy item z: miecz poziom/2 mikstury zdrowia/amulet zdrowia lv 1
2. Złoto od do: 50 - 300

Skrzynie nie są zapisane w word data, bohaterowie muszą je odkryć poprzez sensor (collider), i wtedy aktywuje im się możliwość ich otworzenia. (nie wiem jescze jak to zintegrować ale może podpiąć to pod potrzebe chciwości?)