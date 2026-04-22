import { Fragment, h } from 'preact'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'
import { ShopButton } from '../components/shop-button'

type BuildingDTO = CS.Heroes.Presentation.UI.BuildingPanel.BuildingDTO;

export type BuildingCategory = "Economy" | "Civilian" | "Defense" | "Guilds"

const iconEconomy = Resources.Load("Icons/buildings-economy") as Texture2D
const iconCivilian = Resources.Load("Icons/buildings-civilian") as Texture2D
const iconDefense = Resources.Load("Icons/buildings-defense") as Texture2D
const iconGuilds = Resources.Load("Icons/buildings-guilds") as Texture2D

export function BuildingPanelContent(props: {
  selectedCategory: BuildingCategory
  onSelectCategory: (category: BuildingCategory) => void
}) {
  const [buildings] = useEventfulState(buildingPanelPresenter, "Buildings")
  const [selectedId] = useEventfulState(buildingPanelPresenter, "Selected")
  const [gold] = useEventfulState(kingdomResourcesPanelPresenter, "Gold")

  const selectedBuildings = useMemo(
    () => toJsArray(buildings).filter((building) => building.Category == props.selectedCategory) ?? [],
    [props.selectedCategory, buildings],
  )

  return (
    <>
      <div class='flex w-7 h-full flex-col justify-evenly items-start'>
        <IconButton icon={iconEconomy} active={props.selectedCategory == "Economy"} onClick={() => props.onSelectCategory("Economy")} />
        <IconButton icon={iconCivilian} active={props.selectedCategory == "Civilian"} onClick={() => props.onSelectCategory("Civilian")} />
        <IconButton icon={iconDefense} active={props.selectedCategory == "Defense"} onClick={() => props.onSelectCategory("Defense")} />
        <IconButton icon={iconGuilds} active={props.selectedCategory == "Guilds"} onClick={() => props.onSelectCategory("Guilds")} />
      </div>

      <div class='flex w-full h-full justify-center items-center'>
        {selectedBuildings.map((data: BuildingDTO) => (
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
    </>
  )
}

function toJsArray<T>(csArr: System.Array$1<T>): T[] {
  if (!csArr) {
    return []
  }

  const arr = new Array((csArr as any).Length)
  let i = (csArr as any).Length

  while (i--) {
    arr[i] = csArr.get_Item(i)
  }

  return arr
}
