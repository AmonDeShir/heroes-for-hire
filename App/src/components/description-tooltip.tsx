import { h } from 'preact'
import { createPortal } from 'preact/compat'

type Props = {
  text: string,
  visible: boolean,
}

export function DescriptionTooltip({ text, visible}: Props) {
  if (!visible || !text || typeof document === 'undefined') {
    return null
  }

  return createPortal(
    <div
      class='absolute top-0 left-0 pointer-events-none max-w-[220px] m-1 px-2 py-1 border-2 border-main bg-tertiary text-[10px] text-main'
    >
      {text}
    </div>,
    document.body as any,
  )
}
