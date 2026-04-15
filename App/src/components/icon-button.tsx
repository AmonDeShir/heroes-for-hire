import { h, render } from 'preact'
import { Texture2D } from "UnityEngine";
import { useClick } from '../hooks/use-click';
import { Icon } from './icon';
import clsx from 'clsx';

type Props = {
  icon: Texture2D,
  active?: boolean,
  onClick?: () => void,
}

export function IconButton({ icon, active, onClick }: Props) {
  const clicked = useClick(onClick);

  return (
    <div class={clsx(
        "w-7 h-7 border-2 p-0.5 transition-[scale] duration-100 ease-in-out",
        active && "bg-tertiaryLight",
        !active && "bg-tertiary hover:bg-tertiaryLight",
        active && "border-active",
        !active && "border-main",
      )}
      style={{scale: clicked.state ? 0.8 : 1}} 
      onClick={clicked.register}
    >
      <Icon icon={icon} />      
    </div>
  );
}
