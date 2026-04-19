import { h, render } from 'preact'
import { useAnimateNumber } from "../hooks/use-animate-number";
import { Texture2D } from 'UnityEngine';
import { useMemo } from 'onejs-preact/hooks';
import { Icon } from './icon';

type Props = {
  icon: Texture2D,
  value: number,
  max: number,
  text: string,
  displayValue?: boolean
}

export function ProgressBar({ value, max, icon, text, displayValue }: Props) {
  const animatedValue = useAnimateNumber(value);
const percentage = useMemo(() => Math.max(Math.min((animatedValue / max) * 100, 100), 0), [animatedValue, max]);

  return (
    <div class={`w-[148px] h-[18px] flex flex-row`}>
      <div class='flex-shrink-0 h-[18px] w-[18px] border-box p-0.5 border-2 border-main bg-tertiary'>
        <Icon icon={icon} />
      </div>  

      <div class='h-full w-full relative border-2 border-main border-l-0'>
        <div class='absolute left-0 top-0 h-full transition-[width] bg-mainDark' style={{ width: `${percentage}%` }}/>
        
        <div class='absolute w-full h-full text-[10px] top-0 left-0 flex justify-center items-center text-secondary'>
          {`${text.toUpperCase()}${displayValue ? ': ' + animatedValue.toFixed(0) : ""}`}
        </div>
      </div>
    </div>
  );
}