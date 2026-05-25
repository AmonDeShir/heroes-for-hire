import { h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'
import { Bar } from '../components/bar'
import { Icon } from '../components/icon'
import { IconFrame } from '../components/icon-frame'

const HEART_ICON = Resources.Load('Icons/hearts') as Texture2D

export function Heroes() {
  const [heroesCs] = useEventfulState(heroesPanelPresenter, 'Heroes')
  const [selected] = useEventfulState(selectionPanelPresenter, 'Selected')

  const heroes = useMemo(() => toJsArray(heroesCs), [heroesCs])

  return (
    <div style={{ width: 200 }}>
      <Bar details={1} title="Heroes" />

      <div style={{ maxHeight: 180, overflow: 'auto' }}>
        <div class='flex flex-row flex-wrap p-2'>
          {heroes.map((hero, idx) => {
            const icon = (hero.Icon ? (Resources.Load(hero.Icon) as Texture2D) : null) ?? HEART_ICON
            const pct = hero.MaxHp > 0 ? Math.max(0, Math.min(1, hero.Hp / hero.MaxHp)) : 0
            const ml = idx % 5 === 0 ? 0 : 8
            const isSelected = selected?.Id === hero.Id

            return (
              <div
                key={hero.Id}
                class='cursor-pointer select-none'
                onClick={() => heroesPanelPresenter.SelectHero(hero.Id)}
                style={{ marginLeft: ml, marginTop: 8 }}
              >
                <div class='w-[30px] h-[30px]'>
                  <IconFrame icon={icon} variant={isSelected ? 'active' : 'default'} />
                </div>
                <div class='h-[5px] bg-black/25 mt-1 w-[30px]'>
                  <div class={'h-full ' + (isSelected ? 'bg-active' : 'bg-mainDark')} style={{ width: `${pct * 100}%` }} />
                </div>
              </div>
            )
          })}
        </div>
      </div>
    </div>
  )
}

function toJsArray<T>(csArr?: System.Array$1<T> | null): T[] {
  if (!csArr) {
    return []
  }

  const arr = new Array((csArr as any).Length)
  let i = (csArr as any).Length
  while (i--) {
    arr[i] = (csArr as any).get_Item(i)
  }
  return arr
}
