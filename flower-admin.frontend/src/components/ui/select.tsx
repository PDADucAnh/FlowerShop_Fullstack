"use client"

import * as React from "react"
import { cn } from "@/lib/utils"
import { Select } from "@base-ui/react/select"
import { Check, ChevronDown } from "lucide-react"

function SelectRoot({ children, ...props }: Select.Root.Props<string, false>) {
  return <Select.Root {...props}>{children}</Select.Root>
}

const SelectTrigger = React.forwardRef<
  HTMLButtonElement,
  React.ComponentPropsWithoutRef<typeof Select.Trigger>
>(({ className, children, ...props }, ref) => (
  <Select.Trigger
    ref={ref}
    className={cn(
      "flex h-9 w-full items-center justify-between rounded-lg border border-input bg-background px-3 py-2 text-sm shadow-sm outline-none transition-colors placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50",
      className
    )}
    {...props}
  >
    {children}
    <Select.Icon>
      <ChevronDown className="size-4 opacity-50" />
    </Select.Icon>
  </Select.Trigger>
))
SelectTrigger.displayName = "SelectTrigger"

function SelectValue({ className, ...props }: Select.Value.Props) {
  return <Select.Value className={cn("flex-1 text-start", className)} {...props} />
}

const SelectContent = React.forwardRef<
  HTMLDivElement,
  React.ComponentPropsWithoutRef<typeof Select.Popup>
>(({ className, children, ...props }, ref) => (
  <Select.Portal>
    <Select.Positioner>
      <Select.Popup
        ref={ref}
        className={cn(
          "relative z-50 max-h-96 min-w-32 overflow-hidden rounded-lg border bg-popover text-popover-foreground shadow-lg shadow-foreground/5 outline-none animate-in fade-in zoom-in-95",
          className
        )}
        {...props}
      >
        <Select.List>{children}</Select.List>
      </Select.Popup>
    </Select.Positioner>
  </Select.Portal>
))
SelectContent.displayName = "SelectContent"

const SelectItem = React.forwardRef<
  HTMLDivElement,
  React.ComponentPropsWithoutRef<typeof Select.Item>
>(({ className, children, ...props }, ref) => (
  <Select.Item
    ref={ref}
    className={cn(
      "relative flex w-full cursor-default items-center rounded-sm px-2 py-1.5 text-sm outline-none select-none transition-colors hover:bg-accent hover:text-accent-foreground focus-visible:bg-accent focus-visible:text-accent-foreground data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
      className
    )}
    {...props}
  >
    <Select.ItemIndicator className="absolute left-2 flex items-center">
      <Check className="size-4" />
    </Select.ItemIndicator>
    <Select.ItemText>{children}</Select.ItemText>
  </Select.Item>
))
SelectItem.displayName = "SelectItem"

export {
  SelectRoot as Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
}
