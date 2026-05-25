import { Fragment, h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'
import { Icon } from '../components/icon'

const HEART_ICON = Resources.Load('Icons/hearts') as Texture2D

export function ChapelTabContent(props: { active: boolean, onAvailabilityChange: (hasChapelTab: boolean) => void }) {
  const [building] = useEventfulState(selectionPanelPresenter, 'SelectedBuilding')
  const [chapelRevivesCs] = useEventfulState(selectionPanelPresenter, 'ChapelRevives')

  const revives = useMemo(() => toJsArray(chapelRevivesCs as any), [chapelRevivesCs])
  const hasChapelTab = !!building?.IsAlive && !!building?.IsChapel

  useEffect(() => {
    props.onAvailabilityChange(hasChapelTab)
  }, [hasChapelTab, props.onAvailabilityChange])

  if (!props.active || !hasChapelTab) {
    return <div class='hidden' />
  }

  if (revives.length === 0) {
    return <div class='text-[10px] text-disabled flex-1 flex items-center justify-center'>No pending revives</div>
  }

  return (
    <Fragment>
      <div class='flex w-full h-full flex-row justify-center items-start'>
        <div class='flex flex-col justify-center h-full'>
          <div class='text-[10px] text-main mb-1'>Reviving</div>
          <div class='flex flex-row flex-wrap'>
            {revives.map((it: any, idx: number) => {
              const tex = (it.Icon ? (Resources.Load(it.Icon) as Texture2D) : null) ?? HEART_ICON
              const s = Math.ceil(Math.max(0, it.RemainingSeconds ?? 0))
              const ml = idx % 10 === 0 ? 0 : 6

              return (
                <div key={it.HeroId ?? idx} style={{ marginLeft: ml, marginTop: 4 }}>
                  <div class='w-[26px] h-[26px] border-box border-2 border-main bg-tertiary'>
                    <Icon icon={tex} />
                  </div>
                  <div class='text-[9px] text-secondary text-center w-[26px]'>{s}s</div>
                </div>
              )
            })}
          </div>
        </div>
      </div>
    </Fragment>
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
