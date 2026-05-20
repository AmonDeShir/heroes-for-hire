import { h } from 'preact'
import { useEffect, useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'
import { ProgressBar } from '../components/progress-bar'
import { Resources, Texture2D } from 'UnityEngine'
import { useTooltipBinding } from '../hooks/use-tooltip'
import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { Arrow } from '../components/arrow'

const WANDER_ICON = Resources.Load("Icons/all/lorc/treasure-map") as Texture2D;
const DEFAULT_STEP_ICON = Resources.Load("Icons/all/lorc/brain") as Texture2D;

export function GoapTabContent(props: { active: boolean, onAvailabilityChange: (hasGoapTab: boolean) => void }) {
  const [goap] = useEventfulState(selectionPanelPresenter, 'SelectedGoap')
  const [hero] = useEventfulState(selectionPanelPresenter, 'SelectedHero')

  const goals = useMemo(() => toJsArray(goap?.Goals), [goap])
  const steps = useMemo(() => toJsArray(goap?.Steps), [goap])
  const hasGoapTab = !!hero && !!goap

  const bindTooltip = useTooltipBinding();

  useEffect(() => {
    props.onAvailabilityChange(hasGoapTab)
  }, [hasGoapTab, props.onAvailabilityChange])

  if (!props.active || !hasGoapTab) {
    return <div class='hidden' />
  }

  return (
    <div class='ml-3 flex w-full h-full flex-row justify-center items-start'>
      <div class='flex-1 h-full flex flex-row items-start'>
        <div class='flex flex-col justify-center h-full'>
          <div class='flex h-[90px] items-start justify-between'>
            {goals.map((goal) => (
              <ProgressBar 
                key={goal.Name}
                icon={Resources.Load(goal.Icon) as Texture2D}
                max={1.0}
                value={goal.Value}
                text={goal.Name}
                {...bindTooltip(goal.Description)}
              />
            ))}
          </div>
        </div>

        <div class='w-0.5 h-[90px] bg-main ml-4 mr-4 my-auto' />

        <div class='flex flex-col justify-center h-full'>
          <div class='flex h-[90px] items-start justify-between'>
            {steps.length > 0 ? (
              <div class='flex flex-row flex-wrap items-start gap-y-1'>
                {steps.map((step, index) => (
                  <div key={index} class='flex flex-row items-center'>
                    <StepNode step={step} bindTooltip={bindTooltip} />
                    {index < steps.length - 1 && (
                      <div class='mx-1 mt-[-10px]'>
                        <Arrow />
                      </div>
                    )}
                  </div>
                ))}
              </div>
            ) : (
              <div class='flex-shrink-0 flex flex-col w-[78px] h-full justify-center items-center ml-2'>
                <DecorativeFrame>
                  <div class='w-full h-full p-2 bg-tertiary'>
                    <Icon icon={WANDER_ICON} />
                  </div>
                </DecorativeFrame>
                <div class='text-xs mt-0.5'>Wander</div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

function StepNode(props: { step: any, bindTooltip: (text: string) => any }) {
  const step = props.step as any
  const icon = useMemo(() => {
    const p = step?.Icon as string | null | undefined
    return (p ? (Resources.Load(p) as Texture2D) : null) ?? DEFAULT_STEP_ICON
  }, [step?.Icon])
  const faded = !step?.PreconditionsMet

  return (
    <div
      class={'flex-shrink-0 flex flex-col w-[78px] h-full justify-center items-center ' + (faded ? 'opacity-60' : '')}
      {...props.bindTooltip(step?.Description)}
    >
      <DecorativeFrame>
        <div class='w-full h-full p-2 bg-tertiary'>
          <Icon icon={icon} />
        </div>
      </DecorativeFrame>
      <div class={'text-[10px] mt-0.5 text-center ' + (faded ? 'text-disabled' : 'text-main')}>{step?.Name ?? ''}</div>
    </div>
  )
}

function toJsArray<T>(csArr: System.Array$1<T> | null | undefined): T[] {
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
