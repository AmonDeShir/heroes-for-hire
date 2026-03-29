import { ComponentChildren, h } from 'preact'
import { Bar } from './bar'
import { Blur } from './blur';

type Props = {
  title: string,
	children?: ComponentChildren,
}

export function Panel({title, children}: Props) {
  return (
    <div class='w-full h-full'>
      <div class='h-[23px]' style={{height: 23}}>
        <Bar title={title} details={6} />
      </div>
      <div class='w-full h-full' style={{ maxHeight: "calc(100% - 23px)" }}>
        <Blur blur={15} color='#AFA089' opacity={0.5}>
          <div class="w-full h-full text-xs">
            {children}
          </div>
        </Blur>
      </div>
    </div>
  )
}