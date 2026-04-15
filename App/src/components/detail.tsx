import { h, render } from 'preact'

export function Detail() {
  return (
    <div class="relative w-[36px] h-[16px]">
      <div class="absolute inset-0 border border-secondary box-border"/>
      
      <div class="absolute left-[3px] top-[3px] w-[14px] h-[10px] border border-secondary box-border"/>
      <div class="absolute right-[3px] top-[3px] w-[14px] h-[10px] border border-secondary box-border"/>

      <div class="absolute left-[6px] top-[6px] w-[8px] h-[4px] border border-secondary box-border"/>
      <div class="absolute right-[6px] top-[6px] w-[8px] h-[4px] border border-secondary box-border"/>
    </div>
  )
}