import { useCallback, useContext } from "onejs-preact/hooks"
import { TooltipContext } from "../context/tooltip-context"

export const useTooltip = () => {
  const context = useContext(TooltipContext)

  if (!context) {
    throw new Error("useTooltip must be used within TooltipProvider")
  }

  return context
}

export const useTooltipBinding = () => {
  const { show, hide } = useTooltip()

  return useCallback((text: string | null | undefined) => ({
    onMouseEnter: () => text ? show(text) : null,
    onMouseLeave: hide
  }), [show, hide])
}