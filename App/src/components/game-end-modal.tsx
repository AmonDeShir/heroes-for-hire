import { Fragment, h } from 'preact'
import { useEventfulState } from 'onejs-preact/hooks'

export function GameEndModal() {
  const [open] = useEventfulState(gameEndPresenter, 'IsOpen')
  const [message] = useEventfulState(gameEndPresenter, 'Message')

  if (!open) {
    return <Fragment />
  }

  return (
    <div class='absolute inset-0 flex items-center justify-center' style={{ backgroundColor: 'rgba(0,0,0,0.65)' }}>
      <div class='border-2 border-main bg-tertiary p-3 w-[280px]'>
        <div class='text-[14px] text-main text-center'>{message}</div>
        <div class='h-[8px]' />
        <div
          class='border-2 border-main bg-tertiaryLight text-[12px] text-main text-center cursor-pointer select-none py-1'
          onClick={() => gameEndPresenter.QuitGame()}
        >
          OK
        </div>
      </div>
    </div>
  )
}
