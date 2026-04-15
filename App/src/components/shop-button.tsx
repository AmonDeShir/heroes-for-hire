import { h } from 'preact'
import { Texture2D } from 'UnityEngine';
import clsx from 'clsx';
import { DecorativeFrameButton } from './decorative-frame-button';

type Props = {
  icon: Texture2D,
  name: string,
  price: number,
  active?: boolean,
  disabled?: boolean,
  onClick?: () => void,
}

export function ShopButton({icon, name, price, active, disabled, onClick }: Props) {
  return (
    <div class="w-[78px] h-[101px]">
      <DecorativeFrameButton icon={icon} active={active} disabled={disabled} onClick={onClick} />
      
      <div class={clsx("text-[9px] transition-colors text-center py-[3px]", disabled ? "text-disabled" : active ? "text-textInverse" : "text-main")}>
        <div>{name.toUpperCase()}</div>
        <div>{price}</div>
      </div>
      
      <div class={clsx("w-full h-[2px] transition-colors", disabled ? "bg-disabled" : active ? "bg-active" : "bg-main")} />
    </div>
  );
}
