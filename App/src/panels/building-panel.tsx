import { Fragment, h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'
import { ShopButton } from '../components/shop-button'
import { ButtonConfig, SideButtonGroup } from '../components/side-button-group'
import { useTooltipBinding } from '../hooks/use-tooltip'

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
  const [population] = useEventfulState(kingdomResourcesPanelPresenter, "Population")

  const bindTooltip = useTooltipBinding();

  const selectedBuildings = useMemo(
    () => toJsArray(buildings).filter((building) => building.Category == props.selectedCategory) ?? [],
    [props.selectedCategory, buildings],
  )

  const menuButtons: ButtonConfig[] = [
    { icon: iconEconomy, active: props.selectedCategory === "Economy", onClick: () => props.onSelectCategory("Economy") },
    { icon: iconCivilian, active: props.selectedCategory === "Civilian", onClick: () => props.onSelectCategory("Civilian") },
    { icon: iconDefense, active: props.selectedCategory === "Defense", onClick: () => props.onSelectCategory("Defense") },
    { icon: iconGuilds, active: props.selectedCategory === "Guilds", onClick: () => props.onSelectCategory("Guilds") },
  ];

  return (
    <>
      <SideButtonGroup buttons={menuButtons} />

      <div class='flex flex-row w-full h-full justify-evenly items-center'>
        {selectedBuildings.map((data: BuildingDTO) => (
          <ShopButton
            key={data.Id}
            name={data.Name}
            price={data.Price}
            icon={Resources.Load(data.Icon) as Texture2D}
            active={selectedId == data.Id}
            disabled={!data.CanBuild}
            onClick={data.CanBuild ? () => buildingPanelPresenter.SelectBuilding(data.Id) : undefined}
            {...bindTooltip(buildBuildingTooltip(data, gold, population))}
          />
        ))}
      </div>
    </>
  )
}

function buildBuildingTooltip(data: any, gold: number, population: number): string {
  if (!data) {
    return ''
  }

  const lines: string[] = []
  if (data.Description) {
    lines.push(data.Description)
  }

  lines.push('')
  lines.push(`Cost: ${data.Price} gold, ${data.PopulationCost} pop`)

  if (data.LockReason) {
    lines.push('')
    lines.push(data.LockReason)
  } else {
    if (data.Price > gold) {
      lines.push('')
      lines.push('Not enough gold')
    }
    if (data.PopulationCost > population) {
      lines.push('')
      lines.push('Not enough population')
    }
  }

  return lines.join('\n')
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
