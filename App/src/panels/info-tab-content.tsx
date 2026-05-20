import { h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo } from 'onejs-preact/hooks'
import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { ProgressBar } from '../components/progress-bar'
import { useTooltipBinding } from '../hooks/use-tooltip'
import { useRenderTexture } from '../hooks/use-render-texture'
import System from 'System'

const HEART_ICON = Resources.Load("Icons/hearts") as Texture2D
const COIN_ICON = Resources.Load("Icons/coin") as Texture2D;
const FALLBACK_ICON = COIN_ICON

export function InfoTabContent(props: {
  selected: any
}) {
  const [damageable] = useEventfulState(selectionPanelPresenter, 'SelectedDamageable')
  const [hero] = useEventfulState(selectionPanelPresenter, 'SelectedHero')
  const [building] = useEventfulState(selectionPanelPresenter, 'SelectedBuilding')
  const [heroEq] = useEventfulState(selectionPanelPresenter, 'SelectedHeroEquipment')
  
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

      { hero && (
        <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />
      )}

      { hero && (
        <div class='text-[10px] w-20 h-full flex flex-col justify-center'>
          <div class='flex flex-row justify-between w-20'>Attack: {hero.Attack}</div>
          <div class='flex flex-row justify-between w-20'>Defence: {hero.Defence}</div>
          <div class='flex flex-row justify-between w-20'>Speed: {hero.Speed}</div>
          <div class='flex flex-row justify-between w-20'>Gear: {hero.GearLevel.toFixed(1)}</div>
          <div class='flex flex-row justify-between w-20'>Danger: {hero.DangerLevel.toFixed(1)}</div>
        </div>
      )}
      
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

            {!hero && building && null}
            </div>
          )}

          <div class='font-[10px]'>
            {props.selected.Description}
          </div>
        </div>
      </div>

      { hero && (
        <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />
      )}

      { hero && (
        <EquipmentGrid value={heroEq} bindTooltip={bindTooltip} />
      )}
    </div>
  )
}

type WeaponItem = CS.Heroes.Presentation.UI.SelectionPanel.HeroEquipmentItemDTO;

function EquipmentGrid(props: { value?: any, bindTooltip: (text: string) => any }) {
  const items = useMemo(() => {
    const eq = props.value as any
    if (!eq) return [] as WeaponItem[]

    const list: WeaponItem[] = []

    if (eq.Weapon) list.push(eq.Weapon as WeaponItem)
    if (eq.Armor) list.push(eq.Armor as WeaponItem)
      
    for (const it of toJsArray(eq.Artifacts)) list.push(it as WeaponItem)
    for (const it of toJsArray(eq.Consumables)) list.push(it as WeaponItem)
    for (const it of toJsArray(eq.Backpack)) list.push(it as WeaponItem)

    return list.filter(Boolean)
  }, [props.value])

  const textures = useMemo(() => {
    return items.map(it => (it?.Icon ? (Resources.Load(it.Icon) as Texture2D) : null) ?? FALLBACK_ICON)
  }, [items])

  if (items.length === 0) {
    return <div class='text-[10px] text-disabled'>No items</div>
  }

  return (
    <div class='flex flex-row h-full items-center flex-wrap'>
      {textures.map((tex, idx) => (
        <div
          key={idx}
          class='h-[30px] w-[30px] border-box border-2 border-main bg-tertiary mr-0.5'
          {...props.bindTooltip(items[idx]?.Name)}
        >
          <Icon icon={tex} />
        </div>
      ))}
    </div>
  )
}

function toJsArray<T>(csArr?: System.Array$1<T>): T[] {
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

function joinArray(csArr: any): string {
  if (!csArr) {
    return ''
  }

  const len = (csArr as any).Length ?? 0
  if (!len) {
    return ''
  }

  const parts: string[] = []
  for (let i = 0; i < len; i++) {
    parts.push((csArr as any).get_Item(i))
  }
  return parts.join(', ')
}
