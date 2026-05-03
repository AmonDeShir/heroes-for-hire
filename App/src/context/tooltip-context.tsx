import { createContext, h, ComponentChildren } from 'preact'
import { useState, useMemo, useCallback } from 'onejs-preact/hooks'
import { DescriptionTooltip } from '../components/description-tooltip'

export interface TooltipContextType {
	show: (text: string) => void
	hide: () => void
}

export const TooltipContext = createContext<TooltipContextType | null>(null)

export function TooltipProvider({ children }: { children?: ComponentChildren }) {
	const [text, setText] = useState<string>("")
	const [visible, setVisible] = useState(false)

	const show = useCallback((msg: string) => {
		if (!msg) return
		setText(msg)
		setVisible(true)
	}, [])

	const hide = useCallback(() => setVisible(false), [])

	const value = useMemo(() => ({ show, hide }), [show, hide])

	return (
		<TooltipContext.Provider value={value}>
			{children}
			<DescriptionTooltip text={text} visible={visible} />
		</TooltipContext.Provider>
	)
}