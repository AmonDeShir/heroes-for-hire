import { h, render } from 'preact'

const Image = resource.loadImage("src/assets/detail.png")

export function Detail() {
  return (
    <div class="w-[36px] h-[16px]" style={{ backgroundImage: Image }}></div>
  )
}