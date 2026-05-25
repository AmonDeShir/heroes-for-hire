import { h, ComponentChildren } from 'preact'

export function UiBlocker(props: { children?: ComponentChildren }) {
  return (
    <div
      onMouseEnter={() => uiInputGatePresenter.SetCursorOnUi(true)}
      onMouseLeave={() => uiInputGatePresenter.SetCursorOnUi(false)}
      onMouseDown={() => uiInputGatePresenter.SetCursorOnUi(true)}
      onMouseUp={() => uiInputGatePresenter.SetCursorOnUi(false)}
    >
      {props.children}
    </div>
  )
}
