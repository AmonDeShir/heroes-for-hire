import { h, render } from 'preact'

const Image = resource.loadImage("src/assets/detail.png")

export function Detail() {
  return (
    <div class="w-[36px] h-[16px] my-[1px] mx-[1.5px]" style={{ backgroundImage: Image }}></div>
  )
}