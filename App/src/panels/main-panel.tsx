import { h } from 'preact'
import { useEffect, useEventfulState, useState } from 'onejs-preact/hooks'
import { Panel } from '../components/panel'
import { BuildingCategory, BuildingPanelContent } from './building-panel'
import { InfoPanelContent } from './info-panel'

export function MainPanel() {
  const [mode, setMode] = useState("buildings" as "buildings" | "info")
  const [selectedCategory, setSelectedCategory] = useState<BuildingCategory>("Economy")
  const [selected] = useEventfulState(selectionPanelPresenter, "Selected")

  useEffect(() => {
    if (selected != null) {
      setMode("info")
      return
    }

    setMode("buildings")
  }, [selected])

  const title = mode === "info" && selected != null
    ? selected.Name
    : `Buildings - ${selectedCategory}`

  return (
    <div class='w-[850px]'>
      <Panel title={title}>
        <div class='w-full h-full p-[2px] flex flex-row '>
          {mode === "info" ? (
            <InfoPanelContent selected={selected} />
          ) : (
            <BuildingPanelContent selectedCategory={selectedCategory} onSelectCategory={setSelectedCategory} />
          )}
        </div>
      </Panel>
    </div>
  )
}
