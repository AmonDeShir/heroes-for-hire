import { h } from 'preact'
import { Panel } from '../components/panel'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo, useState } from 'onejs-preact/hooks'
import System from 'System'
import { ShopButton } from '../components/shop-button'

type BuildingDTO = CS.Heroes.Presentation.UI.BuildingPanel.BuildingDTO;

type Mode = "Economy" | "Civilian" | "Defense" | "Guilds"

const iconEconomy = Resources.Load("Icons/buildings-economy") as Texture2D
const iconCivilian = Resources.Load("Icons/buildings-civilian") as Texture2D
const iconDefense = Resources.Load("Icons/buildings-defense") as Texture2D
const iconGuilds = Resources.Load("Icons/buildings-guilds") as Texture2D

export function BuildingPanel() {
  const [selectedCategory, setSelectedCategory] = useState<Mode>("Economy");

  const [buildings] = useEventfulState(buildingPanelPresenter, "Buildings");
  const [selectedId] = useEventfulState(buildingPanelPresenter, "Selected");
  const [gold] = useEventfulState(kingdomResourcesPanelPresenter, "Gold");

  const selectedBuildings = useMemo(() => toJsArray(buildings).filter(b => b.Category == selectedCategory) ?? [], [selectedCategory, buildings]);

  function toJsArray<T>(csArr: System.Array$1<T>): T[] {
    if (!csArr) {
      return [];
    }

    let arr = new Array((csArr as any).Length);
    
    var i = (csArr as any).Length;
    
    while (i--) {
        arr[i] = csArr.get_Item(i);
    }
    
    return arr;
  }

  return (
    <div class="w-[850px]">
      <Panel title={`Buildings - ${selectedCategory}`}>
        <div class='w-full h-full p-[2px] flex flex-row '>
          <div class='flex w-7 h-full flex-col justify-evenly items-start'>
            <IconButton icon={iconEconomy} active={selectedCategory == "Economy"} onClick={() => setSelectedCategory("Economy")} />
            <IconButton icon={iconCivilian} active={selectedCategory == "Civilian"} onClick={() => setSelectedCategory("Civilian")} />
            <IconButton icon={iconDefense} active={selectedCategory == "Defense"} onClick={() => setSelectedCategory("Defense")} />
            <IconButton icon={iconGuilds} active={selectedCategory == "Guilds"} onClick={() => setSelectedCategory("Guilds")} />
          </div>

          <div class='flex w-full h-full justify-center items-center'>
            {selectedBuildings.map(data => (
              <ShopButton 
                key={data.Id}
                name={data.Name}
                price={data.Price} 
                icon={Resources.Load(data.Icon) as Texture2D} 
                active={selectedId == data.Id}
                disabled={data.Price > gold}
                onClick={() => buildingPanelPresenter.SelectBuilding(data.Id)}
              />
            ))}
          </div>
        </div>
      </Panel>
    </div>
  )
}
