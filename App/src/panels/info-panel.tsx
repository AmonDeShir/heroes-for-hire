import { Fragment, h } from 'preact'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useState } from 'onejs-preact/hooks'
import { InfoTabContent } from './info-tab-content'
import { GoapTabContent } from './goap-tab-content'
import { UpgradesTabContent } from './upgrades-tab-content'

const INFO_ICON = Resources.Load("Icons/info") as Texture2D
const UPGRADE_ICON = Resources.Load("Icons/upgrade") as Texture2D
const GOAP_ICON = Resources.Load("Icons/capybara") as Texture2D

type Mode = "info" | "goap" | "upgrades"

export function InfoPanelContent(props: {
  selected: any
}) {
  const [mode, setMode] = useState<Mode>("info")
  const [hasUpgradeTab, setHasUpgradeTab] = useState(false)
  const [hasGoapTab, setHasGoapTab] = useState(false)

  useEffect(() => {
    if ((!hasUpgradeTab && mode === "upgrades") || (!hasGoapTab && mode === "goap")) {
      setMode("info")
    }
  }, [hasGoapTab, hasUpgradeTab, mode])

  if (props.selected == null) {
    return <Fragment />
  }

  return (
    <Fragment>
      <div class='flex w-7 h-full flex-col justify-evenly items-start'>
        <IconButton active={mode === "info"} icon={INFO_ICON} onClick={() => setMode("info")} />
        {hasGoapTab && (
          <IconButton active={mode === "goap"} icon={GOAP_ICON} onClick={() => setMode("goap")} />
        )}
        {hasUpgradeTab && (
          <IconButton active={mode === "upgrades"} icon={UPGRADE_ICON} onClick={() => setMode("upgrades")} />
        )}
      </div>

      {mode === "info" && (
        <InfoTabContent selected={props.selected} />
      )}

      <GoapTabContent
        active={mode === 'goap'}
        onAvailabilityChange={setHasGoapTab}
      />

      <UpgradesTabContent
        active={mode === 'upgrades'}
        onAvailabilityChange={setHasUpgradeTab}
      />
    </Fragment>
  )
}
