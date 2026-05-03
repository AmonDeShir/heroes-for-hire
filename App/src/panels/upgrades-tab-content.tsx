import { Fragment, h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo, useState } from 'onejs-preact/hooks'
import System from 'System'
import { Icon } from '../components/icon'
import { ProgressBar } from '../components/progress-bar'
import { ShopButton } from '../components/shop-button'
import { useTooltipBinding } from '../hooks/use-tooltip'

const INFO_ICON = Resources.Load("Icons/info") as Texture2D

export function UpgradesTabContent(props: { active: boolean, onAvailabilityChange: (hasUpgradeTab: boolean) => void }) {
  const [building] = useEventfulState(selectionPanelPresenter, 'SelectedBuilding')
  const [buildingUpgrades] = useEventfulState(selectionPanelPresenter, 'BuildingUpgrades')
  const [queuedBuildingUpgrades] = useEventfulState(selectionPanelPresenter, 'QueuedBuildingUpgrades')

  const upgrades = useMemo(() => toJsArray(buildingUpgrades), [buildingUpgrades])
  const queuedUpgradeItems = useMemo(() => toJsArray(queuedBuildingUpgrades), [queuedBuildingUpgrades])
  const activeUpgrade = useMemo(() => upgrades.find((upgrade) => upgrade.IsActive) ?? null, [upgrades])
  
  const bindTooltip = useTooltipBinding();
  const getUpgradeDesc = (u: any) => u.LockReason ? `${u.Description}\n\n${u.LockReason}` : (u.Description ?? "")

  const remainingUpgrades = useMemo(
    () => upgrades.filter((upgrade) => upgrade.IsCompleted || upgrade.LockReason !== 'Usage limit reached'),
    [upgrades],
  )

  const activeUpgradeIcon = useMemo(
    () => activeUpgrade ? Resources.Load(activeUpgrade.Icon) as Texture2D : INFO_ICON,
    [activeUpgrade],
  )

  const hasUpgradeTab = !!building?.IsAlive && upgrades.length > 0

  useEffect(() => {
    props.onAvailabilityChange(hasUpgradeTab)
  }, [hasUpgradeTab, props.onAvailabilityChange])

  if (!props.active || !hasUpgradeTab) {
    return <div class='hidden' />
  }

  return (
    <Fragment>
      <div class='flex w-full h-full flex-row justify-center items-start'>
        <div class='flex-shrink-0 flex flex-col w-[148px] ml-4 h-full justify-between items-start py-0.5'>
          <div class='w-full flex flex-row flex-wrap items-start'>
            {queuedUpgradeItems.map((upgrade) => (
              <div
                key={upgrade.Id}
                class='flex-shrink-0 h-[18px] w-[18px] mr-0.5 border-box border-2 border-main bg-tertiary'
                onClick={() => selectionPanelPresenter.SelectUpgrade(upgrade.Id)}
                {...bindTooltip(getUpgradeDesc(upgrade))}
              >
                <Icon icon={Resources.Load(upgrade.Icon) as Texture2D} />
              </div>
            ))}
          </div>

          <div class='w-full min-h-[32px] flex flex-col justify-between'>
            <div class='text-center text-[10px]'>{activeUpgrade ? activeUpgrade?.Name : "Upgrades" }</div>

            <div class='w-full min-h-[18px]'>
              {activeUpgrade ? (
                <ProgressBar
                  icon={activeUpgradeIcon}
                  max={1}
                  value={activeUpgrade.Progress}
                  text={`${(activeUpgrade.Progress * 100).toFixed(0)}%`}
                  {...bindTooltip(activeUpgrade.Description)}
                />
              ) : (
                <div class='w-[148px] h-[18px] flex flex-row border-2 border-main bg-tertiary/40' />
              )}
            </div>
          </div>
        </div>

        <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />

        <div class='flex h-full items-start justify-evenly flex-1'>
          <div class='flex w-full h-full justify-center items-start flex-wrap'>
            {remainingUpgrades.map((upgrade) => (
              <div class='h-full flex flex-row items-center justify-center mr-2'>
                <ShopButton
                  key={upgrade.Id}
                  name={upgrade.Name}
                  price={upgrade.Price}
                  icon={Resources.Load(upgrade.Icon) as Texture2D}
                  active={upgrade.IsActive}
                  color={upgrade.IsCompleted ? 'secondary' : 'main'}
                  disabled={!upgrade.IsCompleted && !upgrade.CanQueue}
                  extraText
                  onClick={() => selectionPanelPresenter.SelectUpgrade(upgrade.Id)}
                  {...bindTooltip(getUpgradeDesc(upgrade))}
                />
              </div>
            ))}

            {remainingUpgrades.length === 0 && (
              <div class='text-[10px] text-disabled'>No additional upgrades</div>
            )}
          </div>
        </div>
      </div>
    </Fragment>
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

function buildTooltipText(upgrade: any): string {
  if (!upgrade) {
    return ''
  }

  if (upgrade.LockReason) {
    return `${upgrade.Description}\n\n${upgrade.LockReason}`
  }

  return upgrade.Description ?? ''
}
