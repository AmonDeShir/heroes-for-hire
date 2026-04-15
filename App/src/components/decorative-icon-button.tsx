import { h } from 'preact'
import { Texture2D } from 'UnityEngine';
import clsx from 'clsx';
import { useClick } from '../hooks/use-click';
import { DecorativeFrame } from './decorative-frame';
import { Icon } from './icon';

type Props = {
  icon: Texture2D,
  active?: boolean,
  onClick?: () => void,
  size?: number,
  mask?: string,
}

export function DecorativeIconButton({
  icon,
  active,
  onClick,
  size = 39,
  mask,
}: Props) {
  const clicked = useClick(onClick);

  return (
    <div
      class="group transition-[scale] duration-100 ease-in-out"
      style={{ width: size, height: size, scale: clicked.state ? 0.8 : 1 }}
      onClick={clicked.register}
    >
      <DecorativeFrame size={size} mask={mask} color={active ? "active" : "main"}>
        <div
          class={clsx(
            'w-full h-full p-1',
            active && 'bg-tertiaryLight',
            !active && 'bg-tertiary group-hover:bg-tertiaryLight',
          )}
        >
          <Icon icon={icon} />
        </div>
      </DecorativeFrame>
    </div>
  );
}
