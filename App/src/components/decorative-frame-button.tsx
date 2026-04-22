import { h } from 'preact'
import { Texture2D } from 'UnityEngine';
import clsx from 'clsx';
import { useClick } from '../hooks/use-click';
import { DecorativeFrame } from './decorative-frame';
import { Icon } from './icon';

type Props = {
  icon: Texture2D,
  active?: boolean,
  disabled?: boolean,
  onClick?: () => void,
  size?: number,
  mask?: string,
  color?: "main" | "secondary" | "active" | "disabled",
  onMouseEnter?: (event: any) => void,
  onMouseLeave?: () => void,
}

export function DecorativeFrameButton({icon, active, disabled, onClick, size = 78, mask, color, onMouseEnter, onMouseLeave}: Props) {
  const clicked = useClick(disabled ? undefined : onClick);
  const frameColor = disabled ? "disabled" : active ? "active" : color ?? "main";

  return (
    <div
      class={clsx("flex-sh group transition-[scale] duratdion-100 ease-in-out", disabled && "pointer-events-none")}
      style={{ width: size, height: size, scale: disabled ? 1 : clicked.state ? 0.8 : 1 }}
      onClick={disabled ? undefined : clicked.register}
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
    >
      <DecorativeFrame size={size} mask={mask} color={frameColor}>
        <div
          class={clsx(
            'w-full h-full p-2',
            active && 'bg-tertiaryLight',
            !active && !disabled && color === 'secondary' && 'bg-[#4a4330] group-hover:bg-[#5a5138]',
            !active && !disabled && color !== 'secondary' && 'bg-tertiary group-hover:bg-tertiaryLight',
            !active && disabled && 'bg-tertiaryDisabled',
          )}
        >
          <Icon icon={icon} class={clsx(disabled && 'opacity-60')} />
        </div>
      </DecorativeFrame>
    </div>
  );
}
