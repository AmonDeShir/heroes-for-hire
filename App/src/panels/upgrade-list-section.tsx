import { h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { ShopButton } from '../components/shop-button'

export function UpgradeListSection(props: {
  remainingUpgrades: any[]
  setHoveredUpgradeId: (value: string | null | ((current: string | null) => string | null)) => void
  setTooltipPos: (value: { x: number, y: number }) => void
}) {
  return (
    <div class='flex h-full items-start justify-evenly flex-1'>
      <div class='flex w-full justify-center items-start gap-2 flex-wrap'>
        {props.remainingUpgrades.map((upgrade) => (
          <ShopButton
            key={upgrade.Id}
            name={upgrade.Name}
            price={upgrade.Price}
            icon={Resources.Load(upgrade.Icon) as Texture2D}
            active={upgrade.IsActive}
            color={upgrade.IsCompleted ? "secondary" : "main"}
            disabled={!upgrade.IsCompleted && !upgrade.CanQueue}
            onClick={() => selectionPanelPresenter.SelectUpgrade(upgrade.Id)}
            onMouseEnter={(event) => {
              props.setHoveredUpgradeId(upgrade.Id)
              props.setTooltipPos({ x: event.clientX, y: event.clientY })
            }}
            onMouseLeave={() => props.setHoveredUpgradeId((current) => current === upgrade.Id ? null : current)}
          />
        ))}

        {props.remainingUpgrades.length === 0 && (
          <div class='text-[10px] text-disabled'>No additional upgrades</div>
        )}
      </div>
    </div>
  )
}
