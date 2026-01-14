import { ComponentChildren, h } from 'preact'
import { Detail } from './detail'
import { Bar } from './bar'
import { Blur } from './blur';

type Props = {
  title: string,
	children?: ComponentChildren,
}

export function Panel({title, children}: Props) {
  return (
    <div class='w-full h-full'>
      <Bar title={title} details={6} />
      <Blur blur={15} color='#AFA089' opacity={0.5}>
        <div class="w-full h-full text-xs">
          {children}
        </div>
      </Blur>
    </div>
  )
}