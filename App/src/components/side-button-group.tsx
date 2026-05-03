import { h } from 'preact'
import { Texture2D } from "UnityEngine";
import { IconButton } from './icon-button';

export type ButtonConfig = {
  icon: Texture2D;
  active: boolean;
  show?: boolean;
  onClick: () => void;
}

type Props = {
  buttons: ButtonConfig[];
}

export function SideButtonGroup({ buttons }: Props) {
  const visibleButtons = buttons.filter(b => b.show !== false);
  const slots = Array(4).fill(null);

  return (
    <div class='flex w-7 h-full flex-col justify-evenly items-start'>
      {slots.map((_, index) => {
        const btn = visibleButtons[index];
        
        if (btn) {
          return (
            <IconButton 
              key={index}
              icon={btn.icon} 
              active={btn.active} 
              onClick={btn.onClick} 
            />
          );
        }

        return <div key={index} class="w-7 h-7" />;
      })}
    </div>
  );
}