import { h } from 'preact'
import { useEffect, useEventfulState, useMemo } from 'onejs-preact/hooks'
import System from 'System'

export function GoapTabContent(props: { active: boolean, onAvailabilityChange: (hasGoapTab: boolean) => void }) {
  const [goap] = useEventfulState(selectionPanelPresenter, 'SelectedGoap')
  const [hero] = useEventfulState(selectionPanelPresenter, 'SelectedHero')

  const beliefs = useMemo(() => toJsArray(goap?.Beliefs), [goap])
  const steps = useMemo(() => toJsArray(goap?.Steps), [goap])
  const hasGoapTab = !!hero && !!goap

  useEffect(() => {
    props.onAvailabilityChange(hasGoapTab)
  }, [hasGoapTab, props.onAvailabilityChange])

  if (!props.active || !hasGoapTab) {
    return <div class='hidden' />
  }

  return (
    <div class='flex w-full h-full flex-row justify-center items-start'>
      <div class='flex-1 h-full flex flex-row items-start'>
        <div class='w-[150px] h-full text-[10px] ml-4 flex-shrink-0'>
          <div class='font-bold'>AI</div>
          <div class='mt-1'>Goal: {goap.GoalName || 'None'}</div>
          <div>{goap.IsIdle ? `Idle: ${goap.IdleName}` : 'Executing plan'}</div>
          <div class='mt-2 font-bold'>Beliefs</div>
          {beliefs.map((belief) => (
            <div>{belief.Name}: {belief.Value.toFixed(2)}</div>
          ))}
        </div>

        <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />

        <div class='flex-1 h-full text-[10px] overflow-hidden'>
          <div class='font-bold'>Plan</div>
          {steps.length > 0 ? (
            <div class='mt-1'>
              {steps.map((step, index) => (
                <div class='mb-2'>
                  <div>{index + 1}. {step.Name}</div>
                  <div class={step.PreconditionsMet ? 'text-main/80' : 'text-disabled'}>{step.Description || 'No description'}</div>
                </div>
              ))}
            </div>
          ) : (
            <div class='mt-1 text-disabled'>No active plan</div>
          )}
        </div>
      </div>
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
