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
  color?: "main" | "secondary" | "active" | "disabled",
  size?: number,
  extraText?: boolean,
  onMouseEnter?: (event: any) => void,
  onMouseLeave?: () => void,
}

export function ShopButton({icon, name, price, active, disabled, onClick, extraText, color = "main", size = 78, onMouseEnter, onMouseLeave }: Props) {
  const textClass = disabled
    ? "text-disabled"
    : active
      ? "text-textInverse"
      : color === "secondary"
        ? "text-secondary"
        : "text-main";

  const barClass = disabled
    ? "bg-disabled"
    : active
      ? "bg-active"
      : color === "secondary"
        ? "bg-secondary"
        : "bg-main";

  return (
    <div style={{ width: size, height: size + 23 + (extraText ? 12.5 : 0) }}>
      <DecorativeFrameButton icon={icon} active={active} disabled={disabled} onClick={onClick} color={color} size={size} onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave} />
      
      <div class={clsx("text-[9px] transition-colors text-center py-[3px]", textClass)}>
        <div style={{ marginBottom: extraText ? 2.5 : 0 }}>{name.toUpperCase()}</div>
        <div>{price}</div>
      </div>
      
      <div class={clsx("w-full h-[2px] transition-colors", barClass)} />
    </div>
  );
}
