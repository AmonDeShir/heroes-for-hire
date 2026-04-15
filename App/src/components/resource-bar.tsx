import { h } from "onejs-preact";

import { Blur } from "./blur";
import { Icon } from "./icon";
import { Texture2D } from "UnityEngine";
import { useAnimateNumber } from "../hooks/use-animate-number";

type Props = {
  value: number;
  icon: Texture2D;
}

export function ResourceBar({ value, icon }: Props) {
  const animatedValue = useAnimateNumber(value);

  return (
    <div class="p-1">
      <div class="border-main border-2 w-[106px] h-[31px] text-secondary">
        <Blur blur={15} color='#AFA089' opacity={0.5}>
          <div class="flex flex-row h-full w-full">
            <div class="w-[31px] h-[27px] flex-shrink-0 flex justify-center items-center border-r-2 border-main bg-tertiary">
              <div class="w-[18px] h-[18px]">
                <Icon icon={icon} />
              </div>
            </div>

            <div class="text-center w-full h-full justify-center items-center">
              {animatedValue}
            </div>
          </div>  
        </Blur>
      </div>
    </div>
  );
}