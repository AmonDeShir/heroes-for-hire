import { h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo } from 'onejs-preact/hooks'

import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { ProgressBar } from '../components/progress-bar'

const HEART_ICON = Resources.Load('Icons/hearts') as Texture2D

export function CombatTabContent(props: {
  active: boolean
  onAvailabilityChange: (has: boolean) => void
}) {
  const [combat] = useEventfulState(selectionPanelPresenter, 'SelectedCombat')

  const hasCombatTab = combat != null

  useEffect(() => {
    props.onAvailabilityChange(hasCombatTab)
  }, [hasCombatTab, props.onAvailabilityChange])

  if (!props.active || !hasCombatTab) {
    return <div class='hidden' />
  }

  const leftIcon = useMemo(
    () =>
      combat?.LeftIcon
        ? (Resources.Load(combat.LeftIcon) as Texture2D)
        : HEART_ICON,
    [combat?.LeftIcon],
  )

  const rightIcon = useMemo(
    () =>
      combat?.RightIcon
        ? (Resources.Load(combat.RightIcon) as Texture2D)
        : HEART_ICON,
    [combat?.RightIcon],
  )

  return (
    <div class='ml-3 flex w-full h-full flex-row justify-center items-start'>
      <div class='flex flex-row items-start'>
        <CombatCard
          icon={leftIcon}
          description={combat.LeftDescription}
          name={combat.LeftName}
          hp={combat.LeftHp}
          maxHp={combat.LeftMaxHp}
        />

        <div class='h-[90px] flex items-center justify-center text-[12px] text-main w-[40px]'>
          VS
        </div>

        <CombatCard
          icon={rightIcon}
          description={combat.RightDescription}
          name={combat.RightName}
          hp={combat.RightHp}
          maxHp={combat.RightMaxHp}
        />
      </div>
    </div>
  )
}

function CombatCard(props: {
  icon: Texture2D
  name: string
  description: string
  hp: number
  maxHp: number
}) {
  return (
    <div class='flex flex-row h-[120px] items-start'>
      <div class='flex-shrink-0 flex flex-col w-[120px] items-center'>
        <DecorativeFrame>
          <div class='w-full h-full p-2 bg-tertiary'>
            <Icon icon={props.icon} />
          </div>
        </DecorativeFrame>

        <div class='text-[10px] mt-0.5 text-center text-main w-[120px]'>
          {props.name}
        </div>
      </div>

      <div class='ml-3 mt-2 flex flex-col items-center'>
        <ProgressBar
          icon={HEART_ICON}
          max={props.maxHp}
          value={props.hp}
          text='Health'
          displayValue
        />

        <div class='text-[8px] mt-1 text-center text-main w-[160px]'>
          {props.description}
        </div>
      </div>
    </div>
  )
}