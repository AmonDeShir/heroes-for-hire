import { h, render } from 'preact'
import { Panel } from '../components/panel'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo, useState } from 'onejs-preact/hooks'
import clsx from 'clsx'
import System from 'System'

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

  const selectedBuildings = useMemo(() => toJsArray(buildings).filter(b => b.Category == selectedCategory) ?? [], [selectedCategory, buildings]);

  function toJsArray<T>(csArr: System.Array$1<T>): T[] {
    if (!csArr) {
      return [];
    }

    let arr = new Array(csArr.Length);
    
    var i = csArr.Length;
    
    while (i--) {
        arr[i] = csArr.get_Item(i);
    }
    
    return arr;
  }

  useEffect(() => {
    console.log("Selected building id:", selectedId);
  }, [selectedId]);

  return (
    <div class="w-[850px]">
      <Panel title={`Budynki - ${selectedCategory}`}>
        <div class='w-full h-full p-[2px] flex flex-row '>
          <div class='flex w-7 h-full flex-col justify-evenly items-start'>
            <IconButton icon={iconEconomy} active={selectedCategory == "Economy"} onClick={() => setSelectedCategory("Economy")} />
            <IconButton icon={iconCivilian} active={selectedCategory == "Civilian"} onClick={() => setSelectedCategory("Civilian")} />
            <IconButton icon={iconDefense} active={selectedCategory == "Defense"} onClick={() => setSelectedCategory("Defense")} />
            <IconButton icon={iconGuilds} active={selectedCategory == "Guilds"} onClick={() => setSelectedCategory("Guilds")} />
          </div>

          <div class='flex w-full h-full justify-center items-center'>
            {selectedBuildings.map(data => (
              <div class={clsx("hover:text-red-500", selectedId == data.Id ? "text-blue-400" : "text-main")} key={data.Id} onClick={() => buildingPanelPresenter.SelectBuilding(data.Id)}>
                {data.Name}
                {data.Price}
              </div>
            ))}
          </div>
        </div>
      </Panel>
    </div>
  )
}
