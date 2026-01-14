import { h } from "onejs-core";

type Props = {
  icon: string,
  onClick?: () => void,
}

export function IconButton({ icon, onClick }: Props) {
  return (
    <div class="w-7 h-7 flex justify-center items-center border-2 border-main" onClick={onClick}>
      <div class="icon w-full h-full bg-center bg-cover bg-no-repeat">{icon}</div>
    </div>
  );
}