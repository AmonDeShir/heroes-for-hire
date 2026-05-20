import { Fragment, h } from 'preact'
import { Resources, Texture2D } from 'UnityEngine'
import { useEffect, useEventfulState, useMemo, useState } from 'onejs-preact/hooks'
import System from 'System'
import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { useTooltipBinding } from '../hooks/use-tooltip'

const FALLBACK_ICON = Resources.Load('Icons/coin') as Texture2D

export function ShopTabContent(props: { active: boolean, onAvailabilityChange: (hasShopTab: boolean) => void }) {
  const [building] = useEventfulState(selectionPanelPresenter, 'SelectedBuilding')
  const [shopItemsCs] = useEventfulState(selectionPanelPresenter, 'ShopItems')

  const shopItems = useMemo(() => toJsArray(shopItemsCs), [shopItemsCs])
  const hasShopTab = !!building?.IsAlive && shopItems.length > 0

  const [selectedId, setSelectedId] = useState<string | null>(null)
  const selectedItem = useMemo(
    () => shopItems.find((x) => x.Id === selectedId) ?? shopItems[0] ?? null,
    [selectedId, shopItems],
  )

  useEffect(() => {
    props.onAvailabilityChange(hasShopTab)
  }, [hasShopTab, props.onAvailabilityChange])

  useEffect(() => {
    if (!hasShopTab) {
      setSelectedId(null)
      return
    }

    if (selectedId && shopItems.some((x) => x.Id === selectedId)) {
      return
    }

    setSelectedId(shopItems[0]?.Id ?? null)
  }, [hasShopTab, selectedId, shopItems])

  const bindTooltip = useTooltipBinding()
  const getItemTooltip = (it: any) => {
    if (!it) {
      return ''
    }

    const lines = [
      `${it.Name} (${it.Slot})`,
      it.Description ?? '',
      '',
      `Cost: ${it.GoldCost}`,
      `Attack: ${it.Attack}`,
      `Defense: ${it.Defense}`,
      `Speed: ${it.Speed}`,
      `HP Regen: ${it.HpRegeneration}`,
    ].filter((x) => x !== '')

    if (!it.IsUnlocked && it.LockReason) {
      lines.push('', it.LockReason)
    }

    return lines.join('\n')
  }

  if (!props.active || !hasShopTab) {
    return <div class='hidden' />
  }

  return (
    <Fragment>
      <div class='flex w-full h-full flex-row justify-center items-start'>
        <div class='flex-shrink-0 flex flex-col w-[148px] ml-4 h-full justify-start items-start py-0.5'>
          <div class='w-full flex flex-row flex-wrap items-start'>
            {shopItems.map((it) => (
              <div
                key={it.Id}
                class={'flex-shrink-0 h-[18px] w-[18px] mr-0.5 border-box border-2 ' + (it.Id === selectedItem?.Id ? 'border-secondary' : 'border-main') + ' bg-tertiary ' + (!it.IsUnlocked ? 'opacity-50' : '')}
                onClick={() => setSelectedId(it.Id)}
                {...bindTooltip(getItemTooltip(it))}
              >
                <Icon icon={(it.Icon ? (Resources.Load(it.Icon) as Texture2D) : null) ?? FALLBACK_ICON} />
              </div>
            ))}
          </div>
        </div>

        <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />

        <div class='flex h-full items-start justify-evenly flex-1'>
          {selectedItem ? (
            <div class='w-full h-full flex flex-row items-start'>
              <div class='flex-shrink-0 flex flex-col w-[78px] h-full justify-start items-center ml-2'>
                <DecorativeFrame>
                  <div class='w-full h-full p-2 bg-tertiary'>
                    <Icon icon={(selectedItem.Icon ? (Resources.Load(selectedItem.Icon) as Texture2D) : null) ?? FALLBACK_ICON} />
                  </div>
                </DecorativeFrame>
                {!selectedItem.IsUnlocked && (
                  <div class='text-[10px] text-disabled mt-0.5'>Locked</div>
                )}
              </div>

              <div class='flex-1 ml-4 mr-4'>
                <div class='text-[12px] text-main'>{selectedItem.Name}</div>
                <div class='text-[10px] text-main/80'>{selectedItem.Description}</div>

                <div class='mt-2 text-[10px]'>Cost: {selectedItem.GoldCost}</div>
                <div class='text-[10px]'>Attack: {selectedItem.Attack}</div>
                <div class='text-[10px]'>Defense: {selectedItem.Defense}</div>
                <div class='text-[10px]'>Speed: {selectedItem.Speed}</div>
                <div class='text-[10px]'>HP Regen: {selectedItem.HpRegeneration}</div>

                {!selectedItem.IsUnlocked && selectedItem.LockReason && (
                  <div class='mt-2 text-[10px] text-disabled'>{selectedItem.LockReason}</div>
                )}
              </div>
            </div>
          ) : (
            <div class='text-[10px] text-disabled'>No shop items</div>
          )}
        </div>
      </div>
    </Fragment>
  )
}

function toJsArray<T>(csArr: System.Array$1<T> | null | undefined): T[] {
  if (!csArr) {
    return []
  }

  const arr = new Array((csArr as any).Length)
  let i = (csArr as any).Length

  while (i--) {
    arr[i] = csArr.get_Item(i)
  }

  return arr
}
