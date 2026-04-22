import { h } from 'preact'
import { createPortal } from 'preact/compat'

type Props = {
  text: string,
  visible: boolean,
  x: number,
  y: number,
}

export function DescriptionTooltip({ text, visible, x, y }: Props) {
  if (!visible || !text || typeof document === 'undefined') {
    return null
  }

  return createPortal(
    <div
      class='pointer-events-none max-w-[220px] px-2 py-1 border-2 border-main bg-tertiary text-[10px] text-main leading-[1.2]'
      style={{ position: 'absolute', left: x + 12, top: y + 12 } as any}
    >
      {text}
    </div>,
    document.body,
  )
}
