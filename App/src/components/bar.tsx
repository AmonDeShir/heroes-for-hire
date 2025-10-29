import { h } from 'preact'
import { Detail } from './detail'

type Props = {
  details: number,
  title: string
}

export function Bar({details, title}: Props) {
  return (
    <div class="flex-row justify-between bg-main text-sm text-secondary h-[20px] items-center">
      <div class="flex-row items-center">
        {Array.from(Array(details).keys()).map(() => (
          <div>
            <Detail />
          </div>
        ))}
      </div>

      <div>{title}</div>

      <div class="flex-row items-center">
        {Array.from(Array(details).keys()).map(() => (
          <div>
            <Detail />
          </div>
        ))}
      </div>
    </div>
  )
}