import { Fragment, h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'
import { Icon } from '../components/icon'

const HEART_ICON = Resources.Load('Icons/hearts') as Texture2D

export function QuestTabContent(props: { active: boolean, onAvailabilityChange: (hasQuestTab: boolean) => void }) {
  const [questCs] = useEventfulState(selectionPanelPresenter, 'SelectedQuest')
  const quest = questCs as any
  const hasQuestTab = !!quest

  useEffect(() => {
    props.onAvailabilityChange(hasQuestTab)
  }, [hasQuestTab, props.onAvailabilityChange])

  if (!props.active || !hasQuestTab) {
    return <div class='hidden' />
  }

  return (
    <Fragment>
      <div class='flex w-full h-full flex-row justify-center items-start'>
        <div class='flex flex-col justify-center h-full'>
          <div class='flex flex-row items-center'>
            <div class='text-[10px] text-main'>Quest pool: {quest.PoolGold}</div>
            <div
              class={'ml-2 px-1 border-2 text-[10px] select-none ' + (quest.CanIncrease ? 'border-main bg-tertiary cursor-pointer' : 'border-disabled text-disabled')}
              onClick={() => quest.CanIncrease && selectionPanelPresenter.IncreaseSelectedQuestGold()}
            >
              +100
            </div>
          </div>

          <div class='flex flex-row flex-wrap'>
            {toJsArray(quest.Participants).map((p: any, idx: number) => {
              const tex = (p.Icon ? (Resources.Load(p.Icon) as Texture2D) : null) ?? HEART_ICON
              const ml = idx === 0 ? 0 : 6

              return (
                <div key={p.HeroId ?? idx} style={{ marginLeft: ml, marginTop: 4 }}>
                  <div class='w-[26px] h-[26px] border-box border-2 border-main bg-tertiary'>
                    <Icon icon={tex} />
                  </div>
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
