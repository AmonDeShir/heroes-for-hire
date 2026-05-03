import { Fragment, h } from 'preact'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useState } from 'onejs-preact/hooks'
import { InfoTabContent } from './info-tab-content'
import { GoapTabContent } from './goap-tab-content'
import { UpgradesTabContent } from './upgrades-tab-content'
import { ButtonConfig, SideButtonGroup } from '../components/side-button-group'

const INFO_ICON = Resources.Load("Icons/info") as Texture2D
const UPGRADE_ICON = Resources.Load("Icons/upgrade") as Texture2D
const GOAP_ICON = Resources.Load("Icons/all/lorc/brain") as Texture2D

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

  const sidebarButtons: ButtonConfig[] = [
    { icon: INFO_ICON, active: mode === "info", onClick: () => setMode("info") },
    { icon: GOAP_ICON, active: mode === "goap", onClick: () => setMode("goap"), show: hasGoapTab },
    { icon: UPGRADE_ICON, active: mode === "upgrades", onClick: () => setMode("upgrades"), show: hasUpgradeTab },
  ];

  return (
    <Fragment>
      <SideButtonGroup buttons={sidebarButtons} />;

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
