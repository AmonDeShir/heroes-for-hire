import { h } from 'preact'

export function Arrow(props: { class?: string }) {
  return (
    <div
      class={`w-[19px] h-0 border-t-2 ${props.class ?? ''}`}
      style={{ borderColor: '#D7BA17' }}
    />
  )
}
