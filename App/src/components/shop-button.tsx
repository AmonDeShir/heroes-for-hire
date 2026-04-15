import { h } from 'preact'
import { Texture2D } from 'UnityEngine';
import clsx from 'clsx';
import { useClick } from '../hooks/use-click';
import { DecorativeFrame } from './decorative-frame';
import { Icon } from './icon';
import { DecorativeFrameButton } from './decorative-frame-button';

type Props = {
  icon: Texture2D,
  name: string,
  price: number,
  active?: boolean,
  onClick?: () => void,
}

export function ShopButton({icon, name, price, active, onClick }: Props) {
  return (
    <div class="w-[78px] h-[101px]">
      <DecorativeFrameButton icon={icon} active={active} onClick={onClick} />
      
      <div class={clsx("text-[9px] transition-colors text-center py-[3px]", active ? "text-textInverse" : "text-main")}>
        <div>{name.toUpperCase()}</div>
        <div>{price}</div>
      </div>
      
      <div class={clsx("w-full h-[2px] transition-colors", active ? "bg-active" : "bg-main")} />
    </div>
  );
}
