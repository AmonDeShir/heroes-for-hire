import { Fragment, h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useState } from 'onejs-preact/hooks'
import { InfoTabContent } from './info-tab-content'
import { GoapTabContent } from './goap-tab-content'
import { UpgradesTabContent } from './upgrades-tab-content'
import { ShopTabContent } from './shop-tab-content'
import { ButtonConfig, SideButtonGroup } from '../components/side-button-group'
import { CombatTabContent } from './combat-tab-content'
import { ChapelTabContent } from './chapel-tab-content'
import { QuestTabContent } from './quest-tab-content'

const INFO_ICON = Resources.Load("Icons/info") as Texture2D
const UPGRADE_ICON = Resources.Load("Icons/upgrade") as Texture2D
const GOAP_ICON = Resources.Load("Icons/all/lorc/brain") as Texture2D
const SHOP_ICON = Resources.Load("Icons/coin") as Texture2D
const COMBAT_ICON = Resources.Load("Icons/all/lorc/crossed-swords") as Texture2D
const CHAPEL_ICON = Resources.Load("Icons/all/lorc/ankh") as Texture2D
const QUEST_ICON = (Resources.Load("Icons/all/lorc/compass") as Texture2D)

type Mode = "info" | "combat" | "quest" | "shop" | "goap" | "upgrades" | "chapel"

export function InfoPanelContent(props: {
  selected: any
}) {
  const [mode, setMode] = useState<Mode>("info")
  const [hasShopTab, setHasShopTab] = useState(false)
  const [hasUpgradeTab, setHasUpgradeTab] = useState(false)
  const [hasGoapTab, setHasGoapTab] = useState(false)
  const [hasCombatTab, setHasCombatTab] = useState(false)
  const [hasChapelTab, setHasChapelTab] = useState(false)
  const [hasQuestTab, setHasQuestTab] = useState(false)

  useEffect(() => {
    if ((!hasShopTab && mode === "shop") || (!hasUpgradeTab && mode === "upgrades") || (!hasGoapTab && mode === "goap") || (!hasCombatTab && mode === "combat") || (!hasChapelTab && mode === "chapel") || (!hasQuestTab && mode === "quest")) {
      setMode("info")
    }
  }, [hasGoapTab, hasShopTab, hasUpgradeTab, hasCombatTab, hasChapelTab, hasQuestTab, mode])

  if (props.selected == null) {
    return <Fragment />
  }

  const sidebarButtons: ButtonConfig[] = [
    { icon: INFO_ICON, active: mode === "info", onClick: () => setMode("info") },
    { icon: COMBAT_ICON, active: mode === "combat", onClick: () => setMode("combat"), show: hasCombatTab },
    { icon: QUEST_ICON, active: mode === "quest", onClick: () => setMode("quest"), show: hasQuestTab },
    { icon: CHAPEL_ICON, active: mode === "chapel", onClick: () => setMode("chapel"), show: hasChapelTab },
    { icon: SHOP_ICON, active: mode === "shop", onClick: () => setMode("shop"), show: hasShopTab },
    { icon: GOAP_ICON, active: mode === "goap", onClick: () => setMode("goap"), show: hasGoapTab },
    { icon: UPGRADE_ICON, active: mode === "upgrades", onClick: () => setMode("upgrades"), show: hasUpgradeTab },
  ];

  return (
    <Fragment>
      <SideButtonGroup buttons={sidebarButtons} />;

      {mode === "info" && (
        <InfoTabContent selected={props.selected} />
      )}

      <CombatTabContent
        active={mode === 'combat'}
        onAvailabilityChange={setHasCombatTab}
      />

      <ChapelTabContent
        active={mode === 'chapel'}
        onAvailabilityChange={setHasChapelTab}
      />

      <QuestTabContent
        active={mode === 'quest'}
        onAvailabilityChange={setHasQuestTab}
      />

      <ShopTabContent
        active={mode === 'shop'}
        onAvailabilityChange={setHasShopTab}
      />

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
