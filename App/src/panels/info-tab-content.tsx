import { h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo } from 'onejs-preact/hooks'
import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { ProgressBar } from '../components/progress-bar'
import { useTooltipBinding } from '../hooks/use-tooltip'

const HEART_ICON = Resources.Load("Icons/hearts") as Texture2D
const COIN_ICON = Resources.Load("Icons/coin") as Texture2D;

export function InfoTabContent(props: {
  selected: any
}) {
  const [damageable] = useEventfulState(selectionPanelPresenter, 'SelectedDamageable')
  const [hero] = useEventfulState(selectionPanelPresenter, 'SelectedHero')
  
  const icon = useMemo(
    () => props.selected ? Resources.Load(props.selected.Icon) as Texture2D : HEART_ICON,
    [props.selected],
  )

  const bindTooltip = useTooltipBinding();

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

      <div class='flex flex-col justify-center h-full'>
        <div class='flex h-[70px] items-start justify-between'>
          {damageable && (
            <div>
              <ProgressBar
                icon={HEART_ICON}
                max={damageable.MaxHealth}
                value={damageable.CurrentHealth}
                text='Health'
                displayValue
                {...bindTooltip("The current physical condition of this entity. Reaching zero will result in destruction or death.")}
              />

            {hero && (
              <div class='mt-0.5'>
              <ProgressBar
                icon={COIN_ICON}
                max={hero.Gold}
                value={hero.Gold}
                text='Gold'
                displayValue
                {...bindTooltip(
                  hero
                  ? "Personal treasury. Gold is used by the hero for upgrades, purchasing equipment, and personal maintenance."
                  : "Accumulated wealth. This gold represents local revenue and will be transferred to the national treasury during the next tax collection"
                )}
              />
              </div>
            )
            }
            </div>
          )}

          <div class='font-[10px]'>
            {props.selected.Description}
          </div>
        </div>
      </div>
    </div>
  )
}
