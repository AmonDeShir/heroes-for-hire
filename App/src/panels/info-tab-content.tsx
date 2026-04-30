import { h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo } from 'onejs-preact/hooks'
import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { ProgressBar } from '../components/progress-bar'

const HEART_ICON = Resources.Load("Icons/hearts") as Texture2D

export function InfoTabContent(props: {
  selected: any
}) {
  const [damageable] = useEventfulState(selectionPanelPresenter, 'SelectedDamageable')
  const [hero] = useEventfulState(selectionPanelPresenter, 'SelectedHero')
  const icon = useMemo(
    () => props.selected ? Resources.Load(props.selected.Icon) as Texture2D : HEART_ICON,
    [props.selected],
  )

  return (
    <div class='flex w-full h-full flex-row justify-center items-start'>
      <div class='flex-shrink-0 flex flex-col w-[78px] h-full justify-center items-center ml-2'>
        <DecorativeFrame>
          <div class='w-full h-full p-2 bg-tertiary'>
            <Icon icon={icon} />
          </div>
        </DecorativeFrame>
      </div>

      <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />

      <div class='flex h-full items-start justify-evenly gap-3'>
        {damageable && (
          <div>
            <ProgressBar
              icon={HEART_ICON}
              max={damageable.MaxHealth}
              value={damageable.CurrentHealth}
              text='Health'
              displayValue
            />
          </div>
        )}

        <div class='font-[10px]'>
          {props.selected.Description}

          {hero && (
            <div class='mt-2'>
              <div>Gold: {hero.Gold}</div>
              <div>Gear: {hero.GearLevel.toFixed(1)}</div>
              <div>Danger: {hero.DangerLevel.toFixed(2)}</div>
              <div>{hero.IsInHome ? 'Inside home' : 'Outside'}</div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
